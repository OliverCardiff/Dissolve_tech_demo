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
    enum CursorMode
    {
        Follow, Free
    }
    class Cursor : UIEffect
    {
        GamePadState state;
        const float CURSOR_OFFSET = 50;
        const float CURSOR_SPEED = 6;
        new CursorMode mode;
        Color normalColor;

        public Point Position
        {
            get
            {
                return new Point((int)position.X, (int)position.Y);
            }
        }

        public Cursor(Color drawcol)
            : base(Player.Position, Vector2.Zero, UILayer.MouseTex)
        {
            normalColor = drawcol;
            mode = CursorMode.Free;
            scale = 1.5f;

        }

        public override void Update(float time)
        {
            state = GamePad.GetState(PlayerIndex.One);

            ManageStates();

            if (mode == CursorMode.Follow)
            {
                drawColor = new Color((float)normalColor.R / 255, (float)normalColor.G / 255,
               (float)normalColor.B / 255, state.ThumbSticks.Right.Length());

                position = Player.Position;
                position.X += state.ThumbSticks.Right.X * CURSOR_OFFSET;
                position.Y += state.ThumbSticks.Right.Y * -CURSOR_OFFSET;
            }
            else
            {
                drawColor = Color.Black;
                position.X += state.ThumbSticks.Left.X * CURSOR_SPEED;
                position.Y -= state.ThumbSticks.Left.Y * CURSOR_SPEED;

                RestrictToScreen();
            }
        }

        private void RestrictToScreen()
        {
            if (position.X > Game1.ScreenX)
            {
                position.X = Game1.ScreenX;
            }
            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y > Game1.ScreenY)
            {
                position.Y = Game1.ScreenY; 
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
        }

        private void ManageStates()
        {
            if (LevelManager.State != GameState.Running)
            {
                mode = CursorMode.Free;
            }
            else
            {
                mode = CursorMode.Follow;
            }
        }
    }
}
