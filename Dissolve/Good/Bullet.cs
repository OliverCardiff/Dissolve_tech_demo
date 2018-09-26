using System;
using System.Collections.Generic;
using System.Linq;
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
    /// This is the base class for all bullets
    /// </summary>
    class Bullet
    {
        /// <summary>
        /// The bullets position
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        protected Vector2 position;
        protected Vector2 velocity;
        protected Texture2D tex;

        /// <summary>
        /// Control Variable used to check if the bullet should be destroyed
        /// </summary>
        public bool IsDead { get; set; }

        /// <summary>
        /// Describes the length from the centre of the bullets texture
        /// to its furthest edge
        /// </summary>
        public float Size { get; set; }

        //Size of boundary around the edge of the screen which bullets must pass through before being
        //destroyed
        const float OFFSET = 10;

        public int Damage{get;set;}

        public Bullet(Vector2 p, Vector2 v, Texture2D t)
        {
            position = p;
            velocity = v;
            tex = t;
            Damage = 1;
            Size = Math.Max(t.Height / 2, t.Width / 2);
        }

        //Basic implementation of Update and Draw
        public virtual void Update(GameTime time)
        {
            if (!IsDead)
            {
                IsDead = CheckOffScreen();
            }
            position += velocity;
        }

        public virtual void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, Color.White);
        }

        //Used to give bullets alternate behaviour on collision in Worm mode
        public virtual void Sustain(int value)
        {
        }

        protected bool CheckOffScreen()
        {
            if (position.X + OFFSET < 0 || position.X - OFFSET > Game1.ScreenX ||
                position.Y + OFFSET < 0 || position.Y - OFFSET > Game1.ScreenY)
            {
                return true;
            }
            return false;
        }

        //Not used as standard, but made available for screen-wrapping bullets
        protected virtual void ScreenWrap()
        {
            if (position.X - Size > Game1.ScreenX)
            {
                position.X = -Size;
            }
            if (position.X + Size < 0)
            {
                position.X = (float)Game1.ScreenX + Size;
            }
            if (position.Y - Size > Game1.ScreenY)
            {
                position.Y = -Size;
            }
            if (position.Y + Size < 0)
            {
                position.Y = (float)Game1.ScreenY + Size;
            }
        }
    }
}
