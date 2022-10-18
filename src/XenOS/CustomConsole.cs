using Cosmos.System.Network.Config;
using System;
using System.IO;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics;
using System.Threading;
using LibDotNetParser.CILApi;
using LibDotNetParser;
using System.Linq;
using Cosmos.Core.IOGroup;
using CosmosELFCore;
using System.Text;

namespace XenOS
{
    internal class CustomConsole
    {
        /* Warning suppression statements */
        #pragma warning disable CA1416 // Validate platform compatibility
        #pragma warning disable CS0642 // Possible mistaken empty statement

        /* Resource files */
        [ManifestResourceStream(ResourceName = "XenOS.Audio.StartupSound.wav")]
        private readonly static byte[] StartupSound;

        [ManifestResourceStream(ResourceName = "XenOS.DLLs.mscorlib.dll")]
        private readonly static byte[] mscorlib;

        [ManifestResourceStream(ResourceName = "XenOS.TestApps.HelloWorld.exe")]
        private readonly static byte[] HelloWorldExample;

        [ManifestResourceStream(ResourceName = "XenOS.TestApps.ELF_TEST.bin")]
        private readonly static byte[] ELFTest;

        /* Variables */
        public string CWD = "0:\\";
        public bool KeepCMDOpen = true;
        public static string PlayStartupSound = "1";

        /* Functions */
        private byte[] UnmanagedString(string s)
        {
            var re = new byte[s.Length + 1];

            for (int i = 0; i < s.Length; i++)
            {
                re[i] = (byte)s[i];
            }

            re[s.Length + 1] = 0; //c requires null terminated string
            return re;
        }

