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
    class NormalBullet : BlastWave
    {
        Vector2 velocity;

        public NormalBullet(float scale, float life, Vector2 position, Vector2 velocity, Texture2D tex)
            :base(scale,life,position,tex)
        {
            this.velocity = velocity;
        }

        public override void Update(float time)
        {
            position += velocity;
            base.Update(time);
            force = Color.White;
        }
    }
}
