using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class P1H8R : AbstractEnemy
    {
        private Texture2D bullet;
        private const int BULLET_DAMAGE = 4;
        private const int BULLET_SPEED = 9;

        private double shootTime;
        private bool chasing;

        private bool stopDownMovement;
        private bool stopUpMovement;
        private bool stopLeftMovement;
        private bool stopRightMovement;

        struct H8Bullet
        {
            public bool isAlive;
            public int X;
            public int Y;

        }
        private H8Bullet[] bullets = new H8Bullet[3];


        public P1H8R(int type, int x, int y) : base(type, x, y)

        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            sprite = sprites[0];
            bullet = sprites[1];

            SubReset();
        }

        protected sealed override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

            shootTime = 0;
            chasing = false;

            stopDownMovement = false;
            stopUpMovement = false;
            stopLeftMovement = false;
            stopRightMovement = false;
        }


        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
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

                    if (playerY < thisY + 76 && !stopUpMovement)
                        location.Y--;
                    else if (playerY > thisY + 76 && !stopDownMovement)
                        location.Y++;

                    if (thisX - playerX > 4 && !stopLeftMovement)
                    {
                        location.X--;
                        direction = SpriteEffects.None;
                    }
                    else if (playerX - thisX > 4 && !stopRightMovement)
                    {
                        location.X++;
                        direction = SpriteEffects.FlipHorizontally;
                    }
                }

                shootTime += deltaTime;
                if (shootTime > 1.5)
                {
                    shootTime = 0;
                    int index = -1;

                    for (int i = 0; i <= bullets.Length - 1; i++)
                        if (!bullets[i].isAlive)
                            index = i;

                    if (index > -1)
                    {
                        bullets[index].isAlive = true;
                        bullets[index].Y = thisY;
                        bullets[index].X = thisX;
                    }
                }

                stopDownMovement = false;
                stopUpMovement = false;
                stopLeftMovement = false;
                stopRightMovement = false;
            }

            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive)
                {
                    bullets[i].Y += BULLET_SPEED;

                    if (bullets[i].Y < -20 || bullets[i].Y > viewport.Height + 20)
                        bullets[i].isAlive = false;
                }
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive && !player.IsLurking() && player.Hitbox.Intersects(new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height)))
                {
                    player.Hit(BULLET_DAMAGE);
                    bullets[i].isAlive = false;
                }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, 0f, new Vector2(), direction, 0);

        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (bullets[i].isAlive)
                    spriteBatch.Draw(bullet, new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height), Color.White);
            }
        }

        protected override void SubMove(int x, int y)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                bullets[i].X += x;
                bullets[i].Y += y;
            }
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 6, location.Y + 6, drawRect.Width - 12, drawRect.Height - 12);
        }
        private Rectangle Left() { return new Rectangle(location.X + 6, location.Y + 6, 10, drawRect.Height - 12); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width - 16, location.Y + 6, 10, drawRect.Height - 12); }
        private Rectangle Top() { return new Rectangle(location.X + 6, location.Y, drawRect.Width - 12, 10); }
        private Rectangle Bottom() { return new Rectangle(location.X + 6, location.Y + drawRect.Height - 10, drawRect.Width - 12, 10); }


        public override void DetectTileCollision(AbstractTile tile)
        {

            if (tile.Type == 1 || tile.Type == 3 && tile.IsTop || tile.Type == 4)
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
    }
}