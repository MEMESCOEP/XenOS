using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class LockScreen
    {
        public void Lock(string UnlockPassword)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[========================= The screen has been locked =========================]");
            Console.ForegroundColor = Shell.TextColor;
            Console.WriteLine("Enter the correct password to continue.");
            while (true)
            {
                Console.Write(">> ");
                var pswd = Console.ReadLine();
                if(pswd != null)
                {
                    if(pswd == UnlockPassword)
                    {
                        Console.Clear();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect password entered.\n");
                    }
                }
                else
                {
                    Console.WriteLine("You need to enter a password!\n");
                    continue;
                }
            }
        }
    }
}
