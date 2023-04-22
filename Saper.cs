using System;
using System.Collections.Generic;
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
            displayed = '∎';
            surrounded = 0;
        }

        public void open() {
            if (bombed) Console.WriteLine("game over");
            else displayed = Convert.ToChar(surrounded);
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

            Random r = new Random();
            //while (true)
            //{
            //    for (int i = 0; i < mines; i++)
            //    {
            //        int row;
            //        int column;

            //        while (true)
            //        {
            //            row = r.Next(rows);
            //            column = r.Next(columns);
            //            if (!field[row, column].bombed)
            //            {
            //                field[row, column].bombed = true;//place a bomb if cell isn't bombed
            //                break;
            //            }
            //        }
            //    }

            //    if (safeField()) break;
            //}

            field[1, 0].bombed = true;
            field[1, 1].bombed = true;
            field[2, 1].bombed = true;
            field[3, 0].bombed = true;
            field[3, 1].bombed = true;

            Console.WriteLine($"safe field: {safeField()}");
        }
        private void move(int i, int j)
        {
            field[i, j].open();
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
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if(field[i, j].bombed) Console.Write("*");
                    else Console.Write("0");
                }
                    //Console.Write(field[i, j].displayed);

                Console.WriteLine();
            }
        }
    }
}
