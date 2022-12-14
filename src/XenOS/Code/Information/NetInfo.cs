/* Directives */
using Cosmos.System.Network.Config;
using System;

/* Namespaces */
namespace XenOS.Code.Information
{
    /* Classes */
    internal class NetInfo
    {
        /* Functions */
        // Display the current IP address
        public static void DisplayIPAddress()
        {
            try
            {
                // Make sure there are usable network devices
                if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                {
                    throw new Exception("There are no usable network devices installed in the system!");
                }
                var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                Console.WriteLine("IPv4 Address: " + ip);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
