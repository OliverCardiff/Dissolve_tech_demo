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
    /// This is the main type for your game
    /// </summary>
    enum GameState
    {
        Running, Paused, MainMenu, RestartMenu
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region DrawingClasses
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D bg;
        ShaderWrapper shaderWrapper;
        UILayer uILayer;
        #endregion

        Player player;
        EnemyManager eManager;
        ForceCounter counter;
        //Menu startMenu;

        public static int ScreenX = 800;
        public static int ScreenY = 600;
        public static Random rand;

        float currentTime = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = ScreenY;
            graphics.PreferredBackBufferWidth = ScreenX;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LevelManager.Assemble();

            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            rand = new Random();
            player = new Player(this);
            eManager = new EnemyManager(this);
            uILayer = new UILayer(this, spriteBatch);
            shaderWrapper = new ShaderWrapper(this);
            counter = new ForceCounter(this);

            ShaderWrapper.ClearBG(Color.SteelBlue);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (LevelManager.State == GameState.Running)
            {
                player.Update(gameTime);
                eManager.Update(gameTime);
                currentTime += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                counter.Update();

                if (currentTime >= 0.5f)
                {
                    counter.CountForce();
                }
            }

            uILayer.Update(gameTime);
            shaderWrapper.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (LevelManager.State == GameState.Running)
            {
                bg = shaderWrapper.Draw(gameTime, spriteBatch, player, eManager);

                if (currentTime > 0.5f)
                {
                    currentTime = 0;
                    counter.DrawToColorTarget(spriteBatch, bg);
                }
                uILayer.DrawToScreen(bg);
            }
            else
            {
                bg = shaderWrapper.DrawDist(spriteBatch, uILayer.DrawToTarget(bg));
            }
            base.Draw(gameTime);
        }
    }
}
