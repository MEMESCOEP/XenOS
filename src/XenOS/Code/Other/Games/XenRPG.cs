using System;
using System.Collections.Generic;

namespace XenOS.Code.Other.Games
{
    internal class XenRPG
    {
        public string ProgramVersion = "Alpha 1";
        public List<string> classes = new List<string>() { "Mage", "Knight", "Gaud", "Witch", "Matyr" };
        public string PlayerName = null;
        public string PlayerClass = null;

        public void StartRPG()
        {
            Console.WriteLine("XenRPG " + ProgramVersion);
            PlayerName = Prompt("What's your character's name? >> ");
            SelectClass();
            Console.WriteLine("Welcome to XenRPG, " + PlayerName);
            Console.WriteLine("You'll be playing as a " + PlayerClass);
        }

        public void SelectClass()
        {
            foreach (var Class in classes)
            {
                Console.WriteLine("[{0}] " + Class, classes.IndexOf(Class));
            }

            PlayerClass = Prompt("What's your character's class? (type the number) >> ");

            if (int.Parse(PlayerClass) > classes.Count)
            {
                Console.WriteLine("Invalid choice!\n");
                SelectClass();
            }
            else if (int.Parse(PlayerClass) < 0)
            {
                Console.WriteLine("Invalid choice!\n");
                SelectClass();
            }
            else
            {
                PlayerClass = classes[int.Parse(PlayerClass)];
            }
        }

        public string Prompt(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }
    }
}
