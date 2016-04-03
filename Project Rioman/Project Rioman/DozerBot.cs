using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class DozerBot : AbstractEnemy
    {
        private Texture2D bullet;

        private int frame;
        private double frameTime;
        private double fallTime;

        private bool isGroundBelow;

        private enum State { rolling, falling };
        private State state;
        private bool shooting;
        private double shootTime;
        private double timeBetweenShooting;
        private int bulletCounter;

        struct DozerBullet
        {
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;

            public void Reset()
            {
                isAlive = false;
                X = 0;
                Y = 0;
                direction = -1;
            }

        }

        private DozerBullet[] bullets = new DozerBullet[3];
        private const int BULLET_DAMAGE = 2;
        private const int BULLET_SPEED = 12;


        public DozerBot(int type, int r, int c) : base(type, r, c)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            sprite = sprites[0];
            bullet = sprites[1];

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

            location.Y -= sprite.Height;

            shooting = false;
            frame = 0;
            frameTime = 0;
            Fall();
            isGroundBelow = false;

        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                frameTime += deltaTime;
                if (frameTime > 0.25)
                {
                    frameTime = 0;
                    frame++;
                    if (frame >= 2)
                        frame = 0;
                }


                if (IsFalling())
                {
                    fallTime += deltaTime;

                    if (fallTime * 30 > 10)
                        location.Y += 10;
                    else
                        location.Y += Convert.ToInt32(fallTime * 10);
                }

                if (FacingLeft())
                    location.X -= 3;
                else
                    location.X += 3;


                if (!isGroundBelow)
                    Fall();

                isGroundBelow = false;

                if (player.Hitbox.Intersects(KillPoint()))
                    player.Hit(Constant.MAX_HEALTH);

                if(!shooting)
                    CheckShoot(deltaTime);
                else
                    UpdateShoot(deltaTime);
            }
          
            UpdateBullets(deltaTime, viewport);

        }

        private void CheckShoot(double deltaTime)
        {
            timeBetweenShooting += deltaTime;

            if (timeBetweenShooting > 1)
            {
                bool flag = true;

                for (int i = 0; i <= bullets.Length - 1; i++)
                    if (bullets[i].isAlive)
                        flag = false;

                shooting = flag;
                shootTime = 0;
                bulletCounter = 0;
            }
        }

        private void UpdateBullets(double deltaTime, Viewport viewport)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (bullets[i].isAlive)
                {
                    bullets[i].X += BULLET_SPEED * bullets[i].direction;

                    if (bullets[i].X > viewport.Width + 20 || bullets[i].X < -20)
                        bullets[i].isAlive = false;
                }
            }
        }

        private void UpdateShoot(double deltaTime)
        {
            timeBetweenShooting = 0;
            shootTime += deltaTime;

            if (shootTime > 0.1)
            {
                shootTime = 0;

                if (bulletCounter < bullets.Length)
                {
                    //Audio.PlayEnemyShoot1();

                    bullets[bulletCounter].isAlive = true;

                    if (FacingLeft())
                    {
                        bullets[bulletCounter].direction = -1;
                        bullets[bulletCounter].X = Left().Left;
                    }
                    else {
                        bullets[bulletCounter].direction = 1;
                        bullets[bulletCounter].X = Right().Left;
                    }

                    bullets[bulletCounter].Y = location.Y + drawRect.Height * 3 / 5;

                    bulletCounter++;
                }
                else
                    shooting = false;
            }
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive && player.Hitbox.Intersects(new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height)))
                {
                    player.Hit(BULLET_DAMAGE);
                    bullets[i].isAlive = false;
                }
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (bullets[i].isAlive)
                    spriteBatch.Draw(bullet, new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height), Color.White);

            }

        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            drawRect = new Rectangle(frame * sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
            Rectangle locRect = new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height);

            spriteBatch.Draw(sprite, locRect, drawRect, Color.White, 0f, new Vector2(), direction, 0);

        }


        public override void DetectTileCollision(Tile tile)
        {
            if (tile.type == 1 || tile.type == 3 && tile.isTop)
            {
                if (Feet().Intersects(tile.Floor))
                    GroundCollision(tile.location.Y);
            }

            if (tile.type == 1 || tile.type == 4)
            {
                if (Right().Intersects(tile.Left))
                    LeftCollision();
                if (Left().Intersects(tile.Right))
                    RightCollision();
            }

        }

        private void GroundCollision(int groundTop)
        {

            isGroundBelow = true;

            if (IsFalling())
            {
                Ground();
                location.Y = groundTop - drawRect.Height;
            }

        }

        private void LeftCollision()
        {
            direction = SpriteEffects.None;
            shooting = false;
        }

        private void RightCollision()
        {
            direction = SpriteEffects.FlipHorizontally;
            shooting = false;
        }


        protected override void SubMove(int x, int y)
        {

            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                bullets[i].X += x;
                bullets[i].Y += y;
            }
        }

        private Rectangle Left() { return new Rectangle(location.X + 2, location.Y + 20, 10, drawRect.Height - 30); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width - 13, location.Y + 20, 10, drawRect.Height - 30); }
        private Rectangle Feet() { return new Rectangle(location.X + 10, location.Y + drawRect.Height - 10, drawRect.Width - 20, 10); }

        private Rectangle KillPoint()
        {
            if (FacingLeft())
                return new Rectangle(location.X + 26, location.Y + 6, 6, 20);
            else
                return new Rectangle(location.X + 21, location.Y + 6, 6, 20);
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 5, location.Y + 20, drawRect.Width - 10, drawRect.Height - 20);

        }

        public bool IsFalling() { return state == State.falling; }
        public bool IsGrounded() { return state == State.rolling; }

        private void Ground()
        {
            state = State.rolling;
        }

        private void Fall()
        {
            if (!IsFalling())
            {
                state = State.falling;
                fallTime = 0;
            }
        }
    }
}
