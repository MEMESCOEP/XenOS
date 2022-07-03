using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;

namespace XenOS
{
    internal class UrlToIP
    {
        public void ConvertToIP(string url)
        {
            Console.WriteLine("Getting ip address of " + url + "...");
            try
            {
                if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                {
                    throw new Exception("You're not connected to the internet!");
                }
                using (var xClient = new DnsClient())
                {
                    xClient.Connect(new Address(8, 8, 8, 8)); //DNS Server address

                    /** Send DNS ask for a single domain name **/
                    xClient.SendAsk(url);

                    /** Receive DNS Response **/
                    Address destination = xClient.Receive(); //can set a timeout value
                    Console.WriteLine(url + "'s ip address is: " + destination.ToString());
                }
            }
            catch(Exception EX)
            {
                Console.WriteLine("ERROR: " + EX.Message);
            }
        }
    }
}
