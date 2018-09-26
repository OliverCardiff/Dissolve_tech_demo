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
    class ShaderWrapper
    {
        public static Effect DissolveEffect
        {
            get
            {
                return diss;
            }
        }
        static Effect diss;
        static Effect normals;
        public static Effect ParticleShader { get; set; }

        static RenderTarget2D target;
        static RenderTarget2D target2;

        static Texture2D bg;
        static Texture2D saveTex;
        static Texture2D norms;

        static NormalMap nMap;
        static Game1 gRef;

        static bool returnToSaveTex;

        public ShaderWrapper(Game1 game)
        {
            gRef = game;
            diss = game.Content.Load<Effect>("HLSL/Dissolve");
            normals = game.Content.Load<Effect>("HLSL/NormalDistortion");
            bg = game.Content.Load<Texture2D>("bigblack");
            ParticleShader = game.Content.Load<Effect>("HLSL/Particles");

            nMap = new NormalMap(game);
            target = new RenderTarget2D(gRef.GraphicsDevice, Game1.ScreenX, Game1.ScreenY, 1, SurfaceFormat.Rgba64);
            target2 = new RenderTarget2D(gRef.GraphicsDevice, Game1.ScreenX, Game1.ScreenY, 1, SurfaceFormat.Rgba64);
            returnToSaveTex = false;
        }

        public static void ResetAll()
        {
            NormalMap.ResetAll();
            DestroySaveTex();
            ClearBG(Color.Black);

            Game1.bg = bg;

        }

        public static void SaveScreen()
        {
            saveTex = bg;
            returnToSaveTex = true;
        }

        public static void DestroySaveTex()
        {
            saveTex = null;
            returnToSaveTex = false;
        }

        public Texture2D Draw(GameTime gameTime, SpriteBatch spriteBatch, Player player, EnemyManager eManager)
        {
            if (returnToSaveTex)
            {
                bg = saveTex;
                saveTex = null;
                returnToSaveTex = false;
            }
            DrawDistorted(spriteBatch, player, eManager);

            DrawWithNormals(spriteBatch);

            return bg;
        }

        private void DrawDistorted(SpriteBatch spriteBatch, Player player, EnemyManager eManager)
        {
            gRef.GraphicsDevice.SetRenderTarget(0, target);
            gRef.GraphicsDevice.Clear(Color.Black);

            diss.Parameters["xScreenSize"].SetValue(new Vector2(Game1.ScreenX, Game1.ScreenY));
            diss.Parameters["xAllowAlpha"].SetValue(false);
            diss.Parameters["xBoost"].SetValue(Player.BoostGrowth);
            diss.Begin();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            diss.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(bg, new Rectangle(0, 0, Game1.ScreenX, Game1.ScreenY), Color.White);

            //diss.Parameters["xAllowAlpha"].SetValue(true);
            player.Draw(spriteBatch);
            eManager.Draw(spriteBatch);
            diss.CurrentTechnique.Passes[0].End();
            spriteBatch.End();

            diss.End();

            gRef.GraphicsDevice.SetRenderTarget(0, null);
            bg = target.GetTexture();
        }

        public Texture2D DrawDist(SpriteBatch spriteBatch, Texture2D fin)
        {
            gRef.GraphicsDevice.SetRenderTarget(0, target);
            gRef.GraphicsDevice.Clear(Color.Black);

            diss.Parameters["xScreenSize"].SetValue(new Vector2(Game1.ScreenX, Game1.ScreenY));
            diss.Parameters["xAllowAlpha"].SetValue(false);
            diss.Parameters["xBoost"].SetValue(Player.BoostGrowth);
            diss.Begin();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            diss.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(fin, new Rectangle(0, 0, Game1.ScreenX, Game1.ScreenY), Color.White);
            diss.CurrentTechnique.Passes[0].End();
            spriteBatch.End();

            diss.End();

            gRef.GraphicsDevice.SetRenderTarget(0, null);
            gRef.GraphicsDevice.Clear(Color.Black);

            bg = target.GetTexture();

            spriteBatch.Begin();
            spriteBatch.Draw(bg, Vector2.Zero, Color.White);
            spriteBatch.End();

            return bg;

        }

        private void DrawWithNormals(SpriteBatch spriteBatch)
        {
            norms = nMap.DrawNormals(spriteBatch);

            gRef.GraphicsDevice.SetRenderTarget(0, target2);
            gRef.GraphicsDevice.Clear(Color.Black);

            normals.Parameters["xNormalStrength"].SetValue(0.01f);
            normals.Parameters["xTexture"].SetValue(norms);

            normals.Begin();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            normals.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(bg, Vector2.Zero, Color.White);

            normals.CurrentTechnique.Passes[0].End();
            spriteBatch.End();
            normals.End();

            gRef.GraphicsDevice.SetRenderTarget(0, null);

            bg = target2.GetTexture();
        }

        public static void ClearBG(Color color)
        {
            gRef.GraphicsDevice.SetRenderTarget(0, target);

            gRef.GraphicsDevice.Clear(color);
            gRef.GraphicsDevice.SetRenderTarget(0, null);
            bg = target.GetTexture();

            Game1.bg = bg;
        }

        public void Update(GameTime time)
        {
            nMap.Update(time);
        }
    }
}
