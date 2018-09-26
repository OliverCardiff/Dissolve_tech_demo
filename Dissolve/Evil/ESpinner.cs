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
    class ESpinner : Enemy
    {
        public static float SPEED = 0.25f;

        public static int CurrentCount { get; set; }

        public ESpinner(Texture2D tex, Vector2 position, Vector2 velocity)
            : base(tex, position, velocity)
        {
            behaviour = Behaviour.Spin;
            damageTaken = 1f;
            CurrentCount++;
            pointValue = 10;
        }

        protected override void Behave(bool trigger)
        {
            if (trigger)
            {
                velocity = RandUnitVector2();

                velocity *= SPEED;
            }

            angle += SPEED/10.0f;
        }

        public override void Hit()
        {
            Vector2 nV = new Vector2();
            nV = RandUnitVector2();

            Player.AddGrower(position, nV * Player.BulletSpeed, (float)Game1.rand.NextDouble() * 0.5f + 0.2f);

            base.Hit();
        }

        public override void Die()
        {
            CurrentCount -= 1;
            base.Die();
        }
    }
}
