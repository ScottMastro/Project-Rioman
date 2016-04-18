using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    abstract class AbstractBullet
    {
        protected int type;
        protected Texture2D sprite;
        protected bool isAlive;
        protected Rectangle location;
        protected Rectangle drawRect;
        protected int direction;
        protected int damage;
        protected int speed;
        protected List<string> hitList;

        protected AbstractBullet(int type, bool facingRight)
        {
            this.type = type;
            if (facingRight)
                direction = 1;
            else
                direction = -1;

            speed = BulletAttributes.GetSpeedAttribute(type);
            damage = BulletAttributes.GetDamageAttribute(type);

            isAlive = true;
            hitList = new List<string>();
        }

        public void Update(double deltaTime, Viewport viewport)
        {
            if (location.X > viewport.Width || location.X < 0 - drawRect.Width)
                isAlive = false;

            SubUpdate(deltaTime);

        }

        public abstract void Draw(SpriteBatch spriteBatch);
        protected abstract void SubUpdate(double deltaTime);
        public abstract int TakeDamage(string enemyID);

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
