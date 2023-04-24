//ConsoleKeyInfo key;

//Console.WriteLine("press any letter button");

//while(true)
//{
//    key = Console.ReadKey(true);
//    if (!char.IsLetter((char)key.Key))
//        Console.WriteLine("only letters!");
//    else
//    {
//        Console.WriteLine((char)key.Key);
//        Console.Beep((int)key.Key * 10, 300);
//    }
//}

using praktik_07._04._2023;

int rows, columns, mines;

while (true)
{
    Console.WriteLine("difficulty level:\n1 - small\n2 - medium\n3 - large");

    switch (Console.ReadLine())
    {
        case "1":
            rows = 10;
            columns = 10;
            mines = 15;
            break;
        case "2":
            rows = 15;
            columns = 15;
            mines = 50;
            break;
        case "3":
            rows = 20;
            columns = 20;
            mines = 120;
            break;
        default:
            Console.WriteLine("must be 1-3");
            continue;
    }
    break;
}

Minesweeper minesweeper = new Minesweeper(rows, columns, mines);

minesweeper.Play();

