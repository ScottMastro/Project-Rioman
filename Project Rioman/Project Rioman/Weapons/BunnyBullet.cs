using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class BunnyBullet : AbstractBullet
    {
        private int frame;
        private double frameTime;
        private float floatAngle;
        private AbstractEnemy target;

        public BunnyBullet(int x, int y, bool facingRight) : base(Constant.BUNNYBULLET, facingRight)
        {

            sprite = BulletAttributes.GetSprites(Constant.BUNNYBULLET)[0];

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x - 24;

            location.Y = y + 20;
            frame = 0;
            frameTime = 0;
            target = null;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                drawRect = new Rectangle(frame * sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);

                spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(),
                    direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

                if (target != null)
                    DebugDraw.DrawRect(spriteBatch, target.GetCollisionRect(), 0.3f);
            }
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
            if (isAlive)
            {
                isAlive = false;
                return damage;
            }
            return 0;
        }

        protected override void SubMove(int x, int y)
        {
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies)
        {
            if (isAlive)
            {
                frameTime += deltaTime;

                if (frameTime > 0.15)
                {
                    frameTime = 0;
                    if (frame <= 0)
                        frame = 1;
                    else
                        frame = 0;
                }

                location.X += direction * speed;

                floatAngle += MathHelper.WrapAngle((float)deltaTime *10);
                location.Y += (int)(Math.Sin(floatAngle) * 3);



                target = null;
                Point bulletCenter = GetCollisionRect().Center;

                for(int i = 0; i<= enemies.Length -1; i++)
                {
                    Point enemyCenter = enemies[i].GetCollisionRect().Center;

                    if (enemies[i].IsAlive() && !enemies[i].IsInvincible() && TargetCandiate(enemyCenter, bulletCenter))
                    {
                        if (target == null || ACloserThanB(enemyCenter, target.GetCollisionRect().Center, bulletCenter))
                            target = enemies[i];
                    }
                }

                if (target != null)
                {
                    if (target.GetCollisionRect().Top - bulletCenter.Y > 30 )
                        location.Y += 3;
                    else if (target.GetCollisionRect().Top - bulletCenter.Y > 3)
                        location.Y += 1;
                    else if(target.GetCollisionRect().Bottom - bulletCenter.Y < 30)
                        location.Y -= 3;
                    else if (target.GetCollisionRect().Bottom - bulletCenter.Y < 3)
                        location.Y -= 1;
                }


            }
        }

        private bool TargetCandiate(Point target, Point bullet)
        {
            if (direction < 0 && target.X < bullet.X)
                return true;
            else if (direction > 0 && target.X > bullet.X)
                return true;

            return false;
        }

        private bool ACloserThanB(Point a, Point b, Point bullet)
        {
            double aToTarget = Math.Sqrt((bullet.X - a.X) ^ 2 + (bullet.Y - a.Y) ^ 2);
            double bToTarget = Math.Sqrt((bullet.X - b.X) ^ 2 + (bullet.Y - b.Y) ^ 2);

            return aToTarget < bToTarget;

        }
    }
}
