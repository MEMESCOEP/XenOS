using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;

namespace XenOS
{
    public class Kernel : Sys.Kernel
    {
        // Variables
        public static string KernelVersion = "7.0";

        // Functions
        protected override void BeforeRun()
        {
            try
            {
                Console.WriteLine("[INFO -> Kernel] >> XenOS Kernel Version {0} loaded.", KernelVersion);
                Console.WriteLine("[INFO -> Kernel] >> Checking system ram amount...");
                if(Cosmos.Core.CPU.GetAmountOfRAM() + 2 < 64)
                {
                    Console.WriteLine("[ERROR -> Kernel] >> INSUFFICIENT RAM!");
                    KernelPanic("INSUFFICIENT MEMORY", "The system does not have enough ram to run XenOS!\nAt least 64 megabytes of ram is required, but you only have " + (Cosmos.Core.CPU.GetAmountOfRAM() + 2) + " megabytes.");
                    
                }

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
            if (Drivers.AudioEnabled)
                Drivers.audioManager.Disable();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.WriteLine("[================================ KERNEL PANIC ================================]");
            Console.WriteLine(Shell.OsName + " encountered an unrecoverable error!");
            Console.WriteLine("EXCEPTION: " + exception + "\n" + "MESSAGE: " + msg + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            Console.CursorVisible = false;
            Console.Write("Please restart your computer using the power button.");
            foreach (var device in Cosmos.HAL.PCI.Devices)
            {
                if (device.VendorID == 5549 || device.VendorID == 8384 || device.VendorID == 32902 || device.VendorID == 4203 || device.VendorID == 4660)
                {
                    Console.Beep(1000, 500);
                    Console.Beep(750, 500);
                    break;
                }
            }
                    
            while (true)
            {
                Cosmos.Core.CPU.Halt();
            }
            /*while (true)
            {
                for(int i = 0; i < 10; i++)
                {
                    Console.Write("System will attempt to reboot in " + (10 - i) + " second(s) \r");
                    Thread.Sleep(1000);
                }
                Power power = new Power();
                power.reboot();
            }*/
        }
    }
}
