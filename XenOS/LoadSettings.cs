using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class LoadSettings
    {
        public string OS_Name = "";

        public void Load()
        {
            if (File.Exists("0:\\SETTINGS\\settings"))
            {
                try
                {
                    if (Helpers.GetLine("0:\\SETTINGS\\settings", 1) != "$Default")
                    {
                        Shell.OsName = Helpers.GetLine("0:\\SETTINGS\\settings", 1);
                    }

                    if (Helpers.GetLine("0:\\SETTINGS\\settings", 2) != "$Default")
                    {
                        Shell.Version = Helpers.GetLine("0:\\SETTINGS\\settings", 2);
                    }

                    if (Helpers.GetLine("0:\\SETTINGS\\settings", 3) != "$Default")
                    {
                        CustomConsole.PlayStartupSound = Helpers.GetLine("0:\\SETTINGS\\settings", 3);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Failed to load settings!: " + ex.Message);
                }
            }
        }
    }
}
