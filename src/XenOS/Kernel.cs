/* Directives */
using System;
using Cosmos.Core;
using Cosmos.Debug.Kernel;
using Sys = Cosmos.System;

/* Namespaces */
namespace XenOS
{
    /* Classes */
    public class Kernel : Sys.Kernel
    {
        /* Variables */
        public static string KernelVersion = "10.0";
        public static Debugger DEBUGGER = new Debugger("User", "Kernel");
        public static string KeyboardBuffer;

        /* Functions */
        public override void Start()
        {
            base.Start();
        }

        protected override void BeforeRun()
        {
            try
            {
                Console.WriteLine("[INFO -> Kernel] >> XenOS Kernel Version {0} loaded.", KernelVersion);
                Console.WriteLine("[INFO -> Kernel] >> Checking system ram amount...");
                if(CPU.GetAmountOfRAM() + 2 < 64)
                {
                    Console.WriteLine("[ERROR -> Kernel] >> INSUFFICIENT RAM!");
                    KernelPanic("INSUFFICIENT MEMORY", "The system does not have enough ram to run XenOS!\nAt least 64 megabytes of ram is required, but you only have " + (Cosmos.Core.CPU.GetAmountOfRAM() + 2) + " megabytes.");
                }

                GCImplementation.Init();

                Console.WriteLine("[INFO -> Kernel] >> Loading shell...");
                Shell shell = new Shell();
                DEBUGGER.SendMessageBox("KERNEL TASKS DONE.");
                shell.init();
            }
            catch(Exception ex)
            {
                KernelPanic(ex.Message, ex.Message);        
            }
        }

        protected override void Run()
        {
            Console.WriteLine("An error has occured. Please reboot your computer.");
            while (true) ;
        }

        public static void KernelPanic(string exception, string msg)
        {
            if (Drivers.AudioEnabled)
                Drivers.audioManager.Disable();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.WriteLine("[================================ KERNEL PANIC ================================]");
            Console.WriteLine(Shell.OsName + " encountered a serious error!");
            Console.WriteLine("EXCEPTION: " + exception + "\n" + "MESSAGE: " + msg + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            Console.CursorVisible = false;
            Console.Write("Please restart your computer.");
            foreach (var device in Cosmos.HAL.PCI.Devices)
            {
                if (Sys.VMTools.IsVMWare || Sys.VMTools.IsVirtualBox || Sys.VMTools.IsQEMU)
                {
                    Console.Beep(1000, 500);
                    Console.Beep(750, 500);
                    break;
                }
            }

            while (true) ;
        }
    }
}
