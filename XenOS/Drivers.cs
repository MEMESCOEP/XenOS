using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using System.Threading;

namespace XenOS
{
    internal class Drivers
    {
        // Variables
        public static Cosmos.System.FileSystem.CosmosVFS vfs;

        // Functions
        public void Filesystem()
        {
            try
            {
                Console.WriteLine("[INFO -> Drivers:Filesystem] >> Initializing virtual filesystem...");
                vfs = new Cosmos.System.FileSystem.CosmosVFS();
                Console.WriteLine("[INFO -> Drivers:Filesystem] >> Registering virtual filesystem...");
                Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(vfs);
            }
            catch(Exception EX)
            {
                Console.WriteLine("[ERROR -> Drivers:Filesystem] >> " + EX.Message);
            }
        }

        public void ConsoleSize()
        {
            Console.WriteLine("[INFO -> Drivers:ConsoleSize] >> Changing console size...");
            try
            {
                Console.WindowHeight = 50;
                Console.WindowWidth = 80;
            }
            catch(Exception ex)
            {
                Console.WriteLine("[ERROR -> Drivers:ConsoleSize] >> " + ex.Message);
                Thread.Sleep(1000);
            }
        }

        public void Network_DHCP()
        {
            try
            {
                Console.WriteLine("[INFO -> Drivers:NET] >> Attempting DHCP autoconfig...");
                try
                {
                    if(Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                    {
                        throw new Exception("There are no usable network devices installed in the system!");
                    }

                    var xClient = new DHCPClient();
                    xClient.SendDiscoverPacket();
                    var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                    xClient.Close();
                    Console.WriteLine("[INFO -> Network:DHCP] >> Etablished Network connection via DHCP.\nIPv4 Address: " + ip, 2);
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[INFO -> Network:DHCP] >> DHCP Discover failed.\nDetails: " + ex.Message);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception EX)
            {
                Console.WriteLine("[ERROR -> Drivers:NET] >> " + EX.Message);
            }
        }
    }
}
