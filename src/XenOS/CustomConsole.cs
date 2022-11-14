using Cosmos.System.Network.Config;
using System;
using System.IO;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics;
using System.Threading;
using LibDotNetParser.CILApi;
using LibDotNetParser;
using System.Linq;
using CosmosELFCore;
using Cosmos.System.FileSystem.ISO9660;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;

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
        public static bool AutoStartGUI = false;

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

            /* If required libraries and files don't exist, write them to the disk */
            Console.Write("[INFO -> Console] >> Writing libraries and files to disk...");
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

            /* Clear the screen and show the welcome message */
            Console.Clear();
            Console.WriteLine(Shell.Logo);
            Console.WriteLine("\nWelcome to {0}! ({1})\nType 'help' for a list of commands.", Shell.OsName, Shell.Version);

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
                    //AudioPlayer player = new AudioPlayer();
                    //player.PlayWAVFromBytes(StartupSound);
                    MemoryAudioStream SS = MemoryAudioStream.FromWave(StartupSound);
                    PrismAudio.AudioPlayer.Play(SS);
                }
            }

            /* If XenOS is running in VMWare (or anything that has a VMWareSVGAII-compatible graphics adapter), start the GUI automatically */
            if (Cosmos.System.VMTools.IsVMWare && AutoStartGUI)
            {
                GUI gui = new GUI();
                gui.INIT();
            }

            /* Set the current working directory only if a filesystem exists, otherwise create an ISO9660 filesystem */
            if (Directory.Exists(CWD))
            {
                Directory.SetCurrentDirectory(CWD);
            }
            else
            {
                var CD_FS = new ISO9660FileSystemFactory();
                try
                {
                    foreach (var partition in Cosmos.HAL.BlockDevice.Partition.Partitions)
                    {
                        if (partition.Type == Cosmos.HAL.BlockDevice.BlockDeviceType.RemovableCD)
                        {
                            int drivenum = Cosmos.HAL.BlockDevice.Partition.Partitions.IndexOf(partition);
                            if(drivenum == 0)
                            {
                                drivenum = Cosmos.HAL.BlockDevice.Partition.Partitions.Count - 1;
                            }
                            var pt = CD_FS.Create(partition, drivenum + ":\\", (long)partition.BlockSize);
                            Console.Write(pt.RootPath + ": ");
                            pt.DisplayFileSystemInfo();
                            CWD = pt.RootPath + "_NoFS";
                        }
                    }
                }
                catch
                {
                    CWD = "NoFS";
                }
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
                Kernel.KernelPanic("USER INVOKED PANIC", "User invoked kernel panic from the command line!");
            }

            // XMLTEST (Run an XML test)
            else if (input == "xmltest")
            {
                PrismTools.Tests.XMLTest.Run();
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
                help.ShowHelp(Console.ReadLine());
            }

            // ABOUT
            else if (input == "about")
            {
                About.ShowInfo();
            }

            // CPUINFO (Show CPU information
            else if (input == "cpuinfo")
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
                Console.WriteLine("Press ESCAPE to exit");
                var key = Console.ReadKey();
                while (key.Key != ConsoleKey.Escape)
                {
                    //Console.WriteLine(key.KeyChar);
                    key = Console.ReadKey();
                    if(key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                    }

                    if(key.Key == ConsoleKey.LeftArrow)
                    {
                        if (Console.CursorLeft > 0)
                        {
                            Console.CursorLeft--;
                        }
                    }

                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        if (Console.CursorLeft < Console.WindowWidth - 1)
                        {
                            Console.CursorLeft++;
                        }                            
                    }

                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (Console.CursorTop > 0)
                        {
                            Console.CursorTop--;
                        }
                    }

                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (Console.CursorTop < Console.WindowHeight - 1)
                        {
                            Console.CursorTop++;
                        }
                    }

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        if(Console.CursorLeft > 0)
                        {
                            Console.CursorLeft--;
                            Console.Write(" ");
                            Console.CursorLeft--;
                        }
                    }

                    if (key.Key == ConsoleKey.PageDown)
                    {
                        Console.CursorTop = Console.WindowHeight - 1;
                    }

                    if (key.Key == ConsoleKey.PageUp)
                    {
                        Console.CursorTop = 0;
                    }

                    if (key.Key == ConsoleKey.Home)
                    {
                        Console.CursorLeft = 0;
                    }

                    if (key.Key == ConsoleKey.End)
                    {
                        Console.CursorLeft = Console.WindowWidth - 1;
                    }
                }
                Console.Clear();
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
                                fixed (byte* ptr = File.ReadAllBytes(path))
                                {
                                    PrismELF.UnmanagedExecutible exe = new PrismELF.UnmanagedExecutible(ptr);
                                    Console.WriteLine("Loading");
                                    exe.Load();

                                    Console.WriteLine("Linking");
                                    exe.Link();

                                    Console.WriteLine("Executing");
                                    exe.Invoke("main");
                                }
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

            // TESTELF (Test the elf executor)
            else if (input == "testelf")
            {
                unsafe
                {
                    fixed (byte* ptr = Helpers.test_so)
                    {
                        var exe = new UnmanagedExecutible(ptr);
                        exe.Load();
                        exe.Link();

                        Console.WriteLine("Executing");

                        new ArgumentWriter();
                        exe.Invoke("tty_clear");

                        new ArgumentWriter()
                            .Push(5)  //fg
                            .Push(15); //bg
                        exe.Invoke("tty_set_color");

                        fixed (byte* str = UnmanagedString("Hello World"))
                        {
                            new ArgumentWriter()
                                .Push((uint)str);
                            exe.Invoke("tty_puts");
                        }
                    }
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
                foreach (var disk in Drivers.vfs.GetVolumes())
                {
                    Console.WriteLine(disk.mFullPath);
                }
            }

            // FORMAT (Format a disk)
            else if (input.StartsWith("format "))
            {
                try
                {
                    var name = input.Substring(7);
                    int index = 0;
                    foreach (var disk in Cosmos.System.FileSystem.VFS.VFSManager.GetDisks())
                    {
                        foreach (var partition in disk.Partitions)
                        {
                            if (partition.RootPath == name)
                            {
                                index = disk.Partitions.IndexOf(partition);
                                if (index >= 0)
                                {
                                    if (partition.MountedFS != null)
                                    {
                                        Console.WriteLine("Formatting partition {0} of disk {1}...", index, name);
                                        partition.MountedFS.Format("FAT32", false);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Unable to format partition {0} of disk {1} because the filesystem isn't mounted.", index, name);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Formatting partition 0 of disk {0}...", name);
                                    foreach (var file in Directory.GetFiles(partition.RootPath))
                                    {
                                        File.Delete(file);
                                    }
                                    foreach (var dir in Directory.GetDirectories(partition.RootPath))
                                    {
                                        Directory.Delete(dir);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            // MKPART (Make a partition)
            else if(input == "mkpart")
            {
                Console.Write("Disk to partition >> ");
                var disknum = Console.ReadLine();

                Console.Write("Partition size >> ");
                var partsize = Console.ReadLine();

                foreach (var disk in Cosmos.System.FileSystem.VFS.VFSManager.GetDisks())
                {
                    foreach (var partition in disk.Partitions)
                    {
                        if (partition.RootPath == disknum)
                        {
                            Console.WriteLine("Creating new partition on disk {1}...", disknum);
                            disk.CreatePartition(Int32.Parse(partsize));
                        }
                    }
                }
            }

            // CDSK (Clear disk)
            else if(input == "cdsk")
            {
                Console.Write("Disk to clear >> ");
                var disknum = Console.ReadLine();

                foreach (var disk in Drivers.vfs.GetDisks())
                {
                    foreach (var partition in disk.Partitions)
                    {
                        if (partition.RootPath == disknum)
                        {
                            Console.WriteLine("Clearing disk {0}...", disknum);
                            disk.Clear();
                            int index = disk.Partitions.IndexOf(partition);
                            if(index >= 0)
                             disk.DeletePartition(index);
                        }
                    }
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
            // POOTIS
            else if(input == "pootis")
            {
                Console.WriteLine("POOTIS!!!!!1!11!!!11!111!!!!!");
            }

            // BEEP
            else if (input.StartsWith("beep "))
            {
                var freq = input.Substring(5);
                try
                {
                    //Console.Beep(Convert.ToInt32(freq), 250);
                    Cosmos.System.PCSpeaker.Beep(Convert.ToUInt32(freq), 250);
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
                /*
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
                */

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
