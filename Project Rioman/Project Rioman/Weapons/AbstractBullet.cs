using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    abstract class AbstractBullet
    {
        protected int weight;
        protected bool canDie;
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
            canDie = true;
            this.type = type;
            if (facingRight)
                direction = 1;
            else
                direction = -1;

            speed = BulletAttributes.GetSpeedAttribute(type);
            damage = BulletAttributes.GetDamageAttribute(type);
            weight = BulletAttributes.GetWeight(type);

            isAlive = true;
            hitList = new List<string>();

            Audio.PlayShoot();
        }

        public void Update(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies)
        {
            if (canDie)
                if (location.X > viewport.Width || location.X < - drawRect.Width ||
                    location.Y < - drawRect.Height || location.Y > viewport.Height)
                    isAlive = false;

            SubUpdate(player, deltaTime, viewport, enemies);

        }

        public abstract void Draw(SpriteBatch spriteBatch);
        protected abstract void SubUpdate(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies);
        protected abstract void SubMove(int x, int y);
        public abstract int TakeDamage(string enemyID);
        public abstract bool Hits(Rectangle collisionRect);

        public void MoveBullet(int x, int y)
        {
            location.X += x;
            location.Y += y;

            SubMove(x, y);
        }

        public void Kill()
        {
            isAlive = false;
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public int GetWeight()
        {
            return weight;
        }

        public abstract Rectangle GetCollisionRect();
    }

}
