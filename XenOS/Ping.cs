using Cosmos.System.Network.IPv4;
using System;

namespace XenOS
{
    internal class Ping
    {
        public void PingIP(string IP)
        {
            float successful = 0;
            if(Cosmos.HAL.NetworkDevice.Devices.Count > 0)
            {
                try
                {
                    Console.WriteLine("Pinging \"" + IP + "\"...");
                    EndPoint endPoint = new EndPoint(Address.Zero, 0); ;
                    using (var xClient = new ICMPClient())
                    {
                        xClient.Connect(Address.Parse(IP));

                        for (int i = 0; i < 4; i++)
                        {
                            /** Send ICMP Echo **/
                            xClient.SendEcho();

                            /** Receive ICMP Response **/
                            int time = xClient.Receive(ref endPoint); //return elapsed time / timeout if no response
                            if (time >= 0)
                            {
                                Console.WriteLine("Response recieved in " + time + " millisecond(s)");
                                successful++;
                            }
                            else
                            {
                                Console.WriteLine("Ping failed.");
                            }
                        }
                    }
                    Console.WriteLine("Success rate: " + (successful / 4) * 100 + " percent. (" + successful + "/4)");
                }
                catch
                {

                }
            }
            else
            {
                Console.WriteLine("There aren't any usable network devices installed!");
            }
        }
    }
}
