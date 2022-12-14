using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS.Code.Information
{
    internal class ShowMouseState
    {
        public void Show()
        {
            while (true)
            {
                Console.WriteLine(Cosmos.System.MouseManager.MouseState);
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }
    }
}
