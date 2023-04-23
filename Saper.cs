using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace praktik_07._04._2023
{
    struct cell
    {
        public char displayed;
        public bool bombed;
        public bool flagged;
        public int surrounded;
        public cell(bool bombed)
        {
            this.bombed = bombed;
            flagged = false;
            displayed = '?';
            surrounded = 0;
        }

        public void open() {
            if (bombed)
                displayed = '*';
            else if (surrounded != 0) displayed = Convert.ToChar(surrounded.ToString());
            else displayed = ' ';
        }
    }

    internal class Minesweeper 
    {
        cell[,] field;
        int rows;
        int columns;
        int mines;
        public Minesweeper(int height, int width, int mines)
        {
            rows = height;
            columns = width;
            this.mines = mines;
            field = new cell[height, width];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    field[i, j] = new cell(false);

        }

        private void countMines()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!field[i, j].bombed)
                    {
                        for (int i1 = -1; i1 <= 1; i1++)
                        {
                            for (int j1 = -1; j1 <= 1; j1++)
                            {
                                if (j1 == 0 && i1 == 0) continue;

                                try
                                {
                                    if (field[i + i1, j + j1].bombed)
                                        field[i, j].surrounded++;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void placeMines(int avoidedRow, int avoidedColumn)
        {
            Random r = new Random();
            while (true)
            {
                for (int i = 0; i < mines; i++)
                {
                    int row;
                    int column;

                    while (true)
                    {
                        row = r.Next(rows);
                        column = r.Next(columns);
                        if (Math.Abs(row - avoidedRow) <= 1 &&
                            Math.Abs(column - avoidedColumn) <= 1)
                            continue;
                        if (!field[row, column].bombed)
                        {
                            field[row, column].bombed = true;//place a bomb if cell isn't bombed
                            break;
                        }
                    }
                }

                if (safeField()) break;
            }
        }

        public void play()
        {
            display();

            int startRow = default;
            int startColumn = default;
            try
            {
                while (true)
                {
                    Console.WriteLine("cell:");
                    string cell = Console.ReadLine();
                    startRow = Convert.ToInt32(cell.Substring(1)) - 1;
                    if (startRow < 0 || startRow >= rows) throw new Exception("wrong row");

                    startColumn= (int)cell[0] - 97;
                    if (startColumn < 0 || startColumn >= columns) throw new Exception("wrong column");
                    break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            placeMines(startRow, startColumn);
            countMines();

            dig(startRow, startColumn);
            display();

            while (true)
            {
                try
                {
                    Console.WriteLine("cell:");
                    string chosenCell = Console.ReadLine();
                    int index;
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("1 - dig\n2 - flag/unflag\n3 - back");
                            if (!int.TryParse(Console.ReadLine(), out index))
                                throw new Exception("not a number");

                            if (index < 1 || index > 3)
                                throw new Exception("must be between 1 and 3");

                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    switch (index)
                    {
                        case 1:
                            dig(chosenCell);
                            break;
                        case 2:
                            flag(chosenCell);
                            break;
                    }


                    if (loseCheck())
                    {
                        Thread.Sleep(1000);
                        for (int i = 0; i < rows; i++)
                            for (int j = 0; j < columns; j++)
                                if(field[i, j].bombed) field[i, j].displayed = '*';
                        display();

                        Console.WriteLine("you lose");
                        break;
                    }

                    if (winCheck())
                    {
                        Console.WriteLine("you win!");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void flag(string cell)
        {
            int row = Convert.ToInt32(cell.Substring(1)) - 1;
            if (row < 0 || row >= rows) throw new Exception("wrong row");

            int column = (int)cell[0] - 97;
            if (column < 0 || column >= columns) throw new Exception("wrong column");

            field[row, column].flagged = !field[row, column].flagged;
            field[row, column].displayed = field[row, column].displayed == '?' ? '&' : '?';
            //set or remove the flag

            display();
        }
        
        private void dig(int i, int j)
        {
            if (field[i, j].displayed != '?') throw new Exception("already opened");

            //if (!field[i, j].open()) Console.WriteLine("game over");

            field[i, j].open();

            if (field[i, j].surrounded == 0 && !field[i, j].bombed)
                openEmpty(i, j);
            else
            {
                for (int i1 = -1; i1 <= 1; i1++)
                {
                    for (int j1 = -1; j1 <= 1; j1++)
                    {
                        if (j == 0 && i == 0) continue;

                        try
                        {
                            if (field[i + i1, j + j1].surrounded == 0 && !field[i + i1, j + j1].bombed)
                            {
                                field[i + i1, j + j1].open();
                                openEmpty(i + i1, j + j1);
                            }
                                
                        }
                        catch (IndexOutOfRangeException)
                        {
                            continue;
                        }
                    }
                }//opening empty area
            }
        }

        private void openEmpty(int row, int column)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    try
                    {
                        if (field[row + i, column + j].displayed == '?' && !field[row + i, column + j].bombed)
                        {
                            field[row + i, column + j].open();//open if hidden

                            if (field[row + i, column + j].surrounded == 0 && !field[row + i, column + j].bombed)
                                openEmpty(row + i, column + j);
                            //proceed to the cell if it's empty
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
        }

        public void dig(string cell)
        {
            
            int row = Convert.ToInt32(cell.Substring(1)) - 1; 
            if (row < 0 || row >= rows) throw new Exception("wrong row");

            int column = (int)cell[0] - 97;
            if (column < 0 || column >= columns) throw new Exception("wrong column");

            dig(row, column);

            display();
        }

        private bool winCheck()
        {
            int bombsGuessed = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (field[i, j].displayed == '?' || field[i, j].displayed == '&') 
                        bombsGuessed++;

            return bombsGuessed == mines;
        }

        private bool loseCheck()
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (field[i, j].displayed == '*') return true;

            return false;
        }

        private bool safeField()
        {
            connectedCells(0, 0);//placing the flags in every accesible cell

            int flaggedCount = 0;//how many cells are accesible
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (field[i, j].flagged)
                    {
                        flaggedCount++;
                        field[i, j].flagged = false;
                    }
                }
            }

            return flaggedCount == rows * columns - mines;
        }

        private void connectedCells(int row, int column)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    try
                    {
                        if (!field[row + i, column + j].bombed
                            && !field[row + i, column + j].flagged)
                        {
                            field[row + i, column + j].flagged = true;
                            connectedCells(row + i, column + j);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
        }

        public void display()
        {
            Console.Clear();

            Console.Write("    ");
            for (int i = 0; i < columns; i++)
                Console.Write((char)(i + 97) + " ");

            Console.WriteLine("\n");

            for (int i = 0; i < rows; i++)
            {
                Console.Write(i + 1 + "   ");
                for (int j = 0; j < columns; j++)
                {
                    //if (field[i, j].bombed) Console.Write("*");
                    //else if (field[i, j].surrounded != 0)
                    //    Console.Write(field[i, j].surrounded);
                    //else Console.Write(' ');
                    //Console.Write(" ");
                    Console.Write(field[i, j].displayed + " ");
                }

                Console.WriteLine();
            }
        }


    }
}
