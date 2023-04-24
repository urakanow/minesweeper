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

        public void Open()
        {
            if (bombed)
                displayed = '*';
            else if (surrounded != 0) displayed = Convert.ToChar(surrounded.ToString());
            else displayed = ' ';
        }
    }

    internal class Minesweeper
    {
        private Dictionary<int, ConsoleColor> colors = new Dictionary<int, ConsoleColor>(){
            { 1, ConsoleColor.DarkBlue },
            { 2, ConsoleColor.Green },
            { 3, ConsoleColor.Red },
            { 4, ConsoleColor.Magenta },
            { 5, ConsoleColor.Yellow },
            { 6, ConsoleColor.Blue },
            { 7, ConsoleColor.Black },
            };
        cell[,] field;
        int rows { get; }
        int columns { get; }
        int mines { get; }
        int placedFlags;
        public Minesweeper(int height, int width, int mines)
        {
            rows = height;
            columns = width;
            this.mines = mines;
            placedFlags = 0;
            field = new cell[height, width];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    field[i, j] = new cell(false);
        }

        private (int, int) CellToIndexes(string cell)
        {
            int row = Convert.ToInt32(cell.Substring(1)) - 1;
            if (row < 0 || row >= rows) throw new Exception("wrong row");

            int column = (int)cell[0] - 97;
            if (column < 0 || column >= columns) throw new Exception("wrong column");

            return (row, column);
        }

        private void CountMines()
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

        private void PlaceMines(int avoidedRow, int avoidedColumn)
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

                if (SafeField()) break;
            }
        }

        public void Play()
        {
            Display();

            int startRow = default;
            int startColumn = default;
            try
            {
                while (true)
                {
                    Console.WriteLine("cell:");
                    string cell = Console.ReadLine();
                    startRow = CellToIndexes(cell).Item1;
                    startColumn = CellToIndexes(cell).Item2;
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            PlaceMines(startRow, startColumn);
            CountMines();

            Dig(startRow, startColumn);
            Display();

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
                            Dig(chosenCell);
                            break;
                        case 2:
                            Flag(chosenCell);
                            break;
                        case 3:
                            Display();
                            break;
                    }


                    if (LoseCheck())
                    {
                        Thread.Sleep(1000);
                        for (int i = 0; i < rows; i++)
                            for (int j = 0; j < columns; j++)
                                if (field[i, j].bombed) field[i, j].displayed = '*';
                        Display();

                        Console.WriteLine("you lose");
                        break;
                    }

                    if (WinCheck())
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

        private void Flag(string cell)
        {
            int row = CellToIndexes(cell).Item1;
            int column = CellToIndexes(cell).Item2;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                

            if (field[row, column].displayed != '?' && field[row, column].displayed != '&')
                throw new Exception("already opened");

            field[row, column].flagged = !field[row, column].flagged;
            field[row, column].displayed = field[row, column].displayed == '?' ? '&' : '?';
            //set or remove the flag
            if (field[row, column].displayed == '?') placedFlags--;
            else placedFlags++;

            Display();
        }

        private void Dig(int row, int column)
        {
            if (field[row, column].displayed != '?') {
                if (field[row, column].displayed != '&')
                    throw new Exception("already opened");
                else placedFlags--;
            }

                field[row, column].Open();

            if (field[row, column].surrounded == 0 && !field[row, column].bombed)
                OpenEmpty(row, column);
            else
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (column == 0 && row == 0) continue;

                        try
                        {
                            if (field[row + i, column + j].surrounded == 0 && !field[row + i, column + j].bombed)
                            {
                                field[row + i, column + j].Open();
                                OpenEmpty(row + i, column + j);
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

        public void Dig(string cell)
        {

            int row = CellToIndexes(cell).Item1;
            int column = CellToIndexes(cell).Item2;

            Dig(row, column);

            Display();
        }

        private void OpenEmpty(int row, int column)
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
                            field[row + i, column + j].Open();//open if hidden

                            if (field[row + i, column + j].surrounded == 0 && !field[row + i, column + j].bombed)
                                OpenEmpty(row + i, column + j);
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

        private bool WinCheck()
        {
            int minesGuessed = 0;
            int hidden = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (field[i, j].displayed == '&' && field[i, j].bombed)
                        minesGuessed++;
                    if (field[i, j].displayed == '?')
                        hidden++;
                }
            }

            return minesGuessed == mines || minesGuessed + hidden == mines;
        }

        private bool LoseCheck()
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (field[i, j].displayed == '*') return true;

            return false;
        }

        private bool SafeField()
        {
            ConnectedCells(0, 0);//placing flags in every accesible cell

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

        private void ConnectedCells(int row, int column)
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
                            ConnectedCells(row + i, column + j);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
        }

        public void Display()
        {
            Console.Clear();

            Console.Write($"&:  ");
            if(mines - placedFlags <= 9) Console.Write(" ");

            for (int i = 0; i < columns; i++)
                Console.Write((char)(i + 97) + " ");

            Console.WriteLine($"\n{mines - placedFlags}");

            for (int i = 0; i < rows; i++)
            {
                Console.Write(i + 1 + "  ");
                if (i < 9) Console.Write(" ");

                for (int j = 0; j < columns; j++)
                {
                    if (int.TryParse(field[i, j].displayed.ToString(), out int number))
                        Console.ForegroundColor = colors[number];
                    Console.Write(field[i, j].displayed + " ");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine();
            }
        }
    }
}