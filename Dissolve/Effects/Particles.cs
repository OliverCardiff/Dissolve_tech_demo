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
    public struct VertexVelocity
    {
        public Vector3 Position;
        public Vector2 Velocity;

        public static int SizeInBytes = 5 * 4;
        public static VertexElement[] VertexElements = new VertexElement[]
                {
                    new VertexElement(0,0, VertexElementFormat.Vector3, VertexElementMethod.Default,
                    VertexElementUsage.Position, 0),
                    new VertexElement(0,sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default,
                    VertexElementUsage.TextureCoordinate, 0),
                };
    }

    class VertexBag
    {
        static VertexDeclaration dec;

        Vector4 startColor;
        Vector4 endColor;
        VertexBuffer vBuffer;
        float duration;
        int count;
        Vector2 location;
        float currentTime;
        Texture2D backTex;
        int size;
        bool doSample;

        private const float SPEED = 400f;

        public Vector2 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public bool KillMe { get; set; }

        public int Size { get; set; }

        public VertexBag(Vector2 effectLocation, Color start, Color end, float duration, int count, int size, Texture2D tex)
        {
            this.location = effectLocation;
            location.X = ((location.X - Game1.ScreenX / 2) / Game1.ScreenX) * 2;
            location.Y = ((location.Y - Game1.ScreenY / 2) / Game1.ScreenY) * -2;
            startColor = ColorToV4(start, 1.7f);
            endColor = ColorToV4(end, 0.4f);
            this.duration = duration;
            this.count = Math.Max(count, 1);
            currentTime = 0;
            this.size = size;
            Size = this.count;
            KillMe = false;
            doSample = true;
            MakeParticles();
            backTex = tex;

        }
        public VertexBag(Vector2 effectLocation, Color start, Color end, float duration, int count, int size)
        {
            this.location = effectLocation;
            location.X = ((location.X - Game1.ScreenX / 2) / Game1.ScreenX) * 2;
            location.Y = ((location.Y - Game1.ScreenY / 2) / Game1.ScreenY) * -2;
            startColor = ColorToV4(start, 16.0f);
            endColor = ColorToV4(end, 16.0f);
            this.duration = duration;
            this.count = Math.Max(count, 1);
            currentTime = 0;
            this.size = size;
            Size = this.count;
            KillMe = false;
            MakeParticles();
            doSample = false;
            backTex = null;

        }

        private void MakeParticles()
        {
            VertexVelocity[] vertices = new VertexVelocity[count];
            double angle;
            double speed;

            for (int i = 0; i < vertices.Length; i++)
            {
                angle = Game1.rand.NextDouble() * Math.PI * 2;
                speed = Game1.rand.NextDouble() * SPEED;

                vertices[i] = new VertexVelocity();
                vertices[i].Position.X = location.X;
                vertices[i].Position.Y = location.Y;
                vertices[i].Velocity.X = (float)(Math.Cos(angle) * speed);
                vertices[i].Velocity.Y = (float)(Math.Sin(angle) * speed);
            }

            dec = new VertexDeclaration(Game1.game.GraphicsDevice, VertexVelocity.VertexElements);

            vBuffer = new VertexBuffer(Game1.game.GraphicsDevice, VertexVelocity.SizeInBytes * vertices.Length, BufferUsage.WriteOnly);
            vBuffer.SetData<VertexVelocity>(vertices);

        }

        private Vector4 ColorToV4(Color c, float alphaMod)
        {
            return new Vector4((float)c.R / 255.0f, (float)c.G / 255.0f, (float)c.B / 255.0f, (float)c.A / (255.0f * alphaMod));
        }

        public void Draw(GameTime time)
        {
            int passNo;
            if (doSample) passNo = 1; else passNo = 0;
            currentTime += (float)time.ElapsedGameTime.Milliseconds / 1000.0f;

            if (currentTime > duration)
            {
                KillMe = true;
            }
            ShaderWrapper.ParticleShader.Parameters["xTime"].SetValue(currentTime);
            ShaderWrapper.ParticleShader.Parameters["xXScreenDimension"].SetValue(Game1.ScreenX);
            ShaderWrapper.ParticleShader.Parameters["xYScreenDimension"].SetValue(Game1.ScreenY);
            ShaderWrapper.ParticleShader.Parameters["xDuration"].SetValue(duration);
            ShaderWrapper.ParticleShader.Parameters["xStartColor"].SetValue(startColor);
            ShaderWrapper.ParticleShader.Parameters["xEndColor"].SetValue(endColor);
            ShaderWrapper.ParticleShader.Parameters["xSample"].SetValue(false);


            Game1.game.GraphicsDevice.RenderState.PointSize = size;
            //Game1.graphics.GraphicsDevice.RenderState.PointSpriteEnable = true;

            ShaderWrapper.ParticleShader.Begin();

            ShaderWrapper.ParticleShader.CurrentTechnique.Passes[passNo].Begin();

            Game1.game.GraphicsDevice.VertexDeclaration = dec;
            Game1.game.GraphicsDevice.Vertices[0].SetSource(vBuffer, 0, VertexVelocity.SizeInBytes);
            Game1.game.GraphicsDevice.DrawPrimitives(PrimitiveType.PointList, 0, vBuffer.SizeInBytes / VertexVelocity.SizeInBytes);
            ShaderWrapper.ParticleShader.CurrentTechnique.Passes[passNo].End();

            ShaderWrapper.ParticleShader.End();
        }
    }
}
