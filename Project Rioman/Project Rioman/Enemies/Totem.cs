using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Totem : AbstractEnemy
    {

        private int numberOfTotems = 4;

        private int fire = 0;
        private int bubble = 1;
        private int water = 2;
        private int light = 3;
        private int rock = 4;

        private TotemBullet fireBullet = new TotemBullet();
        private TotemBullet bubbleBullet = new TotemBullet();
        private TotemBullet waterBullet = new TotemBullet();
        private TotemBullet lightBullet = new TotemBullet();
        private TotemBullet rockBullet = new TotemBullet();

        private TotemBullet[] bullets;
        private TotemPart[] totems;

        //----------------------------------------------------------------------------------------------
        //TOTEM BULLET
        //----------------------------------------------------------------------------------------------

        struct TotemBullet
        {
            Texture2D sprite;
            int type;
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;
            public int damage;
            private int speed;
            private int frame;
            private double frameTime;

            public void Init(Texture2D bulletSprite, int bulletType, int bulletDamage, int bulletSpeed)
            {
                sprite = bulletSprite;
                type = bulletType;
                damage = bulletDamage;
                speed = bulletSpeed;
            }

            public Rectangle CollisionRect()
            {
                return new Rectangle(X, Y, sprite.Width / 3, sprite.Height);
            }

            public void Update(double deltaTime, Viewport viewport)
            {
                if (isAlive)
                {
                    frameTime += deltaTime;
                    if (frameTime > 0.15)
                    {
                        frameTime = 0;
                        frame++;
                        if (frame > 2)
                            frame = 0;
                    }

                    X += direction * speed;

                    if (X < -20 || X > viewport.Width + 20)
                        isAlive = false;
                }
            }

            public void SetAlive(int posX, int posY, SpriteEffects dir)
            {
                isAlive = true;
                frame = 0;
                frameTime = 0;
                X = posX;
                Y = posY;

                direction = (dir == SpriteEffects.None ? -1 : 1);
            }

            public TotemBullet Clone()
            {
                TotemBullet bullet = new TotemBullet();
                bullet.Init(sprite, type, damage, speed);
                return bullet;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (isAlive)
                {
                    SpriteEffects dir = (direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                    spriteBatch.Draw(sprite, new Rectangle(X, Y, sprite.Width/3, sprite.Height),
                        new Rectangle(frame * sprite.Width/3, 0, sprite.Width/3, sprite.Height), 
                        Color.White, 0f, new Vector2(0, sprite.Height/2), dir, 0);

                }
            }
        }

        //----------------------------------------------------------------------------------------------
        //TOTEM PART
        //----------------------------------------------------------------------------------------------

        struct TotemPart
        {
            public bool isAlive;
            public bool justDied;
            public bool shoot;
            public int X;
            public int Y;
            private int width;
            private int height;
            public int type;
            private int blinkFrames;
            private int health;
            private int moveDown;
            private double timeBetweenShots;
            private double shootTime;

            public void Init(int totemType, int posX, int posY, Texture2D sprite, double randomTime)
            {
                isAlive = true;
                blinkFrames = 0;
                health = 2;

                shootTime = randomTime;

                type = totemType;
                X = posX;
                Y = posY;

                width = sprite.Width / 5;
                height = sprite.Height;

                if (type == 0)
                    timeBetweenShots = 3;
                if (type == 1)
                    timeBetweenShots = 6.6;
                if (type == 2)
                    timeBetweenShots = 3.5;
                if (type == 3)
                    timeBetweenShots = 2;
                if (type == 4)
                    timeBetweenShots = 8;
            }

            public void MoveDown()
            {
                if (isAlive)
                    moveDown += height;
            }

            public void Update(double deltaTime)
            {
                if (isAlive)
                {
                    if (moveDown > 0)
                    {
                        moveDown -= 2;
                        Y += 2;
                    }

                    shootTime += deltaTime;
                    if (shootTime > timeBetweenShots)
                    {
                        shootTime = 0;
                        shoot = true;
                    }
                }
            }

            public void CheckHit(Bullet[] rioBullets)
            {
                if (isAlive)
                {
                    for (int i = 0; i <= rioBullets.Length - 1; i++)
                    {
                        if (rioBullets[i].isAlive && rioBullets[i].location.Intersects(CollisionRect()))
                        {
                            blinkFrames = 2;
                            health -= rioBullets[i].TakeDamage();
                            if (health <= 0)
                            {
                                isAlive = false;
                                justDied = true;
                            }
                        }
                    }
                }
            }

            public Rectangle CollisionRect()
            {
                return new Rectangle(X + 6, Y, width - 12, height);
            }

            public void Draw(Texture2D sprite, SpriteBatch spriteBatch, SpriteEffects direction)
            {
                if (isAlive && blinkFrames <= 0)
                    spriteBatch.Draw(sprite, new Rectangle(X, Y, width, height),
                        new Rectangle(type * width, 0, width, height), Color.White,
                        0f, new Vector2(), direction, 0);

                if (blinkFrames > 0)
                    blinkFrames--;
            }

        }

        //----------------------------------------------------------------------------------------------
        //TOTEM LOGIC
        //----------------------------------------------------------------------------------------------

        public Totem(int type, int r, int c) : base(type, r, c)
        {

            Texture2D[] sprites = EnemyAttributes.GetSprites(type);
            sprite = sprites[0];
            SetupTotems(sprites);

            SubReset();
        }

        protected override void SubReset() {
            isInvincible = true;

            drawRect = new Rectangle(0, 0, sprite.Width / 5, sprite.Height);

            location.Y -= sprite.Height * numberOfTotems;

            totems = new TotemPart[numberOfTotems];
            bullets = new TotemBullet[numberOfTotems * 2];

            for (int i = 0; i <= numberOfTotems - 1; i++)
                totems[i].Init(new Random().Next(0, 5),
                    location.X, location.Y + (numberOfTotems - i - 1) * drawRect.Height,
                    sprite, new Random().Next(0, 100) / 10.0);
        }


        private void SetupTotems(Texture2D[] sprites)
        {
            fireBullet.Init(sprites[fire + 1], fire, 3, 5);
            bubbleBullet.Init(sprites[bubble + 1], bubble, 5, 3);
            waterBullet.Init(sprites[water + 1], water, 3, 5);
            lightBullet.Init(sprites[light + 1], light, 1, 8);
            rockBullet.Init(sprites[rock + 1], rock, 4, 8);
        }


        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            bool aliveFlag = false;
            for (int i = 0; i <= numberOfTotems - 1; i++)
                if (totems[i].isAlive)
                    aliveFlag = true;

            if (!aliveFlag)
                isAlive = false;

            for (int i = 0; i <= bullets.Length - 1; i++)
                bullets[i].Update(deltaTime, viewport);

            if (isAlive)
            {
                for (int i = 0; i <= numberOfTotems - 1; i++)
                {
                    if (totems[i].justDied)
                    {
                        totems[i].justDied = false;
                        int counter = i + 1;
                        while (counter < numberOfTotems)
                        {
                            totems[counter].MoveDown();
                            counter++;
                        }
                    }

                    if (totems[i].shoot)
                    {
                        totems[i].shoot = false;

                        int index = -1;
                        for (int j = 0; j <= bullets.Length - 1; j++)
                        {
                            if (!bullets[j].isAlive)
                                index = j;
                        }
                        if (index > -1)
                        {
                            bullets[index] = CreateBullet(totems[i].type);
                            bullets[index].SetAlive(location.X + drawRect.Width / 2, totems[i].Y + drawRect.Height / 2, direction);
                        }

                    }

                    totems[i].Update(deltaTime);
                }
            }
        }

        private TotemBullet CreateBullet(int type)
        {

            if (type == fire)
                return fireBullet.Clone();
            if (type == bubble)
                return bubbleBullet.Clone();
            if (type == water)
                return waterBullet.Clone();
            if (type == light)
                return lightBullet.Clone();

            else return rockBullet.Clone();

        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= numberOfTotems - 1; i++)
                totems[i].Draw(sprite, spriteBatch, direction);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
                bullets[i].Draw(spriteBatch);
        }

        public override Rectangle GetCollisionRect()
        {
            int maxY = -1000;
            int minY = 1000;

            for (int i = 0; i <= numberOfTotems - 1; i++)
            {
                if (totems[i].isAlive && totems[i].Y > maxY)
                    maxY = totems[i].Y;
                else if (totems[i].isAlive && totems[i].Y < minY)
                    minY = totems[i].Y;
            }

            return new Rectangle(location.X, minY, drawRect.Width, maxY - minY + drawRect.Height);

        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            for (int i = 0; i <= numberOfTotems - 1; i++)
                totems[i].CheckHit(rioBullets);

            for (int i = 0; i <= bullets.Length - 1; i++) {
                if (bullets[i].isAlive && bullets[i].CollisionRect().Intersects(player.Hitbox))
                {
                    player.Hit(bullets[i].damage);
                    bullets[i].isAlive = false;
                }
            }


        }

        protected override void SubMove(int x, int y)
        {
            for (int i = 0; i <= numberOfTotems - 1; i++)
            {
                totems[i].X += x;
                totems[i].Y += y;
            }
            for (int i = 0; i <= bullets.Length - 1; i++)
            {
                bullets[i].X += x;
                bullets[i].Y += y;
            }
        }

        public override void DetectTileCollision(Tile tile)
        {
            //do nothing
        }
    }
}
