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

Saper saper = new Saper(10, 10, 15);
saper.display();

while(true)
{
    try
    {
        Console.WriteLine("cell:");
        string answer = Console.ReadLine();
        saper.move(answer);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}