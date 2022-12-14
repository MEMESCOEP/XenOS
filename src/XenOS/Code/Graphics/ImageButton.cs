using Cosmos.System.Graphics;
using System;
using XenOS.Code.Sys.Helpers;

namespace XenOS.Code.Graphics
{
    internal class ImageButton
    {
        public Action OnClickAction;
        public Point location;
        public Point CurrentLocation;
        public Bitmap image;
        public int WindowID = 0;

        public void CreateNewButton(Bitmap bitmap, Action function, int X, int Y, int ID)
        {
            try
            {
                location.X = X;
                location.Y = Y;
                image = bitmap;
                OnClickAction = function;
                WindowID = ID;
            }
            catch
            {

            }
        }

        public bool CheckIfClicked()
        {
            return Helpers.IsBetween(Cosmos.System.MouseManager.X, CurrentLocation.X, CurrentLocation.X + image.Width) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, CurrentLocation.Y, CurrentLocation.Y + image.Height) && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left;
        }

        public void OnClick()
        {
            OnClickAction();
        }
    }
}
