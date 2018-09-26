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
    class NormalMap
    {
        static float Pulse { get; set; }
        static float MaxPulse { get; set; }
        static Texture2D veinMap;
        public static Texture2D blastWave;
        public static Texture2D blastWave2;
        static Texture2D playerDeathWave;
        RenderTarget2D target;
        Color pulseForce;
        Game1 gRef;
        float alpha;
        float pulseCounter;

        int tileSize;
        Color normalClear;

        static List<BlastWave> waves;

        const float MAX_BEAT = 3;
        const float MAX_ALPHA = 1f;
        const float MIN_ALPHA = 0.01f;

        public NormalMap(Game1 game)
        {
            veinMap = game.Content.Load<Texture2D>("Normals/veins3_NRM");
            blastWave = game.Content.Load<Texture2D>("Normals/bw2");
            blastWave2 = game.Content.Load<Texture2D>("Normals/blast1_NRM");
            playerDeathWave = game.Content.Load<Texture2D>("Normals/starbomb_NRM");

            pulseForce = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            target = new RenderTarget2D(game.GraphicsDevice, Game1.ScreenX, Game1.ScreenY, 1, SurfaceFormat.Color);
            pulseCounter = Pulse;
            Pulse = ForceCounter.CurrentForce + 0.01f;
            MaxPulse = 1;
            gRef = game;
            tileSize = veinMap.Width;
            normalClear = new Color(0.5f, 0.5f, 1.0f, 1.0f);
            waves = new List<BlastWave>();
        }

        public static void ResetAll()
        {
            waves.Clear();
            MaxPulse = 1;
            Pulse = ForceCounter.CurrentForce + 0.01f;
        }
        public void Update(GameTime time)
        {
            float nextTime = (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            UpdatePulse();
            BeatHeart(nextTime);

            KillWaves();
            foreach (BlastWave w in waves)
            {
                w.Update(nextTime);
            }
        }

        private void BeatHeart(float time)
        {
            pulseCounter -= time;

            if (pulseCounter < 0)
            {
                pulseCounter = Pulse;
            }
            if (pulseCounter > Pulse - 0.3f)
            {
                alpha = MaxPulse;
            }
            else
            {
                alpha = 0;
            }
            //alpha = (pulseCounter / Pulse) * MaxPulse;

            pulseForce = new Color(1.0f, 1.0f, 1.0f, alpha);

        }

        private void KillWaves()
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i].IsDead)
                {
                    waves.RemoveAt(i);
                }
            }
        }

        private void UpdatePulse()
        {
            MaxPulse = MAX_ALPHA - (((ForceCounter.CurrentForce - 40) / 60.0f) * (MAX_ALPHA - MIN_ALPHA));

            Pulse = ((ForceCounter.CurrentForce - 40) / 60.0f) * MAX_BEAT;
        }

        public Texture2D DrawNormals(SpriteBatch s)
        {
            gRef.GraphicsDevice.SetRenderTarget(0, target);
            gRef.GraphicsDevice.Clear(normalClear);
            s.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            
            int loopX = Game1.ScreenX/tileSize + 1;
            int loopY = Game1.ScreenY/tileSize + 1;

            for (int x = 0; x < loopX; x++)
            {
                for (int y = 0; y < loopY; y++)
                {
                    s.Draw(veinMap, new Vector2(x * tileSize, y * tileSize), pulseForce);
                }
            }

            //s.Draw(veinMap, new Rectangle(0,0, Game1.ScreenX, Game1.ScreenY), pulseForce);
            foreach (BlastWave w in waves)
            {
                w.Draw(s);
            }
            s.End();
            gRef.GraphicsDevice.SetRenderTarget(0, null);

            return target.GetTexture();
        }

        public static BlastWave AddBlast(Vector2 position, float scale, float life)
        {
            BlastWave b = new BlastWave(scale, life, position, blastWave);
            waves.Add(b);
            return b;
        }

        public static void AddBlast2(Vector2 position, float scale, float life)
        {
            waves.Add(new BlastWave(scale, life, position, blastWave2));
        }

        public static void AddStarWave(Vector2 position, float scale, float life)
        {
            waves.Add(new BlastWave(scale, life, position, playerDeathWave));
        }

        public static void AddGenericBlast(BlastWave b)
        {
            waves.Add(b);
        }
    }
}
