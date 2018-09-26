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
    /// <summary>
    /// Contains information relevant to each item in a menu
    /// </summary>
    class MenuItem
    {
        /// <summary>
        ///ID describes the Item's location in the main menu box, and which delegate it
        ///should invoke
        /// </summary>
        public int ID { get; set; }

        //the Name to draw to screen
        string name;

        //the location of the Item in screenspace
        Rectangle location;

        /// <summary>
        /// Whether or not the Item has been Clicked (Pressed A on) during that frame
        /// </summary>
        public bool Clicked;     

        public MenuItem(string name, Rectangle location)
        {
            this.name = name;
            this.location = location;
            Clicked = false;
        }

        /// <summary>
        /// Checks whether the MenuItem has been selected, given the current GamePadState
        /// </summary>
        /// <param name="state">the state to check against</param>
        public void Update(GamePadState state)
        {
            if (state.Buttons.A == ButtonState.Pressed && IsMouseOver())
            {
                Clicked = true;
            }
        }
        /// <summary>
        /// Draws the MenuItem box and its string name
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use to draw it</param>
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
            spriteBatch.DrawString(UILayer.Font, name, new Vector2(location.Location.X + 14, location.Location.Y + 6), Color.White);
            
        }

        //simply checks if the cursor in over the MenuItem
        private bool IsMouseOver()
        {
            if(location.Contains(UILayer.MainCursor.Position))
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
