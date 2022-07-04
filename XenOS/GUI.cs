using System;
using System.Drawing;
using System.Threading;
using Cosmos.Core;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace XenOS
{
    internal class GUI
    {
        // Variables
        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.xp_arrow.bmp")]
        static byte[] mouse_cursor_array;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.xp_pencil.bmp")]
        static byte[] mouse_cursor_array_rc;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.xp_link.bmp")]
        static byte[] mouse_cursor_array_lc;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Wallpapers.BG.bmp")]
        static byte[] Wallpaper;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.PowerOff.bmp")]
        static byte[] PowerOffIcon;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.Restart.bmp")]
        static byte[] RestartIcon;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.terminal.bmp")]
        static byte[] RTCIcon;

        public bool DrawNormalCursors = true;

        // Functions
        public void DrawString(Canvas canvas, string str, Pen pen, Pen BG, int x, int y, Cosmos.System.Graphics.Fonts.Font font)
        {
            canvas.DrawFilledRectangle(BG, new Cosmos.System.Graphics.Point(x, y), x + (str.Length * 4), 16);
            canvas.DrawString(str, font, pen, x, y);
        }

        public void INIT()
        {
            Console.Write("Press ESCAPE to exit the gui.\n");
            Canvas canvas = FullScreenCanvas.GetFullScreenCanvas();
            canvas.Mode = new Mode(Shell.ScreenWidth, Shell.ScreenHeight, ColorDepth.ColorDepth32);
            canvas.Clear(Color.Green);
            Cosmos.System.MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
            Cosmos.System.MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - 10;
            Cosmos.System.MouseManager.MouseSensitivity = 1;
            Bitmap MouseCursor = new Bitmap(mouse_cursor_array);
            Bitmap MouseCursorRC = new Bitmap(mouse_cursor_array_rc);
            Bitmap MouseCursorLC = new Bitmap(mouse_cursor_array_lc);
            Bitmap WallpaperBmp = new Bitmap(Wallpaper);
            Bitmap PowerOff = new Bitmap(PowerOffIcon);
            Bitmap Restart = new Bitmap(RestartIcon);
            Bitmap ReturnToConsole = new Bitmap(RTCIcon);
            Cosmos.System.Graphics.Point topleft = new Cosmos.System.Graphics.Point(0, 0);
            Cosmos.System.Graphics.Point PowerIcon = new Cosmos.System.Graphics.Point(10, 10);
            Cosmos.System.Graphics.Point RestartIconPos = new Cosmos.System.Graphics.Point(10, 50);
            Cosmos.System.Graphics.Point RTCpos = new Cosmos.System.Graphics.Point(10, 90);
            int prevX = 0, prevY = 0;
            canvas.DrawImage(WallpaperBmp, topleft);
            canvas.DrawImageAlpha(PowerOff, PowerIcon);
            canvas.DrawImageAlpha(Restart, RestartIconPos);
            canvas.DrawImageAlpha(ReturnToConsole, RTCpos);

            while (true)
            {
                if (prevX != (int)Cosmos.System.MouseManager.X || prevY != (int)Cosmos.System.MouseManager.Y || Cosmos.System.MouseManager.MouseState != Cosmos.System.MouseManager.LastMouseState)
                {
                    prevX = (int)Cosmos.System.MouseManager.X;
                    prevY = (int)Cosmos.System.MouseManager.Y;
                    canvas.DrawImage(WallpaperBmp, topleft);
                    canvas.DrawImageAlpha(PowerOff, PowerIcon);
                    canvas.DrawImageAlpha(Restart, RestartIconPos);
                    canvas.DrawImageAlpha(ReturnToConsole, RTCpos);
                }

                if (DrawNormalCursors)
                {
                    if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Right)
                    {
                        canvas.DrawImageAlpha(MouseCursorRC, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                    }
                    else if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                    {
                        canvas.DrawImageAlpha(MouseCursorLC, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                    }
                    else
                    {
                        canvas.DrawImageAlpha(MouseCursor, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                    }
                }                

                if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (10 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 40))
                {
                    DrawNormalCursors = false;
                    if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                    {
                        Console.WriteLine("Shutting down...");
                        canvas.Disable();
                        Power power = new Power();
                        power.shutdown();
                        Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
                        break;
                    }
                    else
                    {
                        canvas.DrawImageAlpha(MouseCursorLC, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                    }
                }
                else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (40 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 80))
                {
                    DrawNormalCursors = false;
                    if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                    {
                        Console.WriteLine("Rebooting...");
                        canvas.Disable();
                        Power power = new Power();
                        power.reboot();
                        Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
                        break;
                    }
                    else
                    {
                        canvas.DrawImageAlpha(MouseCursorLC, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                    }
                }
                else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (70 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 110))
                {
                    DrawNormalCursors = false;
                    if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                    {
                        Console.WriteLine("Returning to console...");
                        canvas.Disable();
                        break;
                    }
                    else
                    {
                        canvas.DrawImageAlpha(MouseCursorLC, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                    }
                }
                else
                {
                    DrawNormalCursors = true;
                }

                //DrawString(canvas, counter.ToString(), MousePen, pen, 10, 10, Cosmos.System.Graphics.Fonts.PCScreenFont.Default);

                canvas.Display();
                Cosmos.Core.Memory.Heap.Collect();

                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        canvas.Disable();
                        break;
                    }
                }
            }
        }
    }
}
