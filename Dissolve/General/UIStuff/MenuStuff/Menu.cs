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
    delegate void MenuControlDelegate();

    /// <summary>
    /// The general Menu class
    /// </summary>
    class Menu
    {
        /// <summary>
        /// Control variable used to check whether or not to update and draw the menu
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// The regular texture for a MenuItem
        /// </summary>
        public static Texture2D Normal { get; set; }
        /// <summary>
        /// The texture used when a MenuItem is selected/highlighed
        /// </summary>
        public static Texture2D MouseOver { get; set; }
        /// <summary>
        /// The main background texture for the Menu
        /// </summary>
        public static Texture2D MainTex;
        //Describes the current state of the button A, used to preven multipe 
        //selections in a single press
        static bool aDown;

        //The items contained in the current menu
        List<MenuItem> items;

        //Delegates which contain the functionality for each MenuItem
        List<MenuControlDelegate> delegates;

        //Two delegates which represent additional methods to be called during the
        //Menu's update and draw when it is visible
        MenuControlDelegate[] runningDelegates;

        //Whether or not to attempt to run either of the runningDelegates
        bool runDels;

        //The location and size of the Menu in screenspace
        Rectangle location;

        const int ITEM_XSIXE = 150;
        //Border between each menu item, and between the items and the edge of the menu box
        const int MENU_BORDER = 30;
        const int ITEM_YSIZE = 35;

        #region Constructor Methods

        public Menu(Vector2 offset, List<MenuControlDelegate> delegates, Game1 game, params string[] names)
        {
            Vector2 menuSize = new Vector2(ITEM_XSIXE + MENU_BORDER * 2, names.Length * ITEM_YSIZE + (names.Length + 1) * MENU_BORDER);
            Vector2 position = new Vector2((Game1.ScreenX / 2) - (menuSize.X / 2), (Game1.ScreenY / 2) - (menuSize.Y / 2));
            location = new Rectangle((int)position.X, (int)position.Y, (int)menuSize.X, (int)menuSize.Y);

            Point p = new Point();
            p = location.Location;

            p.X += (int)offset.X;
            p.Y += (int)offset.Y;

            location.Location = p;

            SetUpItems(location.Location, names);
            this.delegates = delegates;
            Show = false;

            LoadTextures(game);
            runDels = false;
        }

        public Menu(Vector2 offset, List<MenuControlDelegate> delegates, MenuControlDelegate[] runningDels, Game1 game, params string[] names)
        {
            Vector2 menuSize = new Vector2(ITEM_XSIXE + MENU_BORDER * 2, names.Length * ITEM_YSIZE + (names.Length + 1) * MENU_BORDER);
            Vector2 position = new Vector2((Game1.ScreenX / 2) - (menuSize.X / 2), (Game1.ScreenY / 2) - (menuSize.Y / 2));
            location = new Rectangle((int)position.X, (int)position.Y, (int)menuSize.X, (int)menuSize.Y);

            Point p = new Point();
            p = location.Location;

            p.X += (int)offset.X;
            p.Y += (int)offset.Y;

            location.Location = p;

            SetUpItems(location.Location, names);
            this.delegates = delegates;
            runningDelegates = runningDels;
            runDels = true;
            Show = false;

            LoadTextures(game);
        }

        public Menu(List<MenuControlDelegate> delegates, Game1 game, params string[] names)
        {
            Vector2 menuSize = new Vector2(ITEM_XSIXE + MENU_BORDER * 2, names.Length * ITEM_YSIZE + (names.Length + 1) * MENU_BORDER);
            Vector2 position = new Vector2((Game1.ScreenX / 2) - (menuSize.X / 2), (Game1.ScreenY / 2) - (menuSize.Y / 2));
            location = new Rectangle((int)position.X, (int)position.Y, (int)menuSize.X, (int)menuSize.Y);

            SetUpItems(location.Location, names);
            this.delegates = delegates;
            Show = false;

            LoadTextures(game);
            runDels = false;
        }

        private void SetUpItems(Point position, string[] names)
        {
            items = new List<MenuItem>();

            for (int i = 0; i < names.Length; i++)
            {
                items.Add(new MenuItem(names[i], new Rectangle(
                    (int)position.X + MENU_BORDER,
                    (int)position.Y + MENU_BORDER + (i * (MENU_BORDER + ITEM_YSIZE)),
                     ITEM_XSIXE,
                     ITEM_YSIZE))
                    );
            }
        }

        private static void LoadTextures(Game1 game)
        {
            if (MainTex == null)
            {
                MainTex = game.Content.Load<Texture2D>("UI/mainmenu");
                MouseOver = game.Content.Load<Texture2D>("UI/mouseover");
                Normal = game.Content.Load<Texture2D>("UI/normal");
            }
        }

        #endregion

        /// <summary>
        /// Updates all MenuItems, checks input and Invokes all delegates where necessary
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            if (Show)
            {
                if (!aDown)
                {
                    foreach (MenuItem m in items)
                    {
                        m.Update(state);
                    }
                }
                
                if (state.Buttons.A == ButtonState.Pressed && !aDown)
                {
                    aDown = true;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].Clicked)
                        {
                            delegates[i].Invoke();
                            items[i].Clicked = false;
                            Show = false;
                            break;
                        }
                    }
                }
                else if (state.Buttons.A != ButtonState.Pressed)
                {
                    aDown = false;
                }

                if (runDels)
                {
                    runningDelegates[0].Invoke();
                }
            }
        }
        /// <summary>
        /// Draws the menu and all MenuItems, and invokes the a runningDelegate where necessary
        /// </summary>
        /// <param name="s">The spritebatch to draw the menu with</param>
        public void Draw(SpriteBatch s)
        {
            if (Show)
            {
                s.Draw(MainTex, location, Color.White);

                foreach (MenuItem m in items)
                {
                    m.Draw(s);
                }

                if (runDels)
                {
                    runningDelegates[1].Invoke();
                }
            }
        }
    }
}
