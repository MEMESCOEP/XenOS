using System;

namespace XenOS
{
    internal class Help
    {
        // Functions
        public void ShowHelp()
        {
            Console.WriteLine("[== POWER ==]\n1. shutdown\n2. reboot\n\n[== FILESYSTEM ==]\n1. ls\n2. cd <dirname>\n3. mkdir <dirname>\n4. rmdir <dirname>\n5. mkf <filename>\n6. rm <filename>\n7. cat <filename>\n8. edit <filename>\n9. cp <src> <dest>\n10. mv <filename> <dirname>\n11. append <filename> <contents> (use '\\n' to make a new line)\n12. write <filename> <contents> (use '\\n' to make a new line)\n13. dsk\n\n[== SYSTEM ==]\n1. cls\n2. sysinfo\n3. exec <file name>");
            Console.Write("\n[PRESS ANY KEY TO CONTINUE]");
            Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("[== SYSTEM ==]\n4. panic\n5. echo <text>\n6. timeout <ms>\n7. time\n8. about\n9. audio <filename>\n10. testaudio\n11. app <file name>\n\n[== CONSOLE ==]\n1. beep <frequency>\n2. logout\n3. adduser\n4. rmuser\n5. chgpswd\n\n[== NETWORK ==]\n1. ipaddr\n2. urltoip <url>\n3. ftpserver\n4. ping <ip address>\n\n");
            Console.Write("[PRESS ANY KEY TO CONTINUE]");
            Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("[== GRAPHICS ==]\n1. gui\n2. modes");
            Console.WriteLine();
        }
    }
}
