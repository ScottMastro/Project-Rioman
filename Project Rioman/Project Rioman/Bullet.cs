using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Bullet
    {

        public Texture2D sprite;
        public bool alive;
        public Rectangle location;
        int direction;
        private int damage = 1;

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

        public int TakeDamage()
        {
            alive = false;
            return damage;
        }
    }
}