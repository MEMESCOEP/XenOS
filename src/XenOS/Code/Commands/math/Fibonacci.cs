using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS.Code.Commands
{
    internal class Fibonacci
    {
        public static void Run()
        {
            try
            {
                List<string> fib_list = new List<string>();
                int a = 0, b = 1, c = 0, len = 999;
                Console.Write("{0} {1}", a, b);
                for (int i = 2; i < len; i++)
                {
                    c = a + b;
                    a = b;
                    b = c;
                    Console.Write(" {0}", c);
                    fib_list.Add(c.ToString());
                }
                File.WriteAllLines("0:\\FIB.txt", fib_list);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
            Console.WriteLine();
        }
    }
}
