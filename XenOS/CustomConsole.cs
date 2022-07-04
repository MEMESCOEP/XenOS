using Cosmos.System.Network.Config;
using System;
using System.IO;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics;
using System.Threading;

namespace XenOS
{
    internal class CustomConsole
    {
        // Suppression statements
        #pragma warning disable IDE0044 // Add readonly modifier
        #pragma warning disable CA1416 // Validate platform compatibility
        #pragma warning disable CS0642 // Possible mistaken empty statement

        // Resource files
        [ManifestResourceStream(ResourceName = "XenOS.isoFiles.he_he_he_ha.wav")]
        static byte[] test_wav;

        // Variables
        public string CWD = "0:\\";
        public bool KeepCMDOpen = true;

        // Functions
        public void CMD()
        {
            Console.WriteLine("[INFO -> Console] >> Console loaded.");
            Console.WriteLine("[INFO -> Console] >> Writing test audio file...");
            if (!File.Exists("0:\\heheheha.wav"))
            {
                try
                {
                    File.WriteAllBytes("0:\\heheheha.wav", test_wav);
                }
                catch
                {

                }
            }

            if (File.Exists(Path.Combine(CWD, "autoexec")))
            {
                foreach (var line in File.ReadLines(Path.Combine(CWD, "autoexec")))
                {
                    Interpret(line);
                }
            }

            Console.Clear();
            Console.WriteLine("Welcome to " + Shell.OsName + " (" + Shell.Version + ")\nType 'help' for a list of commands.");
            if (Directory.Exists(CWD))
            {
                Directory.SetCurrentDirectory(CWD);
            }
            else
            {
                CWD = "NoDisks";
            }
            //BootChime bootChime = new BootChime();
            //bootChime.PlayBootChime();
            while (KeepCMDOpen == true)
            {
                Console.Write(CWD + " >> ");
                var input = Console.ReadLine();
                Interpret(input);
            }
        }

