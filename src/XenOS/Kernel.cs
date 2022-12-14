/* Directives */
using System;
using Cosmos.Core;
using Cosmos.Debug.Kernel;
using Sys = Cosmos.System;
using XenOS.Code.Sys.Boot;
using XenOS.Code.Sys.Drivers;
using Console = System.Console;
using System.Net.Http;

/* Namespaces */
namespace XenOS
{
    /* Classes */
    public class Kernel : Sys.Kernel
    {
        /* Variables */
        public static string KernelVersion = "10.4";
        public static Debugger DEBUGGER;
        public static string KeyboardBuffer;
        public static INTs.IRQDelegate IRQHandler;
        public static bool DEBUG = true;

        /* Functions */
        protected override void BeforeRun()
        {
            try
            {
                Console.WriteLine("[INFO -> Kernel] >> XenOS Kernel Version {0} loaded.", KernelVersion);

                // Create a new debugger if the DEBUG boolean is set to true
                if(DEBUG)
                    DEBUGGER = new Debugger("User", "Kernel");

                // Make sure the computer has enough RAM to run XenOS
                Console.WriteLine("[INFO -> Kernel] >> Checking system ram amount...");
                if(CPU.GetAmountOfRAM() + 2 < 64)
                {
                    // The computer doesn't have enough RAM to run XenOS, so panic and halt
                    Console.WriteLine("[ERROR -> Kernel] >> INSUFFICIENT RAM!");
                    KernelPanic("INSUFFICIENT MEMORY", "The system does not have enough ram to run XenOS!\nAt least 64 megabytes of ram is required, but you only have " + (Cosmos.Core.CPU.GetAmountOfRAM() + 2) + " megabytes.");
                }

                //INTs.SetIrqHandler(0x09, HandlePowerButton);

                // Load the shell
                Console.WriteLine("[INFO -> Kernel] >> Loading shell...");
                Shell shell = new Shell();
                if(DEBUG)
                    DEBUGGER.Send("KERNEL TASKS DONE.");

                shell.init();
            }
            catch(Exception ex)
            {
                // An error occured, so panic and halt
                KernelPanic(ex.Message, ex.Message);        
            }
        }

        protected override void Run()
        {
            // If the shell didn't load properly, panic and halt
            KernelPanic("UNKNOWN ERROR", "UNKNOWN ERROR");
        }

        public void HandlePowerButton(ref INTs.IRQContext aContext)
        {
            Console.WriteLine(aContext.Interrupt);
        }

        // Print an error message and halt the system
        public static void KernelPanic(string exception, string msg)
        {
            try
            {
                // Print the error message
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Clear();
                Console.WriteLine("[================================ KERNEL PANIC ================================]");
                Console.WriteLine("A SERIOUS ERROR HAS OCCURED!\n\n\n[== CRASH DETAILS ==]");
                Console.WriteLine($"EXCEPTION: {exception}\nMESSAGE: {msg}");
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write("[Please restart your computer]");
                Console.CursorVisible = false;

                // If debugging is enabled, send a message
                if (DEBUG)
                    DEBUGGER.Send("KERNEL PANIC! EX: " + exception + " || MSG: " + msg);

                // If we are in a virtual machine, play an error chime through the PC speaker
                if (Sys.VMTools.IsVMWare || Sys.VMTools.IsVirtualBox || Sys.VMTools.IsQEMU)
                {
                    Sys.PCSpeaker.Beep(1000, 500);
                    Sys.PCSpeaker.Beep(750, 500);
                }

                // If audio is enabled, disable it
                if (Drivers.AudioEnabled)
                    Drivers.audioManager.Disable();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }

            // Halt the system
            while (true) ;
        }
    }
}
