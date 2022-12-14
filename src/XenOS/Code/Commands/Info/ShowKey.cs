using System;

namespace XenOS.Code.Commands.Info
{
    internal class ShowKey
    {
        public static void Start()
        {
            Console.WriteLine("Press ESCAPE to exit");
            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.Escape)
            {
                //Console.WriteLine(key.KeyChar);
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (Console.CursorLeft > 0)
                    {
                        Console.CursorLeft--;
                    }
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (Console.CursorLeft < Console.WindowWidth - 1)
                    {
                        Console.CursorLeft++;
                    }
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (Console.CursorTop > 0)
                    {
                        Console.CursorTop--;
                    }
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    if (Console.CursorTop < Console.WindowHeight - 1)
                    {
                        Console.CursorTop++;
                    }
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                    }
                }

                if (key.Key == ConsoleKey.PageDown)
                {
                    Console.CursorTop = Console.WindowHeight - 1;
                }

                if (key.Key == ConsoleKey.PageUp)
                {
                    Console.CursorTop = 0;
                }

                if (key.Key == ConsoleKey.Home)
                {
                    Console.CursorLeft = 0;
                }

                if (key.Key == ConsoleKey.End)
                {
                    Console.CursorLeft = Console.WindowWidth - 1;
                }
            }
            Console.Clear();
        }
    }
}
