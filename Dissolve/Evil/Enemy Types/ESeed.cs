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
    class ESeed : Enemy
    {
        public ESeed(Texture2D tex, Vector2 pos, Vector2 vel)
            : base(tex, pos, vel)
        {
            pointValue = 2;
            damageTaken = 10;
        }

        protected override void Behave(bool trigger, float time)
        {
            if (CheckOffScreen())
            {
                Life = 0;
                IsDead = true;
                pointValue = 0;
                float angle = (float)Math.Atan2(velocity.Y, velocity.X) - MathHelper.Pi;
                EBrancher b = new EBrancher(EnemyManager.EBranchTex, position, angle, 0, 5);
                EnemyManager.AddEnemy((Enemy)b);
            }
        }

        private bool CheckOffScreen()
        {
            if (position.X  < 0 || position.X  > Game1.ScreenX ||
                position.Y < 0 || position.Y  > Game1.ScreenY)
            {
                return true;
            }
            return false;
        }
    }
}
