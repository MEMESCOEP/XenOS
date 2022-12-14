using Cosmos.System.Network.Config;
using System;

namespace XenOS.Code.Commands.Info
{
    internal class Sysinfo
    {
        public static void DisplayCPUInfo()
        {
            Console.WriteLine("CPU Vendor: " + Cosmos.Core.CPU.GetCPUVendorName());
            Console.WriteLine("CPU EBP: " + Cosmos.Core.CPU.GetEBPValue());
            Console.WriteLine("CPU Brand: " + Cosmos.Core.CPU.GetCPUBrandString());
            try
            {
                var cpu_speed = Cosmos.Core.CPU.GetCPUCycleSpeed() / (1024 * 1024);
                Console.WriteLine("CPU Speed: " + cpu_speed + " MHz");
            }
            catch
            {
                try
                {
                    var cpu_speed = Cosmos.Core.CPU.EstimateCPUSpeedFromName(Cosmos.Core.CPU.GetCPUBrandString()) / (1024 * 1024);
                    Console.WriteLine("CPU Speed: " + cpu_speed + " MHz");
                }
                catch
                {
                    Console.WriteLine("CPU Speed: [Failure getting CPU speed]");
                }
            }
            Console.WriteLine("CPU Uptime: " + Cosmos.Core.CPU.GetCPUUptime());
        }

        public static void DisplaySystemInformation()
        {
            Console.WriteLine("[===== SYSTEM INFORMATION =====]");
            try
            {
                Console.WriteLine("CPU: " + Cosmos.Core.CPU.GetCPUBrandString().ToString());
                Console.WriteLine("CPU Vendor: " + Cosmos.Core.CPU.GetCPUVendorName());
                try
                {
                    var cpu_speed = Cosmos.Core.CPU.GetCPUCycleSpeed() / (1024 * 1024);
                    Console.WriteLine("CPU Speed: " + cpu_speed + " MHz");
                }
                catch
                {
                    try
                    {
                        var cpu_speed = Cosmos.Core.CPU.EstimateCPUSpeedFromName(Cosmos.Core.CPU.GetCPUBrandString()) / (1024 * 1024);
                        Console.WriteLine("CPU Speed: " + cpu_speed + " MHz");
                    }
                    catch
                    {
                        Console.WriteLine("CPU Speed: [Failure getting CPU speed]");
                    }
                }
                Console.WriteLine("CPU Uptime: " + Cosmos.Core.CPU.GetCPUUptime());
                Console.WriteLine("Total installed memory: " + Cosmos.Core.CPU.GetAmountOfRAM() + " MB");
                Console.WriteLine("Used memory: " + (Cosmos.Core.CPU.GetAmountOfRAM() - Cosmos.Core.GCImplementation.GetAvailableRAM()) + " MB");
                Console.WriteLine("Available memory: " + Cosmos.Core.GCImplementation.GetAvailableRAM() + " MB");
                try
                {
                    if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                    {
                        Console.WriteLine("IPv4 Address: [No usable network devices!]");
                    }
                    else
                    {
                        var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                        Console.WriteLine("IPv4 Address: " + ip);
                    }
                }
                catch
                {

                }
                Console.WriteLine();
            }
            catch
            {

            }
        }
    }
}
