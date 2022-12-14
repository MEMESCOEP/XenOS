using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using Cosmos.HAL.Audio;
using Cosmos.HAL.Drivers.PCI.Audio;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;

namespace XenOS.Code.Sys.Drivers
{
    internal class Drivers
    {
        // Variables
        public static Cosmos.System.FileSystem.CosmosVFS vfs;
        public static AudioManager audioManager;
        public static AudioDriver driver;
        public static AudioMixer mixer;
        public static SeekableAudioStream audioStream;
        public static bool AudioEnabled = false;
        static DHCPClient xClient;
        public static Cosmos.HAL.PIT.PITTimer Timer;
        public static bool PITTicked = false;

        // Functions
        public static void Serial()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INFO -> Drivers:Serial] >> Initializing serial on COM1...");
            Cosmos.HAL.Debug.Serial.Enable();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[INFO -> DRIVERS:Serial] >> Serial initialized.");
        }

        public static void Audio()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INFO -> DRIVERS:Audio] >> Initializing audio driver...");

            try
            {
                driver = AC97.Initialize(8192);
                mixer = new AudioMixer();
                AudioEnabled = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[INFO -> DRIVERS:Audio] >> Audio driver initialized.");
            }
            catch (InvalidOperationException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR -> DRIVERS:Audio] >> No AC97 device found.");
            }
            catch (Exception EX)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR -> DRIVERS:Audio] >> " + EX.Message);
            }
        }

        public static void Filesystem()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO -> Drivers:Filesystem] >> Initializing virtual filesystem...");
                vfs = new Cosmos.System.FileSystem.CosmosVFS();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[INFO -> DRIVERS:Filesystem] >> Virtual filesystem initialized.");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO -> Drivers:Filesystem] >> Registering virtual filesystem...");
                Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(vfs);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[INFO -> DRIVERS:Filesystem] >> Virtual filesystem registered.");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var disk in Cosmos.System.FileSystem.VFS.VFSManager.GetDisks())
                {
                    disk.DisplayInformation();
                    Console.WriteLine("[INFO -> Drivers:Filesystem] >> Mounting all partitions on disk {0}...", Cosmos.System.FileSystem.VFS.VFSManager.GetDisks().IndexOf(disk));
                    disk.Mount();
                    Console.WriteLine();
                }
            }
            catch (Exception EX)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR -> Drivers:Filesystem] >> " + EX.Message);
            }
        }

        public static void PITTimer()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INFO -> DRIVERS:PIT_Timer] >> Initializing PIT Timer...");
            Timer = new Cosmos.HAL.PIT.PITTimer(TimerCallback, 250000000, true);
            Cosmos.HAL.Global.PIT.RegisterTimer(Timer);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[INFO -> DRIVERS:PIT_Timer] >> PIT Timer initialized.");
        }

        public static void CreateNewPITTimer(ulong freq, Action callback, bool IsReccuring)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INFO -> DRIVERS:PIT_Timer] >> Initializing new PIT Timer...");
            Timer = new Cosmos.HAL.PIT.PITTimer(callback, freq, IsReccuring);
            Cosmos.HAL.Global.PIT.RegisterTimer(Timer);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[INFO -> DRIVERS:PIT_Timer] >> PIT Timer initialized.");
        }

        public static void TimerCallback()
        {
            PITTicked = true;
        }

        public static void ConsoleSize()
        {
            Console.WriteLine("[INFO -> Drivers:ConsoleSize] >> Changing console size...");
            try
            {
                Console.WindowHeight = 50;
                Console.WindowWidth = 80;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR -> Drivers:ConsoleSize] >> " + ex.Message);
            }
        }

        public static void Network_DHCP()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO -> Drivers:NET] >> Attempting DHCP autoconfig...");
                try
                {
                    if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                    {
                        throw new Exception("There are no usable network devices installed in the system!");
                    }

                    Console.WriteLine("[INFO -> Network:DHCP] >> Creating new DHCP client...");
                    xClient = new DHCPClient();
                    Console.WriteLine("[INFO -> Network:DHCP] >> Sending DHCP discover packet...");
                    xClient.SendDiscoverPacket();
                    Console.WriteLine("[INFO -> Network:DHCP] >> Retrieving local IP address...");
                    var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                    Console.WriteLine("[INFO -> Network:DHCP] >> Closing DHCP client...");
                    xClient.Close();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[INFO -> Network:DHCP] >> Etablished Network connection via DHCP.\nIPv4 Address: " + ip, 2);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR -> Network:DHCP] >> DHCP Discover failed.\nDetails: " + ex.Message);
                }
            }
            catch (Exception EX)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR -> Drivers:NET] >> " + EX.Message);
            }
        }

        public static void shutdown()
        {
            try
            {
                Console.WriteLine("[INFO -> Drivers] >> Closing network connections...");
                xClient.Close();

                Console.WriteLine("[INFO -> Drivers] >> Disposing DHCP client...");
                xClient.Dispose();

                if (AudioEnabled)
                {
                    Console.WriteLine("[INFO -> Drivers] >> Disabling audio driver...");
                    driver.Disable();
                }

                Console.WriteLine("Done.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("[ERROR -> Drivers] >> " + ex.Message);
            }
        }
    }
}
