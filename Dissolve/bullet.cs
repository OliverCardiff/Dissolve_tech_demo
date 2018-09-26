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
    class Bullet
    {
        Vector2 position;
        Vector2 velocity;
        Texture2D tex;

        public bool IsDead { get; set; }
        const float OFFSET = 10;

        public Bullet(Vector2 p, Vector2 v, Texture2D t)
        {
            position = p;
            velocity = v;
            tex = t;
        }

        public void Update()
        {
            IsDead = CheckOffScreen();
            position += velocity;
        }

        public void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, Color.White);
        }

        private bool CheckOffScreen()
        {
            if (position.X + OFFSET < 0 || position.X - OFFSET > Game1.ScreenX ||
                position.Y + OFFSET < 0 || position.Y - OFFSET > Game1.ScreenY)
            {
                return true;
            }
            return false;
        }
    }
}
