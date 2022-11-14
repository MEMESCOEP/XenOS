using System;
using System.Collections.Generic;

namespace XenOS
{
    internal class Games
    {
        public void SelectGame()
        {
            Console.Clear();
            Console.WriteLine("[== SELECT A GAME ==]");
            List<string> Games = new List<string>() { "XenRPG", "Conway's Game of Life", "Exit" };

            foreach (var game in Games)
            {
                Console.WriteLine("[{0}] " + game, Games.IndexOf(game));
            }

            while (true)
            {
                Console.Write(">> ");
                var choice = Console.ReadLine();
                Console.Clear();
                if (choice == (Games.Count - 1).ToString())
                {
                    break;
                }
                else
                {
                    if (choice == "0")
                    {
                        XenRPG xenRPG = new XenRPG();
                        xenRPG.StartRPG();
                        break;
                    }
                    else if(choice == "1")
                    {
                        GoL goL = new GoL();
                        goL.StartGame();
                        break;
                    }
                }
            }
        }
    }
}
