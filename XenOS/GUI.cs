using System;
using System.Drawing;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using CosmosTTF;
using System.Collections.Generic;

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

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.xp_move.bmp")]
        static byte[] MoveWindowIcon;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.close_window.bmp")]
        static byte[] CloseWindowIcon;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.clock.bmp")]
        static byte[] ClockIcon;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.notepad.bmp")]
        static byte[] NotepadIcon;

        [ManifestResourceStream(ResourceName = "XenOS.Fonts.OpenSans-Regular.ttf")]
        static byte[] FontData;

        public bool DrawNormalCursors = true;
        Bitmap MouseCursor = new Bitmap(mouse_cursor_array);
        Bitmap MouseCursorRC = new Bitmap(mouse_cursor_array_rc);
        Bitmap MouseCursorLC = new Bitmap(mouse_cursor_array_lc);
        Bitmap WallpaperBmp = new Bitmap(Wallpaper);
        Bitmap PowerOff = new Bitmap(PowerOffIcon);
        Bitmap Restart = new Bitmap(RestartIcon);
        Bitmap ReturnToConsole = new Bitmap(RTCIcon);
        Bitmap MoveWindow = new Bitmap(MoveWindowIcon);
        Bitmap Clock = new Bitmap(ClockIcon);
        Bitmap Notepad = new Bitmap(NotepadIcon);
        public static Bitmap CloseWindow = new Bitmap(CloseWindowIcon);
        public Bitmap CursorBMP = new Bitmap(mouse_cursor_array);
        Cosmos.System.Graphics.Point topleft = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point PowerIcon = new Cosmos.System.Graphics.Point(10, 10);
        Cosmos.System.Graphics.Point RestartIconPos = new Cosmos.System.Graphics.Point(10, 50);
        Cosmos.System.Graphics.Point RTCpos = new Cosmos.System.Graphics.Point(10, 90);
        Cosmos.System.Graphics.Point Clockpos = new Cosmos.System.Graphics.Point(10, 130);
        Cosmos.System.Graphics.Point Notepadpos = new Cosmos.System.Graphics.Point(10, 170);
        int prevX = 0, prevY = 0;
        public static List<WindowManager> windows = new List<WindowManager>();
        bool MouseClicked = false;

        // Functions
        public void DrawString(Canvas canvas, string str, Pen pen, int x, int y, string font, float size)
        {
            TTFManager.DrawStringTTF(canvas, pen, str, font, size, new Cosmos.System.Graphics.Point(x, y));
        }

        public void DrawStrings(Canvas canvas, WindowManager window)
        {
            if (windows.Count > 0)
            {
                if (window.WindowID == 0)
                {
                    DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), new Pen(Color.Black), window.WindowPosX + 10, window.WindowPosY + 60, "OpenSans", 30f);
                }
                /*foreach (var window in windows)
                {
                    
                }*/
            }
        }

        public void Draw(Canvas canvas)
        {
            canvas.DrawImage(WallpaperBmp, topleft);

            if (windows.Count > 0)
            {
                foreach (var window in windows)
                {
                    window.DrawWindow(canvas);
                }
            }
            canvas.DrawImageAlpha(PowerOff, PowerIcon);
            canvas.DrawImageAlpha(Restart, RestartIconPos);
            canvas.DrawImageAlpha(ReturnToConsole, RTCpos);
            canvas.DrawImageAlpha(Clock, Clockpos);
            canvas.DrawImageAlpha(Notepad, Notepadpos);
            canvas.DrawImageAlpha(CursorBMP, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
        }

        public void MakeWindow(int width, int height, int posX, int posY, Color TitleBarColor, Color TitleColor, Color windowColor, string Title, int windowID)
        {
            WindowManager windowManager = new WindowManager();
            windowManager.WindowWidth = width;
            windowManager.WindowHeight = height;
            windowManager.WindowPosX = posX;
            windowManager.WindowPosY = posY;
            windowManager.Title = Title;
            windowManager.TitlebarColor = TitleBarColor;
            windowManager.windowColor = windowColor;
            windowManager.TitleColor = TitleColor;
            windowManager.WindowID = windowID;
            windowManager.ActiveWindow = true;
            windowManager.init();
        }

        public void INIT()
        {
            int ScreenWidth = Shell.ScreenWidth;
            int ScreenHeight = Shell.ScreenHeight;
            Console.WriteLine("GUI started.");
            TTFManager.RegisterFont("OpenSans", FontData);
            Canvas canvas;
            Console.WriteLine("[INFO -> GUI] >> Checking if the Bochs Graphics Adapter (BGA) exists...");
            if (FullScreenCanvas.BGAExists())
            {
                Console.WriteLine("[INFO -> GUI] >> Using VGACanvas.");
                canvas = new VGACanvas();
            }
            else
            {
                Console.WriteLine("[INFO -> GUI] >> Using the best canvas for the current graphics controller.");
                canvas = FullScreenCanvas.GetFullScreenCanvas();
            }
            Console.WriteLine("[INFO -> GUI] >> Current Graphics Controller: " + canvas.Name());
            Console.Write("Press ESCAPE to exit the gui.\n");
            canvas.Mode = new Mode(Shell.ScreenWidth, Shell.ScreenHeight, ColorDepth.ColorDepth32);
            canvas.Clear(Color.Green);
            if (windows.Count == 0)
            {
                MakeWindow(100, 75, 100, 100, Color.Gray, Color.Black, Color.White, "Clock", 0);
                MakeWindow(320, 240, 400, 100, Color.Gray, Color.Black, Color.White, "Notepad", 1);
            }
            Cosmos.System.MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
            Cosmos.System.MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - 10;
            Cosmos.System.MouseManager.MouseSensitivity = 1;
            Draw(canvas);

            while (true)
            {
                try
                {
                    // Move Window
                    if(windows.Count > 0)
                    {
                        foreach (var window in windows)
                        {
                            if (window.CheckIfDragged() == true)
                            {
                                DrawNormalCursors = false;
                                while (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                                {
                                    CursorBMP = MoveWindow;
                                    if ((Cosmos.System.MouseManager.X + window.WindowWidth <= ScreenWidth + 25))
                                    {
                                        window.WindowPosX = (int)Cosmos.System.MouseManager.X - 25;
                                    }
                                    if ((Cosmos.System.MouseManager.Y + window.WindowHeight <= ScreenHeight))
                                    {
                                        window.WindowPosY = (int)Cosmos.System.MouseManager.Y - 5;
                                    }
                                    Draw(canvas);
                                    
                                    foreach (var window2 in windows)
                                    {
                                        DrawStrings(canvas, window2);
                                    }
                                    DrawString(canvas, "Graphics: " + canvas.Name(), new Pen(Color.Red), 10, ScreenHeight - 16, "OpenSans", 20f);
                                    DrawString(canvas, "THIS GUI IS A WIP; THERE WILL BE BUGS.", new Pen(Color.Red), 10, ScreenHeight - 32, "OpenSans", 20f);
                                    canvas.DrawImageAlpha(CursorBMP, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                                   
                                    canvas.Display();
                                    Cosmos.Core.Memory.Heap.Collect();
                                    if (Cosmos.System.MouseManager.MouseState != Cosmos.System.MouseState.Left)
                                    {
                                        CursorBMP = MouseCursor;
                                        break;
                                    }
                                }
                                CursorBMP = MoveWindow;
                            }
                            else if (window.CheckIfClosed() == true)
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                                {
                                    CursorBMP = MouseCursor;
                                    windows.Remove(window);
                                }
                                CursorBMP = MoveWindow;
                            }
                            else
                            {
                                CursorBMP = MouseCursor;
                            }
                        }

                    }

                    // Detect if the mouse was moved or clicked
                    if (prevX != (int)Cosmos.System.MouseManager.X || prevY != (int)Cosmos.System.MouseManager.Y || Cosmos.System.MouseManager.MouseState != Cosmos.System.MouseManager.LastMouseState)
                    {
                        prevX = (int)Cosmos.System.MouseManager.X;
                        prevY = (int)Cosmos.System.MouseManager.Y;
                    }

                    // Draw normal cursors
                    if (DrawNormalCursors)
                    {
                        if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Right)
                        {
                            CursorBMP = MouseCursorRC;
                        }
                        else if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                        {
                            CursorBMP = MouseCursorLC;
                        }
                        else
                        {
                            CursorBMP = MouseCursor;
                        }
                    }

                    // Detect presses for power, reboot, and RTC button(s).
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
                            CursorBMP = MouseCursorLC;
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
                            CursorBMP = MouseCursorLC;
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
                            CursorBMP = MouseCursorLC;
                        }
                    }
                    else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (110 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 160))
                    {
                        DrawNormalCursors = false;
                        if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                        {
                            MakeWindow(100, 75, 100, 100, Color.Gray, Color.Black, Color.White, "Clock", 0);
                            MouseClicked = true;
                        }
                        else
                        {
                            CursorBMP = MouseCursorLC;
                        }
                    }
                    else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (160 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 210))
                    {
                        DrawNormalCursors = false;
                        if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                        {
                            MakeWindow(320, 240, 400, 100, Color.Gray, Color.Black, Color.White, "Notepad", 1);
                            MouseClicked = true;
                        }
                        else
                        {
                            CursorBMP = MouseCursorLC;
                        }
                    }
                    else
                    {
                        DrawNormalCursors = true;
                    }

                    if (MouseClicked)
                    {
                        if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.None)
                        {
                            MouseClicked = false;
                        }
                    }

                    // Display everything
                    Draw(canvas);
                    foreach (var window in windows)
                    {
                        DrawStrings(canvas, window);
                    }
                    DrawString(canvas, "Graphics: " + canvas.Name(), new Pen(Color.Red), 10, Shell.ScreenHeight - 16, "OpenSans", 20f);
                    DrawString(canvas, "THIS GUI IS A WIP; THERE WILL BE BUGS.", new Pen(Color.Red), 10, Shell.ScreenHeight - 32, "OpenSans", 20f);
                    canvas.Display();
                    Cosmos.Core.Memory.Heap.Collect();

                    // If the escape key is pressed, exit the gui
                    if (Console.KeyAvailable)
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        {
                            canvas.Disable();
                            break;
                        }
                    }
                }
                catch
                {

                }                
            }
        }
    }
}
