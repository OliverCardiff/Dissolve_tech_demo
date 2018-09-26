using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dissolve
{
    class EnemyManager
    {
        const float SPLIT_TIMER = 6.0f;
        const float SPAWN_A = 6.0f;
        const float SPAWN_B = 4.0f;
        const float SPAWN_C = 4.0f;
        const float SPAWN_D = 2.0f;

        static int counterA;
        static int counterB;
        static int counterC;
        static int counterD;

        static float currentTime;
        static bool bossMode;
        float splitTime;
        static List<Enemy> enemies;

        #region Textures
        public static Texture2D BulletTex { get; set; }
        public static Texture2D EBranchTex { get; set; }
        public static Texture2D seedTex { get; set; }
        public static Texture2D chaseTex { get; set; }
        public static bool StartSpawns { get; set; }
        public static Texture2D WormTex { get; set; }
        public static Texture2D WBossTex { get; set; }
        Texture2D divideTex;
        Texture2D spinTex;
        Texture2D bossTex;
        #endregion

        Game1 gRef;

        float screenRadius;

        const int MAX_SPINNERS = 2;

        static ManagerStats stats;

        static Boss boss;
        static WormBoss wBoss;

        public EnemyManager(Game1 game)
        {
            gRef = game;
            chaseTex = gRef.Content.Load<Texture2D>("EnemyTex/chaser");
            divideTex = gRef.Content.Load<Texture2D>("EnemyTex/divider");
            spinTex = gRef.Content.Load<Texture2D>("EnemyTex/spinner");
            BulletTex = gRef.Content.Load<Texture2D>("EnemyTex/chaserBullet");
            EBranchTex = gRef.Content.Load<Texture2D>("EnemyTex/brancher");
            bossTex = gRef.Content.Load<Texture2D>("EnemyTex/boss");
            seedTex = gRef.Content.Load<Texture2D>("EnemyTex/seed");
            WormTex = gRef.Content.Load<Texture2D>("EnemyTex/worm");
            WBossTex = gRef.Content.Load<Texture2D>("EnemyTex/wormBoss");
            enemies = new List<Enemy>();

            currentTime = 0;
            counterA = 0;
            counterB = 0;
            counterC = 0;
            counterD = 0;
            splitTime = 0;

            StartSpawns = false;
            float halfH = (float)Game1.ScreenY / 2.0f;
            float halfW = (float)Game1.ScreenX / 2.0f;
            screenRadius = (float)Math.Sqrt(halfH * halfH + halfW * halfW);

            stats = LevelManager.Current.EnemyManage;
        }

        public static void ResetAll()
        {
            StartSpawns = false;
            boss = null;
            wBoss = null;
            enemies.Clear();
            ESpinner.CurrentCount = 0;
            EWorm.CurrentCount = 0;
        }

        public void CheckHits()
        {
            float dist;

            foreach (BlastWave b in Player.blasts)
            {
                foreach (Enemy e in enemies)
                {
                    dist = Vector2.Distance(e.Position, b.Position);

                    if (dist < b.Radius)
                    {
                        e.Hit(5);
                        Player.IncrementNextSBTimer();
                    }
                }
            }

            if (boss != null)
            {
                foreach (Bullet b in Player.bullets)
                {
                    dist = Vector2.Distance(boss.Position, b.Position);

                    if (dist < boss.Size + b.Size && !b.IsDead)
                    {
                        boss.Hit(b.Damage);

                        b.IsDead = true;
                    }
                }
                foreach (BlastWave b in Player.blasts)
                {
                    dist = Vector2.Distance(boss.Position, b.Position);

                    if (dist < b.Radius && b.HitBoss == false)
                    {
                        boss.Hit(50);
                        b.HitBoss = true;
                    }
                }
            }
            if (wBoss != null)
            {
                foreach (Bullet b in Player.bullets)
                {
                    dist = Vector2.Distance(wBoss.CollisionLocation, b.Position);

                    if (dist < wBoss.Size + b.Size && !b.IsDead)
                    {
                        wBoss.Hit(b.Damage);

                        b.IsDead = true;
                    }
                }
            }
        }

        public void Update(GameTime time)
        {
            if (ForceCounter.CurrentForce > 99)
            {
                StartSpawns = true;
            }
            if (StartSpawns)
            {
                if (stats.MaxEnemies > (enemies.Count - (EWorm.CurrentBodyPieces - EWorm.CurrentCount)))
                {
                    Spawn();
                }
                CheckHits();
                RemoveDead();
                currentTime += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;
                splitTime += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

                bool trigger = ManageTimers();

                UpdateEnemies(trigger, time);
                CollisionGrid.CollideAll();
            }
        }

        public void Draw(SpriteBatch s)
        {
            foreach (Enemy e in enemies)
            {
                e.Draw(s);
            }
            if (boss != null)
            {
                boss.Draw(s);
            }
            if (wBoss != null)
            {
                wBoss.Draw(s);
            }
        }

        public static void AddEnemy(Enemy e)
        {
            enemies.Add(e);
        }

        public static void NextLevel()
        {
            stats = LevelManager.Current.EnemyManage;
            KillEveryThing();
            currentTime = 0;
            counterA = 0;
            counterB = 0;
            counterC = 0;
            counterD = 0;
            ESpinner.CurrentCount = 0;
            EWorm.CurrentCount = 0;
            bossMode = false;
        }

        private static void UpdateEnemies(bool trigger, GameTime time)
        {
            float nextTime = (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            int nextLoop = enemies.Count;
            for (int i = 0; i < nextLoop; i++)
            {
                enemies[i].Update(trigger, nextTime);
            }
            if (boss != null)
            {
                boss.Update(trigger, nextTime);
            }
            if (wBoss != null)
            {
                wBoss.Update(trigger, nextTime);
            }
        }

        private bool ManageTimers()
        {
            bool trigger = false;

            if (splitTime > SPLIT_TIMER + stats.SpawnModifier)
            {
                trigger = true;
                splitTime = 0;
            }
            if (currentTime > LevelManager.LevelLength)
            {
                bossMode = true;
            }
            else
            {
                bossMode = false;
            }

            if (bossMode && boss == null)
            {
                KillEveryThing();
            }
            return trigger;
        }

        private void RemoveDead()
        {
            if (boss != null && boss.IsDead)
            {
                boss.Die();
                boss = null;
            }
            if (wBoss != null && wBoss.IsDead)
            {
                wBoss.Die();
                wBoss = null;
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsDead)
                {
                    enemies[i].Die();
                    enemies.RemoveAt(i);
                }
            }
        }

        private void Spawn()
        {
            Vector2 pos = new Vector2();
            Vector2 vel = new Vector2();
            Vector2 centre = new Vector2(Game1.ScreenX / 2, Game1.ScreenY / 2);

            if (!bossMode)
            {
                if (currentTime / (SPAWN_A + stats.SpawnModifier) > counterA && stats.DividersOn)
                {
                    counterA++;
                    int numCreate = Game1.rand.Next(1, 2 + stats.SpawnAmountModifier);

                    for (int i = 0; i < numCreate; i++)
                    {
                        SetSpawnParameters(ref pos, ref vel, centre);
                        EDivider e = new EDivider(divideTex, pos, vel, 0);
                        enemies.Add((Enemy)e);
                    }
                }
                if (currentTime / (SPAWN_B + stats.SpawnModifier) > counterB && stats.ChasersOn)
                {
                    counterB++;
                    int numCreate = Game1.rand.Next(1, 6 + stats.SpawnAmountModifier);

                    for (int i = 0; i < numCreate; i++)
                    {
                        SetSpawnParameters(ref pos, ref vel, centre);
                        EChaser e = new EChaser(chaseTex, pos, vel);
                        enemies.Add((Enemy)e);
                    }
                }
                if (ESpinner.CurrentCount < MAX_SPINNERS && currentTime / (SPAWN_C + stats.SpawnModifier) > counterC && stats.SpinnersOn)
                {
                    counterC++;
                    int numCreate = Game1.rand.Next(1, 3);

                    for (int i = 0; i < numCreate; i++)
                    {
                        SetSpawnParameters(ref pos, ref vel, centre);
                        //vel *= ESpinner.SPEED;
                        ESpinner e = new ESpinner(spinTex, pos, vel);
                        enemies.Add((Enemy)e);
                    }
                }
                if (EWorm.CurrentCount < stats.MaxWorms && currentTime / (SPAWN_D + stats.SpawnModifier) > counterD && stats.WormsOn)
                {
                    counterD++;
                    int loopControl;
                    if (LevelManager.Mode == GameMode.Worms)
                    {
                        loopControl = stats.MaxWorms - EWorm.CurrentCount;
                    }
                    else
                    {
                        loopControl = stats.SpawnAmountModifier / 2;
                    }
                    for (int i = 0; i < loopControl; i++)
                    {
                        SetSpawnParameters(ref pos, ref vel, centre);
                        EWorm w = new EWorm(pos, vel, WormTex, 0, stats.SpawnAmountModifier + 1, null);
                        enemies.Add((Enemy)w);
                    }
                }
            }
            else
            {
                if (LevelManager.Mode != GameMode.Worms)
                {
                    if (boss == null && enemies.Count == 0)
                    {
                        SetSpawnParameters(ref pos, ref vel, centre);
                        boss = new Boss(bossTex, pos, vel);
                    }
                }
                else
                {
                    if (wBoss == null && enemies.Count == 0)
                    {
                        SetSpawnParameters(ref pos, ref vel, centre);
                        wBoss = new WormBoss(pos, vel, WBossTex, 0, (int)LevelManager.Current.BossStats.SpawnRate + 4, null);
                    }
                }
            }
        }

        private static void KillEveryThing()
        {
            while (enemies.Count != 0)
            {
                enemies[enemies.Count - 1].PointsValue = 0;
                enemies[enemies.Count - 1].Die();
                enemies.RemoveAt(enemies.Count - 1);
            }
        }

        private void SetSpawnParameters(ref Vector2 pos, ref Vector2 vel, Vector2 centre)
        {
            double angle;
            angle = Game1.rand.NextDouble() * MathHelper.TwoPi;
            pos.X = (float)Math.Cos(angle) * screenRadius;
            pos.Y = (float)Math.Sin(angle) * screenRadius;
            pos += centre;
            vel = Enemy.RandUnitVector2();
        }
    }
}
