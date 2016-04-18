using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Hellicoptor : AbstractEnemy
    {

        private double frameTime;
        private int frame;
        private bool chasing;

        private const int SPEED = 2;

        private bool stopDownMovement;
        private bool stopUpMovement;
        private bool stopLeftMovement;
        private bool stopRightMovement;

        public Hellicoptor(int type, int x, int y) : base(type, x, y)
        {
            sprite = EnemyAttributes.GetSprites(type)[0];

            SubReset();
        }

        protected override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

            frameTime = 0;
            frame = 0;
            chasing = false;

            stopDownMovement = false;
            stopUpMovement = false;
            stopLeftMovement = false;
            stopRightMovement = false;
        }


        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
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
                    if (Math.Abs(thisX - playerX) > 250 ||
                        (Math.Abs(thisY - playerY) > 250))
                        chasing = false;

                    if (playerY < thisY && !stopUpMovement)
                        location.Y -= SPEED;
                    else if (playerY > thisY && !stopDownMovement)
                        location.Y += SPEED;

                    if (thisX - playerX > 4 && !stopLeftMovement)
                    {
                        location.X -= SPEED;
                        direction = SpriteEffects.None;
                    }
                    else if (playerX - thisX > 4 && !stopRightMovement)
                    {
                        location.X += SPEED;
                        direction = SpriteEffects.FlipHorizontally;
                    }
                }

                frameTime += deltaTime;
                if (frameTime > 0.2)
                {
                    frameTime = 0;

                    if (frame == 0)
                        frame = 1;
                    else
                        frame = 0;
                }

                stopDownMovement = false;
                stopUpMovement = false;
                stopLeftMovement = false;
                stopRightMovement = false;
            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            drawRect = new Rectangle(frame * sprite.Width / 2, 0, sprite.Width/2, sprite.Height);

            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, 0f, new Vector2(), direction, 0);

        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 6, location.Y + 6, drawRect.Width - 12, drawRect.Height - 12);
        }
        private Rectangle Left() { return new Rectangle(location.X + 6, location.Y + 6, 10, drawRect.Height - 12); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width - 16, location.Y + 6, 10, drawRect.Height - 12); }
        private Rectangle Top() { return new Rectangle(location.X + 6, location.Y, drawRect.Width - 12, 10); }
        private Rectangle Bottom() { return new Rectangle(location.X + 6, location.Y + drawRect.Height - 10, drawRect.Width - 12, 10); }


        public override void DetectTileCollision(Tile tile)
        {

            if (tile.type == 1 || tile.type == 3 && tile.isTop || tile.type == 4)
            {
                if (Bottom().Intersects(tile.Floor))
                    stopDownMovement = true;
                if (Top().Intersects(tile.Bottom))
                    stopUpMovement = true;
                if (Right().Intersects(tile.Left))
                    stopRightMovement = true;
                if (Left().Intersects(tile.Right))
                    stopLeftMovement = true;
            }
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            //do nothing
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}