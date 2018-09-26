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
    /// A version of the branching bullet which is both self-sustaining and screenwrapping
    /// </summary>
    class SBBrancher : Brancher
    {
        public SBBrancher(Vector2 position, Texture2D tex, float life, float rotation, int chainPos, int length)
            : base(position, Vector2.Zero, tex, life, rotation, chainPos, length)
        {
        }

        /// <summary>
        /// Increments the current smartbomb countdown, and increments the current chains maximum repetitions
        /// </summary>
        /// <param name="value">The amount by which Repetitions will be incremented</param>
        public override void Sustain(int value)
        {
            if (repetitions < LevelManager.Current.PlayerStats.MaxBranchLength)
            {
                repetitions += value;
            }

            Player.IncrementNextSBTimer();
        }
        //Overrides update to remove CheckOffscreen and add screenwrap
        public override void Update(GameTime time)
        {
            currentLife += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            if (currentLife > lifeTime)
            {
                IsDead = true;
            }

            position += velocity;

            if (LevelManager.Mode == GameMode.Worms)
            {
                ScreenWrap();
            }

            SpawnNext();
        }

        //Overrides SpawnNext to create a continuing single stranded SBBrancher segment 
        protected override void SpawnNext()
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
                    SBBrancher b = new SBBrancher(nextPos, tex, lifeTime, rot, positionInChain + 1, repetitions);
                    Player.AddGrower((Brancher)b);
                }

                hasBranched = true;
            }
        }
    }
}
