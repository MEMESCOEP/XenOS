using Cosmos.System.Network.Config;
using System;
using System.IO;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics;
using System.Threading;
using LibDotNetParser.CILApi;
using LibDotNetParser;
using System.Linq;

namespace XenOS
{
    internal class CustomConsole
    {
        // Suppression statements
        #pragma warning disable CA1416 // Validate platform compatibility
        #pragma warning disable CS0642 // Possible mistaken empty statement

        // Resource files
        [ManifestResourceStream(ResourceName = "XenOS.Audio.StartupSound.wav")]
        private readonly static byte[] StartupSound;

        [ManifestResourceStream(ResourceName = "XenOS.DLLs.mscorlib.dll")]
        private readonly static byte[] mscorlib;

        [ManifestResourceStream(ResourceName = "XenOS.TestApps.HelloWorld.exe")]
        private readonly static byte[] HelloWorldExample;

        // Variables
        public string CWD = "0:\\";
        public bool KeepCMDOpen = true;
        public static string PlayStartupSound = "1";

        // Functions
        public void CMD()
        {
            Console.WriteLine("[INFO -> Console] >> Console loaded.");

            if (File.Exists(Path.Combine("0:\\SETTINGS", "login")))
            {
                Login login = new Login();
                login.SystemLogin();
            }

            LoadSettings loadSettings = new LoadSettings();
            loadSettings.Load();

            Console.Clear();
            Console.WriteLine("Welcome to " + Shell.OsName + "! (" + Shell.Version + ")\nType 'help' for a list of commands.");

            if (File.Exists(Path.Combine("0:\\", "autoexec")))
            {
                foreach (var line in File.ReadLines(Path.Combine("0:\\", "autoexec")))
                {
                    if (Console.KeyAvailable)
                    {
                        if(Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Interpret(line);
                    }
                }
            }

            if (PlayStartupSound == "1")
            {
                if (Drivers.AudioEnabled)
                {
                    AudioPlayer player = new AudioPlayer();
                    player.PlayWAVFromBytes(StartupSound);
                }
                else
                {
                    int freq = 200;
                    for (int i = 0; i < 9; i += 1)
                    {
                        Console.Beep(freq, 25);
                        freq += 100;
                    }
                }
            }
            
            if (!File.Exists(@"0:\framework\mscorlib.dll"))
            {
                try
                {
                    if (!Directory.Exists(@"0:\framework\"))
                    {
                        Directory.CreateDirectory(@"0:\framework\");
                    }
                    File.WriteAllBytes(@"0:\framework\mscorlib.dll", mscorlib);
                }
                catch
                {

                }
            }

            if (!Directory.Exists(@"0:\TestApps\"))
            {
                try
                {
                    Directory.CreateDirectory(@"0:\TestApps\");
                }
                catch
                {

                }
                if (!File.Exists(@"0:\TestApps\program.bin"))
                {
                    try
                    {
                        File.WriteAllBytes(@"0:\TestApps\HelloWorld.exe", HelloWorldExample);
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                if (!File.Exists(@"0:\TestApps\HelloWorld.exe"))
                {
                    try
                    {
                        File.WriteAllBytes(@"0:\TestApps\HelloWorld.exe", HelloWorldExample);
                    }
                    catch
                    {

                    }
                }
            }

            if (Directory.Exists(CWD))
            {
                Directory.SetCurrentDirectory(CWD);
            }
            else
            {
                CWD = "NoFS";
            }

            /*if (CWD != "NoFS" && File.Exists("0:\\SETTINGS\\username"))
            {
                var uname = File.ReadAllText("0:\\SETTINGS\\username");
                Shell.username = uname;
            }*/

            PrintPrompt();
            string input = "";
            while (KeepCMDOpen == true)
            {
                input = Console.ReadLine();
                Interpret(input);
                PrintPrompt();
            }
        }

        public void PrintPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(Shell.username + "@" + Shell.OsName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[" + CWD + "]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" >> ");
        }

        public void Interpret(string input)
        {
            // SHUTDOWN
            if (input == "shutdown")
            {
                KeepCMDOpen = false;
                Power power = new Power();
                power.shutdown();
                Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
            }

            // REBOOT
            else if (input == "reboot")
            {
                Power power = new Power();
                power.reboot();
                Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
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
                    Console.WriteLine("CPU Uptime: " + (Cosmos.Core.CPU.GetCPUUptime()));
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[DIR]\t ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(dir);
                    }
                    foreach (var filename in Directory.GetFiles(CWD))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[FILE]\t");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(filename);
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
            else if (input.StartsWith("cd "))
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
            else if (input.StartsWith("cat "))
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
            else if (input.StartsWith("mkdir "))
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
            else if (input.StartsWith("mkf "))
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
            else if (input.StartsWith("rm "))
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
            else if (input.StartsWith("rmdir "))
            {
                try
                {
                    var path = input.Substring(6);
                    if (Directory.Exists(Path.Combine(CWD, path)))
                    {
                        if (CWD != Path.Combine(CWD, path))
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
            else if (input.StartsWith("exec "))
            {
                try
                {
                    var path = input.Substring(5);
                    if (File.Exists(Path.Combine(CWD, path)))
                    {
                        foreach (var line in File.ReadLines(Path.Combine(CWD, path)))
                        {
                            if (Console.KeyAvailable)
                            {
                                if (Console.ReadKey().Key == ConsoleKey.Escape)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                Interpret(line);
                            }
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
            else if (input.StartsWith("edit "))
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
            else if (input.StartsWith("urltoip "))
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
            else if (input.StartsWith("beep "))
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
            else if (input.StartsWith("audio "))
            {
                float[] l;
                float[] r;
                var path = input.Substring(6);
                if (File.Exists(Path.Combine(CWD, path)))
                {
                    /*
                    AudioHelper.readWav(path, out l, out r);
                    foreach (var value in l)
                    {
                        Console.WriteLine(value.ToString());
                    }

                    foreach (var value in r)
                    {
                        Console.WriteLine(value.ToString());
                    }
                    */

                    AudioPlayer audioPlayer = new AudioPlayer();
                    audioPlayer.PlayWAV(path);
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }

            // MV (Move file)
            else if (input.StartsWith("mv "))
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
            else if (input.StartsWith("cp "))
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
            else if (input == "gui")
            {
                GUI gui = new GUI();
                gui.INIT();
            }

            // PANIC
            else if (input == "panic")
            {
                Kernel.KernelPanic("USER GENERATED PANIC", "User invoked kernel panic from the command line!");
            }

            // MODES (Canvas Modes)
            else if (input == "modes")
            {
                Canvas canvas = FullScreenCanvas.GetFullScreenCanvas();
                foreach (var mode in canvas.AvailableModes)
                {
                    Console.WriteLine(mode.Columns + "x" + mode.Rows);
                }
                canvas.Disable();
                canvas = null;
            }

            // TIME
            else if (input == "time")
            {
                Console.WriteLine(Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second);
            }

            // ECHO (Print text)
            else if (input.StartsWith("echo "))
            {
                var txt = input.Substring(5);
                Console.WriteLine(txt);
            }

            // TIMEOUT (Pause console)
            else if (input.StartsWith("timeout "))
            {
                try
                {
                    var ms = input.Substring(8);
                    Thread.Sleep(Convert.ToInt32(ms));
                }
                catch (Exception EX)
                {
                    Console.WriteLine("ERROR: " + EX.Message);
                }
            }

            // PING
            else if (input.StartsWith("ping "))
            {
                try
                {
                    var ip = input.Substring(5);
                    Ping ping = new Ping();
                    ping.PingIP(ip);
                }
                catch (Exception EX)
                {
                    Console.WriteLine("ERROR: " + EX.Message);
                }
            }

            // AUDIO (Play wav file(s))
            else if (input.StartsWith("audio "))
            {
                var path = input.Substring(6);
                if (File.Exists(path))
                {
                    AudioPlayer player = new AudioPlayer();
                    player.PlayWAV(path);
                }
                else
                {
                    Console.WriteLine("File " + path + " doesn't exist!");
                }
            }

            // TESTAUDIO (Test audio device(s))
            else if (input == "testaudio")
            {
                AudioPlayer player = new AudioPlayer();
                player.PlayWAVFromBytes(StartupSound);
            }

            // ABOUT
            else if (input == "about")
            {
                About.ShowInfo();
            }

            // APP (Execute .NET apps)
            else if (input.StartsWith("app "))
            {
                var path = input.Substring(4);
                if (File.Exists(path))
                {
                    path = Path.GetFullPath(input.Substring(4));
                    try
                    {
                        if (!Directory.Exists(@"0:\framework\"))
                        {
                            throw new DirectoryNotFoundException("The DotNetParser framework wasn't found!");
                        }
                        try
                        {
                            DotNetFile dotNetFile = new DotNetFile(path);
                            var clr = new libDotNetClr.DotNetClr(dotNetFile, "0:\\Framework\\");
                            clr.RegisterCustomInternalMethod("System.Console.WriteLine", WriteLine);
                            clr.RegisterCustomInternalMethod("WriteAllText", WriteAllText);
                            clr.RegisterCustomInternalMethod("ReadAllText", ReadAllText);
                            clr.RegisterCustomInternalMethod("ReadLine", ReadLine);
                            clr.RegisterCustomInternalMethod("ReadKey", ReadKey);
                            clr.RegisterCustomInternalMethod("DeleteFile", DeleteFile);
                            clr.RegisterCustomInternalMethod("CreateFile", CreateFile);
                            clr.Start();
                        }
                        catch (Exception EX)
                        {
                            Console.WriteLine("ERROR: " + EX.Message);
                        }
                    }
                    catch (Exception EX)
                    {
                        Console.WriteLine("ERROR: " + EX.Message);
                    }
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }

            // APPEND (Append text to file)
            else if (input.StartsWith("append "))
            {
                var path = input.Substring(7).Split(' ')[0];
                var txt = input.Substring(7 + path.Length + 1);
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

            // WRITE (Write text to file)
            else if (input.StartsWith("write "))
            {
                var path = input.Substring(6).Split(' ')[0];
                var txt = input.Substring(6 + path.Length + 1);
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

            /*
            // UNAME (Change the username)
            else if (input.StartsWith("uname "))
            {
                var txt = input.Substring(6);
                if (CWD != "NoFS")
                {
                    if (!Directory.Exists("0:\\SETTINGS\\"))
                    {
                        Directory.CreateDirectory("0:\\SETTINGS\\");
                    }
                    if (File.Exists("0:\\SETTINGS\\username"))
                    {
                        File.WriteAllText("0:\\SETTINGS\\username", txt);
                    }
                    else
                    {
                        File.CreateText("0:\\SETTINGS\\username");
                        File.WriteAllText("0:\\SETTINGS\\username", txt);
                    }
                }

                if (CWD != "NoFS" && File.Exists("0:\\SETTINGS\\username"))
                {
                    var uname = File.ReadAllText("0:\\SETTINGS\\username");
                    Shell.username = uname;
                }
                else
                {
                    Console.WriteLine("Your changes are temporary because there is no filesystem.");
                    Shell.username = txt;
                }
            }
            */

            //DSK ()
            else if (input == "dsk")
            {
                foreach (var disk in DriveInfo.GetDrives())
                {
                    if (disk.DriveType == DriveType.CDRom)
                    {
                        Console.WriteLine("CD-Rom: " + disk.Name);
                    }
                    else if (disk.DriveType == DriveType.Removable)
                    {
                        Console.WriteLine("Removable Disk: " + disk.Name);
                    }
                    else if (disk.DriveType == DriveType.Unknown)
                    {
                        Console.WriteLine("Disk: " + disk.Name);
                    }
                    else if (disk.DriveType == DriveType.Removable)
                    {
                        Console.WriteLine("Removable Disk: " + disk.Name);
                    }
                    else if (disk.DriveType == DriveType.Ram)
                    {
                        Console.WriteLine("Ram Disk: " + disk.Name);
                    }
                    else if (disk.DriveType == DriveType.Fixed)
                    {
                        Console.WriteLine("Fixed Disk: " + disk.Name);
                    }
                    else if (disk.DriveType == DriveType.Network)
                    {
                        Console.WriteLine("Network Disk: " + disk.Name);
                    }
                    Console.WriteLine("Volume label: " + disk.VolumeLabel);
                    Console.WriteLine("Size: " + disk.TotalSize);
                    Console.WriteLine("Total free space: " + disk.TotalFreeSpace);
                    Console.WriteLine("Available space: " + disk.AvailableFreeSpace);
                    Console.WriteLine("Root directory: " + disk.RootDirectory);
                    Console.WriteLine("Format: " + disk.DriveFormat);
                    Console.WriteLine("Is Ready: " + disk.IsReady.ToString());
                    Console.WriteLine();
                }
            }

            // LOCK (Lock the screen)
            else if (input.StartsWith("lock "))
            {
                var pswd = input.Substring(5);
                LockScreen lockScreen = new LockScreen();
                lockScreen.Lock(pswd);
            }

            // LOGOUT (Log out of the computer
            else if(input == "logout")
            {
                CMD();
            }

            // AMOGUS
            else if (input == "amogus")
            {
                Console.WriteLine(@"S U S S Y  B A K A !!  ! !  !  !  !  !");
            }

            // LOGINPASS (Change the login password)
            else if (input.StartsWith("loginpass "))
            {
                var pswd = input.Substring(10);
                if (pswd != "-r")
                {
                    if (!File.Exists("0:\\SETTINGS\\login"))
                    {
                        File.CreateText("0:\\SETTINGS\\login");
                    }
                    File.WriteAllText("0:\\SETTINGS\\login", pswd);
                    Console.WriteLine("Login password was changed to: " + pswd);
                }
                else
                {
                    if (File.Exists("0:\\SETTINGS\\login"))
                    {
                        File.Delete("0:\\SETTINGS\\login");
                    }
                    Console.WriteLine("Login password disabled.");
                }
            }

            // ADDUSER (Add a user to the userlist)
            else if(input == "adduser")
            {
                var unames = File.ReadAllText("0:\\SETTINGS\\users");
                var pswds = File.ReadAllText("0:\\SETTINGS\\login");

                Console.Write("Enter a new username >> ");
                var Username = Console.ReadLine();

                if (unames.Contains(Username))
                {
                    Console.WriteLine("User already exists!\n");
                    return;
                }

                Console.Write("Enter a password >> ");
                var Password = Console.ReadLine();

                unames += ("\n" + Username);
                pswds += ("\n" + Password);

                File.WriteAllText("0:\\SETTINGS\\users", unames);
                File.WriteAllText("0:\\SETTINGS\\login", pswds);
            }

            // RMUSER (Remove a user from the userlist)
            else if (input == "rmuser")
            {
                Console.Write("Enter username >> ");
                var Username = Console.ReadLine();

                var unames = File.ReadAllLines("0:\\SETTINGS\\users");
                var pswds = File.ReadAllLines("0:\\SETTINGS\\login");

                if (!unames.Contains(Username))
                {
                    Console.WriteLine("User doesn't exist!\n");
                    return;
                }

                foreach (var u in unames)
                {
                    if (u == Username)
                    {
                        int UnameIndex = 0;
                        int count = 0;
                        var passdata = String.Empty;
                        var namedata = String.Empty;
                        foreach (var un in unames)
                        {
                            if (un == Username)
                            {
                                UnameIndex = count;
                            }
                            else
                            {
                                if (un != String.Empty)
                                {
                                    namedata += un + "\n";
                                    passdata += pswds[count] + "\n";
                                }
                            }

                            count++;
                        }

                        File.WriteAllText("0:\\SETTINGS\\users", namedata);
                        File.WriteAllText("0:\\SETTINGS\\login", passdata);
                        break;
                    }
                }
            }

            // CHGPSWD (Change a user's password)
            else if (input == "chgpswd")
            {
                var unames = File.ReadAllLines("0:\\SETTINGS\\users");
                var pswds = File.ReadAllLines("0:\\SETTINGS\\login");

                Console.Write("Enter username >> ");
                var Username = Console.ReadLine();

                if (!unames.Contains(Username))
                {
                    Console.WriteLine("Invalid username!\n");
                    return;
                }

                Console.Write("Enter current password for {0} >> ", Username);
                var Password = Console.ReadLine();

                int UnameIndex = 0;
                int count = 0;
                var passdata = String.Empty;
                foreach (var un in unames)
                {
                    if (un == Username)
                    {
                        if (pswds[count] == Password)
                        {
                            Console.Write("Enter new password for {0} >> ", Username);
                            var NewPassword = Console.ReadLine();
                            passdata += NewPassword + "\n";
                        }
                    }
                    else
                    {
                        if (pswds[count] != String.Empty)
                        {
                            passdata += pswds[count] + "\n";
                        }
                    }

                    count++;
                }

                File.WriteAllText("0:\\SETTINGS\\login", "\n" + passdata);
            }

            // GAMES (Start playing games)
            else if(input == "games")
            {
                Games games = new Games();
                games.SelectGame();
            }

            // EMPTY COMMAND
            else if (input == "");

            // BAD COMMAND
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid command: \"" + input + "\"");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void WriteLine(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = (string)Stack[Stack.Length - 1].value;
            Console.WriteLine(str);
        }

        public void ReadLine(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var prompt = Stack[Stack.Length - 1].value.ToString();
            Console.Write(prompt);
            returnValue = MethodArgStack.String(Console.ReadLine());
        }

        public void ReadKey(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var prompt = Stack[Stack.Length - 1].value.ToString();
            Console.Write(prompt);
            returnValue = MethodArgStack.String(Console.ReadKey().KeyChar.ToString());
        }

        public void ReadAllText(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var path = Stack[Stack.Length - 1].value.ToString();
            returnValue = MethodArgStack.String(File.ReadAllText(path));
        }

        public void WriteAllText(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            File.WriteAllText(Stack[0].ToString(), Stack[1].ToString());
        }

        public void DeleteFile(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            File.Delete(Stack[0].ToString());
        }

        public void CreateFile(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            File.Create(Stack[0].ToString());
        }

        public void Write(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = (string)Stack[Stack.Length - 1].value;
            Console.Write(str);
        }
    }
}
