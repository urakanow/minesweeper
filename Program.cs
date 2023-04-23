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

Minesweeper saper = new Minesweeper(6, 6, 5);
saper.play();

