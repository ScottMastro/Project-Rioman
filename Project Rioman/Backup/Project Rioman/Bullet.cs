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

namespace Project_Rioman
{
    class Bullet
    {

        public Texture2D sprite;
        public bool alive;
        public Rectangle location;
        int direction;

        public Bullet(Texture2D bullet)
        {
            sprite = bullet;
            alive = false;
            location = new Rectangle(0, 0, bullet.Width, bullet.Height);
        }

        public void BulletSpawn(int x, int y, SpriteEffects dir)
        {
            location.X = x;
            location.Y = y;

            if (dir == SpriteEffects.None)
                direction = 1;
            else
                direction = -1;

            alive = true;
        }

        public void BulletUpdate(int viewport)
        {
            if (location.X > viewport || location.X < 0 - sprite.Width)
                alive = false;

            if (alive)
                location.X += 9 * direction;
        }

        public void MoveBullet(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }
    }
}