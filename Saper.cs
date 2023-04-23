using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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

        public bool open() {
            if (bombed)
            {
                displayed = '*';
                return false;
            }
            else if (surrounded != 0) displayed = Convert.ToChar(surrounded.ToString());
            else displayed = ' ';
            return true;
        }
    }

    internal class Saper 
    {
        cell[,] field;
        int rows;
        int columns;
        int mines;
        public Saper(int height, int width, int mines)
        {
            rows = height;
            columns = width;
            this.mines = mines;
            field = new cell[height, width];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    field[i, j] = new cell(false);

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
                        if (!field[row, column].bombed)
                        {
                            field[row, column].bombed = true;//place a bomb if cell isn't bombed
                            break;
                        }
                    }
                }

                if (safeField()) break;
            }

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
                                    //counting the bombs
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

        private void move(int i, int j)
        {
            if (field[i, j].displayed != '?') throw new Exception("already opened");

            if (!field[i, j].open()) Console.WriteLine("game over");

            if (field[i, j].surrounded == 0 && !field[i, j].bombed)
            {
                field[i, j].open();
                openEmpty(i, j);
            }
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

        public void move(string cell)
        {
            
            int row = Convert.ToInt32(cell.Substring(1)) - 1; 
            if (row < 0 || row >= rows) throw new Exception("wrong row");

            int column = (int)cell[0] - 97;
            if (column < 0 || column >= columns) throw new Exception("wrong column");

            move(row, column);

            display();
        }

        private bool winCheck()
        {
            return false;
        }

        private bool loseCheck()
        {
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
