﻿using System.Runtime.CompilerServices;

namespace HelloWorldTest
{
    internal class HelloWorld
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void WriteLine(string str);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string ReadLine(string prompt);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void WriteAllText(string path, string data);

        static void Main(string[] args)
        {
            WriteLine("Hello World!");
            var Data = ReadLine("Type something: ");
            WriteLine(Data);
        }
    }
}