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

    class Menu
    {
        public bool Show { get; set; }

        public static Texture2D Normal { get; set; }
        public static Texture2D MouseOver { get; set; }
        public static Texture2D MainTex;
        static bool mouseDown;
        List<MenuItem> items;

        List<MenuControlDelegate> delegates;
        MenuControlDelegate[] runningDelegates;
        bool runDels;
        Rectangle location;

        const int ITEM_XSIXE = 150;
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

        public void Update(GameTime time)
        {
            if (Show)
            {
                if (!mouseDown)
                {
                    foreach (MenuItem m in items)
                    {
                        m.Update();
                    }
                }
                
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mouseDown)
                {
                    mouseDown = true;
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
                else if(Mouse.GetState().LeftButton != ButtonState.Pressed)
                {
                    mouseDown = false;
                }

                if (runDels)
                {
                    runningDelegates[0].Invoke();
                }
            }
        }

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
