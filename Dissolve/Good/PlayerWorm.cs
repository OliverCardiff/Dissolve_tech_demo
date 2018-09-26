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
    /// A modified enemy worm type used to attach to the player for
    /// the visual effect of a worm only
    /// </summary>
    class PlayerWorm : EWorm
    {
        public PlayerWorm(Texture2D tex, int chainPos, int length, EWorm e)
            : base(Player.Position, Vector2.Zero, tex, chainPos, length, e)
        {
        }
        //Modified to work its way down the worms body updating each segment
        //unlike the normal way which update individually from a list
        protected override void Behave(bool trigger, float time)
        {
            base.Update(trigger, time);
            if (Child != null)
            {
                Child.Update(trigger, time);
            }
        }

        //Changes the action of the First-in-chain to follow the player
        protected override void FICMove(float time)
        {
            float dx = Player.Position.X - position.X;
            float dy = Player.Position.Y - position.Y;
            angle = (float)Math.Atan2(dy, dx);

            position = SetChildPos(Player.Position);

            velocity = Vector2.Zero;
        }

        /// <summary>
        /// Works its way down the worms body changing each segments texture to the one given
        /// </summary>
        /// <param name="tex">The texture to change it to</param>
        public void UpdateChainTexture(Texture2D tex)
        {
            Texture = tex;
            if (Child != null)
            {
                UpdateChildTex(Child, tex);
            }
        }

        //Called by UpdateChain texture, updates the texture of the child recursively
        private void UpdateChildTex(EWorm e, Texture2D tex)
        {
            e.Texture = tex;
            if (e.Child != null)
            {
                UpdateChildTex(e.Child,tex);
            }
        }

        //Handles the initial growth of the player's worm
        protected override void SpawnChild()
        {
            if (growChild && timer > GROWTH_TIME && Child == null && ChainPosition < wormLength)
            {
                Vector2 pos = SetChildPos(position);
                Child = new PlayerWorm(tex, ChainPosition + 1, wormLength, (EWorm)this);
            }
        }

        /// <summary>
        /// Recursively draws all the body segments of the PlayerWorm
        /// </summary>
        /// <param name="s"></param>
        public override void Draw(SpriteBatch s)
        {
            s.Draw(tex, position, null, Color.White, angle, origin, scale, SpriteEffects.None, 0);

            if (Child != null)
            {
                Child.Draw(s);
            }
        }

        /// <summary>
        /// Overrided update, does not commit to collision grid
        /// </summary>
        /// <param name="trigger">General event trigger, unused in PlayerWorm</param>
        /// <param name="time">The amount of time which has passed since the last frame, in seconds</param>
        public override void Update(bool trigger, float time)
        {
            ScreenWrap();
            IsDead = CheckDeath();
            Behave(trigger, time);
            position += velocity;
        }
    }
}
