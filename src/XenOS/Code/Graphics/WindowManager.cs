/* Directives */

using Cosmos.System.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System;
using XenOS.Code.Sys.Helpers;
using Point = Cosmos.System.Graphics.Point;
using Cosmos.System.Graphics.Fonts;

namespace XenOS.Code.Graphics
{
    internal class WindowManager
    {
        // Variables
        public bool ActiveWindow = false;
        public bool IsDraggable = true;
        public bool CloseWindow = false;
        public int WindowID = 0;
        public int WindowPosX = 0;
        public int WindowPosY = 0;
        public int WindowWidth = 0;
        public int WindowHeight = 0;
        public string Title;
        public string WindowText;
        public Color windowColor;
        public Color TitleBarColor;
        public Color TitleBarColorBG;
        public Color TitleColor;
        public Point TitlePos;
        public Point TitlebarPos;
        public Point BodyPos;
        public List<string> stringElements = new List<string>();
        public List<Image> imageElements = new List<Image>();
        public List<Button> buttonElements = new List<Button>();
        public List<Point> stringPoints = new List<Point>();
        public List<Point> imagePoints = new List<Point>();
        public List<Color> stringColors = new List<Color>();
        Color WindowPen;
        Color TitleBarPen;
        Color TitlePen;

        // Functions
        public void MakeNewStringElement(string str, int X, int Y, Color TextColor)
        {
            try
            {
                stringElements.Add(str);
                stringPoints.Add(new Point(X, Y));
                stringColors.Add(TextColor);
            }
            catch
            {

            }
        }

        public void EditStringElement(int index, string str)
        {
            try
            {
                stringElements[index] = str;
            }
            catch
            {

            }
        }

        public void MakeNewButtonElement(string text, int X, int Y, Action OnClickAction, int id)
        {
            try
            {
                Button button = new Button();
                button.CreateNewButton(text, OnClickAction, X, Y, id);
                buttonElements.Add(button);
            }
            catch
            {
            }
        }

        public void init()
        {
            try
            {
                WindowID = Graphics.windows.Count;
                Graphics.windows.Add(this);
                WindowPen = windowColor;
                TitlePen = TitleColor;
                TitleBarPen = TitleBarColor;
                TitlePos = new Point(0, 0);
                TitlebarPos = new Point(0, 0);
                BodyPos = new Point(0, 0);
            }
            catch
            {

            }
        }

        public void DrawWindow(Canvas canvas)
        {
            try
            {
                if (ActiveWindow)
                {
                    TitleBarPen = TitleBarColor;
                    TitlePen = TitleColor;
                }
                else
                {
                    TitleBarPen = Color.LightGray;
                    TitlePen = Color.Gray;
                }

                BodyPos.X = WindowPosX;
                BodyPos.Y = WindowPosY;
                TitlebarPos.X = WindowPosX;
                TitlebarPos.Y = WindowPosY;
                TitlePos.X = WindowPosX + (WindowWidth / 2 - Title.Length * 4);
                TitlePos.Y = WindowPosY + 20;

                canvas.DrawFilledRectangle(WindowPen, BodyPos, WindowWidth, WindowHeight);
                canvas.DrawFilledRectangle(TitleBarPen, TitlebarPos, WindowWidth, 40);
                canvas.DrawImage(Graphics.CloseWindowButton, WindowPosX + (WindowWidth - 20), WindowPosY + 12);
                canvas.DrawString(Title, PCScreenFont.Default, TitlePen, TitlePos.X, TitlePos.Y);
                foreach (var element in stringElements)
                {
                    int index = stringElements.IndexOf(element);
                    if (element.Contains("\n"))
                    {
                        int Y_pos = WindowPosY + 40 + stringPoints[index].Y;
                        foreach (var part in element.Split("\n"))
                        {
                            canvas.DrawString(part, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, stringColors[index], new Point(WindowPosX + stringPoints[index].X, Y_pos));
                            Y_pos += 12;
                        }
                    }
                    else
                    {
                        canvas.DrawString(element, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, stringColors[index], new Point(WindowPosX + stringPoints[index].X, WindowPosY + 40 + stringPoints[index].Y));
                    }
                }

                foreach (var element in buttonElements)
                {
                    int index = buttonElements.IndexOf(element);
                    element.CurrentLocation = new Point(WindowPosX + element.location.X, WindowPosY + element.location.Y);
                    canvas.DrawFilledRectangle(Color.LightGray, new Point(WindowPosX + element.location.X, WindowPosY + element.location.Y), 50, 30);
                    canvas.DrawString(element.Text, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, TitlePen, new Point(element.CurrentLocation.X, element.CurrentLocation.Y));
                    
                    if (element.CheckIfClicked())
                    {
                        element.OnClick();
                    }
                }

                if (CloseWindow)
                {
                    Close_Window();
                }
            }
            catch
            {

            }
        }

        public void Close_Window()
        {
            try
            {
                Graphics.windows.Remove(this);
            }
            catch
            {

            }
        }

        public bool CheckIfDraggable()
        {
            return Helpers.IsBetween(Cosmos.System.MouseManager.X, WindowPosX - 5, WindowPosX + (WindowWidth - 30)) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + 40) && IsDraggable;
        }

        public bool CheckIfClosable()
        {
            return Helpers.IsBetween(Cosmos.System.MouseManager.X, WindowPosX + WindowWidth - 5, WindowPosX + (WindowWidth - 30)) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + 40);
        }

        public bool CheckIfActive()
        {
            try
            {
                if (ActiveWindow)
                {
                    return true;
                }
                else
                {
                    if (Cosmos.System.MouseManager.MouseState != Cosmos.System.MouseState.None)
                    {
                        return Helpers.IsBetween(Cosmos.System.MouseManager.X, WindowPosX, WindowPosX + WindowWidth) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + WindowHeight);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {

            }
            return false;
        }
    }
}
