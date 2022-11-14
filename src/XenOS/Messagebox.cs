using System.Drawing;
using XSharp.x86.Emitters;

namespace XenOS
{
    public class Messagebox
    {
        public static string Title = "";
        public int MessageBoxHeight = 48;
        public string LongestPart = "";

        /* Functions */
        public void CreateMessageBox(string title, string contents)
        {
            int len = 0;
            foreach (var part in contents.Split("\n"))
            {
                MessageBoxHeight += 16;
                if(part.Length > len)
                {
                    LongestPart = part;
                    len = part.Length;
                }
            }

            Title = title;
            GUI.MakeWindow(LongestPart.Length * 8 + 20, MessageBoxHeight, Shell.ScreenWidth / 2 - 320, Shell.ScreenHeight / 2 - MessageBoxHeight, Color.Gray, Color.Black, Color.White, title, GUI.windows.Count);
            foreach(var window in GUI.windows)
            {
                if (window.Title == title)
                {
                    if (contents.Contains("\n"))
                    {
                        int y_pos = 10;
                        foreach(var part in contents.Split("\n"))
                        {
                            GUI.windows[GUI.windows.IndexOf(window)].MakeNewStringElement(part, 15, y_pos);
                            y_pos += 12;
                        }
                    }
                    else
                    {
                        GUI.windows[GUI.windows.IndexOf(window)].MakeNewStringElement(contents, 15, 10);
                    }

                    GUI.windows[GUI.windows.IndexOf(window)].IsDraggable = false;
                    GUI.windows[GUI.windows.IndexOf(window)].MakeNewButtonElement("OK", 15, 70, CloseMessageBox, GUI.windows.IndexOf(window));
                    break;
                }
            }       
        }

        public static void CloseMessageBox()
        {
            try
            {
                foreach (var window in GUI.windows)
                {
                    if (window.WindowID == window.buttonElements[0].WindowID)
                    {
                        Kernel.DEBUGGER.SendMessageBox(GUI.windows.IndexOf(window).ToString() + ", " + window.buttonElements[0].WindowID.ToString());
                        window.CloseWindow(window);
                    }
                }
            }
            catch
            {

            }
        }
    }
}