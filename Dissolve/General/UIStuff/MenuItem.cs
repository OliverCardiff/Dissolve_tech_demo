using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Dissolve
{
    class MenuItem
    {
        public int ID { get; set; }

        string name;
        Rectangle location;
        public bool Clicked;
        Point mousePos;
        

        public MenuItem(string name, Rectangle location)
        {
            this.name = name;
            this.location = location;
            Clicked = false;
            mousePos = new Point();
        }

        public void Update()
        {
            MouseState state = Mouse.GetState();

            mousePos.X = state.X;
            mousePos.Y = state.Y;

            if (state.LeftButton == ButtonState.Pressed && IsMouseOver())
            {
                Clicked = true;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsMouseOver())
            {
                spriteBatch.Draw(Menu.MouseOver, location, Color.White);
            }
            else
            {
                spriteBatch.Draw(Menu.Normal, location, Color.White);
            }
            spriteBatch.DrawString(UILayer.Font, name, new Vector2(location.Location.X + 10, location.Location.Y + 5), Color.White);
            
        }

        private bool IsMouseOver()
        {
            if(location.Contains(mousePos))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
