using System;
using System.IO;
using System.Linq;

namespace XenOS
{
    internal class Login
    {
        string username = null;
        string passwd = null;

        string unamePrompt = "";
        string pswdPrompt = "";

        int LoginAttempts = 0;

        public void SystemLogin()
        {
            pswdPrompt = "Enter password >> ";
            unamePrompt = "Enter username >> ";

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Shell.OsName + " " + Shell.Version + " Login");
            Console.ForegroundColor = Shell.TextColor;
            if (File.Exists("0:\\SETTINGS\\users"))
            {
                if (File.ReadAllText("0:\\SETTINGS\\users").Length > 0)
                {
                    if (!File.Exists("0:\\SETTINGS\\users") || File.ReadAllLines("0:\\SETTINGS\\users")[1] == String.Empty)
                    {
                        Console.WriteLine("Press any key to log in.");
                        Console.ReadKey();
                        Console.Clear();
                        return;
                    }
                    while (true)
                    {
                        GetUsername(unamePrompt);
                        GetPassword(pswdPrompt);
                        Shell.username = username;
                        Console.Clear();
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Press any key to log in.");
                Console.ReadKey();
                Console.Clear();
                return;
            }
        }

        public void GetUsername(string prompt)
        {
            Console.Write(prompt);
            username = Console.ReadLine();
            var unames = File.ReadAllText("0:\\SETTINGS\\users");
            if (!unames.Contains(username))
            {
                Console.WriteLine("Invalid username!\n");
                GetUsername(prompt);
            }
        }

        public void GetPassword(string prompt)
        {
            Console.Write(prompt);
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Helpers.ClearCurrentConsoleLine();
                    Console.Write(prompt);
                    for (int i = 0; i < pass.Length - 1; i++)
                    {
                        Console.Write("*");
                    }
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            LoginAttempts++;
            Console.WriteLine();
            passwd = pass;
            var passwords = File.ReadAllLines("0:\\SETTINGS\\login");
            var unames = File.ReadAllLines("0:\\SETTINGS\\users");
            int UnameIndex = 0;
            int count = 0;
            foreach (var un in unames)
            {
                if (un == username)
                {
                    UnameIndex = count;
                    break;
                }

                count++;
            }

            if (passwords[UnameIndex] != passwd || !passwords.Contains(passwd))
            {
                if (LoginAttempts >= 3)
                {
                    Console.WriteLine("Login failed.\n", 3 - LoginAttempts);
                    LoginAttempts = 0;
                    SystemLogin();
                }

                Console.WriteLine("Incorrect password entered! You have {0} attempt(s) remaining.\n", 3 - LoginAttempts);
                GetPassword(prompt);
            }
        }
    }
}
