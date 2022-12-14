/* Directives */
using System;
using System.Drawing;
using System.Collections.Generic;
using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Network.Config;
using IL2CPU.API.Attribs;
using XenOS.Code.Sys.Boot;
using XenOS.Code.Sys.Drivers;

/* Namespaces */
namespace XenOS.Code.Graphics
{
    /* Classes */
    internal class Graphics
    {
        /* Variables */
        [ManifestResourceStream(ResourceName = "XenOS.Art.Wallpapers.BG_800x600.bmp")] 
        static byte[] WallpaperData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.mouse.bmp")]
        static byte[] DefaultCursorData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.XenOS_LOGO.bmp")]
        static byte[] OSLogoData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.close_window.bmp")]
        static byte[] CloseWindowData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.PowerOff.bmp")]
        static byte[] PowerButtonData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.Restart.bmp")]
        static byte[] RestartButtonData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.ReturnToConsole.bmp")]
        static byte[] ConsoleButtonData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.clock.bmp")]
        static byte[] ClockButtonData;

        [ManifestResourceStream(ResourceName = "XenOS.Art.Icons.notepad.bmp")]
        static byte[] NotepadButtonData;

        public Bitmap Wallpaper;
        public Bitmap MouseCursor;
        public Bitmap OSLogo;
        public static Bitmap CloseWindowButton;
        public Bitmap MenuPowerButton;
        public Bitmap MenuRestartButton;
        public Bitmap MenuConsoleButton;
        public Bitmap MenuClockButton;
        public Bitmap MenuNotepadButton;

        Canvas canvas;

        public static List<WindowManager> windows = new List<WindowManager>();
        public static List<Button> buttons = new List<Button>();

        public bool ShowFPS = false;
        public bool ShowMenu = false;

        int FPS = 0, FrameCount = 0;
        int Hour = 0, Minute = 0, Second = 0;

        Cosmos.System.Graphics.Point topleft = new Cosmos.System.Graphics.Point(0, 0);
        Cosmos.System.Graphics.Point MenuPowerButtonPos = new Cosmos.System.Graphics.Point(10, 10);
        Cosmos.System.Graphics.Point MenuRestartButtonPos = new Cosmos.System.Graphics.Point(10, 50);
        Cosmos.System.Graphics.Point MenuConsoleButtonPos = new Cosmos.System.Graphics.Point(10, 90);
        Cosmos.System.Graphics.Point MenuClockButtonPos = new Cosmos.System.Graphics.Point(10, 130);
        Cosmos.System.Graphics.Point MenuNotepadButtonPos = new Cosmos.System.Graphics.Point(10, 170);
        public static Cosmos.System.Graphics.Point TaskbarLocation = new Cosmos.System.Graphics.Point(0, Shell.ScreenHeight - 40);
        public static Cosmos.System.Graphics.Point TextLocation = new Cosmos.System.Graphics.Point(0, 0);
        public static Cosmos.System.Graphics.Point IconPos = new Cosmos.System.Graphics.Point(0, 0);
        public static Cosmos.System.Graphics.Point LogoPos;

        /* Functions */
        public void Initialize()
        {
            // Try to create a new canvas with screen width and height
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(Shell.ScreenWidth, Shell.ScreenHeight, ColorDepth.ColorDepth32));

            // Create bitmaps
            Wallpaper = new Bitmap(WallpaperData);
            MouseCursor = new Bitmap(DefaultCursorData);
            OSLogo = new Bitmap(OSLogoData);
            OSLogo = new Bitmap(OSLogoData);
            CloseWindowButton = new Bitmap(CloseWindowData);
            MenuPowerButton = new Bitmap(PowerButtonData);
            MenuRestartButton = new Bitmap(RestartButtonData);
            MenuConsoleButton = new Bitmap(ConsoleButtonData);
            MenuClockButton = new Bitmap(ClockButtonData);
            MenuNotepadButton = new Bitmap(NotepadButtonData);

