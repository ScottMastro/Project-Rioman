using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class BunnyMan : AbstractBoss
    {
        private Texture2D defaultSprite;
        private Texture2D crouchSprite;
        private Texture2D shootSprite;
        private Texture2D seagullBullet;
        private Texture2D jumpCloud;

        private const int SEAGULL_DAMAGE = 5;
        private const int CLOUD_DAMAGE = 5;

        private const int SEAGULL = 1;
        private const int CLOUD = 2;


        private double crouchTime;
        private double airTime;
        private double standTime;
        private double shootTime;
        private int seagullBulletsShot;
        private bool fallingAlongSide;

        private Bullet[] bullets = new Bullet[6];

        struct Bullet
        {
            //type 1 = seagull
            //type 2 = hurricane

            public bool isAlive;
            public int type;
            public int x;
            public int y;
            private int speed;
            private Texture2D sprite;
            private double frameTime;
            private int frame;
            private int direction;
            private float floatAngle;

            public void MakeBullet(Texture2D sprite, int type, int x, int y, int speed, bool facingLeft)
            {
                frame = 0;
                frameTime = 0;
                floatAngle = 0;

                isAlive = true;
                this.sprite = sprite;
                this.type = type;
                this.x = x;
                this.y = y;
                this.speed = speed;
                direction = facingLeft ? -1 : 1;
            }

            public void Update(Viewport viewport, Rioman player, double deltaTime)
            {
                if (isAlive)
                {
                    if (x < 0 - sprite.Height || x > viewport.Width ||
                        y < 0 - sprite.Width / 2 || y > viewport.Height)
                        isAlive = false;

                    frameTime += deltaTime;
                    if (frameTime > 0.15)
                    {
                        frameTime = 0;
                        frame++;
                        if (frame > 1)
                            frame = 0;
                    }

                    if (type == 1)
                    {
                        floatAngle += MathHelper.WrapAngle((float)deltaTime * 10);
                        y += (int)(Math.Sin(floatAngle) * 3);

                        Rectangle target = player.Hitbox;
                        int centerY = y + sprite.Height / 2;

                        if (target.Top - centerY > 30)
                            y += 3;
                        else if (target.Top - centerY > 3)
                            y += 1;
                        else if (target.Bottom - centerY < 30)
                            y -= 3;
                        else if (target.Bottom - centerY < 3)
                            y -= 1;
                    }
                    else if (type == 2) {
                        speed -= 1;
                        if (speed <= 0)
                            isAlive = false;
                    }

                    x += speed * direction;
                }
            }


            public void Draw(SpriteBatch spriteBatch)
            {
                if (isAlive)
                {
                    Rectangle drawRect = new Rectangle(frame * sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
                    Rectangle locationRect = new Rectangle(x, y, drawRect.Width, drawRect.Height);
                    spriteBatch.Draw(sprite, locationRect, drawRect, Color.White, 0f, new Vector2(),
                        direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

                }
            }

            public bool Hits(Rectangle hitbox)
            {
                if (isAlive)
                {
                    if (new Rectangle(x, y, sprite.Width / 2, sprite.Height).Intersects(hitbox))
                    {
                        isAlive = false;
                        return true;
                    }
                }
                return false;
            }

        }

        public BunnyMan(int x, int y) : base(Constant.BUNNYMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.BUNNYMAN);
            defaultSprite = sprites[1];
            shootSprite = sprites[2];
            crouchSprite = sprites[3];
            seagullBullet = sprites[4];
            jumpCloud = sprites[5];

            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);

            Reset();
        }

        protected override void SubReset()
        {
            bullets = new Bullet[6];

            sprite = defaultSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

            seagullBulletsShot = 0;
            crouchTime = 0;
            airTime = 0;
            shootTime = 0;
            standTime = 0;
            fallingAlongSide = false;
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

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {

            for (int i = 0; i <= bullets.Length - 1; i++)
                bullets[i].Update(viewport, player, deltaTime);

            if (IsStanding())
            {
                standTime += deltaTime;

                if (!shooting && standTime > 0.2)
                    ShootSeagull();
                

                if (standTime > 0.8)
                    Crouch();

            }

            if (shooting)
            {
                shootTime += deltaTime;
                if(shootTime > 0.2 && seagullBulletsShot < 3)
                {
                    shootTime = 0;
                    ShootSeagull();
                }

            }


            if (IsCrouching())
            {
                crouchTime += deltaTime;
                if (crouchTime > 0.6)
                    Jump();
            }

            if (IsJumping())
            {
                if (location.Y > -drawRect.Height)
                    location.Y -= 10;
                else
                    airTime += deltaTime;


                if (airTime > 3)
                {
                    Random r = new Random();

                    //don't land on screen
                    if (r.Next(6) < 1)
                    {
                        if (r.Next(2) == 1)
                            location.X = 0;
                        else
                            location.X = viewport.Width - drawRect.Width - 38;

                        fallingAlongSide = true;
                    }
                    //land on screen
                    else
                        location.X = new Random().Next(Constant.TILE_SIZE * 3,
                            viewport.Width - drawRect.Width - Constant.TILE_SIZE * 3);

                    Fall();
                    FacePlayer(player);

                }
            }

            else if (IsFalling())
            {
                location.Y += 8;

                if (fallingAlongSide && seagullBulletsShot == 0 && location.Y > viewport.Height / 6)
                {
                    shootTime = 0;
                    ShootSeagull();
                }
            }


        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (bullets[i].Hits(player.Hitbox))
                {
                    if (bullets[i].type == SEAGULL)
                        player.Hit(SEAGULL_DAMAGE);
                   if (bullets[i].type == CLOUD)
                        player.Hit(CLOUD_DAMAGE);

                }
            }

        }

        private void Crouch()
        {
            if (!IsJumping())
            {
                shooting = false;
                crouchTime = 0;
                sprite = crouchSprite;
                state = State.crouching;
                drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            }
        }
        private void Jump()
        {
            airTime = 0;
            if (!shooting)
                sprite = defaultSprite;
            else
                sprite = shootSprite;

            state = State.jumping;
            drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width/2, sprite.Height);

            CreateJumpClouds();
        }

        private void CreateJumpClouds()
        {
            int index = GetNextBulletIndex();
            if (index > -1)
            {
                bullets[index].MakeBullet(jumpCloud, CLOUD, GetCollisionRect().Center.X,
                    GetCollisionRect().Bottom - jumpCloud.Height, 20, true);
            }

            index = GetNextBulletIndex();
            if (index > -1)
            {
                bullets[index].MakeBullet(jumpCloud, CLOUD, GetCollisionRect().Center.X,
                    GetCollisionRect().Bottom - jumpCloud.Height, 20, false);
            }
        }


        private void Fall()
        {
            seagullBulletsShot = 0;

            if (!shooting)
                sprite = defaultSprite;
            else
                sprite = shootSprite;

            state = State.falling;
            drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
        }

        private void ShootSeagull()
        {
            int index = GetNextBulletIndex();

            if (index > -1)
            {
                shootTime = 0;
                seagullBulletsShot++;
                sprite = shootSprite;
                shooting = true;
                bullets[index].MakeBullet(seagullBullet, SEAGULL, GetCollisionRect().Center.X,
                     GetCollisionRect().Center.Y, 4, FacingLeft());
            }
        }

        private void Stand(int groundTop)
        {
            seagullBulletsShot = 0;
            fallingAlongSide = false;
            standTime = 0;
            location.Y = groundTop - drawRect.Height;
            Stand();
        }

        private void Stand()
        {
            state = State.standing;
            shooting = false;
            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        private int GetNextBulletIndex()
        {
            int index = -1;
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                if (!bullets[i].isAlive)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                Console.Beep();
            return index;
        }

        public override void DetectTileCollision(Tile tile)
        {
            if (tile.type == 1)
            {
                if (Feet().Intersects(tile.Floor))
                    Stand(tile.location.Y);
            }
        }

        public override Rectangle GetCollisionRect()
        {
            int yShift = IsCrouching() ? 28 : 14;
            return new Rectangle(location.X + 10, location.Y + yShift, drawRect.Width - 20, drawRect.Height - yShift);
        }

        public override Rectangle Feet()
        {
            return new Rectangle(location.X + 10, location.Y + drawRect.Height - 20, drawRect.Width - 20, 20);
        }

        public override Rectangle Head()
        {
            int yShift = IsCrouching() ? 22 : 10;
            return new Rectangle(location.X + 16, location.Y + yShift, drawRect.Width - 32, 26);
        }

        public override Rectangle Left()
        {
            return new Rectangle();
        }

        public override Rectangle Right()
        {
            return new Rectangle();
        }
    }
}
