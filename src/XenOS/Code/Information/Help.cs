using System;
using System.Data;

namespace XenOS.Code.Information
{
    internal class Help
    {
        // Functions
        public static void ShowTopics()
        {
            Console.Write("Topics:\nconsole\nsystem\nfilesystem\naudio\nnetwork\npower\n");
        }

        public static void ShowHelp()
        {
            Console.WriteLine("[== SYSTEM ==]\n1. cls\n2. sysinfo\n3. exec <file name>\n4. panic\n5. echo <text>\n6. timeout <ms>\n7. time\n8. about\n9. app <file name>\n10. showkey\n11. date\n12. datetime\n13. pci\n");
            Console.WriteLine("[== POWER ==]\n1. shutdown\n2. reboot\n");
            Console.WriteLine("[== FILESYSTEM ==]\n1. ls\n2. cd <dirname>\n3. mkdir <dirname>\n4. rmdir <dirname>\n5. mkf <filename>\n6. rm <filename>\n7. cat <filename>\n8. edit <filename>\n9. cp <src> <dest>\n10. mv <filename> <dirname>\n11. append <filename> <contents> (use '\\n' to make a new line)\n12. write <filename> <contents> (use '\\n' to make a new line)\n13. dsk\n14. vlist\n15. format <disk number>\n");
            Console.WriteLine("[== CONSOLE ==]\n1. beep <frequency>\n2. logout\n3. adduser\n4. rmuser\n5. chgpswd\n");
            Console.WriteLine("[== NETWORK ==]\n1. ipaddr\n2. urltoip <url>\n3. ftpserver\n4. ping <ip address>\n");
            Console.WriteLine("[== AUDIO ==]\n1. audio <filename>\n2. testaudio");
        }
    }
}
