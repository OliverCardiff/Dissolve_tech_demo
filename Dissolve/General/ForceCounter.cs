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
    class ForceCounter
    {
        Texture2D colorBg;
        RenderTarget2D targetColor;
        Game1 gRef;
        Color[] colors;
        float lifeForce = 0;

        Thread counter;

        const int MIN_FORCE = 45;

        public static bool Victory { get; set; }

        public static float CurrentForce { get; set; }

        static bool checkDeath;

        float maxForce;

        public ForceCounter(Game1 game)
        {
            gRef = game;
            maxForce = (float)(Game1.ScreenX * Game1.ScreenY) / 2;
            colors = new Color[Game1.ScreenX * Game1.ScreenY];

            targetColor = new RenderTarget2D(gRef.GraphicsDevice, Game1.ScreenX, Game1.ScreenY, 1, SurfaceFormat.Color);
            Victory = true;
            checkDeath = false;
        }

        public static void ResetAll()
        {
            checkDeath = false;
            CurrentForce = 0;
            Victory = true;
        }
        public void DrawToColorTarget(SpriteBatch spriteBatch, Texture2D bg)
        {
            gRef.GraphicsDevice.SetRenderTarget(0, targetColor);
            gRef.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.Draw(bg, Vector2.Zero, Color.White);
            spriteBatch.End();
            gRef.GraphicsDevice.SetRenderTarget(0, null);

            colorBg = targetColor.GetTexture();

        }

        public void CountForce()
        {
            if (colorBg != null)
            {
                if (counter == null || counter.ThreadState != ThreadState.Running)
                {
                    ThreadStart p = new ThreadStart(SumLifeForce);
                    counter = new Thread(p);
                    counter.Start();
                }
            }
        }

        public void Update()
        {
            if (CurrentForce < MIN_FORCE && checkDeath && Victory)
            {
                Victory = false;
                Player.StartFinalTimer();
            }

            if (CurrentForce > 99)
            {
                checkDeath = true;
            }
        }

        private void SumLifeForce()
        {
            colorBg.GetData<Color>(colors);
            lifeForce = 0;
            for (int i = 0; i < colors.Length / 2; i++)
            {
                if (colors[i * 2].B > colors[i * 2].R && colors[i * 2].B > colors[i * 2].G)
                {
                    lifeForce++;
                }
            }
            CurrentForce = (lifeForce / maxForce) * 100;
        }
    }
}
