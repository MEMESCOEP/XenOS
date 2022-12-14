using System;

namespace HelloWorldTest
{
    internal class HelloWorld
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.Write("Type something: ");
            var Data = Console.ReadLine();
            Console.WriteLine($"You typed: {Data}");

            Console.Write("Press a key: ");
            var Key = Console.ReadKey();
            Console.WriteLine($"\nYou pressed: {Key.Key}");
        }
    }
}
