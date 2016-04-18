using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class SpikeBomb : AbstractEnemy
    {
        private Texture2D bullet;
        private Texture2D angledBullet;

        private bool playerClose;
        private bool shooting;
        private const int SHOOT_PLAYER_DISTANCE = 100;
        private const int BULLET_SPEED = 6;
        private const int BULLET_DAMAGE = 7;

        struct Spike
        {
            public Texture2D sprite;
            public bool isAlive;
            public int X;
            public int Y;
            public int moveY;
            public int moveX;
            public float rotation;

        
            public void SetUp(Texture2D spr, int xpos, int ypos, int xmove, int ymove, float rot)
            {
                sprite = spr;
                isAlive = true;
                X = xpos;
                Y = ypos;
                moveX = xmove;
                moveY = ymove;
                rotation = rot;
            }

            public Rectangle LocRect() { return new Rectangle(X, Y, sprite.Width, sprite.Height); }
            public Rectangle DrawRect() { return new Rectangle(0, 0, sprite.Width, sprite.Height); }
            public Vector2 Origin() { return new Vector2(sprite.Width/2, sprite.Height/2); }

        }

        private Spike[] spikes = new Spike[8];

        public SpikeBomb(int type, int x, int y) : base(type, x, y)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);
            sprite = sprites[0];
            bullet = sprites[1];
            angledBullet = sprites[2];

            SubReset();
        }

        protected override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location.X -= drawRect.Width / 2;
            location.Y -= drawRect.Height / 2;

            playerClose = false;
            shooting = false;
            CreateBullets();
        }

        private void CreateBullets()
        {

            int h = drawRect.Height;
            int w = drawRect.Width;

            int x = location.X;
            int y = location.Y;

            spikes[0].SetUp(bullet, x + w / 2, y - bullet.Height / 2 + 2, 0, -2, 0f);
            spikes[1].SetUp(angledBullet, x + w - 4, y - angledBullet.Height / 2 + 8, 1, -1, MathHelper.ToRadians(90));
            spikes[2].SetUp(bullet, x + w + bullet.Height / 2 - 2, y + h / 2, 2, 0, MathHelper.ToRadians(90));
            spikes[3].SetUp(angledBullet, x + w - 4, y + h + angledBullet.Height / 2 - 8, 1, 1, MathHelper.ToRadians(180));
            spikes[4].SetUp(bullet, x + w / 2, y + h + bullet.Height / 2 - 2, 0, 2, MathHelper.ToRadians(180));
            spikes[5].SetUp(angledBullet, x + 4, y + h + angledBullet.Height / 2 - 8, -1, 1, MathHelper.ToRadians(270));
            spikes[6].SetUp(bullet, x - bullet.Height / 2 + 2, y + h / 2, -2, 0, MathHelper.ToRadians(270));
            spikes[7].SetUp(angledBullet, x + 4, y - angledBullet.Height / 2 + 8, -1, -1, 0f);


        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive && !shooting)
            {
                if (Math.Abs(player.Hitbox.Center.X - GetCollisionRect().Center.X) < SHOOT_PLAYER_DISTANCE * 2
                    && Math.Abs(player.Hitbox.Center.Y - GetCollisionRect().Center.Y) < SHOOT_PLAYER_DISTANCE * 2)
                    playerClose = true;
                else
                    playerClose = false;

                if (Math.Abs(player.Hitbox.Center.X - GetCollisionRect().Center.X) < SHOOT_PLAYER_DISTANCE
                    && Math.Abs(player.Hitbox.Center.Y - GetCollisionRect().Center.Y) < SHOOT_PLAYER_DISTANCE)
                    shooting = true;
            }

            UpdateBullets(viewport);
        }

        public void UpdateBullets(Viewport viewport)
        {
            if (shooting)
            {
                for (int i = 0; i <= spikes.Length - 1; i++)
                {
                    if (spikes[i].isAlive)
                    {
                        spikes[i].X += spikes[i].moveX * BULLET_SPEED;
                        spikes[i].Y += spikes[i].moveY * BULLET_SPEED;
                    }


                    if (spikes[i].X < -20 || spikes[i].X > viewport.Width ||
                           spikes[i].Y < -20 || spikes[i].Y > viewport.Height)
                        spikes[i].isAlive = false;
                }
            }
        }


        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 4, location.Y + 4, drawRect.Width - 8, drawRect.Height - 8);

        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            if (shooting)
            {
                for (int i = 0; i <= spikes.Length - 1; i++)
                {
                    if (spikes[i].isAlive && spikes[i].LocRect().Intersects(player.Hitbox))
                    {
                        player.Hit(BULLET_DAMAGE);
                        spikes[i].isAlive = false;
                    }
                }
            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            if (!playerClose)
                drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            else
                drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);

            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White);

        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            if (isAlive || shooting)
            {
                for (int i = 0; i <= spikes.Length - 1; i++)
                {
                    if (spikes[i].isAlive)
                        spriteBatch.Draw(spikes[i].sprite, spikes[i].LocRect(), spikes[i].DrawRect(),
                            Color.White, spikes[i].rotation, spikes[i].Origin(), SpriteEffects.None, 0);

                }
            }
        }

        protected override void SubMove(int x, int y)
        {
            for (int i = 0; i <= spikes.Length - 1; i++)
            {
                spikes[i].X += x;
                spikes[i].Y += y;
            }
        }

        public override void DetectTileCollision(Tile tile)
        {
            //do nothing
        }
    }
}
