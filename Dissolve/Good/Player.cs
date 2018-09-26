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
        #region General Variables
        static Vector2 position;
        static Vector2 origin;
        static Vector2 velocity;

        //Normal player texture
        static Texture2D tex;
        //finalform player texture
        static Texture2D finalTex;
        //normal green bullet texture
        static Texture2D bTex;
        //finalform bullet texture
        static Texture2D finalBTex;
        //purple bullet texture
        static Texture2D bTex2;
        //lifebranch segment texture
        static Texture2D bTex3;
        //wormmode smartbomb texture
        static Texture2D bTex4;
        //Player-worm-tail texture for lvl8+ wormmode
        static Texture2D wForm;
        //Player-worm-tail texture for lvl10+ wormmode
        static Texture2D finalWForm;

        float scale;
        float rotation;
        #endregion

        /// <summary>
        /// List of all collidable bullets
        /// </summary>
        public static List<Bullet> bullets;
        /// <summary>
        /// List of all non-collidable bullets
        /// </summary>
        public static List<Bullet> growers;
        /// <summary>
        /// List of all players smartbomb blastwaves (usually only one)
        /// </summary>
        public static List<BlastWave> blasts;

        //reference to the Player-worm-tail
        static PlayerWorm WormForm;

        #region Properties
        /// <summary>
        /// Player's current power
        /// </summary>
        public static int Points
        {
            get
            {
                return points;
            }
            set
            {
                if (value > stats.MaxPower)
                {
                    points = stats.MaxPower;
                }
                else
                {
                    points = value;
                }
            }
        }
        static int points;

        /// <summary>
        /// Describes the standard bullet speed
        /// </summary>
        public static float BulletSpeed
        {
            get
            {
                return BULLET_SPEED;
            }
        }
        /// <summary>
        /// The player's current position
        /// </summary>
        public static Vector2 Position
        {
            get
            {
                return position;
            }
        }
        /// <summary>
        /// The Player's current score
        /// </summary>
        public static float Score { get; set; }
        /// <summary>
        /// The player's current multiplier
        /// </summary>
        public static float Multiplier { get; set; }
        /// <summary>
        /// The current countdown to the next use of the smartbomb
        /// </summary>
        public static float SBCountdown
        {
            get
            {
                return sBombTimer;
            }
        }
        /// <summary>
        /// Indicates the current state of the growth-boost power
        /// </summary>
        public static bool BoostGrowth { get; set; }
        /// <summary>
        /// Control Varible used to check if game is over
        /// </summary>
        public static bool IsDead { get; set; }
        #endregion

        #region Constants
        //Amount of time between player dieing and the game ending
        const float TIME_TILL_END = 5;
        const float OFFSET = 5;
        const float MIN_SCALE = 0.3f;
        const int POINTS_TO_BRANCH = 5;
        const int BOOST_COST = 5;
        const int SBOMB_COST = 20;
        const float BOOST_TIME = 0.3f;
        const float BRANCH_SPEED = 1.5f;
        const float MIN_SBOMB_TIMER = 5;
        const float MAX_SBOMB_TIMER = 15;
        const float SB_INCREMENT = 0.1f;
        public static float BULLET_SPEED = 6;
        #endregion

        #region Control Variables
        //describes the current state of the right and left bumbers
        //used to stop multiple activations of an ability in one press
        bool rbDown = false;
        bool lbDown = false;
        //Used to tell when to start the countdown to ending the game after the player is dead
        static bool startFinalTimer;
        //final countdown timer
        static float finalTimer;
        //is incremented whenever the player uses the growth boost
        //growth boost is on while this != 0
        float growthTimer;
        //countdown to next possible smartbomb use
        static float sBombTimer;
        #endregion

        //Level specific player statistics retrieved from level manager
        static PlayerFlags stats;

        /// <summary>
        /// Player constructor loads all textures and initializes all static members
        /// </summary>
        /// <param name="game"></param>
        public Player(Game1 game)
        {
            position = new Vector2(Game1.ScreenX / 2, Game1.ScreenY / 2);
            velocity = Vector2.Zero;
            tex = game.Content.Load<Texture2D>("PlayerTex/player");
            finalTex = game.Content.Load<Texture2D>("PlayerTex/finalPlayer");
            bTex = game.Content.Load<Texture2D>("PlayerTex/bullet");
            bTex2 = game.Content.Load<Texture2D>("PlayerTex/bullet2");
            bTex3 = game.Content.Load<Texture2D>("PlayerTex/bullet3");
            finalBTex = game.Content.Load<Texture2D>("PlayerTex/finalBullet");
            wForm = game.Content.Load<Texture2D>("PlayerTex/wormForm");
            finalWForm = game.Content.Load<Texture2D>("PlayerTex/finalWormForm");
            bTex4 = game.Content.Load<Texture2D>("PlayerTex/bullet4");
            bullets = new List<Bullet>();
            growers = new List<Bullet>();
            blasts = new List<BlastWave>();
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            scale = 1;
            Points = 0;
            rotation = 0;
            growthTimer = 0;
            stats = LevelManager.Current.PlayerStats;
            CheckCreateWorm();
            sBombTimer = MIN_SBOMB_TIMER;
        }

        //All public static interactions with the player 
        #region Static Interaction

        /// <summary>
        /// Updates the player statistics to the current level state
        /// </summary>
        public static void NextLevel()
        {
            stats = LevelManager.Current.PlayerStats;
            CheckCreateWorm();
            if (stats.WormForm && stats.ColorChange)
            {
                WormForm.UpdateChainTexture(finalWForm);
            }
        }

        /// <summary>
        /// Resets all relevant variables to original state
        /// </summary>
        public static void ResetAll()
        {
            blasts.Clear();
            position = new Vector2(Game1.ScreenX / 2, Game1.ScreenY / 2);
            velocity = Vector2.Zero;
            bullets.Clear();
            growers.Clear();
            Points = 0;
            Multiplier = 1;
            Score = 0;
            sBombTimer = MIN_SBOMB_TIMER;
            finalTimer = 0;
            startFinalTimer = false;
            BoostGrowth = false;
            IsDead = false;
            WormForm = null;
        }

        /// <summary>
        /// Adds a timed bullet to the non-collidable bullets list
        /// </summary>
        /// <param name="posi">Position to spawn the bullet at</param>
        /// <param name="velo">Start velocity of the bullet</param>
        /// <param name="life">Lifetime of the bullet in seconds</param>
        public static void AddGrower(Vector2 posi, Vector2 velo, float life)
        {
            growers.Add((Bullet)new TimedBullet(posi, velo, bTex2, life));
        }
        /// <summary>
        /// Adds a timed bullet to the non-collidable bullets list
        /// </summary>
        /// <param name="b">The Timed bullet to add</param>
        public static void AddGrower(TimedBullet b)
        {
            growers.Add((Bullet)b);
        }
        /// <summary>
        /// Adds a brancher bullet to the collidable bullets list
        /// </summary>
        /// <param name="b">The brancher to add</param>
        public static void AddGrower(Brancher b)
        {
            bullets.Add((Bullet)b);
        }

        /// <summary>
        /// Increments the smartbomb countdown timer by the standard amount
        /// Cannot increase it past the limit
        /// </summary>
        public static void IncrementNextSBTimer()
        {
            if (sBombTimer < MAX_SBOMB_TIMER)
            {
                sBombTimer += SB_INCREMENT;
            }
            else
            {
                sBombTimer = MAX_SBOMB_TIMER;
            }
        }

        /// <summary>
        /// Kills the player and starts the countdown to the end of the game
        /// </summary>
        public static void StartFinalTimer()
        {
            startFinalTimer = true;
            if (!ForceCounter.Victory)
            {
                IsDead = true;
                NormalMap.AddStarWave(Position, 3.4f, 8);
            }
        }
        #endregion

        #region General Update
        public void Update(GameTime time)
        {
            HandleDeath(time);
            if (!IsDead)
            {
                ProcessInput(time);
                ScreenWrap();
                position += velocity;
                rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            }

            Multiplier = Math.Max(((ForceCounter.CurrentForce - 50.0f) / 50.0f) * stats.MaxMultiplier, 1.0f);

            AddUIParticles();

            ManageBullets(time);

            if (stats.WormForm) WormForm.Update(false, (float)time.ElapsedGameTime.Milliseconds / 1000.0f);
            if (ForceCounter.CurrentForce > 0)
            {
                scale = (ForceCounter.CurrentForce / 40) + MIN_SCALE;
            }
            else
            {
                scale = MIN_SCALE;
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

        public void Draw(SpriteBatch s)
        {
            DrawPlayer(s);
            foreach (Bullet b in bullets)
            {
                b.Draw(s);
            }
            foreach (Bullet b in growers)
            {
                b.Draw(s);
            }
        }

        private void DrawPlayer(SpriteBatch s)
        {
            if (!IsDead)
            {
                if (!stats.ColorChange)
                {
                    s.Draw(tex, position, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
                }
                else
                {
                    s.Draw(finalTex, position, null, Color.White, rotation, origin, (scale + 0.5f), SpriteEffects.None, 0);
                }

                if (stats.WormForm)
                {
                    WormForm.Draw(s);
                }
            }
        }
        #endregion

        //Handling all input from the gamepad and all weapon firing/use
        #region HandleInput
        private void ProcessInput(GameTime time)
        {
            GamePadState gState = GamePad.GetState(PlayerIndex.One);

            HandleMovement(gState);

            if (EnemyManager.StartSpawns)
            {
                HandleFiring(gState);
            }

            if (stats.BranchingOn)
            {
                HandleBranches(gState);
            }

            if (stats.GrowthBoostOn)
            {
                HandleBoost(time, gState);
            }

            if (stats.SmartBombOn)
            {
                HandleSBomb(gState, time);
            }
        }

        private void HandleSBomb(GamePadState gState, GameTime time)
        {
            sBombTimer -= (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            if (sBombTimer < 0)
            {
                sBombTimer = 0;
            }

            if (gState.Buttons.LeftShoulder == ButtonState.Pressed && !lbDown && sBombTimer == 0)
            {
                lbDown = true;
                if (points >= SBOMB_COST)
                {
                    sBombTimer = MIN_SBOMB_TIMER;

                    if (LevelManager.Mode != GameMode.Worms)
                    {
                        blasts.Add(NormalMap.AddBlast(Position, 5, 1));
                        points -= SBOMB_COST;
                    }
                    else
                    {
                        float angle;
                        Vector2 v = new Vector2();

                        for (int i = 0; i < 6; i++)
                        {
                            angle = MathHelper.TwoPi * ((float)(i + 1) / 6.0f);
                            v.X = (float)Math.Cos(angle) * Player.BULLET_SPEED;
                            v.Y = (float)Math.Sin(angle) * Player.BULLET_SPEED;
                            SBBrancher b = new SBBrancher(position, bTex4, 2, angle, 0, 20);

                            bullets.Add(b);
                        }

                        points = 0;
                    }
                }
            }
            else if (gState.Buttons.LeftShoulder == ButtonState.Released)
            {
                lbDown = false;
            }
        }

        private void HandleMovement(GamePadState gState)
        {
            Vector2 movement = new Vector2();
            movement.X = gState.ThumbSticks.Left.X;
            movement.Y = -gState.ThumbSticks.Left.Y;

            velocity += movement * stats.Acceleration;

            velocity *= 0.95f;
            //velocity.X = MathHelper.Clamp(velocity.X, -stats.Speed, stats.Speed);
            //velocity.Y = MathHelper.Clamp(velocity.Y, -stats.Speed, stats.Speed);
        }

        private void HandleFiring(GamePadState gState)
        {
            if (gState.Triggers.Right > 0)
            {
                Vector2 mPos;
                if (gState.ThumbSticks.Right.Length() > 0)
                {
                    mPos = new Vector2(gState.ThumbSticks.Right.X, -gState.ThumbSticks.Right.Y);
                }
                else
                {
                    mPos = velocity;
                }
                mPos.Normalize();
                Bullet b;

                if (!stats.ColorChange)
                {
                    b = new Bullet(position, mPos * BULLET_SPEED, bTex);
                }
                else
                {
                    b = new Bullet(position, mPos * BULLET_SPEED, finalBTex);
                }
                
                bullets.Add(b);
            }
        }

        private void HandleBranches(GamePadState gState)
        {
            if (gState.Buttons.RightShoulder == ButtonState.Pressed && Points >= POINTS_TO_BRANCH && !rbDown)
            {
                float rotation;

                if (gState.ThumbSticks.Right.Length() > 0)
                {
                    rotation = (float)Math.Atan2(-gState.ThumbSticks.Right.Y, gState.ThumbSticks.Right.X);
                }
                else
                {
                    rotation = (float)Math.Atan2(velocity.Y, velocity.X);
                }
                int length = Math.Min((Math.Min(Points, 60) / POINTS_TO_BRANCH) + stats.BranchAmp, stats.MaxBranchLength);
                Brancher b = new Brancher(position, Vector2.Zero, bTex3, BRANCH_SPEED, rotation, 0, length);
                growers.Add((Bullet)b);

                b = new Brancher(position, Vector2.Zero, bTex3, BRANCH_SPEED, rotation, 0, length);
                growers.Add((Bullet)b);
                b = new Brancher(position, Vector2.Zero, bTex3, BRANCH_SPEED, rotation, 0, length);
                growers.Add((Bullet)b);

                rbDown = true;
                if (Points <= 60)
                {
                    Points = 0;
                }
                else
                {
                    Points -= 60;
                }


            }
            if (gState.Buttons.RightShoulder == ButtonState.Released)
            {
                rbDown = false;
            }
        }

        private void HandleBoost(GameTime time, GamePadState gState)
        {
            if (gState.Triggers.Left > 0 && Points >= BOOST_COST)
            {
                growthTimer -= (float)time.ElapsedGameTime.Milliseconds / 1000.0f;
                if (growthTimer <= 0)
                {
                    Points -= BOOST_COST;
                    growthTimer = BOOST_TIME;
                }

                BoostGrowth = true;
            }
            else
            {
                BoostGrowth = false;
            }
        }
        #endregion

        //Adds effects to the UI appropriate to the current scores
        private static void AddUIParticles()
        {
            if (Multiplier > 1)
            {
                VertexBag v = new VertexBag(UIPositions.Multiplier + UIPositions.MultiplierOffset, Color.White, Color.Yellow, 1, (int)((Multiplier / 5) * 50), (int)Multiplier + 4);
                UILayer.AddParticleEffect(v);
            }

            if (Points > 0)
            {
                VertexBag v = new VertexBag(UIPositions.Power + UIPositions.PowerOffset, Color.CadetBlue, Color.Cyan, 1, (int)((float)(Points / (float)stats.MaxPower) * 80), (int)((float)Points / (float)stats.MaxPower) * 5 + 3);
                UILayer.AddParticleEffect(v);
            }
        }

        //Tries to create the Players tail in wormmode
        private static void CheckCreateWorm()
        {
            if (stats.WormForm && WormForm == null)
            {
                WormForm = new PlayerWorm(wForm, 0, stats.WormLength, null);
            }
        }

        //Finishes the game as the finaltimer ends
        private static void HandleDeath(GameTime time)
        {
            if (startFinalTimer)
            {
                finalTimer += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;
                if (finalTimer > TIME_TILL_END)
                {
                    LevelManager.State = GameState.RestartMenu;
                    MenuManager.ShowRestartMenu();
                }
            }
        }

        #region Bullets
        //Updates and calls DeleteBullets for all bullets blastwave
        private static void ManageBullets(GameTime time)
        {
            int loopControl = bullets.Count;

            for (int i = 0; i < loopControl; i++)
            {
                CollisionGrid.Commit(bullets[i]);
                bullets[i].Update(time);
            }
            loopControl = growers.Count;

            for (int i = 0; i < loopControl; i++)
            {
                growers[i].Update(time);
            }
            DeleteBullets(bullets, false);
            DeleteBullets(growers, true);

            for (int i = 0; i < blasts.Count; i++)
            {
                if (blasts[i].HalfDead)
                {
                    blasts.RemoveAt(i);
                }
            }
        }

        //Checks for and removes Dead bullets
        public static void DeleteBullets(List<Bullet> bs, bool large)
        {
            float scale;
            if (large)
            {
                scale = 0.7f;
            }
            else
            {
                scale = 0.5f;
            }

            for (int i = 0; i < bs.Count; i++)
            {
                if (bs[i].IsDead)
                {
                    NormalMap.AddBlast(bs[i].Position, scale, scale);

                    bs.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
