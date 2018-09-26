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
    class TimedBullet : Bullet
    {
        protected float lifeTime;
        protected float currentLife;

        public TimedBullet(Vector2 pos, Vector2 vel, Texture2D tex, float life)
            : base(pos, vel, tex)
        {
            lifeTime = life;
            currentLife = 0;
        }

        public override void Update(GameTime time)
        {
            currentLife += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            if (currentLife > lifeTime)
            {
                IsDead = true;
            }
            base.Update(time);
        }
    }
}
