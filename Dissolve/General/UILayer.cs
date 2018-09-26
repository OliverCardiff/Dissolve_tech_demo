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
    class UILayer
    {
        Effect final;
        SpriteFont font;
        Texture2D mouseTex;
        Game1 gRef;
        Vector2 mTexOrigin;

        float boostFactor;
        static float currentTime;
        const float MAX_BOOST = 1;
        const float MIN_BOOST = 0;
        const float CHANGE_RATE = 0.01f;
        //const float MSG_DISP_TIME = 30;
        //bool displayMsg;

        static UIStats stats;

        public UILayer(Game1 game)
        {
            final = game.Content.Load<Effect>("HLSL/final");
            font = game.Content.Load<SpriteFont>("UI/Arial");
            mouseTex = game.Content.Load<Texture2D>("UI/dot");
            gRef = game;
            boostFactor = 0;

            stats = LevelManager.Current.UIStats;
            mTexOrigin = new Vector2(mouseTex.Width / 2, mouseTex.Height / 2);
            //displayMsg = false;
        }
        public static void NextLevel()
        {
            stats = LevelManager.Current.UIStats;
            currentTime = 0;
        }
        public void Update(GameTime time)
        {
            currentTime += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            //if (currentTime < MSG_DISP_TIME)
            //{
            //    displayMsg = true;
            //}
            //else
            //{
            //    displayMsg = false;
            //}
            ControlBoostFactor();
        }

        public void DrawToScreen(SpriteBatch spriteBatch, Texture2D bg)
        {
            MouseState state = Mouse.GetState();
            Vector2 currentMouse = new Vector2(state.X, state.Y);

            gRef.GraphicsDevice.Clear(Color.Black);
            final.Parameters["xBoostFactor"].SetValue(boostFactor);
            final.Parameters["xScreenSize"].SetValue(new Vector2(Game1.ScreenX, Game1.ScreenY));
            final.Parameters["xUIBlur"].SetValue(true);
            final.Begin();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            final.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(bg, Vector2.Zero, Color.White);

            DrawUI(spriteBatch, currentMouse);

            final.CurrentTechnique.Passes[0].End();
            spriteBatch.End();
            final.End();
        }

        private void DrawUI(SpriteBatch spriteBatch, Vector2 currentMouse)
        {
            final.Parameters["xUIBlur"].SetValue(false);
            
            spriteBatch.DrawString(font, "Lifeforce: " + Math.Round(Game1.currentForce, 1), new Vector2(10, 10), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            if (stats.PowerShown)
            {
                spriteBatch.DrawString(font, "Power: " + Player.Points + " / " + LevelManager.Current.PlayerStats.MaxPower.ToString(), new Vector2(100, 10), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            }
            spriteBatch.DrawString(font, "Muliplier: " + Math.Round(Player.Multiplier, 1), new Vector2(600, 10), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Score: " + Math.Round(Player.Score, 1), new Vector2(700, 10), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, stats.Message, new Vector2(10, (float)Game1.ScreenY / 8f), Color.Gold, 0, Vector2.Zero, 0.9f, SpriteEffects.None, 0);

            spriteBatch.Draw(mouseTex, currentMouse, null, Color.White, 0, mTexOrigin, 1,SpriteEffects.None, 0);
        }

        private void ControlBoostFactor()
        {
            if (Player.BoostGrowth)
            {
                if (boostFactor < MAX_BOOST) boostFactor += CHANGE_RATE;
                else boostFactor = MAX_BOOST;
            }
            else
            {
                if (boostFactor > MIN_BOOST) boostFactor -= CHANGE_RATE * 2;
                else boostFactor = MIN_BOOST;
            }
        }
    }
}
