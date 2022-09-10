using System;
using System.Drawing;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using CosmosTTF;
using System.Collections.Generic;
using System.IO;

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

        [ManifestResourceStream(ResourceName = "XenOS.Art.Wallpapers.BG_1360x768.bmp")]
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

        [ManifestResourceStream(ResourceName = "XenOS.Fonts.segoeuil.ttf")]
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
        Canvas canvas;
        bool ShowMenu = false;

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
                    canvas.DrawStringTTF(new Pen(Color.Black), (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), "Font", 30f, new Cosmos.System.Graphics.Point(window.WindowPosX + 10, window.WindowPosY + 60));
                }
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
                    if(window.Title == "Hardware Monitor")
                    {
                        DrawString(canvas, "RAM USAGE: " + Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) + " MB / " + Cosmos.Core.GCImplementation.GetAvailableRAM() + " MB", new Pen(Color.Black), window.WindowPosX + 4, window.WindowPosY + 60, "OpenSans", 16f);
                        DrawString(canvas, "INSTALLED RAM: " + Cosmos.Core.CPU.GetAmountOfRAM() + " MB", new Pen(Color.Black), window.WindowPosX + 4, window.WindowPosY + 75, "OpenSans", 16f);
                        DrawString(canvas, "CPU NAME: " + Cosmos.Core.CPU.GetCPUBrandString(), new Pen(Color.Black), window.WindowPosX + 4, window.WindowPosY + 90, "OpenSans", 16f);
                    }                       

                    DrawStrings(canvas, window);
                }
            }

            if (ShowMenu)
            {
                canvas.DrawImageAlpha(PowerOff, PowerIcon);
                canvas.DrawImageAlpha(Restart, RestartIconPos);
                canvas.DrawImageAlpha(ReturnToConsole, RTCpos);
                canvas.DrawImageAlpha(Clock, Clockpos);
                canvas.DrawImageAlpha(Notepad, Notepadpos);
            }
            canvas.DrawFilledRectangle(new Pen(Color.DarkRed), new Cosmos.System.Graphics.Point(0, Shell.ScreenHeight - 40), Shell.ScreenWidth, 40);
            canvas.DrawImageAlpha(PowerOff, new Cosmos.System.Graphics.Point(5, Shell.ScreenHeight - (int)PowerOff.Height - 4));
            DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), new Pen(Color.Black), Shell.ScreenWidth - 60, Shell.ScreenHeight - 15, "OpenSans", 20f);
            canvas.DrawImageAlpha(CursorBMP, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
            canvas.Display();
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
            windowManager.init();
        }

        public void INIT()
        {
            try
            {
                int ScreenWidth = Shell.ScreenWidth;
                int ScreenHeight = Shell.ScreenHeight;
                var font = Cosmos.System.Graphics.Fonts.PCScreenFont.Default;

                try
                {
                    if (File.Exists("0:\\SETTINGS\\wallppr"))
                    {
                        string path = Helpers.GetLine("0:\\SETTINGS\\wallppr", 0);
                        WallpaperBmp = new Bitmap(File.ReadAllBytes(path));
                    }
                }
                catch
                {
                    WallpaperBmp = new Bitmap(Wallpaper);
                }

                Console.WriteLine("GUI started.");
                TTFManager.RegisterFont("OpenSans", FontData);
                Console.WriteLine("[INFO -> GUI] >> Checking if the Bochs Graphics Adapter (BGA) exists...");

                Console.WriteLine("[INFO -> GUI] >> Using the best canvas for the current graphics controller.");
                canvas = FullScreenCanvas.GetFullScreenCanvas();

                Console.WriteLine("[INFO -> GUI] >> Current Graphics Controller: " + canvas.Name());
                Console.WriteLine("[INFO -> GUI] >> Screen resolution: {0}x{1}", canvas.Mode.Columns, canvas.Mode.Rows);
                Console.Write("Press ESCAPE to exit the gui.\n");
                canvas.Mode = new Mode(Shell.ScreenWidth, Shell.ScreenHeight, ColorDepth.ColorDepth32);
                canvas.Clear(Color.Green);

                if (windows.Count == 0)
                {
                    MakeWindow(100, 75, 100, 100, Color.Gray, Color.Black, Color.White, "Clock", 0);
                    MakeWindow(320, 240, 400, 100, Color.Gray, Color.Black, Color.White, "Hardware Monitor", 1);
                }

                Cosmos.System.MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
                Cosmos.System.MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - 10;
                Cosmos.System.MouseManager.MouseSensitivity = 1;

                //var strpit = new Cosmos.HAL.PIT.PITTimer(DrawStrPIT, 2500000, true);
                //Cosmos.HAL.Global.PIT.RegisterTimer(strpit);

                Draw(canvas);
                bool ActiveWindow = false;
                bool typing = false;
                int count = 0;

                while (true)
                {
                    try
                    {
                        // Move Window
                        if (windows.Count > 0)
                        {
                            foreach (var window in windows)
                            {
                                typing = Console.KeyAvailable;

                                if (window.CheckIfActive() && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                {
                                    ActiveWindow = true;
                                    window.ActiveWindow = ActiveWindow;
                                    ActiveWindow = false;
                                }

                                if (window.CheckIfDragged() && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                {
                                    while (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                                    {
                                        if (windows.IndexOf(window) != 0)
                                        {
                                            Helpers.MoveItemAtIndex(windows, windows.IndexOf(window), windows.Count - 1);
                                        }

                                        window.WindowPosX = (int)Cosmos.System.MouseManager.X - 25;
                                        window.WindowPosY = (int)Cosmos.System.MouseManager.Y - 5;
                                        window.DrawWindow(canvas); 
                                        Draw(canvas);

                                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 2)
                                        {
                                            Cosmos.Core.Memory.Heap.Collect();
                                        }
                                    }
                                }

                                if (window.CheckIfClosed() && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                {
                                    windows[windows.IndexOf(window)] = null;
                                    windows.Remove(window);
                                    continue;
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
                                //CursorBMP = MouseCursorRC;
                            }
                            else if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                            {
                                //CursorBMP = MouseCursorLC;
                            }
                            else
                            {
                                CursorBMP = MouseCursor;
                            }
                        }

                        if (ShowMenu)
                        {
                            // Detect presses for power, reboot, and RTC button(s).
                            if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (10 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 40))
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                                {
                                    canvas.Disable();
                                    Console.WriteLine("Shutting down...");
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
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
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
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
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
                                    MakeWindow(320, 240, 400, 100, Color.Gray, Color.Black, Color.White, "Hardware Monitor", 1);
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
                        }

                        if ((5 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (Shell.ScreenHeight - 40 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= Shell.ScreenHeight))
                        {
                            DrawNormalCursors = false;
                            if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                            {
                                ShowMenu = !ShowMenu;
                                MouseClicked = !MouseClicked;
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
                        canvas.Display();

                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024 * 1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 2)
                        {
                            Cosmos.Core.Memory.Heap.Collect();
                        }

                        // If the escape key is pressed, exit the gui
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                canvas.Disable();
                                canvas = null;
                                MouseCursor = null;
                                MouseCursorRC = null;
                                MouseCursorLC = null;
                                WallpaperBmp = null;
                                PowerOff = null;
                                Restart = null;
                                ReturnToConsole = null;
                                MoveWindow = null;
                                Clock = null;
                                Notepad = null;
                                CloseWindow = null;
                                CursorBMP = null;
                                windows = null;
                                break;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch(Exception ex)
            {
                canvas = FullScreenCanvas.GetFullScreenCanvas();
                if(canvas != null)
                {
                    canvas.Disable(); 
                    canvas = null;
                    MouseCursor = null;
                    MouseCursorRC = null;
                    MouseCursorLC = null;
                    WallpaperBmp = null;
                    PowerOff = null;
                    Restart = null;
                    ReturnToConsole = null;
                    MoveWindow = null;
                    Clock = null;
                    Notepad = null;
                    CloseWindow = null;
                    CursorBMP = null;
                    windows = null;
                    Console.WriteLine("GUI ERROR: " + ex.Message);
                }                
            }
        }
    }
}