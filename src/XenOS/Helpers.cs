using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class Helpers
    {
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }

        public static double GetDistance(int p1, int p2)
        {
            return Math.Abs(p1 - p2);
        }

        public static void MoveItemAtIndexToFront<T>(List<T> list, int index)
        {
            T item = list[index];
            for (int i = index; i > 0; i--)
                list[i] = list[i - 1];
            list[0] = item;
        }

        public static void MoveItemAtIndex<T>(List<T> list, int OldIndex, int NewIndex)
        {
            T item = list[OldIndex];
            list.RemoveAt(OldIndex);
            list.Insert(NewIndex, item);
        }
    }
}
