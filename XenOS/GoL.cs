using System;

namespace XenOS
{
    public class GoL
    {
        public void StartGame()
        {
            Console.WriteLine("# = on\n@ = off");
            Console.Write("Enter game string >> ");
            string buffer = Console.ReadLine();

            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
    }
}
