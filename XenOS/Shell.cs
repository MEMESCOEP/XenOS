using System;

namespace XenOS
{
    internal class Shell
    {
        // Variables
        public static API auraApi;
        public static string username = "root";
        public static string OsName = "XenOS";
        public static string Version = "Alpha 073122_845P";
        public static string Logo = @"|\  \  /  /|\  ___ \ |\   ___  \|\   __  \|\   ____\     
\ \  \/  / | \   __/|\ \  \\ \  \ \  \|\  \ \  \___|_    
 \ \    / / \ \  \_|/_\ \  \\ \  \ \  \\\  \ \_____  \   
  /     \/   \ \  \_|\ \ \  \\ \  \ \  \\\  \|____|\  \  
 /  /\   \    \ \_______\ \__\\ \__\ \_______\____\_\  \ 
/__/ /\ __\    \|_______|\|__| \|__|\|_______|\_________\
|__|/ \|__|                                  \|_________|";
        public static int ScreenWidth = 800;
        public static int ScreenHeight = 600;

        // Functions
        public void init()
        {
            Console.WriteLine("[INFO -> Shell] >> Shell loaded.");
            DriverSetup();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[INFO -> Shell] >> Loading console...");
            CustomConsole customConsole = new CustomConsole();
            customConsole.CMD();
        }

        public void DriverSetup()
        {
            Console.WriteLine("[INFO -> Shell:DriverSetup] >> Loading drivers...");
            Drivers drivers = new Drivers();
            drivers.Filesystem();
            drivers.Audio();
            drivers.Network_DHCP();
            Console.WriteLine("[INFO -> Shell:DriverSetup] >> Driver tasks finished.");
        }

        public void AuraAPISetup()
        {
            Console.WriteLine("[INFO -> Shell:AuraAPISetup] >> Initializing the Aura API...");
            auraApi = new API();
            auraApi.Initialize();
            Console.WriteLine("[INFO -> Shell:AuraAPISetup] >> Aura API initialized.");
        }
    }
}
