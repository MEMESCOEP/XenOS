using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenOS.Code.Sys.Drivers;

namespace XenOS.Code.Commands.Info
{
    internal class DriveInformation
    {
        public static void ListDrives()
        {
            foreach (var disk in DriveInfo.GetDrives())
            {
                if (disk.DriveType == DriveType.CDRom)
                {
                    Console.WriteLine("CD-ROM          : " + disk.Name);
                }
                else if (disk.DriveType == DriveType.Removable)
                {
                    Console.WriteLine("Removable Disk  : " + disk.Name);
                }
                else if (disk.DriveType == DriveType.Unknown)
                {
                    Console.WriteLine("Unknown Disk    : " + disk.Name);
                }
                else if (disk.DriveType == DriveType.Ram)
                {
                    Console.WriteLine("RAM Disk        : " + disk.Name);
                }
                else if (disk.DriveType == DriveType.Fixed)
                {
                    Console.WriteLine("Fixed Disk      : " + disk.Name);
                }
                else if (disk.DriveType == DriveType.Network)
                {
                    Console.WriteLine("Network Disk    : " + disk.Name);
                }
                Console.WriteLine("Volume label    : " + disk.VolumeLabel);
                Console.WriteLine("Size            : " + disk.TotalSize);
                Console.WriteLine("Total free space: " + disk.TotalFreeSpace);
                Console.WriteLine("Available space : " + disk.AvailableFreeSpace);
                Console.WriteLine("Root directory  : " + disk.RootDirectory);
                Console.WriteLine("Format          : " + disk.DriveFormat);
                Console.WriteLine("Is Ready        : " + disk.IsReady.ToString());
                Console.WriteLine();
            }
        }

        public static void ListVolumes()
        {
            foreach (var disk in Drivers.vfs.GetVolumes())
            {
                Console.WriteLine(disk.mFullPath);
            }
        }
    }
}
