using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class RioBullet : AbstractBullet
    {
        public RioBullet(int x, int y, bool facingRight) : base(Constant.RIOBULLET, facingRight)
        {
            sprite = BulletAttributes.GetSprites(Constant.RIOBULLET)[0];

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            if (facingRight)
                location.X = x - 20;
            else
                location.X = x + 20;

            location.Y = y + 22;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, location, Color.White);
        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }


        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (isAlive)
                location.X += speed * direction;
        }

        public override int TakeDamage(string enemyID)
        {
            if (isAlive)
            {
                isAlive = false;
                return damage;
            }
            else return 0;
        }

        public override bool Hits(Rectangle collisionRect)
        {
            return isAlive && collisionRect.Intersects(GetCollisionRect());
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}