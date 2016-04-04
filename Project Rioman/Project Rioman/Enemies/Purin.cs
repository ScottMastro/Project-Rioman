using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Purin : AbstractEnemy
    {
        Texture2D purinMove;
        Texture2D purinJump;
        Texture2D bullet;

        private enum State { walking, jumping, falling };
        private State state;

        private double jumpTime;
        private double fallTime;

        private bool stopLeftMovement;
        private bool stopRightMovement;

        private int frame;
        private int frameDir;
        private double frameTime;

        private bool isGroundBelow;

        //out of 1000
        private const int JUMP_PROB = 20;

        private const int BULLET_SPEED = 10;
        private const int BULLET_DAMAGE = 3;


        struct PurinBullet
        {
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;

            public int frame;
            public double frameTime;

            public void Update(double deltaTime)
            {
                frameTime += deltaTime;
                if (frameTime > 0.1)
                {
                    frameTime = 0;
                    frame++;
                    if (frame > 3)
                        frame = 0;
                }
            }

            public Rectangle DrawRect(Texture2D bullet)
            {
                return new Rectangle(frame * bullet.Width / 4, 0, bullet.Width / 4, bullet.Height);
            }

            public void Reset()
            {
                isAlive = false;
                X = 0;
                Y = 0;
                direction = -1;
            }
        }

        private PurinBullet[] bullets = new PurinBullet[6];


        public Purin(int type, int r, int c) : base(type, r, c)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            purinMove = sprites[0];
            bullet = sprites[1];
            purinJump = sprites[2];

            sprite = purinMove;
            drawRect = new Rectangle(0, 0, sprite.Width / 3, sprite.Height);
            frame = 0;
            frameDir = 1;
            location.Y -= drawRect.Height;
            Walk();

            isGroundBelow = true;

            stopLeftMovement = false;
            stopRightMovement = false;

        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                if (IsWalking())
                    UpdateWalk(deltaTime);

                if (IsJumping())
                    UpdateJump(deltaTime);

                if (IsFalling())
                    UpdateFall(deltaTime);

                CheckHit(player, rioBullets);


                Console.WriteLine(isGroundBelow);

                if (!isGroundBelow && !IsJumping())
                    Fall();

                isGroundBelow = false;

            }

            UpdateBullets(deltaTime, viewport);
        }

        private void UpdateWalk(double deltaTime)
        {
            if (r.Next(1000) < JUMP_PROB)
                Jump();

            if (FacingLeft())
                MoveThis(-1, 0);
            else
                MoveThis(1, 0);

            frameTime += deltaTime;

            if (frameTime > 0.15)
            {
                frameTime = 0;

                frame += frameDir;
                if (frame < 0)
                {
                    frame = 1;
                    frameDir = 1;
                }
                else if (frame == 0)
                    frameDir = 1;
                else if (frame >= 2)
                    frameDir = -1;

                drawRect = new Rectangle(sprite.Width / 3 * frame, 0, sprite.Width / 3, sprite.Height);

            }

        }

        private void UpdateJump(double deltaTime)
        {
            jumpTime += deltaTime;

            if (FacingLeft())
                MoveThis(-1, 0);
            else
                MoveThis(1, 0);


            MoveThis(0, -Convert.ToInt32(10 - jumpTime * 22));

            if (jumpTime > 0.3)
                Fall();

            frameTime += deltaTime;

            if (frameTime > 0.15)
            {
                frameTime = 0;
                frame++;

                if (frame > 1)
                    frame = 0;
                drawRect = new Rectangle(sprite.Width / 2 * frame, 0, sprite.Width / 2, sprite.Height);

            }
        }


        private void UpdateFall(double deltaTime)
        {
            fallTime += deltaTime;

            if (FacingLeft())
                MoveThis(-1, 0);
            else
                MoveThis(1, 0);

            if (fallTime * 30 > 10)
                MoveThis(0, 10);
            else
                MoveThis(0, Convert.ToInt32(fallTime * 30));

        }

        private void UpdateBullets(double deltaTime, Viewport viewport)
        {
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
                bullets[index].isAlive = true;

                if (FacingLeft())
                    bullets[index].direction = -1;
                else
                    bullets[index].direction = 1;

                bullets[index].X = GetCollisionRect().Center.X;
                bullets[index].Y = GetCollisionRect().Top;

            }
        }


        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive && player.Hitbox.Intersects(new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width/4, bullet.Height)))
                {
                    player.Hit(BULLET_DAMAGE);
                    bullets[i].isAlive = false;
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

        private void MoveThis(int x, int y)
        {
            if (x > 0 && !stopRightMovement)
                location.X += x;
            if (x < 0 && !stopLeftMovement)
                location.X += x;

            location.Y += y;
        }

        private void Walk()
        {
            frame = 0;
            frameTime = 0;
            sprite = purinMove;
            state = State.walking;
            drawRect = new Rectangle(sprite.Width / 3 * frame, 0, sprite.Width / 3, sprite.Height);
        }

        private void Jump()
        {
            Shoot();

            frame = 0;
            frameTime = 0;
            sprite = purinJump;
            state = State.jumping;
            jumpTime = 0;
            drawRect = new Rectangle(sprite.Width / 2 * frame, 0, sprite.Width / 2, sprite.Height);

        }


        private void Fall()
        {
            if (frame > 1)
                frame = 1;

            if (!IsFalling())
            {
                Shoot();
                sprite = purinJump;
                state = State.falling;
                fallTime = 0;
                drawRect = new Rectangle(sprite.Width / 2 * frame, 0, sprite.Width / 2, sprite.Height);
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
                    spriteBatch.Draw(bullet, new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width/4, bullet.Height),
                        bullets[i].DrawRect(bullet), Color.White);
            }
        }


        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X, location.Y + 6, drawRect.Width, drawRect.Height - 2);
        }

        private Rectangle Left() { return new Rectangle(location.X, location.Y + 10, 10, drawRect.Height/4); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width -10, location.Y + 10, 10, drawRect.Height / 4); }
        private Rectangle Top() { return new Rectangle(location.X, location.Y - 5, drawRect.Width, 10); }

        public override void DetectTileCollision(Tile tile)
        {
            if (tile.type == 1 || tile.type == 3 && tile.isTop)
            {
                if (GetCollisionRect().Intersects(tile.Floor))
                    GroundCollision(tile.location.Y);
            }

            if (tile.type == 1 ||  tile.type == 4)
            {
                if (Top().Intersects(tile.Bottom))
                    BottomCollision();
                if (Right().Intersects(tile.Left))
                    LeftCollision();
                if (Left().Intersects(tile.Right))
                    RightCollision();
            }
        }

        private void GroundCollision(int groundTop)
        {
            if (IsFalling())
            {
                Walk();
                location.Y = groundTop - drawRect.Height;
                stopLeftMovement = false;
                stopRightMovement = false;
            }

            isGroundBelow = true;

        }

        private void BottomCollision()
        {
            Fall();
        }

        private void LeftCollision()
        {
            stopRightMovement = true;
            direction = SpriteEffects.None;
        }

        private void RightCollision()
        {
            stopLeftMovement = true;
            direction = SpriteEffects.FlipHorizontally;

        }

        private bool IsJumping() { return state == State.jumping; }
        private bool IsFalling() { return state == State.falling; }
        private bool IsWalking() { return state == State.walking; }

    }
}
