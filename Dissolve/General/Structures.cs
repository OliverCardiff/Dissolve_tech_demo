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
    struct ManagerStats
    {
        public float SpawnModifier;
        public int MaxEnemies;
        public int MaxWorms;
        public bool SpinnersOn;
        public bool WormsOn;
        public bool ChasersOn;
        public bool DividersOn;
        public int SpawnAmountModifier;
    }
    struct PlayerFlags
    {
        public bool BranchingOn;
        public bool SmartBombOn;
        public bool GrowthBoostOn;
        public bool WormForm;
        public int WormLength;
        public float Speed;
        public float Acceleration;
        public bool ColorChange;
        public int MaxPower;
        public float BulletDamage;
        public float MaxMultiplier;


        public int MaxBranchLength;
        public int BranchAmp;
    }
    struct UIStats
    {
        public bool PowerShown;
        public bool SBTimerShown;
        public string Message;
    }
    struct BossStats
    {
        public float Size;
        public float Speed;

        public float BranchRate;
        public float SpawnRate;
        public bool Curve;

        public float Health;
    }
    struct Level
    {
        public ManagerStats EnemyManage;
        public PlayerFlags PlayerStats;
        public UIStats UIStats;
        public BossStats BossStats;

        public int LevelNo;
        public bool LevelOver;

    }
}