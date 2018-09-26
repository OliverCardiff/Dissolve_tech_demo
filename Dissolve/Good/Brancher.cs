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
    /// <summary>
    /// A version of timed bullet which is stationary and spawns more of inself in 
    /// a branch-like way
    /// </summary>
    class Brancher : TimedBullet
    {
        //Control variable used to check if the segment has branched or not
        protected bool hasBranched;
        //the timer which counts down to 'branch-time'
        protected float branchTime;
        protected float rotation;
        protected float length;

        //The number of branching sessions the current life branch will undergo
        protected int repetitions;

        //the segments position in the entire branch, used to check whether to keep branching
        protected int positionInChain;
        //the number of branches to make in SpawnNext()
        protected int numBranches;

        public Brancher(Vector2 pos, Vector2 vel, Texture2D tex, float life, float rot, int chainPos, int length)
            : base(pos, vel, tex, life)
        {
            positionInChain = chainPos;
            if (LevelManager.Mode != GameMode.Worms)
            {
                numBranches = Game1.rand.Next(1, 4);
            }
            else
            {
                numBranches = 2;
            }

            if (numBranches > 1 || chainPos > 13) numBranches = 1;
            else if (numBranches == 1) numBranches = 2;

            hasBranched = false;

            branchTime = life / 20;

            velocity = Vector2.Zero;
            this.length = tex.Width;
            rotation = rot;

            repetitions = length;

            Damage = 5;
        }

        //Overrides draw to uses more complex spritebatch call
        public override void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, null, Color.White, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        //Overrides update to include the SpawnNext method
        public override void Update(GameTime time)
        {
            currentLife += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            if (currentLife > lifeTime)
            {
                IsDead = true;
            }

            position += velocity;

            if (!IsDead)
            {
                IsDead = CheckOffScreen();
            }

            SpawnNext();
        }

        //Creates the next branches in the chain, if the conditions are right
        protected virtual void SpawnNext()
        {
            if (!hasBranched && currentLife > branchTime && positionInChain < repetitions)
            {
                Vector2 nextPos = new Vector2();
                float rot;

                for (int i = 0; i < numBranches; i++)
                {
                    nextPos.X = position.X + (float)Math.Cos(rotation) * length;
                    nextPos.Y = position.Y + (float)Math.Sin(rotation) * length;
                    rot = rotation + (float)((Game1.rand.NextDouble() - 0.5f) * 2) * (MathHelper.PiOver4 / 2);
                    Brancher b = new Brancher(nextPos, Vector2.Zero, tex, lifeTime, rot, positionInChain + 1, repetitions);
                    Player.AddGrower(b);
                }

                hasBranched = true;
            }
        }
    }
}
