/* Directives */
using Cosmos.Core.IOGroup;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Console = System.Console;

/* Namespaces */
namespace XenOS
{
    /* Classes */
    internal class GUI
    {
        /* Variables */
        // Resources
        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.mouse.bmp")]
        static byte[] mouse_cursor_array;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.xp_pencil.bmp")]
        static byte[] mouse_cursor_array_rc;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.xp_link.bmp")]
        static byte[] mouse_cursor_array_lc;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Wallpapers.BG_1024x768.bmp")]
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
        bool ShowFPS = false;
        bool typing = false;
        public bool SupportsLargeRes = false;

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
        public static Cosmos.System.Graphics.Point TextLocation = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point IconPos = new Cosmos.System.Graphics.Point(0, 0);

        // Lists
        public static List<WindowManager> windows = new List<WindowManager>();

        // Canvases
        public static Canvas canvas;

        // Pens
        Pen TaskbarPen = new Pen(System.Drawing.Color.DarkRed);
        Pen BlackPen = new Pen(System.Drawing.Color.Black);

        // Integers
        int FrameCount = 0;
        int prevX = 0, prevY = 0;
        int FPS = 0, TempFrameCount = 0;
        int Hour = 0, Minute = 0, Second = 0;
        int str_len_notepad = 32;

        // Window(s)
        WindowManager wind;

        /* Functions */
        // Draw a string
        public static void DrawString(Canvas canvas, string str, Pen pen, int x, int y)
        {
            TextLocation.X = x;
            TextLocation.Y = y;
            canvas.DrawString(str, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, pen, TextLocation);
            
        }

        // Draw all strings
        public void DrawStrings(Canvas canvas, WindowManager window)
        {
            if (windows.Count > 0)
            {
                if (window.Title == "Clock")
                {
                    window.EditStringElement(0, (Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second));
                    window.EditStringElement(1, (Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year));
                }

                if (window.Title == "System Information")
                {
                    window.EditStringElement(0, "RAM: " + Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) + "/" + Cosmos.Core.GCImplementation.GetAvailableRAM() + " MB");
                    if (Cosmos.HAL.NetworkDevice.Devices.Count >= 1)
                    {
                        window.EditStringElement(1, "IPv4: " + NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress);
                    }
                    //window.EditStringElement(2, "FRAMES: " + FrameCount);
                    window.EditStringElement(2, "FPS: " + FPS);
                }
            }
        }

