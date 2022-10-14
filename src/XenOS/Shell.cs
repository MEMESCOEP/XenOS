﻿using System;

namespace XenOS
{
    internal class Shell
    {
        // Variables
        public static API auraApi;
        public static string username = "root";
        public static string OsName = "XenOS";
        public static string Version = "Alpha 083022_1005P";
        public static string Logo = @"|\  \  /  /|\  ___ \ |\   ___  \|\   __  \|\   ____\     
\ \  \/  / | \   __/|\ \  \\ \  \ \  \|\  \ \  \___|_    
 \ \    / / \ \  \_|/_\ \  \\ \  \ \  \\\  \ \_____  \   
  /     \/   \ \  \_|\ \ \  \\ \  \ \  \\\  \|____|\  \  
 /  /\   \    \ \_______\ \__\\ \__\ \_______\____\_\  \ 
/__/ /\ __\    \|_______|\|__| \|__|\|_______|\_________\
|__|/ \|__|                                  \|_________|";
        public static int ScreenWidth = 1360;
        public static int ScreenHeight = 768;
        public static ConsoleColor TextColor = ConsoleColor.White;

        // Functions
        public void init()
        {
            Console.WriteLine("[INFO -> Shell] >> Shell loaded.");
            DriverSetup();
            Console.ForegroundColor = Shell.TextColor;
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