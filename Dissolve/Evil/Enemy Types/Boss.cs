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
    class Boss : Enemy
    {
        float rotationalVelocity;

        float counter;
        public static Vector4 DeathColor = new Vector4(1.0f, 0, 0, 1.0f);

        const int DEATH_GROWTH = 40;
        const float MAX_ROT_VELOCITY = 0.05f;
        const float CURVE_RATE = 0.01f;
        BossStats stats;

        public Boss(Texture2D tex, Vector2 pos, Vector2 vel)
            : base(tex, pos, vel)
        {
            scale = LevelManager.Current.BossStats.Size;
            Size *= scale;
            Life = LevelManager.Current.BossStats.Health;

            stats = LevelManager.Current.BossStats;
            pointValue = 1000;

            damageTaken = 1;
            rotationalVelocity = 0;
        }

        protected override void Behave(bool trigger, float time)
        {
            if (trigger)
            {
                velocity = RandUnitVector2();
                velocity *= stats.Speed;
                angle = (float)Math.Atan2(velocity.Y, velocity.X);

                rotationalVelocity = 0;
                if (LevelManager.Current.EnemyManage.WormsOn)
                {
                    for (int i = 0; i < stats.BranchRate/2 - 1; i++)
                    {
                        EWorm e = new EWorm(Position + RandUnitVector2() * 30, RandUnitVector2(),
                            EnemyManager.WormTex, 0, (int)stats.BranchRate - 2, null);
                        EnemyManager.AddEnemy((Enemy)e);
                    }
                }
                else
                {
                    for (int i = 0; i < stats.BranchRate; i++)
                    {
                        EChaser c = new EChaser(EnemyManager.chaseTex, position + RandUnitVector2() * 100, Vector2.Zero);
                        EnemyManager.AddEnemy((Enemy)c);
                    }
                }
            }
            else if (stats.Curve)
            {
                Curve();
            }
            counter += time;

            if (counter > stats.SpawnRate)
            {
                counter = 0;

                for (int i = 0; i < stats.BranchRate; i++)
                {
                    ESeed e = new ESeed(EnemyManager.seedTex, position, RandUnitVector2() * 3);
                    EnemyManager.AddEnemy((Enemy)e);
                }
            }
        }
        private void Curve()
        {
            rotationalVelocity += (float)(Game1.rand.NextDouble() - 0.5f) * 2 * CURVE_RATE;

            rotationalVelocity = MathHelper.Clamp(rotationalVelocity, -MAX_ROT_VELOCITY, MAX_ROT_VELOCITY);
            angle += rotationalVelocity;

            velocity.X = (float)Math.Cos(angle) * stats.Speed;
            velocity.Y = (float)Math.Sin(angle) * stats.Speed;
        }
        public override void Die()
        {
            Vector2 nV;

            for (int i = 0; i < DEATH_GROWTH; i++)
            {
                nV = RandUnitVector2();
                Player.AddGrower(position, nV * Player.BulletSpeed, (float)Game1.rand.NextDouble() * 0.5f + 1f);
            }

            LevelManager.NextLevel(true);
            NormalMap.AddBlast2(position, 20, 5);
            base.Die();
        }

        public override void Draw(SpriteBatch s)
        {
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].End();
            s.End();

            ShaderWrapper.DissolveEffect.End();

            ShaderWrapper.DissolveEffect.Parameters["xLerp"].SetValue(true);
            ShaderWrapper.DissolveEffect.Parameters["xLerpFactor"].SetValue(1 - Life / stats.Health);
            ShaderWrapper.DissolveEffect.Parameters["xLerpColor"].SetValue(DeathColor);
            ShaderWrapper.DissolveEffect.Begin();
            s.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].Begin();

            s.Draw(tex, position, null, Color.White, angle, origin, scale, SpriteEffects.None, 0);
            
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].End();

            s.End();

            ShaderWrapper.DissolveEffect.End();

            ShaderWrapper.DissolveEffect.Parameters["xLerp"].SetValue(false);
            ShaderWrapper.DissolveEffect.Begin();
            s.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            ShaderWrapper.DissolveEffect.CurrentTechnique.Passes[0].Begin();
        }
    }
}
