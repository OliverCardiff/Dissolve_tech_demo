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
        Chase, Divide
    }
    class Enemy
    {
        Texture2D tex;
        Vector2 position;
        Vector2 velocity;
        Behaviour mode;

        public Enemy(Texture2D t, Vector2 p, Vector2 v, Behaviour b)
        {
            tex = t;
            position = p;
            velocity = v;
            mode = b;
        }
    }
}
