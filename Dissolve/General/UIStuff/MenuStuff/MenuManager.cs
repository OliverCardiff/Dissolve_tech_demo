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
    /// Manages, Draws, Updates and Creates all Menus
    /// Also handles pause requests
    /// </summary>
    static class MenuManager
    {
        //Menu to be shown on death or victory
        static Menu restartMenu;
        //Menu to be shown when the player loads the game, the "Main Menu"
        static Menu startMenu;
        //Simply displays text-based instructions with the option to return
        //to the main menu
        static Menu instructionMenu;
        //menu to be shown when the game is paused
        static Menu pauseMenu;
        //Displays bonus game modes
        static Menu bonusMenu;
        //The spritebatch to draw all the menus with
        static SpriteBatch spriteBatch;

        const float TITLE_FLASH_RATE = 3f;
        const float TITLE_REFRESH_RATE = 10f;
        static float titleTimer;
        static string instructions;
        //Colours which the flashing title cycles through
        static Color[] titleColors;
        //Colours which the title-menu clear cycles through
        static Color[] titleColors2;
        //represents the current colour used for the title
        static int colorIndex;
        //represents the current colour for the screen clear
        static int colorIndex2;

        static bool startDown = false;
        static bool doReset = false;

        /// <summary>
        /// Builds all menus, arrays and delegates
        /// </summary>
        /// <param name="batch">The spriteBatch to use to draw menus</param>
        public static void Assemble(SpriteBatch batch)
        {
            spriteBatch = batch;
            titleTimer = 0;
            WriteInstructions();
            SetUpMenus();
            startMenu.Show = true;
            titleColors = new Color[3];
            titleColors[0] = Color.Red;
            titleColors[1] = Color.Blue;
            titleColors[2] = Color.Green;
            titleColors2 = new Color[3];
            titleColors2[0] = Color.White;
            titleColors2[1] = Color.Tomato;
            titleColors2[2] = Color.Cyan;
        }
        
        /// <summary>
        /// Draws all menus if the GameState isn't running
        /// </summary>
        public static void Draw()
        {
            if (LevelManager.State != GameState.Running)
            {
                startMenu.Draw(spriteBatch);
                instructionMenu.Draw(spriteBatch);
                restartMenu.Draw(spriteBatch);
                pauseMenu.Draw(spriteBatch);
                bonusMenu.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Handles all menu updates when the GameState isn't running
        /// and pause requests when it is
        /// </summary>
        /// <param name="time">Snapshot fo the curretn gameTime</param>
        public static void Update(GameTime time)
        {
            HandlePauseRequest();

            float nexTime = (float)time.ElapsedGameTime.Milliseconds / 1000.0f;
            titleTimer += nexTime;

            if (doReset)
            {
                colorIndex2++;
                if (colorIndex2 == titleColors2.Length) colorIndex2 = 0;
                ShaderWrapper.ClearBG(titleColors2[colorIndex2]);
                doReset = false;
            }
            if (LevelManager.State != GameState.Running)
            {
                pauseMenu.Update(time);
                startMenu.Update(time);
                restartMenu.Update(time);
                instructionMenu.Update(time);
                bonusMenu.Update(time);
            }
        }

        //Checks whether the player is trying to pause the game and displays the pause menu if they are
        //Also checks whether the player has pressed start whilst on the pause menu, and runs the game again
        private static void HandlePauseRequest()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
            {
                if (LevelManager.State == GameState.Running && !startDown)
                {
                    ShaderWrapper.SaveScreen();
                    LevelManager.State = GameState.Paused;
                    pauseMenu.Show = true;
                }
                else if (LevelManager.State == GameState.Paused && !startDown)
                {
                    RunGame();
                }
                startDown = true;
            }
            else
            {
                startDown = false;
            }
        }

        //Builds all menus and delegates
        private static void SetUpMenus()
        {
            //Creates the restart menu
            List<MenuControlDelegate> ds = new List<MenuControlDelegate>();
            ds.Add(new MenuControlDelegate(ShowStartMenu));
            ds.Add(new MenuControlDelegate(LevelManager.ResetGame));
            ds.Add(new MenuControlDelegate(Game1.game.Exit));

            restartMenu = new Menu(ds, Game1.game, "Main Menu", "Restart", "Exit");



            //Creates the Start Menu and adds the 'draw title' running delegate
            List<MenuControlDelegate> ds2 = new List<MenuControlDelegate>();
            ds2.Add(new MenuControlDelegate(RunNormalGame));
            ds2.Add(new MenuControlDelegate(ShowBonusMenu));
            ds2.Add(new MenuControlDelegate(ShowIntructionMenu));
            ds2.Add(new MenuControlDelegate(Game1.game.Exit));

            MenuControlDelegate[] ds5 = new MenuControlDelegate[2];
            ds5[0] = new MenuControlDelegate(NullMethod);
            ds5[1] = new MenuControlDelegate(DrawTitle);

            startMenu = new Menu(new Vector2(0, 0), ds2, ds5, Game1.game, "New Game", "Bonus Modes", "Instructions", "Exit");


            //Creates the instruction menu, and adds the Draw instructions running delegate
            List<MenuControlDelegate> ds3 = new List<MenuControlDelegate>();
            ds3.Add(new MenuControlDelegate(ShowStartMenu));

            MenuControlDelegate[] ds4 = new MenuControlDelegate[2];
            ds4[0] = new MenuControlDelegate(NullMethod);
            ds4[1] = new MenuControlDelegate(DrawInstructions);

            instructionMenu = new Menu(new Vector2(0, 250), ds3, ds4, Game1.game, "Return");



            //Creates the Pause menu and adds the DrawPause Title running delegate
            List<MenuControlDelegate> ds6 = new List<MenuControlDelegate>();

            ds6.Add(new MenuControlDelegate(RunGame));
            ds6.Add(new MenuControlDelegate(LevelManager.ResetGame));
            ds6.Add(new MenuControlDelegate(ShowStartMenu));

            MenuControlDelegate[] ds7 = new MenuControlDelegate[2];
            ds7[0] = new MenuControlDelegate(NullMethod);
            ds7[1] = new MenuControlDelegate(DrawPauseTitle);

            pauseMenu = new Menu(Vector2.Zero, ds6, ds7, Game1.game, "Resume", "Restart", "MainMenu");


            //Creates the Bonus menu
            List<MenuControlDelegate> ds8 = new List<MenuControlDelegate>();

            ds8.Add(new MenuControlDelegate(RunWormGame));
            ds8.Add(new MenuControlDelegate(ShowStartMenu));

            bonusMenu = new Menu(ds8, Game1.game, "Worm Hell", "Return");
        }

        public static void ShowRestartMenu()
        {
            restartMenu.Show = true;
        }

        public static void ShowStartMenu()
        {
            startMenu.Show = true;
            ShaderWrapper.DestroySaveTex();
            if (LevelManager.State != GameState.OtherMenu)
            {
                LevelManager.ResetGame();
                LevelManager.State = GameState.MainMenu;
                ShaderWrapper.ClearBG(Color.White);
            }
        }

        static void ShowIntructionMenu()
        {
            LevelManager.State = GameState.OtherMenu;
            instructionMenu.Show = true;
        }
        static void ShowBonusMenu()
        {
            LevelManager.State = GameState.OtherMenu;
            bonusMenu.Show = true;
        }
        static void DrawTitle()
        {
            if (titleTimer > TITLE_FLASH_RATE)
            {
                titleTimer = 0;
                colorIndex++;
                if (colorIndex == titleColors.Length)
                {
                    colorIndex = 0;
                    doReset = true;
                }
                else
                {

                    spriteBatch.DrawString(UILayer.Font, "DISSOLVE", new Vector2(150, 30), titleColors[colorIndex],
                        0, Vector2.Zero, 5, SpriteEffects.None, 0);
                }
            }
        }

        static void DrawPauseTitle()
        {
            if (titleTimer > TITLE_FLASH_RATE)
            {
                titleTimer = 0;

                spriteBatch.DrawString(UILayer.Font, "PAUSED", new Vector2(200, 30), Color.WhiteSmoke,
                    0, Vector2.Zero, 5, SpriteEffects.None, 0);
            }
        }

        static void RunGame()
        {
            LevelManager.State = GameState.Running;
        }

        static void RunNormalGame()
        {
            LevelManager.Mode = GameMode.Normal;
            LevelManager.RunGame();
            LevelManager.NextLevel(false);
        }

        static void RunWormGame()
        {
            LevelManager.Mode = GameMode.Worms;
            LevelManager.RunGame();
            LevelManager.NextLevel(false);
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
            spriteBatch.DrawString(UILayer.Font, instructions, new Vector2(130, 90), Color.White, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
        }
    }
}
