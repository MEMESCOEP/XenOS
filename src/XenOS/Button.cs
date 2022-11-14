using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class Button
    {
        public Action OnClickAction;
        public Point location;
        public string Text = "";
        public int WindowID = 0;

        public void CreateNewButton(string text, Action function, int X, int Y, int ID)
        {
            location.X = X;
            location.Y = Y;
            Text = text;
            OnClickAction = function;
            WindowID = ID;
        }

        public bool CheckIfClicked()
        {
            if(Helpers.IsBetween(Cosmos.System.MouseManager.X, location.X, location.X + 50) && Helpers.IsBetween(Cosmos.System.MouseManager.Y, location.Y, location.Y + 30) && Cosmos.System.MouseManager.MouseState == Cosmos.System.MouseState.Left)
            {
                return true;
            }
            return false;
        }

        public void OnClick()
        {
            OnClickAction.Invoke();
        }
    }
}
