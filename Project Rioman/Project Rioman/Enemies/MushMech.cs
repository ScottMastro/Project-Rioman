using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class MushMech : AbstractEnemy
    {
        private Texture2D bullet;

        private bool jumping;
        private double jumpTime;

        private const int BULLET_SPEED = 10;
        private int bulletDamage;

        private bool stopLeftMovement;
        private bool stopRightMovement;
        private bool leftColliderIntersect;
        private bool rightColliderIntersect;

        struct MushBullet
        {
            public bool isAlive;
            public bool isAttached;
            public int X;
            public int Y;

        }

        private MushBullet leftBullet = new MushBullet();
        private MushBullet rightBullet = new MushBullet();


        public MushMech(int type, int x, int y) : base(type, x, y)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);
            sprite = sprites[0];
            bullet = sprites[1];

            bulletDamage = 4;
            if (type == Constant.TMUSHMECH)
                bulletDamage = 7;

            SubReset();
        }

        protected override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location.Y -= sprite.Height;

            jumping = false;
            jumpTime = 0;

            stopLeftMovement = false;
            stopRightMovement = false;

            leftColliderIntersect = false;
            rightColliderIntersect = false;

            leftBullet.isAlive = true;
            rightBullet.isAlive = true;
            leftBullet.isAttached = true;
            rightBullet.isAttached = true;
            leftBullet.X = location.X - bullet.Width + 7;
            rightBullet.X = location.X + drawRect.Width - 7;
            leftBullet.Y = location.Y + drawRect.Height / 2;
            rightBullet.Y = location.Y + drawRect.Height / 2;

        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {

            if (isAlive)
            {

                if (!leftColliderIntersect)
                    stopLeftMovement = false;
                if (!rightColliderIntersect)
                    stopRightMovement = false;

                leftColliderIntersect = false;
                rightColliderIntersect = false;


                if (!jumping)
                {
                    if (player.Hitbox.Center.X > GetCollisionRect().Center.X)
                        direction = SpriteEffects.None;
                    else if (player.Hitbox.Center.X < GetCollisionRect().Center.X)
                        direction = SpriteEffects.FlipHorizontally;


                    if (Math.Abs(player.Hitbox.Center.Y - GetCollisionRect().Center.Y) < 50 &&
                        Math.Abs(player.Hitbox.Center.X - GetCollisionRect().Center.X) < 200)
                    {

                        if (FacingLeft() && !stopLeftMovement || !FacingLeft() && !stopRightMovement)
                        {
                            leftBullet.isAttached = false;
                            rightBullet.isAttached = false;

                            Jump();
                        }
                    }
                }
                if (jumping)
                {
                    if (FacingLeft() && !stopLeftMovement)
                        location.X -= 3;
                    else if (!FacingLeft() && !stopRightMovement)
                        location.X += 3;

                    jumpTime += deltaTime;
                    location.Y -= Math.Max(-12, Convert.ToInt32(12 - jumpTime * 24));
                }
            }

            if (leftBullet.isAlive && !leftBullet.isAttached)
                leftBullet.X -= BULLET_SPEED;
            if (rightBullet.isAlive && !rightBullet.isAttached)
                rightBullet.X += BULLET_SPEED;

        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            
            if (leftBullet.isAlive && !leftBullet.isAttached &&
                player.Hitbox.Intersects(new Rectangle(leftBullet.X, leftBullet.Y, bullet.Width, bullet.Height)))
            {
                leftBullet.isAlive = false;
                player.Hit(bulletDamage);
            }

            if(rightBullet.isAlive && !leftBullet.isAttached &&
                player.Hitbox.Intersects(new Rectangle(rightBullet.X, rightBullet.Y, bullet.Width, bullet.Height)))
            {
                rightBullet.isAlive = false;
                player.Hit(bulletDamage);
            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, 0f, new Vector2(), direction, 0);

        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {

            if (leftBullet.isAlive && (isAlive || !isAlive && !leftBullet.isAttached))
                spriteBatch.Draw(bullet, new Rectangle(leftBullet.X, leftBullet.Y, bullet.Width, bullet.Height),
                    new Rectangle(0, 0, bullet.Width, bullet.Height), Color.White, 0f, new Vector2(), SpriteEffects.None, 0);

            if (rightBullet.isAlive && (isAlive || !isAlive && !rightBullet.isAttached))
                spriteBatch.Draw(bullet, new Rectangle(rightBullet.X, rightBullet.Y, bullet.Width, bullet.Height),
                    new Rectangle(0, 0, bullet.Width, bullet.Height), Color.White, 0f, new Vector2(), SpriteEffects.FlipHorizontally, 0);
        }

        protected override void SubMove(int x, int y)
        {
            leftBullet.X += x;
            leftBullet.Y += y;
            rightBullet.X += x;
            rightBullet.Y += y;
        }

        private void Jump()
        {
            jumping = true;
            jumpTime = 0;
            drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
        }

        private void Stand()
        {
            jumping = false;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
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
                if (Head().Intersects(tile.Bottom))
                    BottomCollision();
                if (Right().Intersects(tile.Left))
                    LeftCollision();
                if (Left().Intersects(tile.Right))
                    RightCollision();
            }
        }

        private void GroundCollision(int groundTop)
        {
            if (jumping)
            {
                Stand();
                location.Y = groundTop - sprite.Height;
            }

        }

        private void BottomCollision()
        {
            jumpTime = 0.5;
        }

        private void LeftCollision()
        {
            rightColliderIntersect = true;
            stopRightMovement = true;
        }

        private void RightCollision()
        {
            leftColliderIntersect = true;
            stopLeftMovement = true;
        }
        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 10, location.Y + 8, drawRect.Width - 20, drawRect.Height - 8);
        }

        private Rectangle Left() { return new Rectangle(location.X, location.Y + 10, 10, drawRect.Height /2); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width - 10, location.Y + 10, 10, drawRect.Height /2); }
        private Rectangle Head() { return new Rectangle(location.X + 16, location.Y, drawRect.Width - 32, 10); }
        private Rectangle Feet() { return new Rectangle(location.X + 16, location.Y + drawRect.Height - 10, drawRect.Width - 32, 10); }

    }
}