using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    class Neolucky : AbstractEnemy
    {

        private Texture2D stand;
        private Texture2D jump;
        private Texture2D attack;
        private Texture2D bullet;

        private int frame;

        private double standTime;
        private double jumpTime;
        private double fallTime;
        private double shootTime;

        private bool stopLeftMovement;
        private bool stopRightMovement;


        //out of 1000
        private const int JUMP_PROB = 10;
        private const int SHOOT_PROB = 20;

        private const int BULLET_SPEED = 10;
        private const int BULLET_DAMAGE = 3;


        private enum State { standing, jumping, falling };
        private State state;
        private bool shooting;

        struct NeoBullet
        {
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;

        }

        private NeoBullet[] bullets = new NeoBullet[3];

        public Neolucky(int type, int r, int c) : base(type, r, c)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            stand = sprites[0];
            bullet = sprites[1];
            attack = sprites[2];
            jump = sprites[3];

            SubReset();
        }

        protected override void SubReset()
        {
            shooting = false;
            location.Y -= stand.Height;
            Stand();

            stopLeftMovement = false;
            stopRightMovement = false;
        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport) {

            if (isAlive)
            {
                if (IsStanding())
                    UpdateStand(player, deltaTime);
                
                if (IsJumping())
                    UpdateJump(deltaTime);
                
                if(IsFalling())
                    UpdateFall(deltaTime);

                if (shooting)
                    UpdateShooting(deltaTime);
                else if( new  Random().Next(1000) < SHOOT_PROB)
                    Shoot();


            }


            UpdateBullets(viewport);

        }


        private void UpdateStand(Rioman player, double deltaTime)
        {
            if (player.Hitbox.Left > GetCollisionRect().Right)
                direction = SpriteEffects.FlipHorizontally;
            else if (player.Hitbox.Right < GetCollisionRect().Left)
                direction = SpriteEffects.None;

            if (!shooting)
            {
                standTime += deltaTime;
                if (standTime > 0.2)
                {
                    standTime = 0;
                    if (frame == 1)
                    {
                        frame = 0;
                        drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
                    }
                    else if (frame == 0)
                    {
                        frame++;
                        drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
                    }
                }
            }

            if (new Random().Next(1000) < JUMP_PROB)
                Jump();
        }

        private void UpdateJump(double deltaTime)
        {
            jumpTime += deltaTime;

            if (FacingLeft())
                MoveThis(-1, 0);
            else
                MoveThis(1, 0);


            MoveThis(0, -Convert.ToInt32(10 - jumpTime * 22));

            if (jumpTime > 0.7)
                Fall();
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

        private void UpdateShooting(double deltaTime)
        {
            shootTime += deltaTime;
            if (shootTime > 0.5)
            {
                shooting = false;
                if (IsStanding())
                    Stand();
            }
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {

            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive && player.Hitbox.Intersects(new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height))){
                    player.Hit(BULLET_DAMAGE);
                    bullets[i].isAlive = false;
                }
        }

       
        protected override void SubMove (int x, int y)
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


        //---------------------------------------------------
        //State logic
        //---------------------------------------------------


        private void Stand()
        {
            frame = 0;
            standTime = 0;
            sprite = stand;
            state = State.standing;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
        }

        private void Jump()
        {
            sprite = jump;
            state = State.jumping;
            jumpTime = 0;
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

        }


        private void Fall()
        {
            sprite = jump;
            state = State.falling;
            fallTime = 0;
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
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

        private bool IsJumping() { return state == State.jumping; }
        private bool IsFalling() { return state == State.falling; }
        private bool IsStanding() { return state == State.standing; }

        //---------------------------------------------------
        //Bullet logic
        //---------------------------------------------------

        private void UpdateBullets(Viewport viewport)
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

        private void Shoot()
        {
            int index = -1;

            for (int i = 0; i <= bullets.Length - 1; i++)
                if (!bullets[i].isAlive)
                    index = i;

            if (index > -1)
            {
                Audio.PlayEnemyShoot1();
                shooting = true;
                shootTime = 0;
                if (IsStanding())
                {
                    sprite = attack;
                    drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
                }

                CreateBullet(index);
            }
        }

        private void CreateBullet(int index)
        {
            bullets[index].isAlive = true;

            if (FacingLeft())
            {
                bullets[index].direction = -1;
                bullets[index].X = Left().Left;
            }
            else {
                bullets[index].direction = 1;
                bullets[index].X = Right().Left;
            }

            bullets[index].Y = location.Y + drawRect.Height *2/5;
        }

        //---------------------------------------------------
        //Collision logic
        //---------------------------------------------------

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
            if (IsFalling())
            {
                Stand();
                location.Y = groundTop - sprite.Height;
                stopLeftMovement = false;
                stopRightMovement = false;
            }

        }

        private void BottomCollision()
        {
            Fall();
        }

        private void LeftCollision()
        {
            stopRightMovement = true;
        }

        private void RightCollision()
        {
            stopLeftMovement = true;
        }

        private Rectangle Left() { return new Rectangle(location.X + 10, location.Y, 10, drawRect.Height * 2/3); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width -20, location.Y, 10, drawRect.Height * 2/3); }
        private Rectangle Head() { return new Rectangle(location.X + 20, location.Y + 10, drawRect.Width -40, 10); }
        private Rectangle Feet() { return new Rectangle(location.X + 10, location.Y + drawRect.Height - 10, drawRect.Width - 20, 10); }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 10, location.Y + 5, jump.Width - 20, stand.Height);
        }

    }
}
