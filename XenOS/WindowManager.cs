using System;
using System.Drawing;
using Cosmos.System.Graphics;
using CosmosTTF;
using IL2CPU.API.Attribs;

namespace XenOS
{
    internal class WindowManager
    {
        // Variables
        [ManifestResourceStream(ResourceName = "XenOS.Fonts.segoeuil.ttf")]
        static byte[] FontData;

        public bool ActiveWindow = false;
        public int WindowID = 0;
        public int WindowPosX = 0;
        public int WindowPosY = 0;
        public int WindowWidth = 0;
        public int WindowHeight = 0;
        public string Title;
        public string WindowText;
        public Color windowColor;
        public Color TitlebarColor;
        public Color TitleColor;

        // Functions
        public bool IsBetween(double testValue, double bound1, double bound2)
        {
            return (testValue >= Math.Min(bound1, bound2) && testValue <= Math.Max(bound1, bound2));
        }

        public void init()
        {
            TTFManager.RegisterFont("Font", FontData);
            //WindowID = GUI.windows.Count;
            GUI.windows.Add(this);
        }

        public void DrawWindow(Canvas canvas)
        {
            Pen WindowPen = new Pen(windowColor);
            Pen TitlePen = new Pen(TitlebarColor);

            canvas.DrawFilledRectangle(WindowPen, new Cosmos.System.Graphics.Point(WindowPosX, WindowPosY), WindowWidth, WindowHeight);
            canvas.DrawFilledRectangle(TitlePen, new Cosmos.System.Graphics.Point(WindowPosX, WindowPosY), WindowWidth, 40);
            canvas.DrawImageAlpha(GUI.CloseWindow, WindowPosX + (WindowWidth - 20), WindowPosY + 12);

            //TTFManager.DrawStringTTF(canvas, new Pen(TitleColor), Title, "Font", 16f, new Cosmos.System.Graphics.Point(WindowPosX + ((WindowWidth / 2) - (Title.Length * 4)), WindowPosY + 20));
            canvas.DrawString(Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, new Pen(TitleColor), new Cosmos.System.Graphics.Point(WindowPosX + ((WindowWidth / 2) - (Title.Length * 4)), WindowPosY + 20));
        }

        public bool CheckIfDragged()
        {
            if (IsBetween(Cosmos.System.MouseManager.X, WindowPosX, WindowPosX + (WindowWidth - 40)) && IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + 40))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckIfClosed()
        {
            if (IsBetween(Cosmos.System.MouseManager.X, WindowPosX + WindowWidth, WindowPosX + (WindowWidth - 40)) && IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + 40))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckIfActive()
        {
            if (ActiveWindow)
            {
                return true;
            }
            else
            {
                if(Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                {
                    if (IsBetween(Cosmos.System.MouseManager.X, WindowPosX, WindowPosX + WindowWidth) && IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + WindowHeight))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