        public void Interpret(string input)
        {
            // SHUTDOWN
            if (input == "shutdown")
            {
                KeepCMDOpen = false;
                Power power = new Power();
                power.shutdown();
            }

            // REBOOT
            else if (input == "reboot")
            {
                Power power = new Power();
                power.reboot();
            }

            // SYSINFO (System information)
            else if (input == "sysinfo")
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

            // LS (FS Listing)
            else if (input == "ls")
            {
                try
                {
                    foreach (var dir in Directory.GetDirectories(CWD))
                    {
                        Console.WriteLine(dir + "\t[DIR]");
                    }
                    foreach (var filename in Directory.GetFiles(CWD))
                    {
                        Console.WriteLine(filename + "\t[FILE]");
                    }
                }
                catch
                {

                }
            }

            // HELP
            else if (input == "help")
            {
                Help help = new Help();
                help.ShowHelp();
            }

            // CLS (Clear console)
            else if (input == "cls")
            {
                Console.Clear();
            }

            // CD (Change Directory)
            else if (input.Contains("cd "))
            {
                try
                {
                    var path = input.Substring(3);
                    if (Directory.Exists(Path.Combine(CWD, path)))
                    {
                        CWD = Path.Combine(CWD, path);
                        Directory.SetCurrentDirectory(CWD);
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

            // CAT
            else if (input.Contains("cat "))
            {
                try
                {
                    var path = input.Substring(4);
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        var data = File.ReadAllText(Path.Combine(CWD, path));
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

            // MKDIR (Make directory)
            else if (input.Contains("mkdir "))
            {
                try
                {
                    var path = input.Substring(6);
                    if (!Directory.Exists(Path.Combine(CWD, path)))
                    {
                        Directory.CreateDirectory(Path.Combine(CWD, path));
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

            // MKF (Make file)
            else if (input.Contains("mkf "))
            {
                try
                {
                    var path = input.Substring(4);
                    if (!File.Exists(Path.Combine(CWD, path)))
                    {
                        File.CreateText(Path.Combine(CWD, path));
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

            // RM (Remove file)
            else if (input.Contains("rm "))
            {
                try
                {
                    var path = input.Substring(3);
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        File.Delete(Path.Combine(CWD, path));
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

            // RMDIR (Remove directory)
            else if (input.Contains("rmdir "))
            {
                try
                {
                    var path = input.Substring(6);
                    if (Directory.Exists(Path.Combine(CWD, path)))
                    {
                        if(CWD != Path.Combine(CWD, path))
                        {
                            Directory.Delete(Path.Combine(CWD, path));
                        }
                        else
                        {
                            Console.WriteLine("Cannot delete the directory because it's in the current working directory!");
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

            // EXEC (Execute)
            else if (input.Contains("exec "))
            {
                try
                {
                    var path = input.Substring(5);
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        foreach (var line in File.ReadLines(Path.Combine(CWD, path)))
                        {
                            Interpret(line);
                        }
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

            // EDIT (edit file)
            else if (input.Contains("edit "))
            {
                try
                {
                    var path = input.Substring(5);
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        TextEdit editor = new TextEdit();
                        editor.StartMIV(Path.Combine(CWD, path));
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

            // IPADDR (IP address)
            else if (input == "ipaddr")
            {
                try
                {
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

            // URLTOIP (Convert url to ip address)
            else if (input.Contains("urltoip "))
            {
                UrlToIP urlToIP = new UrlToIP();
                var url = input.Substring(8);
                urlToIP.ConvertToIP(url);
            }

            // FTPSERVER
            else if (input == "ftpserver")
            {
                Console.WriteLine("Creating an FTP server on port 21...");
                try
                {
                    if (Directory.Exists("0:\\"))
                    {
                        CosmosFtpServer.FtpServer ftp = new CosmosFtpServer.FtpServer(Drivers.vfs, "0:\\");
                        ftp.Listen();
                    }
                    else
                    {
                        throw new DirectoryNotFoundException("Directory \"0:\\\" doesn't exist!");
                    }
                }
                catch (Exception EX)
                {
                    Console.WriteLine("ERROR: " + EX.Message);
                }
            }

            // BEEP
            else if (input.Contains("beep "))
            {
                var freq = input.Substring(5);
                try
                {
                    Console.Beep(Convert.ToInt32(freq), 250);
                }
                catch (Exception EX)
                {
                    Console.WriteLine("ERROR: " + EX.Message);
                }
            }

            // AUDIOPLAYER
            else if (input.Contains("audio "))
            {
                float[] l;
                float[] r;
                var path = input.Substring(6);
                if (File.Exists(Path.Combine(CWD, path)))
                {
                    AudioHelper.readWav(path, out l, out r);
                    foreach (var value in l)
                    {
                        Console.WriteLine(value.ToString());
                    }

                    foreach (var value in r)
                    {
                        Console.WriteLine(value.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }

            // MV (Move file)
            else if (input.Contains("mv "))
            {
                try
                {
                    var path = input.Substring(3).Split(' ')[0];
                    var dest = input.Substring(3).Split(' ')[1];
                    dest = Path.Combine(dest, Path.GetFileName(path));
                    Console.WriteLine("Moving file \"" + path + "\" to \"" + Path.Combine(Path.GetDirectoryName(path), dest) + "\"");
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        if (!File.Exists(dest))
                        {
                            File.Copy(Path.Combine(CWD, path), dest);
                            File.Delete(Path.Combine(CWD, path));
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

            // CP (Copy file)
            else if(input.Contains("cp "))
            {
                try
                {
                    var path = input.Substring(3).Split(' ')[0];
                    var dest = input.Substring(3).Split(' ')[1];
                    dest = Path.Combine(dest, Path.GetFileName(path));
                    Console.WriteLine("Copying file \"" + path + "\" to \"" + Path.Combine(Path.GetDirectoryName(path), dest) + "\"");
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        if (!File.Exists(dest))
                        {
                            File.Copy(Path.Combine(CWD, path), dest);
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

            // GUI
            else if(input == "gui")
            {
                GUI gui = new GUI();
                gui.INIT();
            }

            // PANIC
            else if(input == "panic")
            {
                Kernel.KernelPanic("USER GENERATED PANIC", "User invoked kernel panic from the command line!");
            }

            // MODES (Canvas Modes)
            else if(input == "modes")
            {
                Canvas canvas = FullScreenCanvas.GetFullScreenCanvas();
                foreach(var mode in canvas.AvailableModes)
                {
                    Console.WriteLine(mode.Columns + "x" + mode.Rows);
                }
                canvas.Disable();
                canvas = null;
            }

            // ECHO (Print text)
            else if(input.Contains("echo "))
            {
                var txt = input.Substring(5);
                Console.WriteLine(txt);
            }

            // TIMEOUT (Pause console)
            else if (input.Contains("timeout "))
            {
                try
                {
                    var ms = input.Substring(8);
                    Thread.Sleep(Convert.ToInt32(ms));
                }
                catch(Exception EX)
                {
                    Console.WriteLine("ERROR: " + EX.Message);
                }
            }

            // EMPTY COMMAND
            else if (input == "");

            // BAD COMMAND
            else
            {
                Console.WriteLine("Invalid command: \"" + input + "\"");
            }
        }
    }
}