            // Set mouse properties
            MouseManager.MouseSensitivity = Shell.MouseSensitivity;
            MouseManager.ScreenWidth = (uint)Shell.ScreenWidth - MouseCursor.Width;
            MouseManager.ScreenHeight = (uint)Shell.ScreenHeight;
            MouseManager.X = (uint)canvas.Mode.Columns / 2 - MouseCursor.Width;
            MouseManager.X = (uint)canvas.Mode.Rows / 2 - MouseCursor.Height;

            // Set time values
            Hour = Cosmos.HAL.RTC.Hour;
            Minute = Cosmos.HAL.RTC.Minute;
            Second = Cosmos.HAL.RTC.Second;

            // Set point values
            LogoPos = new Cosmos.System.Graphics.Point(5, Shell.ScreenHeight - (int)OSLogo.Height - 4);
        }

        public void Run()
        {
            Initialize();
            while (true)
            {
                HandleInput();
                Draw();
                Cosmos.Core.Memory.Heap.Collect();
            }
        }

        public void HandleInput()
        {
            if (System.Console.KeyAvailable)
            {
                var key = System.Console.ReadKey();

                if (key.Key == ConsoleKey.LeftWindows || key.Key == ConsoleKey.RightWindows)
                {
                    ShowMenu = !ShowMenu;
                }

                if (key.Key == ConsoleKey.F1)
                {
                    CreateHelpWindow();
                }

                if (key.Key == ConsoleKey.F2)
                {
                    MakeWindow(640, 480, 121, 121, Color.Gray, Color.Black, Color.White, "???", windows.Count);
                    foreach (var win in windows)
                    {
                        if (win.Title == "???")
                        {
                            win.MakeNewStringElement("???\nThis window serves no purpose atm.", 0, 0, Color.Black);
                        }
                    }
                }

                if (key.Key == ConsoleKey.F3)
                {
                    bool NewWindow = true;
                    foreach (var win in windows)
                    {
                        if (win.Title == "System Information")
                        {
                            NewWindow = false;
                        }
                    }

                    if (NewWindow)
                    {
                        MakeWindow(240, 128, 400, 100, Color.Gray, Color.Black, Color.White, "System Information", windows.Count);
                        foreach (var win in windows)
                        {
                            if (win.Title == "System Information")
                            {
                                win.MakeNewStringElement("RAM USED: " + GCImplementation.GetUsedRAM() / (1024 * 1024) + "/" + GCImplementation.GetAvailableRAM() + " MB", 10, 10, Color.Black);
                                if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                                {
                                    win.MakeNewStringElement("IPv4: N/A", 10, 25, Color.Black);
                                }
                                else
                                {
                                    win.MakeNewStringElement("IPv4: " + NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress, 10, 25, Color.Black);
                                }
                                win.MakeNewStringElement("FPS: " + FPS, 10, 40, Color.Black);
                            }
                        }
                    }
                }

                if (key.Key == ConsoleKey.F4)
                {
                    MakeWindow(320, 200, 69, 69, Color.Gray, Color.Black, Color.Black, "Console", windows.Count);
                    foreach (var win in windows)
                    {
                        if (win.Title == "Console")
                        {
                            win.MakeNewStringElement(">|", 0, 0, Color.White);
                        }
                    }
                }

                if (key.Key == ConsoleKey.F7)
                {
                    MakeWindow(320, 200, 69, 69, Color.Gray, Color.Black, Color.Orange, "TEST WINDOW", windows.Count);
                }

                if (key.Key == ConsoleKey.F12)
                {
                    ShowFPS = !ShowFPS;
                }
            }
        }

