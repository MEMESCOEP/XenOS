using System;

namespace XenOS
{
    internal class Help
    {
        // Functions
        public void ShowHelp()
        {
            Console.WriteLine("[== POWER ==]\n1. shutdown\n2. reboot\n\n[== FILESYSTEM ==]\n1. ls\n2. cd <dir name>\n3. mkdir <dir name>\n4. rmdir <dir name>\n5. mkf <file name>\n6. rm <file name>\n7. cat <file name>\n8. edit <file name>\n9. cp <src> <dest>\n\n[== SYSTEM ==]\n1. cls\n2. sysinfo\n3. exec <file name>4. panic\n\n[== NETWORK ==]\n1. ipaddr\n2. urltoip\n3. ftpserver");
            Console.Write("[PRESS ANY KEY TO CONTINUE]");
            Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("[== CONSOLE ==]\n1. beep <frequency>");
            Console.WriteLine();
        }
    }
}
