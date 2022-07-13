using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;

namespace XenOS
{
    public class Kernel : Sys.Kernel
    {

        protected override void BeforeRun()
        {
            //Console.SetWindowSize(80, 25);

            try
            {
                Console.WriteLine("[INFO -> Kernel] >> Kernel loaded.");
                Console.WriteLine("[INFO -> Kernel] >> Loading shell...");
                Shell shell = new Shell();
                shell.init();
            }
            catch(Exception ex)
            {
                KernelPanic(ex.Message, ex.Message);        
            }
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
            Console.WriteLine("EXCEPTION: " + exception + "\n" + "MESSAGE: " + msg + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            Console.CursorVisible = false;
            Console.Beep(1000, 500);
            Console.Beep(750, 500);
            while (true)
            {
                for(int i = 0; i < 10; i++)
                {
                    Console.Write("System will attempt to reboot in " + (10 - i) + " second(s) \r");
                    Thread.Sleep(1000);
                }
                Power power = new Power();
                power.reboot();
            }
        }
    }
}
