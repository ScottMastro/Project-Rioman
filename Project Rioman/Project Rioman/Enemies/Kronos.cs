using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Kronos : AbstractEnemy
    {
        private double frameTime;
        private int frame;
        private int animateDirection;
        private bool chasing;

        private double hitTimer;

        private const int SPEED = 1;

        public Kronos(int type, int x, int y) : base(type, x, y)
        {
            sprite = EnemyAttributes.GetSprites(type)[0];

            SubReset();
        }

        protected override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width / 10, sprite.Height);

            frameTime = 0;
            frame = 0;
            animateDirection = 1;
            chasing = false;
        }


        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {

                if (hitTimer > 0)
                    hitTimer -= deltaTime;

                if (!player.IsFrozen())
                {
                    int playerX = player.Hitbox.Center.X;
                    int playerY = player.Hitbox.Center.Y;
                    int thisX = GetCollisionRect().Center.X;
                    int thisY = GetCollisionRect().Center.Y;

                    if (!chasing)
                    {
                        if (Math.Abs(thisX - playerX) < 200 &&
                            (Math.Abs(thisY - playerY) < 200))
                            chasing = true;
                    }
                    if (chasing)
                    {
                        if (Math.Abs(thisX - playerX) > 350 ||
                            (Math.Abs(thisY - playerY) > 350))
                            chasing = false;

                        if (playerY - thisY > 4)
                            location.Y += SPEED + 1;
                        else if (thisY - playerY > 4)
                            location.Y -= SPEED + 1;

                        if (thisX - playerX > 4)
                            location.X -= SPEED;
                        else if (playerX - thisX > 4)
                            location.X += SPEED;

                    }
                }

                frameTime += deltaTime;
                if (frameTime > 0.1)
                {
                    frameTime = 0;

                    if (frame >= 9)
                        animateDirection = -1;
                    else if (frame <= 0)
                        animateDirection = 1;

                    frame += animateDirection;

                }

            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            drawRect = new Rectangle(frame * sprite.Width / 10, 0, sprite.Width / 10, sprite.Height);

            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, 0f, new Vector2(), direction, 0);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
           //do nothing
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 6, location.Y + 6, drawRect.Width - 12, drawRect.Height - 30);
        }
        



        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            if (isAlive && !player.IsInvincible() && hitTimer <= 0 && player.Hitbox.Intersects(GetCollisionRect()))
            {
                player.FreezeFor(2);
                hitTimer = 3;
            }
        }

        public override void DetectTileCollision(Tile tile)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}