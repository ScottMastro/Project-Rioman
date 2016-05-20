using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project_Rioman
{
    class PosterMan : AbstractBoss
    {

        private Texture2D defaultSprite;
        private Texture2D jumpSprite;
        private Texture2D runSprite;
        private Texture2D shootSprite;
        private Texture2D bullet1;
        private Texture2D bullet2;

        private const int BULLET1_DAMAGE = 7;
        private const int BULLET2_DAMAGE = 6;

        //out of 1000
        private const int JUMP_PROB = 6;

        private const int NUM_JUMP_BULLETS = 10;


        private int shotsSinceLastJump;

        private double jumpTime = 0;
        private double shootTime;
        private double betweenShotTime;
        private double standTime;
        private double animateTime;
        private int animateDir;
        private int frame;

        private Point netMovement;
        private const int JUMP_HEIGHT = 200;
        private const int MAX_LEFT = -260;
        private const int MAX_RIGHT = 210;

        private const int MOVE_SPEED = 6;

        private Bullet[] bullets;

        private Random r; 

        struct Bullet
        {
            //type 1 = stand
            //type 2 = jump

            public bool isAlive;
            public int type;
            public int x;
            public int y;
            private int speedX;
            private int speedY;
            private Texture2D sprite;
            private double frameTime;
            private double aliveTime;
            private int frame;
            private int direction;
            private float rotation;

            public void MakeBullet(Texture2D sprite, int type, int x, int y, int speedX, int speedY, float rotation, bool facingLeft)
            {
                frame = 0;
                frameTime = 0;
                aliveTime = 0;

                isAlive = true;
                this.sprite = sprite;
                this.type = type;
                this.x = x;
                this.y = y;
                this.speedX = speedX;
                this.speedY = speedY;
                this.rotation = rotation;
                direction = facingLeft ? -1 : 1;
            }

            public void Update(Viewport viewport, Rioman player, double deltaTime)
            {
                if (isAlive)
                {
                    aliveTime += deltaTime;

                    if (x < 0 - sprite.Height || x > viewport.Width ||
                        y < 0 - sprite.Width / 2 || y > viewport.Height)
                        isAlive = false;

                    if (type == 1)
                    {

                        frameTime += deltaTime;
                        if (frameTime > 0.15)
                        {
                            frameTime = 0;
                            frame++;
                            if (frame > 2)
                                frame = 0;
                        }
                    }

                    x += speedX * direction;

                    if (type == 2)
                    {
                        y += speedY;

                    }
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (isAlive)
                {
                    int boxWidth = (type == 1) ? sprite.Width / 3 : sprite.Width;
                    float size = (type == 1) ? (float)Math.Min(aliveTime * 4, 1) : 1f;
                    Rectangle drawRect = new Rectangle(frame * boxWidth, 0, boxWidth, sprite.Height);

                    spriteBatch.Draw(sprite, new Vector2(x, y), drawRect, Color.White, rotation, new Vector2(), size,
                        direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

                }
            }

            public bool Hits(Rectangle hitbox)
            {
                if (isAlive)
                {
                    int boxWidth = (type == 1) ? sprite.Width / 3 : sprite.Width;

                    if (new Rectangle(x, y, boxWidth, sprite.Height).Intersects(hitbox))
                    {
                        isAlive = false;
                        return true;
                    }
                }
                return false;
            }
        }


        public PosterMan(int x, int y) : base(Constant.POSTERMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.POSTERMAN);
            defaultSprite = sprites[1];
            jumpSprite = sprites[2];
            runSprite = sprites[3];
            bullet1 = sprites[4];
            bullet2 = sprites[5];


            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);

            Reset();
        }

        protected override void SubReset()
        {
            bullets = new Bullet[20];
            animateDir = 1;
            frame = 0;

            betweenShotTime = 0;
            jumpTime = 0;
            shootTime = 0;
            animateTime = 0;
            standTime = 0;
            netMovement = new Point(0, 0);
            r = new Random();
            shotsSinceLastJump = 0;

            state = State.standing;
            shooting = false;
            sprite = defaultSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {

            for (int i = 0; i <= bullets.Length - 1; i++)
                bullets[i].Update(viewport, player, deltaTime);

            if (IsStanding())
            {

                if (shooting)
                {
                    drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
                    shootTime += deltaTime;
                    betweenShotTime += deltaTime;

                    if(betweenShotTime > 0.21)
                    {
                        betweenShotTime = 0;
                        ShootBullet(1, 0f);
                    }

                    if (shootTime > 1)
                        shooting = false;


                }
                else
                {
                    standTime += deltaTime;
                    if (standTime > 0.4)
                        Run();
                }
            }

            if (IsJumping())
            {
                if(-netMovement.Y < JUMP_HEIGHT)
                    MoveThis(0, -MOVE_SPEED);
                else
                {
                    drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);


                    shootTime += deltaTime;
                    betweenShotTime += deltaTime;

                    if (shootTime > 1.5)
                        Fall();

                    if(betweenShotTime > 1.5 / NUM_JUMP_BULLETS)
                    {
                        betweenShotTime = 0;

                        int bullet = (int)Math.Round(shootTime / (1.5 / NUM_JUMP_BULLETS)) - 1;
                        float rotation = ((float)bullet / NUM_JUMP_BULLETS) * MathHelper.Pi;
                        ShootBullet(2, rotation + MathHelper.PiOver4);

                    }

                }

            }

            if (IsFalling())
            {
                if (netMovement.Y > 0)
                    Stand(player);
                else
                    MoveThis(0, MOVE_SPEED);
            }

                if (IsRunning())
            {

                jumpTime += deltaTime;

                if (netMovement.X <= 3 && netMovement.X >= -3 && shotsSinceLastJump >= 2)
                    Jump(player);
                else if (jumpTime > 1.5 && r.Next(1000) < JUMP_PROB)
                    Jump(player);
                
                else if (FacingLeft())
                {
                    MoveThis(-MOVE_SPEED, 0);
                    if (netMovement.X < MAX_LEFT)
                    {
                        Stand(player);
                        Shoot();
                    }
                }
                else
                {
                    MoveThis(MOVE_SPEED, 0);

                    if (netMovement.X > MAX_RIGHT)
                    {
                        Stand(player);
                        Shoot();
                    }

                }


                animateTime += deltaTime;
                if (animateTime > 0.1)
                {
                    animateTime = 0;
                    frame += animateDir;

                    if (frame >= 2)
                        animateDir = -1;
                    if (frame <= 0)
                        animateDir = 1;

                    drawRect = new Rectangle(frame * sprite.Width / 3, 0, sprite.Width / 3, sprite.Height);
                }

            }

        }

        private void Shoot()
        {
            if (!IsJumping())
                shotsSinceLastJump++;

            shooting = true;
            shootTime = 0;
            betweenShotTime = 0;
        }

        private void Jump(Rioman player)
        {
            FacePlayer(player);
            shooting = false;
            state = State.jumping;
            jumpTime = 0;
            shootTime = 0;

            sprite = jumpSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            shotsSinceLastJump = 0;

        }

        private void Fall()
        {
            shooting = false;
            state = State.falling;
            shootTime = 0;

            sprite = jumpSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
        }


        private void Stand(Rioman player)
        {
            MoveThis(0, -netMovement.Y);
            FacePlayer(player);
            standTime = 0;
            state = State.standing;
            shooting = false;
            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        private void Run()
        {
            if (netMovement.X >= 0)
                direction = SpriteEffects.None;
            else
                direction = SpriteEffects.FlipHorizontally;

            animateDir = 1;
            animateTime = 0;
            frame = 0;
            state = State.running;
            shooting = false;
            sprite = runSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 3, sprite.Height);

        }

        private void ShootBullet(int type, float rotation)
        {
            int index1 = -1;
            int index2 = -1;

            for (int i = 0; i<= bullets.Length-1; i++)
            {
                if(index1 != -1)
                {
                    if (!bullets[i].isAlive)
                    {
                        index2 = i;
                        break;
                    }
                }
                else if (!bullets[i].isAlive)
                    index1 = i;
                
            }

            if (index1 < 0)
                return;

            if (type == 1)
                bullets[index1].MakeBullet(bullet1, type, location.Center.X, location.Center.Y - 14, 8, 0, 0f, true);
            if (type == 2)
            {

                int speedX = (int)(Math.Cos(rotation) * 8);
                int speedY = (int)(Math.Sin(rotation) * 8);

                bullets[index1].MakeBullet(bullet2, type, location.Center.X, location.Center.Y, speedX, speedY, rotation, false);

            }

            if (index2 < 0)
                return;

            if (type == 1)
                bullets[index2].MakeBullet(bullet1, type, location.Center.X, location.Center.Y - 14, 8, 0, 0f, false);

        }


        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {

            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (bullets[i].Hits(player.Hitbox))
                {
                    if (bullets[i].type == 1)
                        player.Hit(BULLET1_DAMAGE);
                    if (bullets[i].type == 2)
                        player.Hit(BULLET2_DAMAGE);

                }
            }
        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            if (isAlive)
                foreach (Bullet bullet in bullets)
                    bullet.Draw(spriteBatch);
        }

        private void MoveThis(int x, int y)
        {
            location.X += x;
            netMovement.X += x;
            location.Y += y;
            netMovement.Y += y;
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 16, location.Y + 10, drawRect.Width - 32, drawRect.Height - 10);
        }

        public override Rectangle Feet()
        {
            return new Rectangle(location.X + 20, location.Y + drawRect.Height - 12, drawRect.Width - 40, 12);
        }

        public override Rectangle Head()
        {
            return new Rectangle(location.X + 30, location.Y + 10, drawRect.Width - 64, 16);
        }

        public override Rectangle Left()
        {
            return new Rectangle();
        }

        public override Rectangle Right()
        {
            return new Rectangle();
        }

        public override void DetectTileCollision(AbstractTile tile)
        {
            //do nothing
        }
    }
}