        // Draw everything
        public void Draw(Canvas canvas)
        {
            try
            {
                /* If the PIT timer ticked, then update the time and FPS values */
                if (Drivers.PITTicked)
                {
                    FPS = TempFrameCount;
                    TempFrameCount = 0;
                    Hour = Cosmos.HAL.RTC.Hour;
                    Minute = Cosmos.HAL.RTC.Minute;
                    Second = Cosmos.HAL.RTC.Second;
                    Drivers.PITTicked = false;
                }
                else
                {
                    TempFrameCount++;
                }

                /* If the computer supports large resolutions, then draw the wallpaper. Otherwise, just draw a filled rectangle */
                if (SupportsLargeRes)
                {
                    canvas.DrawImage(WallpaperBmp, 0, 0);
                }
                else
                {
                    canvas.DrawFilledRectangle(new Pen(Color.Beige), topleft, Shell.ScreenWidth, Shell.ScreenHeight);
                }

                /* If there are any windows, draw them */
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
                            Helpers.MoveItemAtIndex(windows, windows.IndexOf(window), windows.Count);
                            window.ActiveWindow = true;
                        }
                        else
                        {
                            window.ActiveWindow = true;
                        }

                        window.DrawWindow(canvas);
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

                /* Draw the taskbar */
                canvas.DrawFilledRectangle(TaskbarPen, TaskbarLocation, Shell.ScreenWidth, 40);
                canvas.DrawImageAlpha(XenosLogo, IconPos);

                /* Draw the date and time on the taskbar */
                if (Second < 10 && Minute < 10 && Hour < 10)
                {
                    DrawString(canvas, ("0" + Hour + ":0" + Minute + ":0" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else if (Minute < 10 && Second < 10)
                {
                    DrawString(canvas, (Hour + ":0" + Minute + ":0" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else if (Hour < 10 && Minute < 10)
                {
                    DrawString(canvas, ("0" + Hour + ":0" + Minute + ":" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else if (Hour < 10 && Second < 10)
                {
                    DrawString(canvas, ("0" + Hour + ":" + Minute + ":0" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else if (Hour < 10)
                {
                    DrawString(canvas, ("0" + Hour + ":" + Minute + ":" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else if (Minute < 10)
                {
                    DrawString(canvas, (Hour + ":0" + Minute + ":" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else if(Second < 10)
                {
                    DrawString(canvas, (Hour + ":" + Minute + ":0" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }
                else
                {
                    DrawString(canvas, (Hour + ":" + Minute + ":" + Second), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 35);
                }

                DrawString(canvas, (Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year), BlackPen, Shell.ScreenWidth - 70, Shell.ScreenHeight - 20);

                /* If the current FPS should be visible, draw it */
                if (ShowFPS)
                {
                    DrawString(canvas, FPS.ToString(), new Pen(Color.OrangeRed), 0, 0);
                }

                /* Draw the mouse cursor (or a circle if the current canvas isn't the SVGAII canvas) */
                if (!SupportsLargeRes)
                {
                    canvas.DrawFilledCircle(BlackPen, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y, 5);
                }
                else
                {
                    canvas.DrawImageAlpha(CursorBMP, (int)Cosmos.System.MouseManager.X, (int)Cosmos.System.MouseManager.Y);
                }

                /* Since we're using a double buffered driver, a draw call is required */
                canvas.Display();
            }
            catch
            {

            }            
        }

        // Create a new window with the specified position, dimensions, colors, and title (and possibly window ID, if required)
        public static void MakeWindow(int width, int height, int posX, int posY, System.Drawing.Color TitleBarColor, System.Drawing.Color TitleColor, System.Drawing.Color windowColor, string Title, int windowID)
        {
            foreach(var window in windows)
            {
                if(window.Title == Title)
                {
                    return;
                }
            }
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

                Console.WriteLine("[INFO -> GUI] >> GUI started.");
                Console.WriteLine("[INFO -> GUI] >> Using the best canvas for the current graphics controller.");                
                if (Cosmos.System.VMTools.IsVMWare)
                {
                    Console.WriteLine("[INFO -> GUI] >> Using the VMWareSVGAII canvas (VMWare-compatible gpu detected).");
                    canvas = new SVGAIICanvas();
                }
                else
                {
                    if (Cosmos.Core.VBE.IsAvailable())
                    {
                        Console.WriteLine("[INFO -> GUI] >> Using the VBE canvas (VESA_VBE-compatible gpu detected).");
                        canvas = new VBECanvas();
                    }
                    else
                    {
                        Console.WriteLine("[WARN -> GUI] >> VBE & VMWareSVGAII compatible gpus detected, attempting to use a different canvas...");
                        canvas = FullScreenCanvas.GetFullScreenCanvas();
                        if (canvas.Name() == "VGACanvas")
                        {
                            /*
                            Console.WriteLine("[ERROR -> GUI] >> Unsupported canvas (VGA canvas is far too slow).");
                            canvas.DrawFilledRectangle(new Pen(Color.Red), topleft, Shell.ScreenWidth, Shell.ScreenHeight);
                            DrawString(canvas, "GUI ERROR: VGA mode is not supported! Please restart your computer.", BlackPen, 0, 0);
                            canvas.Display();
                            while (true) ;
                            */
                            SupportsLargeRes = false;
                        }
                    }
                }

                Kernel.DEBUGGER.SendMessageBox("[INFO -> GUI] >> Using the " + canvas.Name() + " canvas.");
                Console.WriteLine("[INFO -> GUI] >> Current Graphics Controller: " + canvas.Name());
                Console.WriteLine("[INFO -> GUI] >> Current screen resolution: {0}x{1}", canvas.Mode.Columns, canvas.Mode.Rows);
                Console.Write("[INFO -> GUI] >> Press ESCAPE to exit the gui.\n");
                foreach(var mode in canvas.AvailableModes)
                {
                    if (mode.Columns == Shell.ScreenWidth && mode.Rows == Shell.ScreenHeight)
                    {
                        SupportsLargeRes = true;
                        break;
                    }
                    else
                    {
                        SupportsLargeRes = false;
                    }
                }

                if (!SupportsLargeRes)
                {
                    Console.WriteLine("[INFO -> GUI] >> The current canvas doesn't support large resolutions, or it isn't VMWareSVGAII compatible. Changing the resolution to 640x480...");
                    Shell.ScreenWidth = 640;
                    Shell.ScreenHeight = 480;
                    TaskbarLocation = new Cosmos.System.Graphics.Point(0, 440);
                }

                Console.WriteLine("[INFO -> GUI] >> Applying canvas mode...");
                canvas.Mode = new Mode(Shell.ScreenWidth, Shell.ScreenHeight, ColorDepth.ColorDepth32);

                Console.WriteLine("[INFO -> GUI] >> Setting mouse boundaries and sensitivity...");
                Cosmos.System.MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
                Cosmos.System.MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - MouseCursor.Width;
                Cosmos.System.MouseManager.MouseSensitivity = Shell.MouseSensitivity;

                IconPos.X = 5;
                IconPos.Y = Shell.ScreenHeight - (int)PowerOff.Height - 4;

                Draw(canvas);
                bool ActiveWindow = false;
                bool typing = false;
                int count = 0;

                Messagebox mb = new Messagebox();
                mb.CreateMessageBox("Welcome!", "Welcome to XenOS!\nIf you need any help, press the F1 key to open the help menu.\nTo open the system information panel, press the F3 key.");

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
                                wind = window;

                                /* If the user is typing something, change the "typing" variable */
                                typing = Console.KeyAvailable;

                                /* If the current window is draggable, change the mouse cursor image to show the user */
                                if (window.CheckIfDraggable())
                                {
                                    DrawNormalCursors = false;
                                    ChangeCursor(MoveWindow);
                                }

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

                                        /* Draw everything and increment the total frame count */
                                        window.DrawWindow(canvas); 
                                        Draw(canvas);
                                        FrameCount++;

                                        /* Collect unused memory, but only when required */
                                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 4)
                                        {
                                            Cosmos.Core.Memory.Heap.Collect();
                                        }

                                        /* If there isn't enough memory to keep the GUI running, so return to the console if the computer supports it */
                                        if (Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) >= Cosmos.Core.GCImplementation.GetAvailableRAM() - 5)
                                        {
                                            canvas.DrawFilledRectangle(new Pen(Color.Red), 0, 0, Shell.ScreenWidth, Shell.ScreenHeight);
                                            DrawString(canvas, "GUI ERROR: Out of memory! Please restart your computer.", BlackPen, 0, 0);
                                            canvas.Display();
                                            Cosmos.Core.CPU.Halt();
                                        }

                                        /* If the GUI is meant to be closing (and if the conmputer supports closing), then break out of the loop */
                                        if (ExitGUI && SupportsLargeRes)
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
                                if (window.CheckIfClosable())
                                {
                                    if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left)
                                    {
                                        window.CloseWindow(window);
                                        continue;
                                    }
                                    else
                                    {
                                        ChangeCursor(MouseCursorLC);
                                    }
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
                            // Power button
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

                            // Reboot button
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

                            // RTC button
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

                            // Clock button
                            else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (110 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 160))
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                                {
                                    bool WindowAlreadyExists = false;
                                    foreach (var win in windows)
                                    {
                                        if (win.Title == "Clock")
                                        {
                                            WindowAlreadyExists = true;
                                            break;
                                        }
                                    }

                                    if (!WindowAlreadyExists)
                                    {
                                        MakeWindow(100, 100, 100, 100, System.Drawing.Color.Gray, System.Drawing.Color.Black, System.Drawing.Color.White, "Clock", 0);
                                        foreach (var win in windows)
                                        {
                                            if (win.Title == "Clock")
                                            {
                                                win.MakeNewStringElement((Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second).ToString(), 10, 15);
                                                win.MakeNewStringElement((Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year).ToString(), 10, 35);
                                            }
                                        }
                                    }
                                    MouseClicked = true;
                                }
                                else
                                {
                                    ChangeCursor(MouseCursorLC);
                                }
                            }

                            // Notepad button
                            else if ((10 <= (int)Cosmos.System.MouseManager.X && (int)Cosmos.System.MouseManager.X <= 40) && (160 <= (int)Cosmos.System.MouseManager.Y && (int)Cosmos.System.MouseManager.Y <= 210))
                            {
                                DrawNormalCursors = false;
                                if (Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left && Cosmos.System.MouseManager.LastMouseState != Cosmos.System.MouseState.Left && MouseClicked == false)
                                {
                                    bool WindowAlreadyExists = false;
                                    foreach (var win in windows)
                                    {
                                        if (win.Title == "Notepad")
                                        {
                                            WindowAlreadyExists = true;
                                            break;
                                        }
                                    }

                                    if (!WindowAlreadyExists)
                                    {
                                        MakeWindow(320, 200, 400, 100, System.Drawing.Color.Gray, System.Drawing.Color.Black, System.Drawing.Color.White, "Notepad", 1);
                                        foreach (var win in windows)
                                        {
                                            if (win.Title == "Notepad")
                                            {
                                                win.MakeNewStringElement("|", 10, 10);
                                            }
                                        }

                                        str_len_notepad = 32;
                                    }
                                    MouseClicked = true;
                                }
                                else
                                {
                                    ChangeCursor(MouseCursorLC);
                                }
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

                        /* Draw everything and increment the frame count */
                        Draw(canvas);
                        FrameCount++;

                        /* Collect memory that isn't in use, but only when required */
                        if (Cosmos.Core.GCImplementation.GetAvailableRAM() / (1024 * 1024) < Cosmos.Core.CPU.GetAmountOfRAM() / 2)
                        {
                            Cosmos.Core.Memory.Heap.Collect();
                        }

                        /* Halt the GUI if there isn't enough memory to continue */
                        if (Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) >= Cosmos.Core.GCImplementation.GetAvailableRAM() - 5)
                        {
                            //canvas.DrawFilledRectangle(0, 0, Shell.ScreenWidth, Shell.ScreenHeight, 5, Color.Red);
                            DrawString(canvas, "GUI ERROR: Out of memory! Please restart your computer.", BlackPen, 0, 0);
                            canvas.Display();
                            while (true) ;
                        }

                        /* If the escape key is pressed, exit the GUI. Handle other key presses too */
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey();

                            if (key.Key == ConsoleKey.Escape && SupportsLargeRes)
                            {
                                DisableGUI();
                                break;
                            }

                            if (key.Key == ConsoleKey.LeftWindows || key.Key == ConsoleKey.RightWindows)
                            {
                                ShowMenu = !ShowMenu;
                            }

                            if (key.Key == ConsoleKey.F12)
                            {
                                ShowFPS = !ShowFPS;
                            }

                            if (key.Key == ConsoleKey.F2)
                            {
                                //MakeWindow(320, 200, 121, 121, System.Drawing.Color.Gray, System.Drawing.Color.Black, System.Drawing.Color.White, "XenOS Debugger", 1);
                            }

                            if (key.Key == ConsoleKey.F3)
                            {
                                bool NewWindow = true;
                                foreach (var win in windows)
                                {
                                    if(win.Title == "System Information")
                                    {
                                        NewWindow = false;
                                    }
                                }

                                if (NewWindow)
                                {
                                    MakeWindow(240, 128, 400, 100, System.Drawing.Color.Gray, System.Drawing.Color.Black, System.Drawing.Color.White, "System Information", 1);
                                    foreach (var win in windows)
                                    {
                                        if (win.Title == "System Information")
                                        {
                                            win.MakeNewStringElement("RAM USED: " + Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024) + "/" + Cosmos.Core.GCImplementation.GetAvailableRAM() + " MB", 10, 10);
                                            if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                                            {
                                                win.MakeNewStringElement("IPv4: N/A", 10, 25);
                                            }
                                            else
                                            {
                                                win.MakeNewStringElement("IPv4: " + NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress, 10, 25);
                                            }
                                            //win.MakeNewStringElement("FRAMES: " + FrameCount, 10, 40);
                                            win.MakeNewStringElement("FPS: " + FPS, 10, 40);
                                        }
                                    }
                                }
                            }

                            if (key.Key == ConsoleKey.F1)
                            {
                                MakeWindow(320, 200, 121, 121, Color.Gray, Color.Black, Color.White, "XenOS Help", 1);
                                foreach(var win in windows)
                                {
                                    if(win.Title == "XenOS Help")
                                    {
                                        win.MakeNewStringElement("There's no help atm lol\njust, uh, pretend there's\nsomething here pls", 0, 0);
                                    }
                                }
                            }

                            if (key.Key != ConsoleKey.NoName)
                            {
                                /* If the user is typing and the active window is the notepad, retrieve the key code and add it to the notepad string element */
                                if (wind.ActiveWindow && wind.stringElements.Count > 0 && wind.Title == "Notepad")
                                {
                                    if (key.Key == ConsoleKey.Backspace)
                                    {
                                        if (wind.stringElements[0].Length > 0 && wind.stringElements[0].Length < str_len_notepad - 32)
                                        {
                                            str_len_notepad -= 32;
                                        }
                                        var data = wind.stringElements[0].Remove(wind.stringElements[0].Length - 2) + "|";
                                        wind.EditStringElement(0, data);
                                    }
                                    else if (key.Key == ConsoleKey.Enter)
                                    {
                                        var data = wind.stringElements[0].Remove(wind.stringElements[0].Length - 1) + "\n|";
                                        wind.EditStringElement(0, data);
                                    }
                                    else
                                    {
                                        if(wind.stringElements[0].Length <= 223)
                                        {
                                            if (wind.stringElements[0].Length > str_len_notepad)
                                            {
                                                wind.EditStringElement(0, wind.stringElements[0].Remove(wind.stringElements[0].Length - 1) + "\n|");
                                                str_len_notepad += 32;
                                            }
                                            if (Char.IsLetterOrDigit(key.KeyChar) || Char.IsPunctuation(key.KeyChar) || Char.IsSymbol(key.KeyChar) || key.KeyChar == ' ')
                                            {
                                                var data = wind.stringElements[0].Remove(wind.stringElements[0].Length - 1) + key.KeyChar + "|";
                                                wind.EditStringElement(0, data);
                                            }
                                            //wind.EditStringElement(0, wind.stringElements[0] + key.KeyChar);
                                        }
                                    }
                                }
                            }
                        }

                        /* If the GUI is meant to be closing (and if the computer supports closing), then break out of the loop */
                        if (ExitGUI && SupportsLargeRes)
                        {
                            break;
                        }
                    }
                    catch
                    {

                    }

                    /* If the GUI is meant to be closing (and if the computer supports closing), then break out of the loop */
                    if (ExitGUI && SupportsLargeRes)
                    {
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                DisableGUI();
                Console.WriteLine("[INFO -> GUI] >> GUI ERROR: " + ex.Message);
            }
        }

        public void DisableGUI()
        {
            ExitGUI = true;
            canvas.Disable();
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
    }
}