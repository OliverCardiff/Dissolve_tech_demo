using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dissolve
{
    class WormBoss : EWorm
    {
        BossStats stats;
        public Vector2 CollisionLocation;
        const int DEATH_GROWTH = 40;

        public WormBoss(Vector2 pos, Vector2 vel, Texture2D tex, int chainPos, int length, WormBoss Parent)
            : base(pos, vel, tex, chainPos, length, (EWorm)Parent)
        {
            stats = LevelManager.Current.BossStats;
            if (chainPos == 0)
            {
                CollisionLocation = new Vector2();
            }
            scale = stats.Size;
            Size *= scale;
            Life = stats.Health;
            damageTaken = 1;
            if (chainPos == 0)
            {
                pointValue = 1000;
            }
            else
            {
                damageTaken = 0;
            }
        }

        protected override void SpawnChild()
        {
            if (growChild && timer > GROWTH_TIME && Child == null && ChainPosition < wormLength)
            {
                Vector2 pos = SetChildPos(position);
                Child = (EWorm)new WormBoss(pos, Vector2.Zero, tex, ChainPosition + 1, wormLength, this);
                EnemyManager.AddEnemy((Enemy)Child);
            }
        }

        public override void Update(bool trigger, float time)
        {

            if (!FirstInChain)
            {
                CollisionGrid.Commit((Enemy)this);
            }
            else
            {
                CollisionLocation.X = position.X + (float)Math.Cos(angle) * Size;
                CollisionLocation.Y = position.Y + (float)Math.Sin(angle) * Size;
            }
            ScreenWrap();
            IsDead = CheckDeath();
            Behave(trigger, time);
            position += velocity;

            if (Child != null)
            {
                Child.Update(trigger, time);
            }
        }

        public override void Die()
        {
            Vector2 nV;

            if (Child != null)
            {
                Child.KillChain();
            }
            if (FirstInChain)
            {
                for (int i = 0; i < DEATH_GROWTH; i++)
                {
                    nV = RandUnitVector2();
                    Player.AddGrower(position, nV * Player.BulletSpeed, (float)Game1.rand.NextDouble() * 0.5f + 1f);
                }

                LevelManager.NextLevel(true);
                NormalMap.AddBlast2(position, 20, 5);
            }
            base.Die();
        }

        public override void Draw(SpriteBatch s)
        {
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].End();
            s.End();

            ShaderWrapper.DissolveEffect.End();

            ShaderWrapper.DissolveEffect.Parameters["xLerp"].SetValue(true);
            ShaderWrapper.DissolveEffect.Parameters["xLerpFactor"].SetValue(1 - Life / stats.Health);
            ShaderWrapper.DissolveEffect.Parameters["xLerpColor"].SetValue(Boss.DeathColor);
            ShaderWrapper.DissolveEffect.Begin();
            s.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].Begin();

            base.Draw(s);

            if (Child != null)
            {
                Child.Draw(s);
            }

            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].End();

            s.End();

            ShaderWrapper.DissolveEffect.End();

            ShaderWrapper.DissolveEffect.Parameters["xLerp"].SetValue(false);
            ShaderWrapper.DissolveEffect.Begin();
            s.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].Begin();
        }

        public override void Hit(float damage)
        {
            if (!FirstInChain)
            {
                NormalBullet b = new NormalBullet(stats.Size / 2, 1, position, RandUnitVector2() * Player.BULLET_SPEED, NormalMap.blastWave2);
                NormalMap.AddGenericBlast((BlastWave)b);
            }
            else
            {
                float dx = position.X - Player.Position.X;
                float dy = position.X - Player.Position.Y;

                float a = (float)Math.Atan2(dy, dx);

                float adjustment = MathHelper.Pi - angle;

                a += adjustment;
                a = WrapAngle(a);

                float diff = (angle + adjustment) - a;

                if (diff < 0)
                {
                    angle += ROTATE_SPEED;
                }
                else
                {
                    angle -= ROTATE_SPEED;
                }
            }
            base.Hit(damage);
        }

        protected override void FICMove(float time)
        {
            base.FICMove(time);
            angle = WrapAngle(angle);

        }

        private float WrapAngle(float angle)
        {
            if (angle > MathHelper.TwoPi) angle -= MathHelper.TwoPi;
            if (angle < 0) angle += MathHelper.TwoPi;

            return angle;
        }
    }
}
