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
    class BlastWave
    {
        float scale;
        float maxScale;
        float lifeTime;
        float currentLife;
        float alpha;
        float rad;

        public bool HitBoss { get; set; }
        public float Radius
        {
            get
            {
                return rad * scale;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        protected Vector2 position;
        Vector2 origin;

        Texture2D tex;

        protected Color force;

        public bool IsDead { get; set; }
        public bool HalfDead { get; set; }

        const float MAX_ALPHA = 0.5f;

        public BlastWave(float scale, float life, Vector2 position, Texture2D tex)
        {
            this.tex = tex;
            this.position = position;
            lifeTime = life;
            maxScale = scale;
            this.scale = 0.1f;
            this.currentLife = 0;
            alpha = MAX_ALPHA;
            rad = tex.Width / 2;
            HitBoss = false;

            force = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            origin = new Vector2(tex.Width / 2, tex.Height / 2);
        }

        public virtual void Update(float time)
        {
            currentLife += time;
            if (currentLife > lifeTime)
            {
                IsDead = true;
            }
            if (currentLife > lifeTime/2)
            {
                HalfDead = true;
            }

            alpha = MAX_ALPHA - (currentLife / lifeTime);

            scale = (1-(alpha / MAX_ALPHA)) * maxScale;
            
            force = new Color(1.0f, 1.0f, 1.0f, alpha);

        }

        public void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, null, force, 0, origin, scale, SpriteEffects.None, 0);
        }
    }
}
