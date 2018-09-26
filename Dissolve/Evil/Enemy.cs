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
    enum Behaviour
    {
        None, Chase, Divide, Spin
    }
    class Enemy
    {
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        public int PointsValue
        {
            get
            {
                return pointValue;
            }
            set
            {
                pointValue = value;
            }
        }
        public Texture2D Texture
        {
            get
            {
                return tex;
            }
            set
            {
                tex = value;
            }
        }

        protected Texture2D tex;
        protected Vector2 position;
        protected Vector2 velocity;
        protected Behaviour behaviour;

        protected float damageTaken;
        protected Vector2 origin;
        protected float angle;
        protected int pointValue;
        protected float scale;

        public bool IsDead { get; set; }
        public float Life { get; set; }
        public float Size { get; set; }

        public Enemy(Texture2D t, Vector2 p, Vector2 v)
        {
            tex = t;
            position = p;
            velocity = v;
            Life = 100;
            Size = Math.Max(tex.Height / 2, tex.Width / 2);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            angle = 0;
            scale = 1;
        }

        public virtual void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, null, Color.White, angle, origin, scale, SpriteEffects.None, 0);
        }

        public virtual void Update(bool trigger, float time)
        {
            CollisionGrid.Commit((Enemy)this);
            ScreenWrap();
            IsDead = CheckDeath();
            Behave(trigger, time);
            position += velocity;
        }

        protected bool CheckDeath()
        {
            if (Life <= 0)
            {
                return true;
            }

            return false;
        }

        protected virtual void Behave(bool trigger, float time)
        {
        }

        public virtual void Hit(float damage)
        {
            Life -= damageTaken * damage * LevelManager.Current.PlayerStats.BulletDamage;
        }

        protected virtual void ScreenWrap()
        {
            if (position.X - Size > Game1.ScreenX)
            {
                position.X = -Size;
            }
            if (position.X + Size < 0)
            {
                position.X = (float)Game1.ScreenX + Size;
            }
            if (position.Y - Size > Game1.ScreenY)
            {
                position.Y = -Size;
            }
            if (position.Y + Size < 0)
            {
                position.Y = (float)Game1.ScreenY + Size;
            }
        }

        public static Vector2 RandUnitVector2()
        {
            Vector2 retVal = new Vector2();

            retVal.X = (float)(Game1.rand.NextDouble() - 0.5f) * 2;
            retVal.Y = (float)(Game1.rand.NextDouble() - 0.5f) * 2;

            retVal.Normalize();

            return retVal;
        }

        public virtual void Die()
        {
            Player.Points += pointValue;
            float pointsToAdd = (float)pointValue * Player.Multiplier;
            Player.Score += pointsToAdd;

            float txtScale = 0.9f;
            float grav = 0.1f;
            if(pointsToAdd > 100)
            {
                txtScale = 1.5f;
                grav = 0.04f;
            }
            if (LevelManager.Current.UIStats.PowerShown)
            {
                if (pointValue != 0)
                {
                    UIEffect e = new UIEffect(position, RandUnitVector2(), 0.1f, "+" + pointValue.ToString(), Color.Turquoise,0.9f);
                    UILayer.AddUIEffect(e);
                }
            }
            if (pointValue != 0)
            {
                VertexBag v = new VertexBag(UIPositions.Score + UIPositions.ScoreOffset, Color.GreenYellow, 
                    Color.LawnGreen, 1, Math.Min((int)(50 * pointsToAdd), 1000), 10);
                UILayer.AddParticleEffect(v);
                UIEffect b = new UIEffect(position, RandUnitVector2(),
                    grav, "+" + Math.Round(pointsToAdd, 1).ToString(),
                    Color.Yellow, txtScale);

                UILayer.AddUIEffect(b);
            }
            NormalMap.AddBlast2(position, Size / 10, Size / 8);
        }
    }
}
