using System;
using System.Text;
using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;

namespace XenOS.Code.Network
{
    internal class Network
    {
        // Functions
        public bool DCHPConnect()
        {
            var xClient = new DHCPClient();

            try
            {
                Console.WriteLine("[INFO -> Network:DHCP] >> Initiating Network connection via DHCP...", 1);
                xClient.SendDiscoverPacket();
                var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                xClient.Close();
                Console.WriteLine("[INFO -> Network:DHCP] >> Etablished Network connection via DHCP. IPv4: " + ip, 2);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[INFO -> Network:DHCP] >> DHCP Discover failed. Can't apply dynamic IPv4 address. " + ex);
                return false;
            }
        }

        public void ManualConnect(Address ipAddress, Address subnet, Address gateway, string networkDevice = "eth0")
        {
            try
            {
                var nic = NetworkDevice.GetDeviceByName(networkDevice);

                IPConfig.Enable(nic, ipAddress, subnet, gateway);

                Address ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                Address sn = NetworkConfiguration.CurrentNetworkConfig.IPConfig.SubnetMask;
                Address gw = NetworkConfiguration.CurrentNetworkConfig.IPConfig.DefaultGateway;

                Console.WriteLine($"[INFO -> Network:NET_MANUAL] >> Applied! IPv4: {ip} subnet mask: {sn} gateway: {gw}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR -> Network:NET_MANUAL] >> ERROR: " + ex.Message);
            }
        }

        /// <summary>
        ///     Create a TCP connection to an IP.
        /// </summary>
        /// <param name="destip">Destination IP</param>
        /// <param name="data">Data to send to destip</param>
        /// <param name="destport">Destination Port</param>
        /// <param name="localport">Local port to open TCP connection to</param>
        /// <param name="timeout">Timeout</param>
        public byte[] TCPconnect(Address destip, int destport, int localport, string data, int timeout = 80)
        {
            using var xClient = new TcpClient(localport);
            xClient.Connect(destip, destport, timeout);

            xClient.Send(Encoding.ASCII.GetBytes(data));

            var endpoint = new EndPoint(Address.Zero, 0);
            var recvData = xClient.Receive(ref endpoint); //set endpoint to remote machine IP:port
            var finalData = xClient.NonBlockingReceive(ref endpoint); //retrieve receive buffer without waiting

            xClient.Close();

            Console.WriteLine($"[INFO -> Network:DHCP] >> " + endpoint.ToString());
            Console.WriteLine($"[INFO -> Network:DHCP] >> " + recvData.ToString());
            Console.WriteLine($"[INFO -> Network:DHCP] >> " + finalData.ToString());

            return finalData;
        }

        /// <summary>
        ///     Create a UDP connection to an IP.
        /// </summary>
        /// <param name="destip">Destination IP</param>
        /// <param name="data">Data to send to destip</param>
        /// <param name="destport">Destination Port</param>
        /// <param name="localport">Local port to open TCP connection to</param>
        /// <param name="timeout">Timeout</param>
        public byte[] UDPconnect(Address destip, int destport, int localport, string data, int timeout = 80)
        {
            using var xClient = new TcpClient(localport);
            xClient.Connect(destip, destport, timeout);

            xClient.Send(Encoding.ASCII.GetBytes(data));

            var endpoint = new EndPoint(Address.Zero, 0);
            var recvData = xClient.Receive(ref endpoint); //set endpoint to remote machine IP:port
            var data2 = xClient.NonBlockingReceive(ref endpoint); //retrieve receive buffer without waiting

            return recvData;
        }
    }
}
