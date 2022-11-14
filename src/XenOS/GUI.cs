/* Directives */
using System;
using System.Drawing;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using CosmosTTF;
using System.Collections.Generic;
using System.IO;
using Cosmos.System.Network.Config;

/* Namespaces */
namespace XenOS
{
    /* Classes */
    internal class GUI
    {
        /* Variables */
        // Resources
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

        // Booleans
        public bool DrawNormalCursors = true;
        bool MouseClicked = false;
        bool ShowMenu = false;
        bool ExitGUI = false;

        // Bitmaps
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

        // Points
        Cosmos.System.Graphics.Point topleft = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point PowerIcon = new Cosmos.System.Graphics.Point(10, 10);
        Cosmos.System.Graphics.Point RestartIconPos = new Cosmos.System.Graphics.Point(10, 50);
        Cosmos.System.Graphics.Point RTCpos = new Cosmos.System.Graphics.Point(10, 90);
        Cosmos.System.Graphics.Point Clockpos = new Cosmos.System.Graphics.Point(10, 130);
        Cosmos.System.Graphics.Point Notepadpos = new Cosmos.System.Graphics.Point(10, 170);
        Cosmos.System.Graphics.Point TaskbarLocation = new Cosmos.System.Graphics.Point(0, Shell.ScreenHeight - 40);
        Cosmos.System.Graphics.Point ClockTimeLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point ClockDateLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point TextLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point IconPos = new Cosmos.System.Graphics.Point(0, 0);

        // Lists
        public static List<WindowManager> windows = new List<WindowManager>();

        // Canvases
        Canvas canvas;

        // Pens
        Pen TaskbarPen = new Pen(Color.DarkRed);
        Pen BlackPen = new Pen(Color.Black);

        // Integers
        int DiskCount = 0;
        int FrameCount = 0;
        int prevX = 0, prevY = 0;

        /* Functions */
        // Draw a string
        public void DrawString(Canvas canvas, string str, Pen pen, int x, int y)
        {
            TextLocation.X = x;
            TextLocation.Y = y;
            canvas.DrawString(str, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, pen, new Cosmos.System.Graphics.Point(x, y));
        }