        public void CMD()
        {
            Console.WriteLine("[INFO -> Console] >> Console loaded.");

            /* If there are one or more user accounts on the system, show the login prompt */
            if (File.Exists(Path.Combine("0:\\SETTINGS", "login")))
            {
                Login login = new Login();
                login.SystemLogin();
            }

            /* Load the system settings */
            LoadSettings loadSettings = new LoadSettings();
            loadSettings.Load();

            /* Clear the screen and show the welcome message */
            Console.Clear();
            Console.WriteLine(Shell.Logo);
            Console.WriteLine("\nWelcome to " + Shell.OsName + "! (" + Shell.Version + ")\nType 'help' for a list of commands.");

            /* If there is an autoexec script, run it */
            if (File.Exists(Path.Combine("0:\\", "autoexec")))
            {
                foreach (var line in File.ReadLines(Path.Combine("0:\\", "autoexec")))
                {
                    if (Console.KeyAvailable)
                    {
                        var ck = Console.ReadKey();
                        if (ck.Key == ConsoleKey.Escape)
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

            /* If the startup sound is enabled and there is an audio device, play it */
            if (PlayStartupSound == "1")
            {
                if (Drivers.AudioEnabled)
                {
                    AudioPlayer player = new AudioPlayer();
                    player.PlayWAVFromBytes(StartupSound);
                }
                else
                {
                    foreach(var device in Cosmos.HAL.PCI.Devices)
                    {
                        if(device.VendorID == 5549 || device.VendorID == 8384 || device.VendorID == 32902 || device.VendorID == 4203 || device.VendorID == 4660)
                        {
                            try
                            {
                                int freq = 200;
                                for (int i = 0; i < 9; i += 1)
                                {
                                    //Cosmos.HAL.PCSpeaker.Beep((uint)freq, 25);
                                    for (int w = 0; w < 999999; w++) ;
                                    freq += 100;
                                }
                            }
                            catch
                            {

                            }

                            break;
                        }
                    }                    
                }
            }

            /* If XenOS is running in VMWare, start the GUI automatically */
            if (Cosmos.System.VMTools.IsVMWare)
            {
                GUI gui = new GUI();
                gui.INIT();
            }

            /* If required libraries and files don't exist, write them to the disk */
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
                if (!File.Exists(@"0:\TestApps\ELF_TEST.bin"))
                {
                    try
                    {
                        File.WriteAllBytes(@"0:\TestApps\ELF_TEST.bin", ELFTest);

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
                if (!File.Exists(@"0:\TestApps\ELF_TEST.bin"))
                {
                    try
                    {
                        File.WriteAllBytes(@"0:\TestApps\ELF_TEST.bin", ELFTest);

                    }
                    catch
                    {

                    }
                }
            }

            /* Set the current working directory (only if a filesystem exists!) */
            if (Directory.Exists(CWD))
            {
                Directory.SetCurrentDirectory(CWD);
            }
            else
            {
                CWD = "NoFS";
            }

            /* Start the command line */
            PrintPrompt();
            string input = "";
            while (KeepCMDOpen == true)
            {
                try
                {
                    input = Console.ReadLine();
                    Interpret(input);
                    PrintPrompt();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    PrintPrompt();
                }
            }
        }

        // Print the command line prompt
        public void PrintPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(Shell.username + "@" + Shell.OsName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[" + CWD + "]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" >> ");
        }

        // Handle commands
        public void Interpret(string input)
        {
            /* SYSTEM COMMANDS */
            // PANIC
            if (input == "panic")
            {
                Kernel.KernelPanic("USER GENERATED PANIC", "User invoked kernel panic from the command line!");
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

            // HELP
            else if (input == "help")
            {
                Help help = new Help();

                Console.Write("Topics:\nconsole\nsystem\nfilesystem\ngraphics\naudio\nnetwork\npower\n\nEnter a topic >> ");

                string topic = Console.ReadLine();
                help.ShowHelp(topic);
            }

            // ABOUT
            else if (input == "about")
            {
                About.ShowInfo();
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

            // SHOWKEY (Display the key that gets pressed)
            else if (input == "showkey")
            {
                var key = Console.ReadKey();
                while (key.Key != ConsoleKey.Escape)
                {
                    Console.WriteLine(key.KeyChar);
                    key = Console.ReadKey();
                }
                Console.WriteLine(key.KeyChar);
            }

            // ELF (Execute ELF apps)
            else if (input.StartsWith("elf "))
            {
                var path = input.Substring(4);
                if (File.Exists(path))
                {
                    path = Path.GetFullPath(input.Substring(4));
                    try
                    {
                        if (!Directory.Exists(@"0:\framework\"))
                        {
                            //throw new DirectoryNotFoundException("The DotNetParser framework wasn't found!");
                        }
                        try
                        {
                            unsafe
                            {
                                var exe = new UnmanagedExecutible(path);
                                Console.WriteLine("Loading");
                                exe.Load();
                                Console.WriteLine("Linking");
                                exe.Link();

                                Console.WriteLine("Executing");

                                new ArgumentWriter();
                                exe.Invoke("main");
                            }
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

            // LOCK (Lock the screen)
            else if (input.StartsWith("lock "))
            {
                var pswd = input.Substring(5);
                LockScreen lockScreen = new LockScreen();
                lockScreen.Lock(pswd);
            }

            // LOGOUT (Log out of the computer)
            else if (input == "logout")
            {
                CMD();
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
            else if (input == "adduser")
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




            /* POWER COMMANDS */
            // SHUTDOWN
            else if (input == "shutdown")
            {
                Drivers drivers = new Drivers();
                drivers.shutdown();
                KeepCMDOpen = false;
                Power power = new Power();
                power.shutdown();
                Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
            }

            // REBOOT
            else if (input == "reboot")
            {
                Drivers drivers = new Drivers();
                drivers.shutdown();
                Power power = new Power();
                power.reboot();
                Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
            }

            /* FILESYSTEM COMMANDS */
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

            // DSK (List disks)
            else if (input == "dsk")
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

            // VLIST (List mounted volumes)
            else if (input == "vlist")
            {
                foreach (var disk in Cosmos.System.FileSystem.VFS.VFSManager.GetVolumes())
                {
                    Console.WriteLine(disk.mFullPath);
                }
            }

            // FORMAT (Format a disk)
            else if (input.StartsWith("format "))
            {
                var name = input.Substring(7);
                int index = 0;
                while (true)
                {
                    foreach (var disk in Cosmos.System.FileSystem.VFS.VFSManager.GetVolumes())
                    {
                        if (disk.mName != name)
                        {
                            index++;
                            continue;
                        }
                    }

                    foreach (var partition in Cosmos.System.FileSystem.VFS.VFSManager.GetDisks()[index].Partitions)
                    {
                        try
                        {
                            Console.WriteLine("Partition #" + Cosmos.System.FileSystem.VFS.VFSManager.GetDisks()[index].Partitions);
                            Console.WriteLine("Host: " + partition.Host);
                            Console.WriteLine("Root path: " + partition.RootPath);
                            Console.WriteLine("Has FS: " + partition.HasFileSystem);
                            Console.WriteLine("Mounted FS: " + partition.MountedFS);
                            Console.WriteLine();
                        }
                        catch (Exception EX)
                        {
                            Console.WriteLine("[ERROR] >> " + EX.Message);
                        }
                    }
                    break;
                }
            }




            /* NETWORKING COMMANDS */
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




            /* CONSOLE COMMANDS */
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

            // CLS (Clear console)
            else if (input == "cls")
            {
                Console.Clear();
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




            /* AUDIO COMMANDS */
            // AUDIOPLAYER
            else if (input.StartsWith("audio "))
            {
                float[] l;
                float[] r;
                var path = input.Substring(6);
                if (File.Exists(Path.Combine(CWD, path)))
                {
                    AudioPlayer audioPlayer = new AudioPlayer();
                    audioPlayer.PlayWAV(path);
                }
                else
                {
                    Console.WriteLine("File \"" + path + "\" doesn't exist!");
                }
            }

            // TESTAUDIO (Test audio device(s))
            else if (input == "testaudio")
            {
                AudioPlayer player = new AudioPlayer();
                player.PlayWAVFromBytes(StartupSound);
            }




            /* GUI COMMANDS */
            // GUI
            else if (input == "gui")
            {
                GUI gui = new GUI();
                gui.INIT();
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

            // CHGSENS (Change the mouse sensitivity)
            else if (input == "chgsens")
            {
                Console.Write("New mouse sensitivity >> ");
                Shell.MouseSensitivity = Convert.ToInt32(Console.ReadLine());
            }




            /* TIME/DATE COMMANDS */
            // TIME
            else if (input == "time")
            {
                Console.WriteLine(Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second);
            }

            // DATE
            else if (input == "date")
            {
                Console.WriteLine(Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year);
            }

            // DATETIME
            else if (input == "datetime")
            {
                Console.WriteLine(Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year);
                Console.WriteLine(Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second);
            }

            /* OTHER COMMANDS */
            // AMOGUS (sussy)
            else if (input == "amogus")
            {
                Console.WriteLine(@"SUSSY BAKA LMAO (You're sus)");
            }

            // GAMES (Start playing games)
            else if (input == "games")
            {
                Games games = new Games();
                games.SelectGame();
            }

            // PCI (List PCI devices)
            else if (input == "pci")
            {
                foreach (var device in Cosmos.HAL.PCI.Devices)
                {
                    Console.WriteLine("ID: {0}\nvID: {1}\nSUBCLASS: {2}\nSTATUS: {3}\nSLOT: {4}\nREV ID: {5}\n", device.DeviceID, device.VendorID, device.Subclass, device.Status, device.slot, device.RevisionID);
                }
            }

            // EMPTY COMMAND
            else if (input == "") ;

            // EXEC APP, OR BAD COMMAND
            else
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid command: \"" + input + "\"");
                    Console.ForegroundColor = ConsoleColor.White;
                }
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
