using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    enum EffectMode
    {
        Gravity, Move, Follow, RotateAround
    }
    class UIEffect
    {
        protected Vector2 position;
        protected Vector2 velocity;
        protected float rotation;
        protected float rotSpeed;
        protected float gravity;
        protected Enemy enemy;
        protected EffectMode mode;
        protected Texture2D tex;
        protected string msg;
        protected bool doOffset;
        protected float scale;
        protected Vector2 offset;
        protected Vector2 origin;
        protected Color drawColor;
        protected float currentTimer;

        protected  const float START_LIFE = 1f;
        public bool IsDead { get; set; }
        public float Life { get; set; }

        #region Constructors
        public UIEffect(Vector2 p, Vector2 v, Texture2D tex)
        {
            scale = 1;
            Life = START_LIFE;
            IsDead = false;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            mode = EffectMode.Move;
            position = p;
            velocity = v;
            this.tex = tex;
            doOffset = false;
            drawColor = Color.White;
        }
        public UIEffect(Vector2 p, Vector2 v, float gravity, Texture2D tex)
        {
            scale = 1;
            Life = START_LIFE;
            IsDead = false;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            mode = EffectMode.Gravity;
            position = p;
            velocity = v;
            this.gravity = gravity;
            this.tex = tex;
            doOffset = false;
            drawColor = Color.White;
        }
        public UIEffect(Vector2 p, Vector2 v, string message, Color col)
        {
            scale = 1;
            Life = START_LIFE;
            IsDead = false;
            origin = Vector2.Zero;
            mode = EffectMode.Move;
            position = p;
            velocity = v;
            msg = message;
            doOffset = false;
            drawColor = col;
        }
        public UIEffect(Vector2 p, Vector2 v, float gravity, string message, Color col, float scale)
        {
            this.scale = scale;
            Life = START_LIFE;
            IsDead = false;
            origin = Vector2.Zero;
            mode = EffectMode.Gravity;
            position = p;
            velocity = v;
            this.gravity = gravity;
            msg = message;
            doOffset = false;
            drawColor = col;
        }
        public UIEffect(Enemy e, Vector2 offset, bool rotate, float rotSpeed, Texture2D tex)
        {
            scale = 1;
            Life = START_LIFE;
            IsDead = false;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            enemy = e;
            if (offset.Equals(Vector2.Zero))
            {
                this.doOffset = false;
            }
            else this.doOffset = true;
            position = enemy.Position + offset;

            if (rotate)
            {
                mode = EffectMode.RotateAround;
            }
            else
            {
                mode = EffectMode.Follow;
            }
            this.rotSpeed = rotSpeed;
            this.tex = tex;
            drawColor = Color.White;
        }
        public UIEffect(Vector2 offset, bool rotate, float rotSpeed, Texture2D tex)
        {
            scale = 1;
            Life = START_LIFE;
            IsDead = false;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            if (offset.Equals(Vector2.Zero))
            {
                this.doOffset = false;
            }
            else this.doOffset = true;
            position = Player.Position + offset;

            if (rotate)
            {
                mode = EffectMode.RotateAround;
            }
            else
            {
                mode = EffectMode.Follow;
            }
            this.rotSpeed = rotSpeed;
            this.tex = tex;
            drawColor = Color.White;
        }
        #endregion

        public void Draw(SpriteBatch s)
        {
            if (!IsDead)
            {
                if (tex != null)
                {
                    s.Draw(tex, position, null, drawColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
                else
                {
                    s.DrawString(UILayer.Font, msg, position, drawColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
            }
        }

        public virtual void Update(float time)
        {
            currentTimer += time;

            if (currentTimer > Life)
            {
                IsDead = true;
            }
            drawColor = new Color((float)drawColor.R / 255, (float)drawColor.G / 255, 
                (float)drawColor.B / 255, 1 - (currentTimer / Life));

            if (!IsDead)
            {
                switch (mode)
                {
                    case EffectMode.Follow:
                        position = enemy.Position;
                        break;
                    case EffectMode.Gravity:
                        velocity.Y += gravity;
                        position += velocity;
                        break;
                    case EffectMode.RotateAround:
                        RotateAround();
                        break;
                    case EffectMode.Move:
                        position += velocity;
                        break;
                    default:
                        break;
                }
            }
        }

        protected void RotateAround()
        {
            rotation += rotSpeed;

            if(doOffset)
            {
                Vector2 vec = new Vector2();
                float cosine = (float)Math.Cos(rotSpeed);
                float sine = (float)Math.Sin(rotSpeed);

                vec.X = offset.X * cosine + offset.Y * sine;
                vec.Y = offset.Y * cosine - offset.X * sine;

                offset = vec;

                position = enemy.Position + offset;
            }
        }
    }
}
