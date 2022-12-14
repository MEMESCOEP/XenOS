using IL2CPU.API.Attribs;
using System.IO;

namespace XenOS.Code.Sys.Helpers
{
    internal class Libraries
    {
        [ManifestResourceStream(ResourceName = "XenOS.DLLs.mscorlib.dll")]
        private readonly static byte[] mscorlib;

        [ManifestResourceStream(ResourceName = "XenOS.TestApps.HelloWorld.exe")]
        private readonly static byte[] HelloWorldExample;

        [ManifestResourceStream(ResourceName = "XenOS.TestApps.ELF_TEST.bin")]
        private readonly static byte[] ELFTest;

        public static void CheckLibraries()
        {
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

            if (!Directory.Exists(@"0:\SETTINGS\"))
            {
                try
                {
                    Directory.CreateDirectory(@"0:\SETTINGS\");
                }
                catch
                {

                }
                if (!File.Exists(@"0:\SETTINGS\SETTINGS"))
                {
                    try
                    {
                        File.WriteAllText(@"0:\SETTINGS\SETTINGS", "$Default\n$Default\n$Default\n$Default");

                    }
                    catch
                    {

                    }
                }
                if (!File.Exists(@"0:\SETTINGS\users"))
                {
                    try
                    {
                        File.CreateText(@"0:\SETTINGS\users");

                    }
                    catch
                    {

                    }
                }
                if (!File.Exists(@"0:\SETTINGS\login"))
                {
                    try
                    {
                        File.CreateText(@"0:\SETTINGS\login");

                    }
                    catch
                    {

                    }
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

        }
    }
}
