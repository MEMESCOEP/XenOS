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
        public Color TitleBarColor;
        public Color TitleColor;
        public Cosmos.System.Graphics.Point TitlePos;
        public Cosmos.System.Graphics.Point TitlebarPos;
        public Cosmos.System.Graphics.Point BodyPos;
        Pen WindowPen;
        Pen TitleBarPen;
        Pen TitlePen;

        // Functions
        public bool IsBetween(double testValue, double bound1, double bound2)
        {
            return (testValue >= Math.Min(bound1, bound2) && testValue <= Math.Max(bound1, bound2));
        }

        public void init()
        {
            try
            {
                WindowID = GUI.windows.Count;
                GUI.windows.Add(this);
                this.WindowPen = new Pen(windowColor);
                this.TitlePen = new Pen(TitleColor);
                this.TitleBarPen = new Pen(TitleBarColor);
                this.TitlePos = new Cosmos.System.Graphics.Point(0, 0);
                this.TitlebarPos = new Cosmos.System.Graphics.Point(0, 0);
                this.BodyPos = new Cosmos.System.Graphics.Point(0, 0);
            }
            catch
            {

            }
        }

        public void DrawWindow(Canvas canvas)
        {
            try
            {
                if (WindowPen != null && TitlePen != null)
                {
                    this.BodyPos.X = WindowPosX;
                    this.BodyPos.Y = WindowPosY;
                    this.TitlebarPos.X = WindowPosX;
                    this.TitlebarPos.Y = WindowPosY;
                    this.TitlePos.X = WindowPosX + ((WindowWidth / 2) - (Title.Length * 4));
                    this.TitlePos.Y = WindowPosY + 20;

                    canvas.DrawFilledRectangle(WindowPen, BodyPos, WindowWidth, WindowHeight);
                    canvas.DrawFilledRectangle(TitleBarPen, TitlebarPos, WindowWidth, 40);
                    canvas.DrawImage(GUI.CloseWindow, WindowPosX + (WindowWidth - 20), WindowPosY + 12);

                    //TTFManager.DrawStringTTF(canvas, TitlePen, Title, "Font", 16f, TitlePos);
                    canvas.DrawString(Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, TitlePen, TitlePos); //new Cosmos.System.Graphics.Point(WindowPosX + ((WindowWidth / 2) - (Title.Length * 4)), WindowPosY + 20)
                }
            }
            catch
            {

            }
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
