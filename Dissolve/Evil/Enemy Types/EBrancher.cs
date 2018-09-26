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
    class EBrancher : Enemy 
    {
        const float BRANCH_TIME = 1;

        float length;
        float currentLife;

        int repetitions;
        int positionInChain;
        int numBranches;

        bool hasBranched;

        public EBrancher(Texture2D tex, Vector2 position, float rot, int chainPos, int repeats)
            : base(tex, position, Vector2.Zero)
        {
            numBranches = Game1.rand.Next(1, 4);
            if (numBranches > 1) numBranches = 1;
            else if (numBranches == 1) numBranches = 2;

            this.length = tex.Width;
            repetitions = repeats;

            positionInChain = chainPos;

            angle = rot;

            currentLife = 0;
            damageTaken = 100;
            pointValue = 1;
            hasBranched = false;
        }

        protected override void Behave(bool trigger, float time)
        {
            currentLife += time;
            if (!hasBranched && currentLife > BRANCH_TIME && positionInChain < repetitions)
            {
                Vector2 nextPos = new Vector2();
                float rot;

                for (int i = 0; i < numBranches; i++)
                {
                    nextPos.X = position.X + (float)Math.Cos(angle) * length;
                    nextPos.Y = position.Y + (float)Math.Sin(angle) * length;
                    rot = angle + (float)((Game1.rand.NextDouble() - 0.5f) * 2) * (MathHelper.PiOver4 / 2);
                    EBrancher b = new EBrancher(tex,nextPos, rot, positionInChain + 1, repetitions);
                    EnemyManager.AddEnemy((Enemy)b);
                }

                hasBranched = true;
            }
        }

        public override void Die()
        {
            Player.Points += pointValue;
            if (LevelManager.Current.UIStats.PowerShown)
            {
                if (pointValue != 0)
                {
                    UIEffect e = new UIEffect(position, RandUnitVector2(), 0.1f, "+" + pointValue.ToString(), Color.Turquoise,0.9f);
                    UILayer.AddUIEffect(e);
                }
            }
            NormalMap.AddBlast2(position, Size / 20, Size / 8);
        }
    }
}
