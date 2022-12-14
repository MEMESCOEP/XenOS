/* DIRECTIVES */
using System;
using System.IO;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using XenOS.Code.Sys.Boot;

/* NAMESPACE(S) */
namespace XenOS.Code.Executable
{
    /* CLASSES */
    internal class AppExecutor
    {
        /* FUNCTIONS */
        // Execute a DOTNET Application
        public static void ExecuteDotNetApp(string input)
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

        // Execute an application that is written in BASIC
        public static void ExecuteBasicApp(string path)
        {
            
        }

        // Execute a shell script
        public static void ExecuteShellScript(string path)
        {
            if (File.Exists(Path.Combine(Shell.CWD, path)))
            {
                foreach (var line in File.ReadLines(Path.Combine(Shell.CWD, path)))
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
                        CustomConsole.Interpret(line);
                    }
                }
            }
            else
            {
                Console.WriteLine("File \"" + path + "\" doesn't exist!");
            }
        }

        // DOTNET Methods
        public static void WriteLine(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = (string)Stack[Stack.Length - 1].value;
            Console.WriteLine(str);
        }

        public static void ReadLine(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var prompt = Stack[Stack.Length - 1].value.ToString();
            Console.Write(prompt);
            returnValue = MethodArgStack.String(Console.ReadLine());
        }

        public static void ReadKey(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var prompt = Stack[Stack.Length - 1].value.ToString();
            Console.Write(prompt);
            returnValue = MethodArgStack.String(Console.ReadKey().KeyChar.ToString());
        }

        public static void ReadAllText(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var path = Stack[Stack.Length - 1].value.ToString();
            returnValue = MethodArgStack.String(File.ReadAllText(path));
        }

        public static void WriteAllText(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            File.WriteAllText(Stack[0].ToString(), Stack[1].ToString());
        }

        public static void DeleteFile(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            File.Delete(Stack[0].ToString());
        }

        public static void CreateFile(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            File.Create(Stack[0].ToString());
        }

        public static void Write(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = (string)Stack[Stack.Length - 1].value;
            Console.Write(str);
        }
    }
}