        public void Draw()
        {
            // Draw the wallpaper
            canvas.DrawImage(Wallpaper, 0, 0);

            // Draw the taskbar
            canvas.DrawFilledRectangle(Color.DarkRed, TaskbarLocation, Shell.ScreenWidth, 40);

            // Draw each window
            foreach(var window in windows)
            {
                window.DrawWindow(canvas);
                if(window.Title == "System Information")
                {
                    window.EditStringElement(0, "RAM USED: " + GCImplementation.GetUsedRAM() / (1024 * 1024) + "/" + GCImplementation.GetAvailableRAM() + " MB");
                    if (Cosmos.HAL.NetworkDevice.Devices.Count < 1)
                    {
                        window.EditStringElement(1, "IPv4: N/A");
                    }
                    else
                    {
                        window.EditStringElement(1, "IPv4: " + NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress);
                    }
                    window.EditStringElement(2, "FPS: " + FPS);
                }
            }

            // Draw the operating system logo on the taskbar
            canvas.DrawImageAlpha(OSLogo, LogoPos);

            // Draw the time and date
            canvas.DrawString($"{Hour}:{Minute}:{Second}", PCScreenFont.Default, Color.Black, new Cosmos.System.Graphics.Point(Shell.ScreenWidth - 70, Shell.ScreenHeight - 35));
            canvas.DrawString($"{Cosmos.HAL.RTC.Month}-{Cosmos.HAL.RTC.DayOfTheMonth}-{Cosmos.HAL.RTC.Year}", PCScreenFont.Default, Color.Black, new Cosmos.System.Graphics.Point(Shell.ScreenWidth - 70, Shell.ScreenHeight - 20));

            // If the menu should be visible, draw it at (X:0, Y:0)
            if (ShowMenu)
            {
                canvas.DrawFilledRectangle(Color.DarkRed, topleft, 55, 210);
                canvas.DrawImageAlpha(MenuPowerButton, MenuPowerButtonPos);
                canvas.DrawImageAlpha(MenuRestartButton, MenuRestartButtonPos);
                canvas.DrawImageAlpha(MenuConsoleButton, MenuConsoleButtonPos);
                canvas.DrawImageAlpha(MenuClockButton, MenuClockButtonPos);
                canvas.DrawImageAlpha(MenuNotepadButton, MenuNotepadButtonPos);
            }

            // If the FPS should be visible, draw it at (X:0, Y:0)
            if (ShowFPS)
            {
                canvas.DrawString($"{FPS} ({FrameCount})", PCScreenFont.Default, Color.Red, topleft);
            }

            // Draw the mouse cursor
            canvas.DrawImageAlpha(MouseCursor, (int)MouseManager.X, (int)MouseManager.Y);

            // Since we're using a doube buffered driver, we need to tell it to copy the second framebuffer into the first one
            canvas.Display();

            // If the PIT timer ticked, update time, FPS, and PIT values
            if (Drivers.PITTicked)
            {
                FPS = (int)(FrameCount / 0.25f);
                Hour = Cosmos.HAL.RTC.Hour;
                Minute = Cosmos.HAL.RTC.Minute;
                Second = Cosmos.HAL.RTC.Second;
                FrameCount = 0;
                Drivers.PITTicked = false;
            }

            // Increment the frame count
            FrameCount++;
        }

        public static void CreateHelpWindow()
        {
            MakeWindow(320, 200, 121, 121, Color.Gray, Color.Black, Color.White, "XenOS Help", windows.Count);
            foreach (var win in windows)
            {
                if (win.Title == "XenOS Help")
                {
                    win.MakeNewStringElement("There's no help atm lol so just, uh,\npretend there's something here pls", 0, 0, Color.Black);
                }
            }
        }

        // Create a new window with the specified position, dimensions, colors, and title (and possibly window ID, if required)
        public static void MakeWindow(int width, int height, int posX, int posY, Color TitleBarColor, Color TitleColor, Color windowColor, string Title, int windowID)
        {
            try
            {
                foreach (var window in windows)
                {
                    if (window.Title == Title)
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
            catch
            {

            }
        }
    }
}
