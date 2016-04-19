using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class InfernoBullet : AbstractBullet
    {

        private int frame;
        private double frameTime;
        private int frameDir;

        public InfernoBullet(int x, int y, bool facingRight) : base(Constant.INFERNOBULLET, facingRight){

            sprite = BulletAttributes.GetSprites(Constant.INFERNOBULLET)[1];

            drawRect = new Rectangle(frame * sprite.Width/4, 0, sprite.Width / 4, sprite.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            frameTime = 0;
            frame = 4;
            frameDir = -1;

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x - 46;

            location.Y = y + 10;

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive) {

                drawRect = new Rectangle(frame * sprite.Width / 4, 0, sprite.Width / 4, sprite.Height);

                SpriteEffects se = SpriteEffects.None;

                if (direction < 0)
                    se = SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), se, 0);
            }
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + drawRect.Width / 2, location.Y + 10, drawRect.Width / 2, drawRect.Height - 20);
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            frameTime += deltaTime;
           
            if(frameTime > 0.06)
            {
                frameTime = 0;
                frame += frameDir;

                if (frame <= 0)
                    frameDir = 1;
                else if (frame >= 4)
                    Kill();
            }


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