        // Draw all strings
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
                    DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), BlackPen, ClockTimeLocation.X, ClockTimeLocation.Y);
                    DrawString(canvas, (Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year), BlackPen, ClockDateLocation.X, ClockDateLocation.Y);
                }
            }
        }

        // Draw everything
        public void Draw(Canvas canvas)
        {
            canvas.DrawImage(WallpaperBmp, topleft);

            if (windows.Count > 0)
            {
                foreach (var window in windows)
                {
                    if (windows.IndexOf(window) != windows.Count - 1)
                    {
                        window.ActiveWindow = false;
                    }
                    else if (window.CheckIfActive())
                    {
                        window.ActiveWindow = true;
                    }
                    else
                    {
                        window.ActiveWindow = true;
                    }

                    window.DrawWindow(canvas);
                    if (window.Title == "System Information")
                    {   
                        DrawString(canvas, "RAM USED: " + Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) + " MB/" + Cosmos.Core.GCImplementation.GetAvailableRAM() + " MB", BlackPen, window.WindowPosX + 4, window.WindowPosY + 45);
                        DrawString(canvas, "DISKS: " + DiskCount, BlackPen, window.WindowPosX + 4, window.WindowPosY + 60);
                        if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                        {
                            DrawString(canvas, "IPv4: NONE", BlackPen, window.WindowPosX + 4, window.WindowPosY + 75);
                        }
                        else
                        {
                            var ip = NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress;
                            DrawString(canvas, "IPv4: " + ip, BlackPen, window.WindowPosX + 4, window.WindowPosY + 75);
                        }
                        DrawString(canvas, "FRAMES: " + FrameCount, BlackPen, window.WindowPosX + 4, window.WindowPosY + 90);

                        //DrawString(canvas, "TOTAL RAM: " + Cosmos.Core.CPU.GetAmountOfRAM() + " MB", BlackPen, window.WindowPosX + 4, window.WindowPosY + 60);
                        /*if ((CPU_Brand.Length * 10) >= window.WindowWidth)
                        {
                            window.WindowWidth = (CPU_Brand.Length * 10);
                        }*/
                    }

                    DrawStrings(canvas, window);
                }
            }

            /* If the menu should be shown, draw any icons and/or shapes */

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
            if(Cosmos.HAL.RTC.Minute < 10)
            {
                DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":0" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
            }
            else
            {
                DrawString(canvas, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
            }
            DrawString(canvas, (Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 20);
            canvas.DrawImageAlpha(CursorBMP, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);            
            canvas.Display();
        }

        // Create a new window with the specified position, dimensions, colors, and title (and possibly window ID, if required)
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

        // Initialize the GUI and draw everything
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
                if (Cosmos.System.VMTools.IsVMWare)
                {
                    Console.WriteLine("[INFO -> GUI] >> Using the VMWareSVGAII graphics controller (Running on VMWare).");
                    canvas = new SVGAIICanvas();
                }
                else
                {
                    if (Cosmos.Core.VBE.IsAvailable())
                    {
                        canvas = new VBECanvas();
                    }
                    else
                    {
                        canvas = FullScreenCanvas.GetFullScreenCanvas();
                    }
                }

                Console.WriteLine("[INFO -> GUI] >> Current Graphics Controller: " + canvas.Name());
                Console.WriteLine("[INFO -> GUI] >> Screen resolution: {0}x{1}", canvas.Mode.Columns, canvas.Mode.Rows);
                Console.Write("Press ESCAPE to exit the gui.\n");
                canvas.Mode = new Mode(Shell.ScreenWidth, Shell.ScreenHeight, ColorDepth.ColorDepth32);
                canvas.Clear(Color.Green);

                Cosmos.System.MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
                Cosmos.System.MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - MouseCursor.Width;
                Cosmos.System.MouseManager.MouseSensitivity = Shell.MouseSensitivity;

                IconPos.X = 5;
                IconPos.Y = Shell.ScreenHeight - (int)PowerOff.Height - 4;

                Draw(canvas);
                bool ActiveWindow = false;
                bool typing = false;
                int count = 0;

                /* Drawing / Event handling loop */
                while (true)
                {
                    try
                    {
                        /* Make sure that there are 1 or more windows before attempting to draw them */
                        if (windows.Count > 0)
                        {
                            /* Loop to handle each window */
                            foreach (var window in windows)
                            {
                                /* If the user is typing something, change the "typing" variable */
                                typing = Console.KeyAvailable;

                                /* Detect if the window is being moved */
                                if (window.CheckIfDraggable() && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                {
                                    /* While the window is being moved, update the screen */
                                    while (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
                                    {
                                        DrawNormalCursors = false;
                                        ChangeCursor(MoveWindow);

                                        /* If the window isn't already last in the window list, move it. This ensures that the active window will appear on top of all other windows */
                                        if (windows.IndexOf(window) != windows.Count)
                                        {
                                            Helpers.MoveItemAtIndex(windows, windows.IndexOf(window), windows.Count);
                                        }

                                        /* Update the window positions to the mouse coords */
                                        window.WindowPosX = (int)Cosmos.System.MouseManager.X - 25;
                                        window.WindowPosY = (int)Cosmos.System.MouseManager.Y - 5;

                                        /* Draw everything */
                                        window.DrawWindow(canvas); 
                                        Draw(canvas);
                                        FrameCount++;

                                        /* Collect unused memory, but only when required */
                                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 2)
                                        {
                                            Cosmos.Core.Memory.Heap.Collect();
                                        }

                                        /* If there isn't enough memory to keep the GUI running, return to the console */
                                        if (Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) >= Cosmos.Core.GCImplementation.GetAvailableRAM() - 5)
                                        {                                   
                                            DisableGUI();
                                            Console.WriteLine("GUI ERROR: Out of memory!");
                                            break;
                                        }

                                        /* If the GUI is meant to be exittig, then break out of the loop */
                                        if (ExitGUI)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ResetCursor();
                                }

                                /* Check if the window should be closed and remove it from the windows list */
                                if (window.CheckIfClosed() && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                {
                                    windows.Remove(window);
                                    continue;
                                }
                            }
                        }

                        /* Detect if the mouse was moved or clicked */
                        if (prevX != (int)Cosmos.System.MouseManager.X || prevY != (int)Cosmos.System.MouseManager.Y || Cosmos.System.MouseManager.MouseState != Cosmos.System.MouseManager.LastMouseState)
                        {
                            prevX = (int)Cosmos.System.MouseManager.X;
                            prevY = (int)Cosmos.System.MouseManager.Y;
                        }

                        /* Draw normal cursors */
                        if (DrawNormalCursors)
                        {
                            if(Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.None)
                            {
                                ResetCursor();
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
                                    DisableGUI();
                                    Console.WriteLine("Shutting down...");
                                    Power power = new Power();
                                    power.shutdown();
                                    Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
                                }
                                else
                                {
                                    ChangeCursor(MouseCursorLC);
                                }
                            }
                            else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (40 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 80))
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                                {
                                    Console.WriteLine("Rebooting...");
                                    DisableGUI();
                                    Power power = new Power();
                                    power.reboot();
                                    Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
                                }
                                else
                                {
                                    ChangeCursor(MouseCursorLC);
                                }
                            }
                            else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (70 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 110))
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                                {
                                    Console.WriteLine("Returning to console...");
                                    DisableGUI();
                                    break;
                                }
                                else
                                {
                                    ChangeCursor(MouseCursorLC);
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
                                    ChangeCursor(MouseCursorLC);
                                }
                            }
                            else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (160 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 210))
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                                {
                                    MakeWindow(240, 128, 400, 100, Color.Gray, Color.Black, Color.White, "System Information", 1);
                                    MouseClicked = true;
                                }
                                else
                                {
                                    ChangeCursor(MouseCursorLC);
                                }
                            }
                            else
                            {
                                ResetCursor();
                            }
                        }

                        /* Detect if the user pressed the menu button or the left windows key */
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
                                ChangeCursor(MouseCursorLC);
                            }
                        }
                        else
                        {
                            DrawNormalCursors = true;
                        }

                        /* Detect if the user released a mouse button */
                        if (MouseClicked)
                        {
                            if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.None)
                            {
                                MouseClicked = false;
                            }
                        }

                        /* Display everything and increment the frame count */
                        Draw(canvas);
                        canvas.Display();
                        FrameCount++;

                        /* Collect memory that isn't in use, but only when required */
                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024 * 1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 2)
                        {
                            Cosmos.Core.Memory.Heap.Collect();
                        }

                        /* Exit the GUI if there isn't enough memory to continue */
                        if (Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) >= Cosmos.Core.GCImplementation.GetAvailableRAM() - 5)
                        {
                            DisableGUI();
                            Console.WriteLine("GUI ERROR: Out of memory!");
                            break;
                        }

                        /* If the escape key is pressed, exit the GUI. Also handle other key presses */
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey().Key;

                            if (key == ConsoleKey.Escape)
                            {
                                DisableGUI();
                                break;
                            }

                            if (key == ConsoleKey.LeftWindows || key == ConsoleKey.RightWindows)
                            {
                                ShowMenu = !ShowMenu;
                            }

                            if(key == ConsoleKey.F2)
                            {
                                MakeWindow(420, 69, 121, 121, Color.Gray, Color.Black, Color.White, "IDK lol", 1);
                            }

                            if (key == ConsoleKey.F1)
                            {
                                MakeWindow(320, 200, 121, 121, Color.Gray, Color.Black, Color.White, "XenOS Help", 1);
                            }
                        }

                        /* Exit the loop if the GUI is closing */
                        if (ExitGUI)
                        {
                            break;
                        }
                    }
                    catch
                    {

                    }

                    /* Exit the loop if the GUI is closing */
                    if (ExitGUI)
                    {
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                DisableGUI();
                Console.WriteLine("GUI ERROR: " + ex.Message);
            }
        }

        public void DisableGUI()
        {
            ExitGUI = true;
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
            Console.Clear();
        }

        public void ChangeCursor(Bitmap newCursor)
        {
            DrawNormalCursors = false;
            CursorBMP = newCursor;
        }

        public void ResetCursor()
        {
            DrawNormalCursors = true;
            CursorBMP = MouseCursor;
        }

        public void DisplayMenu()
        {
            
        }
    }
}