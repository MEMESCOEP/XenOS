

using PrismELF;
using System;
using System.IO;
using XenOS.Code.Sys.Helpers;
using XSharp.Assembler.x86;

namespace XenOS.Code.Commands.Exec
{
    internal class ELF
    {
        private static byte[] UnmanagedString(string s)
        {
            var re = new byte[s.Length + 1];

            for (int i = 0; i < s.Length; i++)
            {
                re[i] = (byte)s[i];
            }

            re[s.Length + 1] = 0; //c requires null terminated string
            return re;
        }

        public static void ELFTest()
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

        public static void ExecuteELF(string input)
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
    }
}
