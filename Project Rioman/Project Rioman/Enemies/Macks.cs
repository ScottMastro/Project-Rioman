using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    class Macks : AbstractEnemy
    {

        private Texture2D bullet;
        private bool stopUpMovement;
        private bool stopDownMovement;
        private bool collideWithTile;

        private double shootTime;

        struct MackBullet
        {
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;
            public int frame;
            public double frameTime;

            public void Reset()
            {
                isAlive = false;
                X = 0;
                Y = 0;
                direction = -1;
            }

            public void Update(double deltaTime)
            {
                frameTime += deltaTime;

                if (frameTime > 0.1)
                {
                    frameTime = 0;
                    frame++;
                    if (frame > 2)
                        frame = 0;
                }
            }

            public SpriteEffects SpriteDirection()
            {
                if (direction < 0)
                    return SpriteEffects.None;
                else
                    return SpriteEffects.FlipHorizontally;
            }
        }

        private MackBullet[] bullets = new MackBullet[3];
        private const int BULLET_SPEED = 8;
        private const int BULLET_DAMAGE = 5;

        public Macks(int type, int x, int y) : base(type, x, y)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);
            sprite = sprites[0];
            bullet = sprites[1];

            SubReset();
        }

        protected override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location.Y -= sprite.Height;

            stopUpMovement = false;
            stopDownMovement = false;
            collideWithTile = false;

            shootTime = 0;
        }


        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                if (player.Hitbox.Left > GetCollisionRect().Right)
                    direction = SpriteEffects.FlipHorizontally;
                else if (player.Hitbox.Right < GetCollisionRect().Left)
                    direction = SpriteEffects.None;


                int distance = GetCollisionRect().Center.Y - player.Hitbox.Center.Y;
                int speed = Math.Min(Math.Abs(distance) * 20 / viewport.Height, 12);
                speed = Math.Max(speed, 1);

                if (distance < 0 && !stopDownMovement)
                {
                    location.Y += speed;
                }
                else if (distance > 0 && !stopUpMovement)
                {
                    location.Y -= speed;

                }

                shootTime += deltaTime;
                distance = GetCollisionRect().Center.Y - player.Hitbox.Center.Y;
                if (shootTime > 0.5 && distance < 10)
                    Shoot();

                if (!collideWithTile)
                {
                    stopUpMovement = false;
                    stopDownMovement = false;
                }
                collideWithTile = false;

            }


            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (bullets[i].isAlive)
                {
                    bullets[i].Update(deltaTime);
                    bullets[i].X += BULLET_SPEED * bullets[i].direction;

                    if (bullets[i].X > viewport.Width + 20 || bullets[i].X < -20)
                        bullets[i].isAlive = false;
                }

            }
        }
        
        private void Shoot()
        {
            int index = -1;

            for (int i = 0; i <= bullets.Length - 1; i++)
                if (!bullets[i].isAlive)
                    index = i;

            if (index > -1)
            {
               // Audio.PlayEnemyShoot1();
                shootTime = 0;

                bullets[index].isAlive = true;

                if (FacingLeft())
                    bullets[index].direction = -1;
                else 
                    bullets[index].direction = 1;

                bullets[index].X = GetCollisionRect().Center.X;
                bullets[index].Y = GetCollisionRect().Center.Y - 3;
            }

        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive && player.Hitbox.Intersects(new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width/3, bullet.Height)))
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
                    spriteBatch.Draw(bullet, new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width/3, bullet.Height),
                        new Rectangle(bullets[i].frame * bullet.Width /3, 0, bullet.Width/3, bullet.Height), Color.White, 0f, new Vector2(), 
                        bullets[i].SpriteDirection(), 0f);

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

        public override void DetectTileCollision(Tile tile)
        {
            if (tile.type == 1)
            {
                if (GetCollisionRect().Intersects(tile.Bottom))
                {
                    stopUpMovement = true;
                    collideWithTile = true;
                }
                if (GetCollisionRect().Intersects(tile.Top))
                {
                    stopDownMovement = true;
                    collideWithTile = true;
                }
            }
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 5, location.Y + 3, drawRect.Width - 10, drawRect.Height - 6);
        }
    }
}
