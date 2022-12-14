using System;

namespace XenOS.Code.Sys.Boot
{
    internal class Shell
    {
        /* Variables */
        public static string username = "xenos";
        public static string OsName = "XenOS";
        public static string Version = "Alpha 12-14-22_10:25A";
        public static string CWD = "0:\\";
        public static string Logo = @"|\  \  /  /|\  ___ \ |\   ___  \|\   __  \|\   ____\     
\ \  \/  / | \   __/|\ \  \\ \  \ \  \|\  \ \  \___|_    
 \ \    / / \ \  \_|/_\ \  \\ \  \ \  \\\  \ \_____  \   
  /     \/   \ \  \_|\ \ \  \\ \  \ \  \\\  \|____|\  \  
 /  /\   \    \ \_______\ \__\\ \__\ \_______\____\_\  \ 
/__/ /\ __\    \|_______|\|__| \|__|\|_______|\_________\
|__|/ \|__|                                  \|_________|";
        public static int ScreenWidth = 800;
        public static int ScreenHeight = 600;
        public static int MouseSensitivity = 1;
        public static ConsoleColor TextColor = ConsoleColor.White;

        /* Functions */
        // Initialize the shell
        public void init()
        {
            // Initialize drivers
            Console.WriteLine("[INFO -> Shell] >> Shell loaded.");
            DriverSetup();
            Console.ForegroundColor = TextColor;

            // Initialize the console
            Console.WriteLine("[INFO -> Shell] >> Loading console...");
            CustomConsole.CMD();
            Console.WriteLine("[INFO -> Shell] >> Done.");
        }

        // Set up devices and interfaces
        public void DriverSetup()
        {
            Console.WriteLine("[INFO -> Shell:DriverSetup] >> Loading drivers...");
            // Set up the filesystem
            Drivers.Drivers.Filesystem();

            // Set up audio
            Drivers.Drivers.Audio();

            // Set up serial
            Drivers.Drivers.Serial();

            // Set up networking
            Drivers.Drivers.Network_DHCP();

            // Set up the PIT timer
            Drivers.Drivers.PITTimer();
            Console.WriteLine("[INFO -> Shell:DriverSetup] >> Driver tasks finished.");
        }
    }
}
