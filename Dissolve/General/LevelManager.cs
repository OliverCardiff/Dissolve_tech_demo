using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dissolve
{
    enum GameMode
    {
        Normal, Worms, Moving, Chaos
    }
    static class LevelManager
    {
        public static List<Level> Levels;
        public static List<Level> WormLevels;
        public static Level MovingLevel;
        public static Level Chaos;

        public static GameMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                currentLevel = 0;
                switch (mode)
                {
                    case GameMode.Normal:
                        Current = Levels[0];
                        break;
                    case GameMode.Worms:
                        Current = WormLevels[0];
                        break;
                    case GameMode.Moving:
                        Current = MovingLevel;
                        break;
                    case GameMode.Chaos:
                        Current = Chaos;
                        break;
                }
            }
        }
        static GameMode mode;

        public static Level Current;
        public static float LevelLength = 30;
        private static int currentLevel = 0;

        public static GameState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;

                switch(state)
                {
                    case GameState.Running:
                        break;
                    case GameState.Paused:
                        break;
                    case GameState.MainMenu:
                        break;
                    case GameState.RestartMenu:
                        break;
                    default:
                        break;

                };
            }
        }
        static GameState state;

        public static void ResetGame()
        {
            State = GameState.Running;
            EnemyManager.ResetAll();
            Player.ResetAll();
            ShaderWrapper.ResetAll();
            ForceCounter.ResetAll();

            currentLevel = 0;

            NextLevel(false);
        }

        public static void Assemble()
        {
            mode = GameMode.Normal;
            Levels = BuildLevels();
            WormLevels = BuildWormLevels();
            Current = Levels[0];
            State = GameState.MainMenu;
        }

        public static void NextLevel(bool advance)
        {
            if (advance) currentLevel++;

            if (currentLevel < 19)
            {
                switch (Mode)
                {
                    case GameMode.Normal:
                        Current = Levels[currentLevel];
                        break;
                    case GameMode.Worms:
                        Current = WormLevels[currentLevel];
                        break;
                    case GameMode.Moving:
                        Current = Levels[Levels.Count - 1];
                        break;
                    case GameMode.Chaos:
                        Current = Levels[Levels.Count - 1];
                        break;
                }
                Player.NextLevel();
                UILayer.NextLevel();
                EnemyManager.NextLevel();
            }
            else
            {
                Player.StartFinalTimer();
            }
        }

        public static void RunGame()
        {
            ShaderWrapper.ResetAll();
            State = GameState.Running;
        }

        #region LevelConstruction
        private static List<Level> BuildLevels()
        {
            List<Level> lvls = new List<Level>();

            for (int i = 0; i < 20; i++)
            {
                Level l = new Level();

                l.LevelNo = i + 1;
                l.LevelOver = false;

                l.EnemyManage.MaxEnemies = 5 + (i * 2) - i / 2;
                l.EnemyManage.SpawnModifier = ((float)(-i - 1) / 20.0f) * 2;
                l.EnemyManage.SpawnAmountModifier = (int)-(l.EnemyManage.SpawnModifier * 5);
                l.EnemyManage.ChasersOn = true;
                l.EnemyManage.DividersOn = true;
                l.EnemyManage.MaxWorms = 1;

                if (i > 5) l.EnemyManage.WormsOn = true;
                else l.EnemyManage.WormsOn = false;

                if (i > 0) l.EnemyManage.SpinnersOn = true;
                else l.EnemyManage.SpinnersOn = false;

                if (i > 1) l.PlayerStats.BranchingOn = true;
                else l.PlayerStats.BranchingOn = false;

                if (i > 3) l.PlayerStats.GrowthBoostOn = true;
                else l.PlayerStats.GrowthBoostOn = false;

                if (i > 7) l.PlayerStats.SmartBombOn = true;
                else l.PlayerStats.SmartBombOn = false;

                if (i > 8) l.PlayerStats.Speed = 1.7f;
                else l.PlayerStats.Speed = 1f;

                l.PlayerStats.Acceleration = l.PlayerStats.Speed * 0.1764f;
                l.PlayerStats.MaxMultiplier = Math.Min(1 + (((float)i) / 9.0f) * 5, 10);

                l.PlayerStats.WormForm = false;
                l.PlayerStats.WormLength = 0;
                l.PlayerStats.MaxBranchLength = 16;
                l.PlayerStats.BranchAmp = 4;

                if (i > 8)
                {
                    l.PlayerStats.ColorChange = true;
                    l.PlayerStats.BulletDamage = 2;
                }
                else
                {
                    l.PlayerStats.ColorChange = false;
                    l.PlayerStats.BulletDamage = 1;
                }

                l.BossStats.Health = 100 + (i * 60);
                l.BossStats.Size = 0.6f + (((float)i + 1.0f) / 20.0f) * 3.0f;
                l.BossStats.SpawnRate = 5 + (1 - (((float)i + 1.0f) / 20.0f)) * 0.5f;
                l.BossStats.Speed = 1 + (((float)i + 1.0f) / 20.0f) * 3;
                l.BossStats.BranchRate = Math.Min(i + 1,10);

                if (i >=0) l.BossStats.Curve = true;
                else l.BossStats.Curve = false;

                if (i > 1) l.UIStats.PowerShown = true;
                else l.UIStats.PowerShown = false;

                if (i > 7) l.UIStats.SBTimerShown = true;
                else l.UIStats.SBTimerShown = false;

                string msg;

                switch (i)
                {
                    case 0:
                        msg = "Level 1 \nBirth";
                        l.PlayerStats.MaxPower = 0;
                        break;
                    case 1:
                        msg = "Level 2 \nA new foe approaches!";
                        l.PlayerStats.MaxPower = 0;
                        break;
                    case 2:
                        msg = "Level 3 \nBrancher power added! \n(Right Bumper)";
                        l.PlayerStats.MaxPower = 20;
                        break;
                    case 3:
                        msg = "Level 4 \nMax Power +20!";
                        l.PlayerStats.MaxPower = 40;
                        break;
                    case 4:
                        msg = "Level 5 \nGrowth Booster added! \n(Left Trigger)";
                        l.PlayerStats.MaxPower = 40;
                        break;
                    case 5:
                        msg = "Level 6 \nMax Power +20!";
                        l.PlayerStats.MaxPower = 60;
                        break;
                    case 6:
                        msg = "Level 7 \nA new foe approaches!";
                        l.PlayerStats.MaxPower = 60;
                        break;
                    case 7:
                        msg = "Level 8 \nMax Power +20!";
                        l.PlayerStats.MaxPower = 80;
                        break;
                    case 8:
                        msg = "Level 9 \nSmartBomb power added! \n(Left Bumper) ";
                        l.PlayerStats.MaxPower = 80;
                        break;
                    case 9:
                        msg = "Level 10 \n\"This is my final form!\"";
                        l.PlayerStats.MaxPower = 80;
                        break;
                    default :
                        msg = "Level " + (i + 1).ToString() + " \n\"For Great Justice!\"";
                        l.PlayerStats.MaxPower = 80;
                        break;
                }
                l.UIStats.Message = msg;

                lvls.Add(l);
            }
            lvls.Add(MakeLimboLevel(lvls[lvls.Count - 1]));
            return lvls;
        }

        private static List<Level> BuildWormLevels()
        {
            List<Level> lvls = new List<Level>();

            for (int i = 0; i < 20; i++)
            {
                Level l = new Level();

                l.LevelNo = i + 1;
                l.LevelOver = false;

                l.EnemyManage.MaxEnemies = 5 + (i * 2) - i / 2;
                l.EnemyManage.SpawnModifier = ((float)(-i - 1) / 20.0f) * 2;
                l.EnemyManage.SpawnAmountModifier = (int)-(l.EnemyManage.SpawnModifier * 5) + 2;
                l.EnemyManage.ChasersOn = false;
                l.EnemyManage.DividersOn = false;
                l.EnemyManage.WormsOn = true;
                l.EnemyManage.SpinnersOn = false;
                l.EnemyManage.MaxWorms = l.EnemyManage.MaxEnemies;

                if (i > 1) l.PlayerStats.BranchingOn = true;
                else l.PlayerStats.BranchingOn = false;


                l.PlayerStats.GrowthBoostOn = true;

                if (i > 3)
                {
                    l.PlayerStats.SmartBombOn = true;
                    l.UIStats.SBTimerShown = true;
                }
                else
                {
                    l.PlayerStats.SmartBombOn = false;
                    l.UIStats.SBTimerShown = false;
                }

                if (i > 8) l.PlayerStats.Speed = 1.7f;
                else l.PlayerStats.Speed = 1f;

                l.PlayerStats.Acceleration = l.PlayerStats.Speed * 0.1764f;
                l.PlayerStats.MaxMultiplier = Math.Min(1 + (((float)i) / 9.0f) * 5, 10);
                l.PlayerStats.MaxPower = 5 * (i + 1);
                l.PlayerStats.WormLength = 6;
                l.PlayerStats.BranchAmp = i + 10;
                l.PlayerStats.MaxBranchLength = 200;

                if (i > 8)
                {
                    l.PlayerStats.ColorChange = true;
                    l.PlayerStats.BulletDamage = 2;
                }
                else
                {
                    l.PlayerStats.ColorChange = false;
                    l.PlayerStats.BulletDamage = 1;
                }

                l.BossStats.Health = 200 + (i * 50);
                l.BossStats.Size = 1.2f + (((float)i + 1.0f) / 20.0f) * 3.0f;
                l.BossStats.SpawnRate = 5 + (1 - (((float)i + 1.0f) / 20.0f)) * 0.5f;
                l.BossStats.Speed = 1 + (((float)i + 1.0f) / 20.0f) * 3;
                l.BossStats.BranchRate = Math.Min(i + 1, 10);

                l.BossStats.Curve = true;
                l.UIStats.PowerShown = true;
                

                string msg;

                switch (i)
                {
                    case 0:
                        msg = "Level 1 \nYou find yourself in\na strange part of\nthe soup...\n\nGrowth Boost enabled";
                        break;
                    case 1:
                        msg = "Level 2 \nShoot the head -\nKill the body!!";
                        break;
                    case 2:
                        msg = "Level 3 \nLife Branch added\nYour powers feel...\n...strange...";
                        break;
                    case 3:
                        msg = "Level 4 \nYou gradually increase\nin power!";
                        break;
                    case 4:
                        msg = "Level 5 \nSmartBomb enabled...\n...sort of...";
                        break;
                    case 5:
                        msg = "Level 6 \nThe worm hordes\nclose in!";
                        break;
                    case 6:
                        msg = "Level 7 \nYour body starts\nto warp..";
                        break;
                    case 7:
                        msg = "Level 8 \nOH GOD...";
                        l.PlayerStats.WormForm = true;
                        break;
                    case 8:
                        msg = "Level 9 \nMust be something\nint the water";
                        l.PlayerStats.WormForm = true;
                        break;
                    case 9:
                        msg = "Level 10 \n\"This is my final\n(worm) form!\"";
                        l.PlayerStats.WormForm = true;
                        break;
                    default:
                        msg = "Level " + (i + 1).ToString() + " \nOnward!";
                        l.PlayerStats.WormForm = true;
                        break;
                }
                l.UIStats.Message = msg;

                lvls.Add(l);
            }
            lvls.Add(MakeLimboLevel(lvls[lvls.Count - 1]));

            return lvls;
        }

        private static Level MakeLimboLevel(Level lvl)
        {
            Level l = new Level();
            l.BossStats = lvl.BossStats;
            l.EnemyManage.WormsOn = false;
            l.EnemyManage.SpinnersOn = false;
            l.EnemyManage.SpawnModifier = 10000;
            l.EnemyManage.SpawnAmountModifier = 0;
            l.EnemyManage.MaxEnemies = 0;

            l.LevelNo = 20;
            l.LevelOver = true;
            l.PlayerStats = lvl.PlayerStats;

            l.UIStats = lvl.UIStats;
            l.UIStats.Message = "Finish! Well Done!";

            return l;
        }
        #endregion
    }
}
