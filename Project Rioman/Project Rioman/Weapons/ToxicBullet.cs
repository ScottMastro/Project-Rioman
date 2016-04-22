using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class ToxicBullet : AbstractBullet
    {
        private Texture2D bubbleInside;
        private Texture2D bubbleBorder;

        private int frame;
        private double frameTime;

        public ToxicBullet(int x, int y, bool facingRight) : base(Constant.TOXICBULLET, facingRight)
        {

            bubbleBorder = BulletAttributes.GetSprites(Constant.TOXICBULLET)[0];
            bubbleInside = BulletAttributes.GetSprites(Constant.TOXICBULLET)[1];

            drawRect = new Rectangle(0, 0, bubbleBorder.Width/3, bubbleBorder.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x - 24;

            location.Y = y + 20;

            frame = 0;
            frameTime = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            drawRect = new Rectangle(bubbleBorder.Width / 3 * frame, 0, bubbleBorder.Width / 3, bubbleBorder.Height);

            spriteBatch.Draw(bubbleBorder, location, drawRect, Color.White);
            spriteBatch.Draw(bubbleInside, location, drawRect, Color.White * 0.8f);

        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }

        public override bool Hits(Rectangle collisionRect)
        {
            return GetCollisionRect().Intersects(collisionRect);
        }

        public override int TakeDamage(string enemyID)
        {
            return damage;
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport)
        {
            location.X += direction * speed;

            double advanceFrame = 0.3;

            frameTime += deltaTime;
            if(frameTime > advanceFrame * 4)
            {
                frameTime = 0;
                frame = 0;
            }
            else if (frameTime > advanceFrame * 3)
                frame = 2;
            else if (frameTime > advanceFrame * 2)
                frame = 0;
            else if (frameTime > advanceFrame)
                frame = 1;
            else
                frame = 0;

        }
    }
}
