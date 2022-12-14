using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenOS.Code.Sys.Boot;

namespace XenOS.Code.Information
{
    internal class About
    {
        public static void ShowInfo()
        {
            Console.WriteLine(Shell.Logo + "\n");
            Console.WriteLine(Shell.OsName + " (" + Shell.Version + ")\n");
        }
    }
}
