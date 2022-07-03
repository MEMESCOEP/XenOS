using System;

namespace XenOS
{
    internal class Shell
    {
        // Variables
        public static string OsName = "XenOS";
        public static string Version = "Alpha 070322_1250A";
        public static int ScreenWidth = 800;
        public static int ScreenHeight = 600;

        // Functions
        public void init()
        {
            Console.WriteLine("[INFO -> Shell] >> Shell loaded.");
            DriverSetup();
            Console.WriteLine("[INFO -> Shell] >> Loading console...");
            CustomConsole customConsole = new CustomConsole();
            customConsole.CMD();
        }

        public void DriverSetup()
        {
            Console.WriteLine("[INFO -> Shell:DriverSetup] >> Loading drivers...");
            Drivers drivers = new Drivers();
            drivers.Filesystem();
            drivers.Network_DHCP();
            Console.WriteLine("[INFO -> Shell:DriverSetup] >> Driver tasks finished.");
        }
    }
}
