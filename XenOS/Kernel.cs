using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace XenOS
{
    public class Kernel : Sys.Kernel
    {

        protected override void BeforeRun()
        {
            Console.WriteLine("[INFO -> Kernel] >> Kernel loaded.");
            Console.WriteLine("[INFO -> Kernel] >> Loading shell...");
            Shell shell = new Shell();
            shell.init();
        }

        protected override void Run()
        {
            Console.WriteLine("ERROR: Shutdown Failed!\nPlease shutdown manually by pressing the power button.");
            while (true);
        }

        public static void KernelPanic(string exception, string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.WriteLine("[================================ KERNEL PANIC ================================]");
            Console.WriteLine("EXCEPTION: " + exception + "\n" + "MESSAGE: " + msg);
            Console.CursorVisible = false;
            Console.Beep(1000, 1000);
            while (true) ;
        }
    }
}
