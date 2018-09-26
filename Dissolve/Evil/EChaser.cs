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
    class EChaser : Enemy
    {
        public static float SPEED = 1.5f;
        const int DEATH_SPAWNS = 10;

        public EChaser(Texture2D tex, Vector2 position, Vector2 velocity)
            : base(tex, position, velocity)
        {
            behaviour = Behaviour.Chase;
            damageTaken = 34;
            pointValue = 3;
        }

        protected override void Behave(bool trigger)
        {
            Vector2 diff = position - Player.Position;
            if (diff.Length() > Size)
            {
                diff.Normalize();

                velocity = -diff * SPEED;

                angle = (float)Math.Atan2(diff.Y, diff.X);
            }
            else
            {
                Life = 0;
                IsDead = true;
                Explode();
            }
        }

        private void Explode()
        {
            float angle;
            Vector2 v = new Vector2();

            for (int i = 0; i < DEATH_SPAWNS; i++)
            {
                angle = MathHelper.TwoPi * ((float)(i + 1) / 10.0f);
                v.X = (float)Math.Cos(angle) * Player.BULLET_SPEED;
                v.Y = (float)Math.Sin(angle) * Player.BULLET_SPEED;
                TimedBullet b = new TimedBullet(position, v, EnemyManager.BulletTex, 0.7f);

                Player.AddGrower(b);

            }
        }

    }
}
