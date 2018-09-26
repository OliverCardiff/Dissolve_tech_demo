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
    struct UIPositions
    {
        public static Vector2 Power = new Vector2(220, 10);
        public static Vector2 Multiplier = new Vector2(450, 10);
        public static Vector2 Score = new Vector2(640, 10);
        public static Vector2 SBTimer = new Vector2(10, 570);
        public static Vector2 LifeForce = new Vector2(10, 10);

        public static Vector2 MultiplierOffset = new Vector2(50, 12);
        public static Vector2 PowerOffset = new Vector2(50, 12);
        public static Vector2 ScoreOffset = new Vector2(70, 12);
        public static Vector2 SBOffset = new Vector2(110, 0);
    }
    class UILayer
    {
        public static SpriteFont Font
        {
            get
            {
                return font;
            }
        }

        Effect final;
        static SpriteFont font;
        public static Texture2D MouseTex {get;set;}
        public static Cursor MainCursor { get; set; }
        RenderTarget2D target;
        Game1 gRef;
        static SpriteBatch spriteBatch;
        static List<UIEffect> effects;
        static List<VertexBag> pEffects;

        static float boostFactor;
        static float currentTime;
        const float MAX_BOOST = 1;
        const float MIN_BOOST = 0;
        const float CHANGE_RATE = 0.01f;
        const float TEXT_SIZE = 1f;
        Color TEXT_COLOR = Color.Gold;
        //const float MSG_DISP_TIME = 30;
        //bool displayMsg;

        static UIStats stats;

        public UILayer(Game1 game, SpriteBatch sBatch)
        {
            final = game.Content.Load<Effect>("HLSL/final");
            font = game.Content.Load<SpriteFont>("UI/Arial");
            MouseTex = game.Content.Load<Texture2D>("UI/dot");
            gRef = game;
            boostFactor = 0;

            target = new RenderTarget2D(game.GraphicsDevice, Game1.ScreenX, Game1.ScreenY, 1, SurfaceFormat.Color);

            MenuManager.Assemble(sBatch);
            stats = LevelManager.Current.UIStats;
            effects = new List<UIEffect>();
            pEffects = new List<VertexBag>();
            //displayMsg = false;
            spriteBatch = sBatch;
            MainCursor = new Cursor(Color.White);
        }

        public static void ResetAll()
        {
            effects.Clear();
            currentTime = 0;
            boostFactor = 0;
        }
        public static void NextLevel()
        {
            stats = LevelManager.Current.UIStats;
            currentTime = 0;
            boostFactor = 0;
        }

        public void Update(GameTime time)
        {
            float nexTime = (float)time.ElapsedGameTime.Milliseconds / 1000.0f;
            currentTime += nexTime;
            MenuManager.Update(time);
            UpdateEffects(nexTime);
            //if (currentTime < MSG_DISP_TIME)
            //{
            //    displayMsg = true;
            //}
            //else
            //{
            //    displayMsg = false;
            //}
            ControlBoostFactor();
            MainCursor.Update(nexTime);
        }

        public void DrawToScreen(Texture2D bg, GameTime time)
        {
            bg = DrawToTarget(bg, time);
            //MouseState state = Mouse.GetState();
            //Vector2 currentMouse = new Vector2(state.X, state.Y);

            gRef.GraphicsDevice.Clear(Color.Black);
            final.Parameters["xBoostFactor"].SetValue(boostFactor);
            final.Parameters["xScreenSize"].SetValue(new Vector2(Game1.ScreenX, Game1.ScreenY));
            final.Parameters["xUIBlur"].SetValue(true);
            final.Begin();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            final.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(bg, Vector2.Zero, Color.White);
            //DrawUI(spriteBatch, currentMouse);


            final.CurrentTechnique.Passes[0].End();
            spriteBatch.End();
            final.End();
        }

        public Texture2D DrawToTarget(Texture2D bg, GameTime time)
        {
            MouseState state = Mouse.GetState();
            Vector2 currentMouse = new Vector2(state.X, state.Y);

            gRef.GraphicsDevice.SetRenderTarget(0, target);
            gRef.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            spriteBatch.Draw(bg, Vector2.Zero, Color.White);

            spriteBatch.End();

            DrawParticles(time);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            DrawEffects();
            DrawUI(currentMouse, time);

            spriteBatch.End();

            gRef.GraphicsDevice.SetRenderTarget(0, null);

            return target.GetTexture();

        }

        private void DrawUI(Vector2 currentMouse, GameTime time)
        {
            if (LevelManager.State == GameState.Paused || LevelManager.State == GameState.Running)
            {
                spriteBatch.DrawString(font, "Lifeforce: " + Math.Max(Math.Round(((ForceCounter.CurrentForce - 45) / 55) * 100, 1), 0), 
                    UIPositions.LifeForce, TEXT_COLOR, 0, Vector2.Zero, TEXT_SIZE, SpriteEffects.None, 0);
                if (stats.PowerShown)
                {
                    spriteBatch.DrawString(font, "Power: " + Player.Points + " / " + LevelManager.Current.PlayerStats.MaxPower.ToString(),
                        UIPositions.Power, TEXT_COLOR, 0, Vector2.Zero, TEXT_SIZE, SpriteEffects.None, 0);
                }
                if (stats.SBTimerShown)
                {
                    spriteBatch.DrawString(font, "SmartBomb: ", UIPositions.SBTimer, TEXT_COLOR, 0, Vector2.Zero, 
                        TEXT_SIZE, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, Player.SBCountdown > 0 ? Math.Round(Player.SBCountdown, 1) + " sec" : "Ready",
                        UIPositions.SBTimer + UIPositions.SBOffset, Color.Orange, 0, Vector2.Zero, TEXT_SIZE, SpriteEffects.None, 0);
                }
                spriteBatch.DrawString(font, "Muliplier: " + Math.Round(Player.Multiplier, 1), 
                    UIPositions.Multiplier, Color.Orange, 0, Vector2.Zero, TEXT_SIZE, SpriteEffects.None, 0);

                spriteBatch.DrawString(font, "Score: " + Math.Round(Player.Score, 1), UIPositions.Score,
                    TEXT_COLOR, 0, Vector2.Zero, TEXT_SIZE, SpriteEffects.None, 0);

                spriteBatch.DrawString(font, stats.Message, new Vector2(10, (float)Game1.ScreenY / 8f),
                    Color.Gold, 0, Vector2.Zero, 0.9f, SpriteEffects.None, 0);

            }

            MenuManager.Draw();

            MainCursor.Draw(spriteBatch);
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

        public static void AddUIEffect(UIEffect effect)
        {
            effects.Add(effect);
        }

        public static void AddParticleEffect(VertexBag effect)
        {
            pEffects.Add(effect);
        }

        private void UpdateEffects(float time)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].IsDead)
                {
                    effects.RemoveAt(i);
                }
            }
            for (int i = 0; i < pEffects.Count; i++)
            {
                if (pEffects[i].KillMe)
                {
                    pEffects.RemoveAt(i);
                }
            }
            
            foreach (UIEffect e in effects)
            {
                e.Update(time);
            }
        }

        private void DrawEffects()
        {
            foreach (UIEffect e in effects)
            {
                e.Draw(spriteBatch);
            }
        }

        private void DrawParticles(GameTime time)
        {
            foreach (VertexBag b in pEffects)
            {
                b.Draw(time);
            }
        }

        /*#region Menu stuff
        private void SetUpMenus()
        {
            List<MenuControlDelegate> ds = new List<MenuControlDelegate>();
            ds.Add(new MenuControlDelegate(ShowStartMenu));
            ds.Add(new MenuControlDelegate(LevelManager.ResetGame));
            ds.Add(new MenuControlDelegate(gRef.Exit));

            restartMenu = new Menu(ds, gRef, "Main Menu", "Restart", "Exit");

            List<MenuControlDelegate> ds2 = new List<MenuControlDelegate>();

            ds2.Add(new MenuControlDelegate(LevelManager.RunGame));
            ds2.Add(new MenuControlDelegate(ShowIntructionMenu));
            ds2.Add(new MenuControlDelegate(gRef.Exit));

            MenuControlDelegate[] ds5 = new MenuControlDelegate[2];
            ds5[0] = new MenuControlDelegate(NullMethod);
            ds5[1] = new MenuControlDelegate(DrawTitle);

            startMenu = new Menu(new Vector2(0, 0), ds2, ds5, gRef, "New Game", "Instructions", "Exit");

            List<MenuControlDelegate> ds3 = new List<MenuControlDelegate>();

            ds3.Add(new MenuControlDelegate(ShowStartMenu));

            MenuControlDelegate[] ds4 = new MenuControlDelegate[2];
            ds4[0] = new MenuControlDelegate(NullMethod);
            ds4[1] = new MenuControlDelegate(DrawInstructions);

            instructionMenu = new Menu(new Vector2(0, 250), ds3, ds4, gRef, "Return");


        }

        public static void ShowRestartMenu()
        {
            restartMenu.Show = true;
        }

        public static void ShowStartMenu()
        {
            startMenu.Show = true;
            if (LevelManager.State == GameState.RestartMenu)
            {
                LevelManager.ResetGame();
                LevelManager.State = GameState.MainMenu;
                ShaderWrapper.ClearBG(Color.Tomato);
            }
        }

        static void ShowIntructionMenu()
        {
            instructionMenu.Show = true;
        }

        static void DrawTitle()
        {
            if (titleTimer > TITLE_FLASH_RATE)
            {
                titleTimer = 0;

                spriteBatch.DrawString(font, "DISSOLVE", new Vector2(145, 30), Color.WhiteSmoke,
                    0, Vector2.Zero, 5, SpriteEffects.None, 0);
            }
        }

        static void NullMethod()
        {
        }
        static void WriteInstructions()
        {
            instructions = "Instructions\n\nYou are the life giver, and the protector of this\nenvironment."
                + " Your bullets destroy intruders, yet\nhurt your world, use them wisely.\n"
                + "Run out of Lifeforce, and you shall perish\n\n"
                + "Right Trigger (hold) - Normal Fire \n"
                + "(Once unlocked) Right Bumper - Create LifeBranch\n"
                + "(Once unlocked) Left Trigger (hold) - Growth Booster\n"
                + "(Once unlocked) Left Bumper - Fire Smartbomb\n";
        }
        static void DrawInstructions()
        {
            spriteBatch.Draw(Menu.MainTex, new Rectangle(50, 50, 700, 400), Color.White);
            spriteBatch.DrawString(font, instructions, new Vector2(130, 90), Color.White, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
        }
        #endregion*/

    }
}
