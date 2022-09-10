using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class GUI_Console
    {
        // Variables
        public char[] command;
        public Color textColor = Color.White;

        // Functions
        static string CharArrayToString(char[] arr)
        {
            // string() is a used to 
            // convert the char array
            // to string
            string s = new string(arr);

            return s;
        }

        public void init(Canvas canvas)
        {
            return;
            CustomConsole console = new CustomConsole();
            GUI gui = new GUI();
            //gui.MakeWindow(320, 240, 400, 100, Color.Gray, Color.Black, Color.Black, "Console", 2);
            if(GUI.windows.Count > 0)
            {
                foreach(var window in GUI.windows)
                {
                    if(window.WindowID == 2)
                    {
                        //gui.DrawString(canvas, CharArrayToString(command), new Pen(textColor), window.WindowPosX + 10, window.WindowPosX + 60, "OpenSans", 16f);
                    }
                }
            }
        }
    }
}
