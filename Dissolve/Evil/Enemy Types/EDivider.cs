using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dissolve
{
    class EDivider : Enemy
    {
        private int divideNo;
        const int MAX_DIVIDES = 2;
        float originalSize;
        public EDivider(Texture2D tex, Vector2 position, Vector2 velocity, int divNo)
            : base(tex, position, velocity)
        {
            behaviour = Behaviour.Divide;
            damageTaken = 50;
            divideNo = divNo;
            Vector2 v = velocity;
            v.Normalize();
            pointValue = 1;
            angle = (float)Math.Atan2(v.Y, v.X);
            originalSize = Size;
        }

        protected override void Behave(bool trigger, float time)
        {
            if (divideNo < MAX_DIVIDES)
            {
                scale += 0.01f;
                Size = originalSize * scale;
            }
            if (trigger && divideNo < MAX_DIVIDES)
            {
                Vector2 nextV;
                for (int i = 0; i < 1; i++)
                {
                    nextV = RandUnitVector2();
                    EDivider e = new EDivider(tex, position, velocity + nextV, divideNo + 1);
                    EnemyManager.AddEnemy((Enemy)e);
                }
               
                nextV = RandUnitVector2();

                velocity += nextV;

                angle = (float)Math.Atan2(velocity.Y, velocity.X);
                scale = 1;
                Size = originalSize;
                divideNo++;
            }
        }
    }
}
