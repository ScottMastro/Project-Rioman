using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Bullet
    {

        public Texture2D sprite;
        public bool isAlive;
        public Rectangle location;
        int direction;
        private int damage = 1;

        public Bullet(Texture2D bullet)
        {
            sprite = bullet;
            isAlive = false;
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

            isAlive = true;
        }

        public void BulletUpdate(int viewport)
        {
            if (location.X > viewport || location.X < 0 - sprite.Width)
                isAlive = false;

            if (isAlive)
                location.X += 9 * direction;
        }

        public void MoveBullet(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public int TakeDamage()
        {
            isAlive = false;
            return damage;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, location, Color.White);

        }
    }
}