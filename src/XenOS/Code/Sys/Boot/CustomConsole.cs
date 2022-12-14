/* Directives */
using System;
using System.IO;
using System.Threading;
using IL2CPU.API.Attribs;
using Cosmos.System.Audio.IO;
using Cosmos.HAL.BlockDevice;
using Cosmos.System.FileSystem.ISO9660;
using XenOS.Code.Network;
using XenOS.Code.Executable;
using XenOS.Code.Information;
using XenOS.Code.Sys.Helpers;
using XenOS.Code.Commands.Info;
using XenOS.Code.Commands.Exec;
using XenOS.Code.Commands.Security;
using XenOS.Code.Commands.Filesystem;
using DateTime = XenOS.Code.Commands.Info.DateTime;

/* Namespaces */
namespace XenOS.Code.Sys.Boot
{
    /* CLasses */
    internal class CustomConsole
    {
        /* Warning suppression statements */
        #pragma warning disable CA1416 // Validate platform compatibility
        #pragma warning disable CS0642 // Possible mistaken empty statement

        /* Resource files */
        [ManifestResourceStream(ResourceName = "XenOS.Audio.StartupSound.wav")]
        private readonly static byte[] StartupSound;

        /* Variables */
        public static bool KeepCMDOpen = true;
        public static string PlayStartupSound = "1";

