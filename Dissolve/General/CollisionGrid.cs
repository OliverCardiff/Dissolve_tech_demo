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
    struct GridBox
    {
        public List<Enemy> enemies;
        public List<Bullet> bullets;
        public bool hasContents;

        public GridBox(bool conts)
        {
            hasContents = conts;
            enemies = new List<Enemy>();
            bullets = new List<Bullet>();
        }
        
    }
    static class CollisionGrid
    {
        const int BOX_SIZE = 30;
        static int indexX;
        static int indexY;
        static float dist;

        static GridBox[,] boxes = new GridBox[Game1.ScreenX / BOX_SIZE + 4, Game1.ScreenY / BOX_SIZE + 4];

        public static void Assemble()
        {
            for (int i = 0; i < boxes.GetLength(0); i++)
            {
                for (int j = 0; j < boxes.GetLength(1); j++)
                {
                    boxes[i, j] = new GridBox(false);
                }
            }
        }

        public static void Commit(Enemy e)
        {
            indexX = Math.Max(((int)e.Position.X + BOX_SIZE * 2) / BOX_SIZE,0);
            indexY = Math.Max(((int)e.Position.Y + BOX_SIZE * 2) / BOX_SIZE,0);

            if (indexX >= boxes.GetLength(0))
            {
                indexX = boxes.GetLength(0)-1;
            }
            if (indexY >= boxes.GetLength(1))
            {
                indexY = boxes.GetLength(1)-1;
            }

            boxes[indexX, indexY].hasContents = true;
            boxes[indexX, indexY].enemies.Add(e);
        }

        public static void Commit(Bullet b)
        {
            indexX = Math.Max(((int)b.Position.X + BOX_SIZE * 2) / BOX_SIZE, 0);
            indexY = Math.Max(((int)b.Position.Y + BOX_SIZE * 2) / BOX_SIZE, 0);

            if (indexX >= boxes.GetLength(0))
            {
                indexX = boxes.GetLength(0) - 1;
            }
            if (indexY >= boxes.GetLength(1))
            {
                indexY = boxes.GetLength(1) - 1;
            }
            boxes[indexX, indexY].bullets.Add(b);
        }

        private static void ClearAll()
        {
            foreach (GridBox b in boxes)
            {
                b.enemies.Clear();
                b.bullets.Clear();
            }
        }

        public static void CollideAll()
        {
            for (int i = 0; i < boxes.GetLength(0); i++)
            {
                for (int j = 0; j < boxes.GetLength(1); j++)
                {
                    if (boxes[i, j].hasContents)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                if (BoundCheck(i + x, j + y))
                                {
                                    CollideLists(boxes[i, j].enemies, boxes[i + x, j + y].bullets);
                                    CollideLists(boxes[i + x, j + y].enemies, boxes[i, j].bullets);
                                }
                            }
                        }
                    }

                    boxes[i, j].bullets.Clear();
                    boxes[i, j].enemies.Clear();
                    boxes[i, j].hasContents = false;
                }
            }
        }

        private static bool BoundCheck(int x, int y)
        {
            if (x >= 0 && x < boxes.GetLength(0) && y >= 0 && y < boxes.GetLength(1))
            {
                return true;
            }
            return false;
        }

        private static void CollideLists(List<Enemy> es, List<Bullet> bs)
        {
            foreach (Enemy e in es)
            {
                foreach (Bullet b in bs)
                {
                    dist = Vector2.Distance(e.Position, b.Position);

                    if (dist < e.Size + b.Size && !b.IsDead)
                    {
                        e.Hit(b.Damage);
                        if (b.Damage < 3)
                        {
                            b.IsDead = true;
                        }
                        if (LevelManager.Mode == GameMode.Worms)
                        {
                            b.Sustain(e.PointsValue * 5);
                        }
                    }
                }
            }
        }
    }
}
