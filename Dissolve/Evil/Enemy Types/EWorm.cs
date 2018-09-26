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
    class EWorm : Enemy
    {
        protected int wormLength;
        public int ChainPosition { get; set; }
        public bool FirstInChain { get; set; }
        protected EWorm parent;
        public EWorm Child;

        protected bool growChild;

        protected float timer;
        protected const float GROWTH_TIME = 0.8f;
        protected const float ROTATE_SPEED = 0.1f;
        protected const float MOVE_SPEED = 0.8f;

        public static int CurrentCount { get; set; }

        public static int CurrentBodyPieces { get; set; }

        public EWorm(Vector2 pos, Vector2 vel, Texture2D tex, int chainPos, int length, EWorm parent)
            : base(tex, pos, vel)
        {
            ChainPosition = chainPos;

            growChild = true;
            damageTaken = 10;

            if (ChainPosition == 0)
            {
                FirstInChain = true;
                damageTaken = 5;
            }
            else if (ChainPosition == wormLength)
            {
                growChild = false;
            }

            this.parent = parent;
            wormLength = length;

            angle = (float)Game1.rand.NextDouble() * MathHelper.TwoPi;

            if (chainPos == 0)
            {
                CurrentCount++;
            }
            CurrentBodyPieces++;
            pointValue = 1;

            origin = Vector2.Zero;
        }

        protected override void Behave(bool trigger, float time)
        {
            timer += time;

            SpawnChild();

            if (FirstInChain)
            {
                FICMove(time);
                if (Child != null)
                {
                    Child.FollowParent();
                }
            }
            if (!IsDead)
            {
                HandleRelativeDeath();
            }
        }

        protected void HandleRelativeDeath()
        {
            if (Child != null && Child.IsDead)
            {
                Child = null;
                timer = 0;
            }
            if (!FirstInChain)
            {
                if (parent != null && parent.IsDead)
                {
                    SetFirstInChain();
                    parent = null;
                }
                else if (parent == null)
                {
                    SetFirstInChain();
                }
            }
        }

        public void FollowParent()
        {
            if (parent != null)
            {
                float dx = parent.Position.X - position.X;
                float dy = parent.Position.Y - position.Y;
                angle = (float)Math.Atan2(dy, dx);

                position = SetChildPos(parent.Position);

                velocity = Vector2.Zero;
            }
            if (Child != null)
            {
                Child.FollowParent();
            }
        }

        public void SetFirstInChain()
        {
            CurrentCount++;
            FirstInChain = true;
            parent = null;
            ChainPosition = 0;
            damageTaken = 5;
            Life = 100;
            timer = 0;
            if (wormLength > 1)
            {
                growChild = true;
            }
            if (Child != null)
            {
                Child.UpdateChain(ChainPosition + 1);
            }
        }

        public void UpdateChain(int chainPos)
        {
            ChainPosition = chainPos;
            timer = 0;

            if (ChainPosition == wormLength)
            {
                growChild = false;
            }
            else
            {
                growChild = true;
            }

            if (Child != null)
            {
                Child.UpdateChain(ChainPosition + 1);
            }
        }

        public void KillChain()
        {
            parent = null;
            Life = 0;
            IsDead = true;

            if (Child != null)
            {
                Child.KillChain();
            }
        }

        protected virtual void FICMove(float time)
        {
            angle += (float)((Game1.rand.NextDouble() - 0.5) * 2) * ROTATE_SPEED;

            velocity.X = (float)Math.Cos(angle) * MOVE_SPEED;
            velocity.Y = (float)Math.Sin(angle) * MOVE_SPEED;
        }

        protected virtual void SpawnChild()
        {
            if (growChild && timer > GROWTH_TIME && Child == null && ChainPosition < wormLength)
            {
                Vector2 pos = SetChildPos(position);
                Child = new EWorm(pos, Vector2.Zero, tex, ChainPosition + 1, wormLength, this);
                EnemyManager.AddEnemy((Enemy)Child);
            }
        }

        protected Vector2 SetChildPos(Vector2 original)
        {
            Vector2 pos = new Vector2();
            pos.X = original.X - (float)Math.Cos(angle) * Size * 2;
            pos.Y = original.Y - (float)Math.Sin(angle) * Size * 2;

            return pos;
        }

        public override void Die()
        {
            CurrentBodyPieces--;
            if (ChainPosition == 0)
            {
                CurrentCount--;
            }
            if (FirstInChain)
            {
                if (Child != null)
                {
                    Child.KillChain();
                    Child = null;
                }
            }
            base.Die();
        }

        protected override void ScreenWrap()
        {
            if (position.X - Size > Game1.ScreenX)
            {
                GetAngleToCentre();// angle -= MathHelper.Pi;
                position.X = Game1.ScreenX + Size;
            }
            if (position.X + Size < 0)
            {
                GetAngleToCentre();//angle -= MathHelper.Pi;
                position.X = - Size;
            }
            if (position.Y - Size > Game1.ScreenY)
            {
                GetAngleToCentre();//angle -= MathHelper.Pi;
                position.Y = Game1.ScreenY + Size;
            }
            if (position.Y + Size < 0)
            {
                GetAngleToCentre();//angle -= MathHelper.Pi;
                position.Y = -Size;
            }
        }

        protected void GetAngleToCentre()
        {
            Vector2 centre = new Vector2(Game1.ScreenX / 2, Game1.ScreenY / 2);

            float dx = position.X - centre.X;
            float dy = position.Y - centre.Y;
            angle = (float)Math.Atan2(dy, dx) - MathHelper.Pi;
        }
    }
}
