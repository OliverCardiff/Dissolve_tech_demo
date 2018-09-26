using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dissolve
{
    class EDivider : Enemy
    {
        private int divideNo;
        const int MAX_DIVIDES = 2;

        public EDivider(Texture2D tex, Vector2 position, Vector2 velocity, int divNo)
            : base(tex, position, velocity)
        {
            behaviour = Behaviour.Divide;
            damageTaken = 20;
            divideNo = divNo;
            Vector2 v = velocity;
            v.Normalize();
            pointValue = 2;
            angle = (float)Math.Atan2(v.Y, v.X);
        }

        protected override void Behave(bool trigger)
        {
            if (trigger && divideNo < MAX_DIVIDES)
            {
                Vector2 nextV = new Vector2();

                for (int i = 0; i < 2; i++)
                {
                    nextV = RandUnitVector2();
                    EDivider e = new EDivider(tex, position, velocity + nextV, divideNo + 1);
                    EnemyManager.AddEnemy((Enemy)e);
                }
                nextV = RandUnitVector2();

                velocity += nextV;

                angle = (float)Math.Atan2(velocity.Y, velocity.X);
                divideNo++;
            }
        }
    }
}
