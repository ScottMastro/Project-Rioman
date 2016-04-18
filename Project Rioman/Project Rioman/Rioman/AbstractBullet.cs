using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project_Rioman
{
    abstract class AbstractBullet
    {
        protected Texture2D sprite;
        protected bool isAlive;
        protected Rectangle location;
        protected int direction;
        protected int damage;

        public void Update(Viewport viewport)
        {
            if (location.X > viewport.Width || location.X < 0 - sprite.Width)
                isAlive = false;

            SubUpdate();

        }

        public abstract void BulletSpawn(int x, int y, SpriteEffects dir);
        public abstract void Draw(SpriteBatch spriteBatch);
        protected abstract void SubUpdate();

        public int TakeDamage()
        {
            if (isAlive)
            {
                isAlive = false;
                return damage;
            }
            else return 0;
        }

        public void MoveBullet(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void Kill()
        {
            isAlive = false;
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public bool Hits(Rectangle collisionRect)
        {
            return isAlive && collisionRect.Intersects(GetCollisionRect());
        }

        public abstract Rectangle GetCollisionRect();
    }

}
