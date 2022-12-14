using Cosmos.System.Graphics;
using System;
using XenOS.Code.Sys.Helpers;

namespace XenOS.Code.Graphics
{
    internal class Button
    {
        public Action OnClickAction;
        public Point location;
        public Point CurrentLocation;
        public string Text = "";
        public int WindowID = 0;

        public void CreateNewButton(string text, Action function, int X, int Y, int ID)
        {
            try
            {
                location.X = X;
                location.Y = Y;
                Text = text;
                OnClickAction = function;
                WindowID = ID;
            }
            catch
            {

            }
        }

        public bool CheckIfClicked()
        {
            return Helpers.IsBetween(Cosmos.System.MouseManager.X, CurrentLocation.X, CurrentLocation.X + 50) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, CurrentLocation.Y, CurrentLocation.Y + 30) && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left;
        }

        public void OnClick()
        {
            OnClickAction();
        }
    }
}
