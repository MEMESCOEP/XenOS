using Cosmos.System.FileSystem;
using System;
using System.IO;
using XenOS.Code.Filesystem;
using XenOS.Code.Sys.Boot;
using XenOS.Code.Sys.Drivers;
using XSharp.Assembler.x86;

namespace XenOS.Code.Commands.Filesystem
{
    internal class FileSystem
    {
        public static void DirectoryListing()
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(Shell.CWD))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[DIR]\t ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(dir);
                }
                foreach (var filename in Directory.GetFiles(Shell.CWD))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[FILE]\t");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(filename);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        public static void ChangeDirectory(string path)
        {
            try
            {
                if (Directory.Exists(Path.Combine(Shell.CWD, path)))
                {
                    Shell.CWD = Path.Combine(Shell.CWD, path);
                    Directory.SetCurrentDirectory(Shell.CWD);
                }
                else
                {
                    Console.WriteLine("Directory \"" + path + "\" doesn't exist!");
                }
            }
            catch
            {

            }
        }

        public static void ShowFileContents(string path)
        {
            try
            {
                if (File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    var data = File.ReadAllText(Path.Combine(Shell.CWD, path));
                    Console.WriteLine(data);
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }
            catch
            {

            }
        }

        public static void MakeDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(Shell.CWD, path)))
                {
                    Directory.CreateDirectory(Path.Combine(Shell.CWD, path));
                }
                else
                {
                    Console.WriteLine("Directory \"" + path + "\" already exists!");
                }
            }
            catch
            {

            }
        }

        public static void MakeFile(string path)
        {
            try
            {
                if (!File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    File.Create(Path.Combine(Shell.CWD, path));
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" already exists!");
                }
            }
            catch
            {

            }
        }

        public static void RemoveFile(string path)
        {
            try
            {
                if (File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    File.Delete(Path.Combine(Shell.CWD, path));
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }
            catch
            {

            }
        }

        public static void RemoveDirectory(string path)
        {
            try
            {
                if (Directory.Exists(Path.Combine(Shell.CWD, path)))
                {
                    if (Shell.CWD != Path.Combine(Shell.CWD, path))
                    {
                        if (Directory.GetFiles(Path.Combine(Shell.CWD, path)).Length > 0)
                        {
                            foreach (var file in Directory.GetFiles(Path.Combine(Shell.CWD, path)))
                            {
                                Console.WriteLine("Deleting file: " + file);
                                File.Delete(file);
                            }
                            foreach (var dir in Directory.GetDirectories(Path.Combine(Shell.CWD, path)))
                            {
                                Console.WriteLine("Deleting file: " + dir);
                                Directory.Delete(dir);
                            }
                        }
                        Directory.Delete(Path.Combine(Shell.CWD, path));
                    }
                    else
                    {
                        Console.WriteLine("Cannot delete the directory because it's the current working directory!");
                    }
                }
                else
                {
                    Console.WriteLine("Directory \"" + path + "\" doesn't exist!");
                }
            }
            catch
            {

            }
        }

        public static void EditFile(string path)
        {
            try
            {
                if (File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    TextEdit editor = new TextEdit();
                    editor.StartMIV(Path.Combine(Shell.CWD, path));
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }
            catch
            {

            }
        }

        public static void MoveFile(string path, string dest)
        {
            try
            {
                dest = Path.Combine(dest, Path.GetFileName(path));
                Console.WriteLine("Moving file \"" + path + "\" to \"" + Path.Combine(Path.GetDirectoryName(path), dest) + "\"");
                if (File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    if (!File.Exists(dest))
                    {
                        File.Copy(Path.Combine(Shell.CWD, path), dest);
                        File.Delete(Path.Combine(Shell.CWD, path));
                    }
                    else
                    {
                        Console.WriteLine("File \"" + path + "\" already exists!");
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        if (!File.Exists(dest))
                        {
                            File.Copy(path, dest);
                            File.Delete(path);
                        }
                        else
                        {
                            Console.WriteLine("File \"" + path + "\" already exists!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("File \"" + path + "\" doesn't exist!");
                    }
                }
            }
            catch (Exception EX)
            {
                Console.WriteLine("ERROR: " + EX.Message);
            }
        }

        public static void CopyFile(string path, string dest)
        {
            try
            {                
                dest = Path.Combine(dest, Path.GetFileName(path));
                Console.WriteLine("Copying file \"" + path + "\" to \"" + Path.Combine(Path.GetDirectoryName(path), dest) + "\"");
                if (File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    if (!File.Exists(dest))
                    {
                        File.Copy(Path.Combine(Shell.CWD, path), dest);
                    }
                    else
                    {
                        Console.WriteLine("File \"" + path + "\" already exists!");
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        if (!File.Exists(dest))
                        {
                            File.Copy(path, dest);
                        }
                        else
                        {
                            Console.WriteLine("File \"" + path + "\" already exists!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("File \"" + path + "\" doesn't exist!");
                    }
                }
            }
            catch (Exception EX)
            {
                Console.WriteLine("ERROR: " + EX.Message);
            }
        }

        public static void AppendToFile(string path, string txt)
        {
            if (File.Exists(path))
            {
                try
                {
                    string filedata = File.ReadAllText(path);
                    txt = txt.Replace("\\n", Environment.NewLine);
                    filedata += txt;
                    File.WriteAllText(path, filedata);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("File " + path + " doesn't exist!");
            }
        }

        public static void WriteToFile(string path, string txt)
        {
            if (File.Exists(path))
            {
                try
                {
                    txt = txt.Replace("\\n", Environment.NewLine);
                    File.WriteAllText(path, txt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("File " + path + " doesn't exist!");
            }
        }

        public static void FormatDrive(string name)
        {
            try
            {                
                int index = 0;
                bool DriveExists = false;
                foreach (var disk in Drivers.vfs.GetDisks())
                {
                    foreach (var partition in disk.Partitions)
                    {
                        if (partition.RootPath == name)
                        {
                            DriveExists = true;
                            Console.WriteLine("Formatting partition {0} of disk {1}...", index, name);
                            disk.FormatPartition(0, "FAT32", true);
                            break;
                        }
                    }
                }

                if (!DriveExists)
                {
                    Console.WriteLine($"The drive \"{name}\" doesn't exist!");
                }
            }
            catch
            {

            }
        }

        public static void MakePartition(string disknum, int partsize)
        {
            foreach (var disk in Cosmos.System.FileSystem.VFS.VFSManager.GetDisks())
            {
                foreach (var partition in disk.Partitions)
                {
                    if (partition.RootPath == disknum)
                    {
                        Console.WriteLine("Creating new partition on disk {0} with size {1}...", disknum, partsize);
                        disk.CreatePartition(partsize);
                        disk.FormatPartition(disk.Partitions.Count - 1, "FAT32", true);
                        disk.MountPartition(disk.Partitions.Count - 1);
                    }
                }
            }
        }

        public static void ListPartitions(string DriveLetter)
        {
            foreach (var disk in Drivers.vfs.GetDisks())
            {
                foreach (var partition in disk.Partitions)
                {
                    if (partition.RootPath == DriveLetter)
                    {
                        Console.WriteLine("Root Path: " + partition.RootPath);
                        Console.WriteLine("Has FS: " + partition.HasFileSystem);
                        Console.WriteLine("Label: " + partition.MountedFS.Label);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
