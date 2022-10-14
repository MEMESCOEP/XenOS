using System;
using System.Drawing;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using CosmosTTF;
using System.Collections.Generic;
using System.IO;
using Cosmos.System.Network.Config;

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

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.XenOS_LOGO.bmp")]
        static byte[] LogoData;

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
        Bitmap XenosLogo = new Bitmap(LogoData);
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
        Pen TaskbarPen = new Pen(Color.DarkRed);
        Pen BlackPen = new Pen(Color.Black);
        Cosmos.System.Graphics.Point TaskbarLocation = new Cosmos.System.Graphics.Point(0, Shell.ScreenHeight - 40);
        Cosmos.System.Graphics.Point ClockTimeLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point ClockDateLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point TextLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point IconPos = new Cosmos.System.Graphics.Point(0, 0);

        int DiskCount = 0;

        // Functions
        public void DrawString(Canvas canvas, string str, Pen pen, int x, int y, string font, float size)
        {
            TextLocation.X = x;
            TextLocation.Y = y;

            //TTFManager.DrawStringTTF(canvas, pen, str, font, size, TextLocation);
            canvas.DrawString(str, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, pen, new Cosmos.System.Graphics.Point(x, y));
        }

        public void DrawStrings(Canvas canvas, WindowManager window)
        {
            if (windows.Count > 0)
            {
                if (window.Title == "Clock")
                {
                    ClockTimeLocation.X = window.WindowPosX + 10;
                    ClockTimeLocation.Y = window.WindowPosY + 50;
                    ClockDateLocation.X = window.WindowPosX + 10;
                    ClockDateLocation.Y = window.WindowPosY + 70;
                    DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), BlackPen, ClockTimeLocation.X, ClockTimeLocation.Y, "Font", 30f);
                    DrawString(canvas, (Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year), BlackPen, ClockDateLocation.X, ClockDateLocation.Y, "Font", 30f);
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
                    var CPU_Brand = Cosmos.Core.CPU.GetCPUBrandString();

                    if (window.Title == "System Information")
                    {
                        if ((CPU_Brand.Length * 10) >= window.WindowWidth)
                        {
                            window.WindowWidth = (CPU_Brand.Length * 10);
                        }
                        DrawString(canvas, "RAM USAGE: " + Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) + " MB / " + Cosmos.Core.GCImplementation.GetAvailableRAM() + " MB", BlackPen, window.WindowPosX + 4, window.WindowPosY + 60, "OpenSans", 16f);
                        DrawString(canvas, "INSTALLED RAM: " + Cosmos.Core.CPU.GetAmountOfRAM() + " MB", BlackPen, window.WindowPosX + 4, window.WindowPosY + 75, "OpenSans", 16f);
                        //DrawString(canvas, "CPU NAME: " + CPU_Brand, BlackPen, window.WindowPosX + 4, window.WindowPosY + 90, "OpenSans", 16f);
                        //DrawString(canvas, "CPU VENDOR: " + Cosmos.Core.CPU.GetCPUVendorName(), BlackPen, window.WindowPosX + 4, window.WindowPosY + 105, "OpenSans", 16f);
                        DrawString(canvas, "DISKS INSTALLED: " + DiskCount, BlackPen, window.WindowPosX + 4, window.WindowPosY + 90, "OpenSans", 16f);
                        if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                        {
                            DrawString(canvas, "IPv4 ADDRESS: [No usable network devices!]", BlackPen, window.WindowPosX + 4, window.WindowPosY + 105, "OpenSans", 16f);
                        }
                        else
                        {
                            var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                            DrawString(canvas, "IPv4 ADDRESS: " + ip, BlackPen, window.WindowPosX + 4, window.WindowPosY + 105, "OpenSans", 16f);
                        }
                    }                       

                    DrawStrings(canvas, window);
                }
            }

            if (ShowMenu)
            {
                canvas.DrawFilledRectangle(TaskbarPen, new Cosmos.System.Graphics.Point(0, 0), 55, 210);
                canvas.DrawImageAlpha(PowerOff, PowerIcon);
                canvas.DrawImageAlpha(Restart, RestartIconPos);
                canvas.DrawImageAlpha(ReturnToConsole, RTCpos);
                canvas.DrawImageAlpha(Clock, Clockpos);
                canvas.DrawImageAlpha(Notepad, Notepadpos);
            }
            canvas.DrawFilledRectangle(TaskbarPen, TaskbarLocation, Shell.ScreenWidth, 40);
            canvas.DrawImageAlpha(XenosLogo, IconPos);
            DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35, "OpenSans", 20f);
            DrawString(canvas, (Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 20, "OpenSans", 20f);
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
            windowManager.TitleBarColor = TitleBarColor;
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
                DiskCount = Cosmos.System.FileSystem.VFS.VFSManager.GetVolumes().Count;

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
                    MakeWindow(100, 100, 100, 100, Color.Gray, Color.Black, Color.White, "Clock", 0);
                    MakeWindow(240, 214, 400, 100, Color.Gray, Color.Black, Color.White, "System Information", 1);
                }

                Cosmos.System.MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
                Cosmos.System.MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - 10;
                Cosmos.System.MouseManager.MouseSensitivity = 1;

                IconPos.X = 5;
                IconPos.Y = Shell.ScreenHeight - (int)PowerOff.Height - 4;

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
                                        /*if (windows.IndexOf(window) != 0)
                                        {
                                            Helpers.MoveItemAtIndex(windows, windows.IndexOf(window), windows.Count - 1);
                                        }*/

                                        window.WindowPosX = (int)Cosmos.System.MouseManager.X - 25;
                                        window.WindowPosY = (int)Cosmos.System.MouseManager.Y - 5;

                                        window.DrawWindow(canvas); 
                                        Draw(canvas);


                                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 2)
                                        {
                                            Cosmos.Core.Memory.Heap.Collect();
                                        }

                                        if (Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) >= Cosmos.Core.GCImplementation.GetAvailableRAM() - 5)
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
                                            Console.WriteLine("GUI ERROR: Out of memory!");
                                            break;
                                        }
                                    }
                                }

                                if (window.CheckIfClosed() && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                {
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
                            if(Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.None)
                            {
                                CursorBMP = MouseCursor;
                            }
                        }

                        /* Show the menu */
                        if (ShowMenu)
                        {
                            /* Detect button presses for power, reboot, etc */
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
                                    MakeWindow(100, 100, 100, 100, Color.Gray, Color.Black, Color.White, "Clock", 0);
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
                                    MakeWindow(240, 214, 400, 100, Color.Gray, Color.Black, Color.White, "System Information", 1);
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

                        /* Detect if the user pressed */
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

                        if (Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) >= Cosmos.Core.GCImplementation.GetAvailableRAM() - 5)
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
                            Console.WriteLine("GUI ERROR: Out of memory!");
                            break;
                        }

                        // If the escape key is pressed, exit the gui
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey().Key;

                            if (key == ConsoleKey.Escape)
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

                            if (key == ConsoleKey.LeftWindows || key == ConsoleKey.RightWindows)
                            {
                                ShowMenu = !ShowMenu;
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