        /* Functions */
        // Start the console
        public static void CMD()
        {
            Console.WriteLine("[INFO -> Console] >> Console loaded.");

            // If there are one or more user accounts on the system, show the login prompt (This is placed before everything for security reasons)
            if (File.Exists(Path.Combine("0:\\SETTINGS", "login")))
            {
                Login login = new Login();
                login.SystemLogin();
            }

            // Load the system settings
            LoadSettings loadSettings = new LoadSettings();
            loadSettings.Load();

            // If required libraries, files, and/or folders don't exist, write them to the disk
            Console.Write("[INFO -> Console] >> Writing libraries and files to disk...");
            Libraries.CheckLibraries();

            // Clear the screen and show the welcome message
            Console.Clear();
            Console.WriteLine(Shell.Logo);
            Console.WriteLine("\nWelcome to {0}! ({1})\nType 'help' for a list of commands.", Shell.OsName, Shell.Version);

            // If there is an autoexec script, run it
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

            // If the startup sound is enabled and there is an audio device, play it
            if (PlayStartupSound == "1")
            {
                if (Drivers.Drivers.AudioEnabled)
                {
                    MemoryAudioStream SS = MemoryAudioStream.FromWave(StartupSound);
                    PrismAudio.AudioPlayer.Play(SS);
                }
            }

            // Set the current working directory only if a filesystem exists, otherwise create an ISO9660 filesystem
            if (Directory.Exists(Shell.CWD))
            {
                Directory.SetCurrentDirectory(Shell.CWD);
            }
            else
            {
                var CD_FS = new ISO9660FileSystemFactory();
                try
                {
                    foreach (var partition in Partition.Partitions)
                    {
                        if (partition.Type == BlockDeviceType.RemovableCD)
                        {
                            int drivenum = Partition.Partitions.IndexOf(partition);
                            if (drivenum == 0)
                            {
                                drivenum = Partition.Partitions.Count - 1;
                            }
                            var pt = CD_FS.Create(partition, drivenum + ":\\", (long)partition.BlockSize);
                            Console.Write(pt.RootPath + ": ");
                            pt.DisplayFileSystemInfo();
                            Shell.CWD = $"{pt.RootPath}_NoFS";
                        }
                    }
                }
                catch
                {
                    Shell.CWD = "NoFS";
                }
            }

            // Start the command line
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
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    PrintPrompt();
                }
            }
        }

        // Print the command line prompt
        public static void PrintPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(Shell.username + "@" + Shell.OsName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[" + Shell.CWD + "]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" >> ");
        }

        // Handle commands
        public static void Interpret(string input)
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
                Sysinfo.DisplaySystemInformation();
            }

            // HELP
            else if (input == "help")
            {
                Help.ShowHelp();
            }

            // HELPTOPICS (Show help topics)
            else if(input == "helptopics")
            {
                Help.ShowTopics();
            }

            // ABOUT (Show operating system information)
            else if (input == "about")
            {
                About.ShowInfo();
            }

            // CPUINFO (Show CPU information)
            else if (input == "cpuinfo")
            {
                Sysinfo.DisplayCPUInfo();
            }

            // EXEC (Execute a XenOS Shell Script)
            else if (input.StartsWith("exec "))
            {
                try
                {
                    var path = input.Substring(5);
                    AppExecutor.ExecuteShellScript(path);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }

            // APP (Execute a .NET application)
            else if (input.StartsWith("app "))
            {                
                AppExecutor.ExecuteDotNetApp(input);
            }

            // BASIC (Execute BASIC scripts)
            else if (input.StartsWith("basic "))
            {
                var path = input.Substring(6);
                AppExecutor.ExecuteBasicApp(path);
            }

            // SHOWKEY (Display the key that gets pressed)
            else if (input == "showkey")
            {
                ShowKey.Start();
            }

            // ELF (Execute an ELF [Executable and linkable format] application)
            else if (input.StartsWith("elf "))
            {
                ELF.ExecuteELF(input);
            }

            // TESTELF (Test the elf executor)
            else if (input == "testelf")
            {
                ELF.ELFTest();
            }

            // LOGOUT (Log out of the computer)
            else if (input == "logout")
            {
                CMD();
            }

            // ADDUSER (Add a user to the userlist)
            else if (input == "adduser")
            {
                LoginManager.AddUser();
            }

            // RMUSER (Remove a user from the userlist)
            else if (input == "rmuser")
            {
                LoginManager.RemoveUser();
            }

            // CHGPSWD (Change a user's password)
            else if (input == "chgpswd")
            {
                LoginManager.ChangePassword();
            }




            /* POWER COMMANDS */
            // SHUTDOWN
            else if (input == "shutdown")
            {
                KeepCMDOpen = false;
                Power.Power.Shutdown();
                Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
            }

            // REBOOT
            else if (input == "reboot")
            {
                KeepCMDOpen = false;
                Power.Power.Reboot();
                Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
            }

            /* FILESYSTEM COMMANDS */
            // LS (FS Listing)
            else if (input == "ls")
            {
                FileSystem.DirectoryListing();
            }

            // CD (Change Directory)
            else if (input.StartsWith("cd "))
            {
                var path = input.Substring(3);
                FileSystem.ChangeDirectory(path);
            }

            // CAT
            else if (input.StartsWith("cat "))
            {
                var path = input.Substring(4);
                FileSystem.ShowFileContents(path);
            }

            // MKDIR (Make directory)
            else if (input.StartsWith("mkdir "))
            {
                var path = input.Substring(6);
                FileSystem.MakeDirectory(path);
            }

            // MKF (Make file)
            else if (input.StartsWith("mkf "))
            {
                var path = input.Substring(4);
                FileSystem.MakeFile(path);
            }

            // RM (Remove file)
            else if (input.StartsWith("rm "))
            {
                var path = input.Substring(3);
                FileSystem.RemoveFile(path);
            }

            // RMDIR (Remove directory)
            else if (input.StartsWith("rmdir "))
            {
                var path = input.Substring(6);
                FileSystem.RemoveDirectory(path);
            }

            // EDIT (edit file)
            else if (input.StartsWith("edit "))
            {
                var path = input.Substring(5);
                FileSystem.EditFile(path);
            }

            // MV (Move file)
            else if (input.StartsWith("mv "))
            {
                var path = input.Substring(3).Split(' ')[0];
                var dest = input.Substring(3).Split(' ')[1];
                FileSystem.MoveFile(path, dest);
            }

            // CP (Copy file)
            else if (input.StartsWith("cp "))
            {
                var path = input.Substring(3).Split(' ')[0];
                var dest = input.Substring(3).Split(' ')[1];
                FileSystem.CopyFile(path, dest);
            }

            // APPEND (Append text to file)
            else if (input.StartsWith("append "))
            {
                var path = input.Substring(7).Split(' ')[0];
                var txt = input.Substring(7 + path.Length + 1);
                FileSystem.AppendToFile(path, txt);
            }

            // WRITE (Write text to file)
            else if (input.StartsWith("write "))
            {
                var path = input.Substring(6).Split(' ')[0];
                var txt = input.Substring(6 + path.Length + 1);
                FileSystem.WriteToFile(path, txt);
            }

            // DSK (List disks)
            else if (input == "dsk")
            {
                DriveInformation.ListDrives();
            }

            // VLIST (List mounted volumes)
            else if (input == "vlist")
            {
                DriveInformation.ListVolumes();
            }

            // FORMAT (Format a disk)
            else if (input.StartsWith("format"))
            {
                if (input.Length > 7 && input.StartsWith("format "))
                {
                    FileSystem.FormatDrive(input.Substring(7));
                }
                else
                {
                    Console.WriteLine("You need to specify a drive!");
                }                
            }

            // MKPART (Make a partition)
            else if (input == "mkpart")
            {
                Console.Write("Disk to partition >> ");
                var disknum = Console.ReadLine();

                Console.Write("Partition size >> ");
                var partsize = Console.ReadLine();
                FileSystem.MakePartition(disknum, Int32.Parse(partsize));
            }

            // LPART (List partitions on a disk)
            else if (input == "lpart")
            {
                Console.Write("Drive letter >> ");
                FileSystem.ListPartitions(Console.ReadLine());
            }




            /* NETWORKING COMMANDS */
            // IPADDR (IP address)
            else if (input == "ipaddr")
            {
                NetInfo.DisplayIPAddress();
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
                        CosmosFtpServer.FtpServer ftp = new CosmosFtpServer.FtpServer(Drivers.Drivers.vfs, "0:\\");
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
            else if (input == "pootis")
            {
                Console.WriteLine("POOTIS!!!!!1!11!!!11!111!!!!!");
            }

            // BEEP
            else if (input.StartsWith("beep "))
            {
                var freq = input.Substring(5);
                try
                {
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
                var path = input.Substring(6);
                if (File.Exists(Path.Combine(Shell.CWD, path)))
                {
                    Audio.AudioPlayer audioPlayer = new Audio.AudioPlayer();
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
                Audio.AudioPlayer player = new Audio.AudioPlayer();
                player.PlayWAVFromBytes(StartupSound);
            }



            /* TIME/DATE COMMANDS */
            // TIME
            else if (input == "time")
            {
                DateTime.ShowTime();
            }

            // DATE
            else if (input == "date")
            {
                DateTime.ShowDate();
            }

            // DATETIME
            else if (input == "datetime")
            {
                DateTime.ShowDateTime();
            }



            /* TEST COMMANDS */
            else if (input == "fib")
            {
                Commands.Fibonacci.Run();
            }

            else if (input == "action")
            {
                Action action = new Action(() =>
                {
                    Console.WriteLine("Action test complete.");
                });
                action.Invoke();
            }



            /* GRAPHICS COMMANDS */
            // GUI (Start the graphical user interface)
            else if(input == "gui")
            {
                Graphics.Graphics graphics = new Graphics.Graphics();
                graphics.Run();
            }



            /* OTHER COMMANDS */
            // AMOGUS (sussy)
            else if (input == "amogus")
            {
                Console.WriteLine("SUSSY BAKA LMAO (You're sus)");
            }

            // GAMES (Start playing games)
            else if (input == "games")
            {
                Other.Games.Games games = new Other.Games.Games();
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

            // BAD COMMAND
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid command: \"" + input + "\"");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
