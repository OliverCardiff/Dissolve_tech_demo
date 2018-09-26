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
    class Player
    {
        Vector2 position;
        Vector2 velocity;
        Texture2D tex;
        Texture2D bTex;
        List<Bullet> bullets;

        const float MAX_SPEED = 1.7f;
        const float ACCELERATION = 0.3f;
        const float OFFSET = 5;

        public Player(Texture2D t, Texture2D t2)
        {
            position = new Vector2(Game1.ScreenX/2, Game1.ScreenY/2);
            velocity = Vector2.Zero;
            tex = t;
            bTex = t2;
            bullets = new List<Bullet>();
        }

        private void ProcessInput()
        {
            KeyboardState state = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            if (state.IsKeyDown(Keys.W))
            {
                velocity.Y -= ACCELERATION;
            }
            if (state.IsKeyDown(Keys.S))
            {
                velocity.Y += ACCELERATION;
            }
            if (state.IsKeyDown(Keys.A))
            {
                velocity.X -= ACCELERATION;
            }
            if (state.IsKeyDown(Keys.D))
            {
                velocity.X += ACCELERATION;
            }

            velocity.X = MathHelper.Clamp(velocity.X, -MAX_SPEED, MAX_SPEED);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MAX_SPEED, MAX_SPEED);

            if (mState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mPos = new Vector2(mState.X, mState.Y);
                mPos -= position;
                mPos.Normalize();
                Bullet b = new Bullet(position, mPos * 6, bTex);
                bullets.Add(b);
            }
        }

        public void Update(GameTime time)
        {
            ProcessInput();
            ScreenWrap();
            position += velocity;

            foreach (Bullet b in bullets)
            {
                b.Update();
            }
        }

        public void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, Color.White);
            foreach (Bullet b in bullets)
            {
                b.Draw(s);
            }
        }

        private void DeleteBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].IsDead)
                {
                    bullets.RemoveAt(i);
                }
            }
        }


        private void ScreenWrap()
        {
            if (position.X - OFFSET > Game1.ScreenX)
            {
                position.X = -OFFSET;
            }
            if (position.X + OFFSET < 0)
            {
                position.X = (float)Game1.ScreenX + OFFSET;
            }
            if (position.Y - OFFSET > Game1.ScreenY)
            {
                position.Y = -OFFSET;
            }
            if (position.Y + OFFSET < 0)
            {
                position.Y = (float)Game1.ScreenY + OFFSET;
            }
        }
    }
}
