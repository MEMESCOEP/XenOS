using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.Debug.Kernel;
using Cosmos.System.Graphics;
using CosmosTTF;
using IL2CPU.API.Attribs;
using Point = Cosmos.System.Graphics.Point;

namespace XenOS
{
    internal class WindowManager
    {
        // Variables
        public bool ActiveWindow = false;
        public bool IsDraggable = true;
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
        Pen WindowPen;
        Pen TitleBarPen;
        Pen TitlePen;

        // Functions
        public void MakeNewStringElement(string str, int X, int Y)
        {
            stringElements.Add(str);
            stringPoints.Add(new Point(X, Y));
        }

        public void EditStringElement(int index, string str)
        {
            stringElements[index] = str;
        }

        public void MakeNewButtonElement(string text, int X, int Y, Action OnClickAction, int id)
        {
            Button button = new Button();
            button.CreateNewButton(text, OnClickAction, X, Y, id);
            buttonElements.Add(button);
        }

        public void init()
        {
            try
            {
                WindowID = GUI.windows.Count;
                GUI.windows.Add(this);
                WindowPen = new Pen(windowColor);
                TitlePen = new Pen(TitleColor);
                TitleBarPen = new Pen(TitleBarColor);
                TitlePos = new Cosmos.System.Graphics.Point(0, 0);
                TitlebarPos = new Cosmos.System.Graphics.Point(0, 0);
                BodyPos = new Cosmos.System.Graphics.Point(0, 0);
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
                    if (ActiveWindow)
                    {
                        TitleBarPen.Color = TitleBarColor;
                        TitlePen.Color = TitleColor;
                    }
                    else
                    {
                        TitleBarPen.Color = Color.LightGray;
                        TitlePen.Color = Color.Gray;
                    }

                    BodyPos.X = WindowPosX;
                    BodyPos.Y = WindowPosY;
                    TitlebarPos.X = WindowPosX;
                    TitlebarPos.Y = WindowPosY;
                    TitlePos.X = WindowPosX + ((WindowWidth / 2) - (Title.Length * 4));
                    TitlePos.Y = WindowPosY + 20;

                    canvas.DrawFilledRectangle(WindowPen, BodyPos, WindowWidth, WindowHeight);
                    canvas.DrawFilledRectangle(TitleBarPen, TitlebarPos, WindowWidth, 40);
                    canvas.DrawImage(GUI.CloseWindow, WindowPosX + (WindowWidth - 20), WindowPosY + 12);
                    canvas.DrawString(Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, TitlePen, TitlePos);
                    foreach(var element in stringElements)
                    {
                        int index = stringElements.IndexOf(element);
                        if (element.Contains("\n"))
                        {
                            int Y_pos = WindowPosY + 40 + stringPoints[index].Y;
                            foreach (var part in element.Split("\n"))
                            {
                                canvas.DrawString(part, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, TitlePen, new Point(WindowPosX + stringPoints[index].X, Y_pos));
                                Y_pos += 12;
                            }
                        }
                        else
                        {
                            canvas.DrawString(element, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, TitlePen, new Point(WindowPosX + stringPoints[index].X, WindowPosY + 40 + stringPoints[index].Y));
                        }
                    }

                    /*foreach (var element in buttonElements)
                    {
                        int index = buttonElements.IndexOf(element);
                        element.location = new Point(WindowPosX + 15, WindowPosY + 95);
                        canvas.DrawFilledRectangle(new Pen(Color.LightGray), element.location, 50, 30);
                        canvas.DrawString(element.Text, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, TitlePen, element.location);
                        if (element.CheckIfClicked())
                        {
                            element.OnClick();
                        }
                    }*/
                }
            }
            catch
            {

            }
        }

        public void CloseWindow(WindowManager window)
        {
            GUI.windows.Remove(window);
        }

        public bool CheckIfDraggable()
        {
            if (IsDraggable)
            {
                return (Helpers.IsBetween(Cosmos.System.MouseManager.X, WindowPosX - 5, WindowPosX + (WindowWidth - 25)) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + 40));
            }
            return false;
        }

        public bool CheckIfClosable()
        {
            return (Helpers.IsBetween(Cosmos.System.MouseManager.X, WindowPosX + WindowWidth - 5, WindowPosX + (WindowWidth - 25)) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + 40));
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
                        if (Helpers.IsBetween(Cosmos.System.MouseManager.X, WindowPosX, WindowPosX + WindowWidth) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, WindowPosY, WindowPosY + WindowHeight))
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
