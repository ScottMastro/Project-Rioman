using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class AuroraBullet : AbstractBullet
    {
        public AuroraBullet(int x, int y, bool facingRight) : base(Constant.AURORABULLET, facingRight)
        {

            sprite = BulletAttributes.GetSprites(Constant.AURORABULLET)[0];

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x - 24;

            location.Y = y + 20;

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                SpriteEffects se = SpriteEffects.None;

                if (direction < 0)
                    se = SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), se, 0);
            }
        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport)
        {
            location.X += direction * speed;
        }

        public override int TakeDamage(string enemyID)
        {
            if (hitList.Contains(enemyID))
                return 0;

            hitList.Add(enemyID);

            if (isAlive)
                return damage;
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
