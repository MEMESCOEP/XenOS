using System;
using System.IO;

namespace XenOS.Code.Sys.Helpers
{
    internal class LoadSettings
    {
        public void Load()
        {
            if (File.Exists("0:\\SETTINGS\\settings"))
            {
                try
                {
                    string os_name = INIParser.ReadValue("SETTINGS", "OS_NAME", "0:\\SETTINGS\\settings");
                    string os_ver = INIParser.ReadValue("SETTINGS", "OS_VER", "0:\\SETTINGS\\settings");
                    string PlayStarupSound = INIParser.ReadValue("SETTINGS", "PlayStartupSound", "0:\\SETTINGS\\settings");
                    
                    if (os_name != "$Default" && os_name != "ERROR" && !string.IsNullOrEmpty(os_name) && !string.IsNullOrWhiteSpace(os_name))
                    {
                        Sys.Boot.Shell.OsName = os_name;
                    }

                    if (os_ver != "$Default" && os_name != "ERROR" && !string.IsNullOrEmpty(os_ver) && !string.IsNullOrWhiteSpace(os_ver))
                    {
                        Boot.Shell.Version = os_ver;
                    }

                    if (PlayStarupSound == "true" && PlayStarupSound != "$Default" && PlayStarupSound != "ERROR" && !string.IsNullOrEmpty(PlayStarupSound) && !string.IsNullOrWhiteSpace(PlayStarupSound))
                    {
                        Boot.CustomConsole.PlayStartupSound = "true";
                    }
                    else
                    {
                        Boot.CustomConsole.PlayStartupSound = "false";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load settings: " + ex.Message);
                }
            }
        }
    }
}
