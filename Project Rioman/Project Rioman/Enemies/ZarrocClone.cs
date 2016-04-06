using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class ZarrocClone : AbstractEnemy
    {

        private Texture2D stand;
        private Texture2D attack;
        private Texture2D bullet;

        private bool attacking;
        private double attackTime;

        struct ZarrocBullet
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

        private ZarrocBullet[] bullets = new ZarrocBullet[3];

        public ZarrocClone(int type, int r, int c) : base(type, r, c)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            stand = sprites[0];
            bullet = sprites[1];
            attack = sprites[2];

            sprite = stand;

            attacking = false;
            location.Y -= stand.Height;


        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            //implement attacking, shooting

            if (isAlive)
            {
                if (player.Hitbox.Left > GetCollisionRect().Right)
                    direction = SpriteEffects.FlipHorizontally;
                else if (player.Hitbox.Right < GetCollisionRect().Left)
                    direction = SpriteEffects.None;

                if (!attacking)
                {
                    attackTime += deltaTime;
                    if (attackTime > 2)
                    {
                        attacking = true;
                        attackTime = 0;
                    }

                }
                else
                {
                    attackTime += deltaTime;
                    if (attackTime > 0.35)
                    {
                        attacking = false;
                        attackTime = 0;
                    }
                }
            }
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            for (int i = 0; i<= rioBullets.Length -1; i++)
                if (rioBullets[i].isAlive && rioBullets[i].location.Intersects(GetShieldRect()))
                {
                    rioBullets[i].isAlive = false;
                    Audio.PlayShieldHit();

                }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            if (attacking)
            {
                sprite = attack;
                drawRect = new Rectangle(0, 0, attack.Width, attack.Height);
            }
            else {
                sprite = stand;
                drawRect = new Rectangle(0, 0, stand.Width, stand.Height);
            }
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
            if (FacingLeft())
                return new Rectangle(location.X + stand.Width / 2, location.Y + 10, stand.Width/2, stand.Height - 10);
            else
                return new Rectangle(location.X, location.Y + 10, stand.Width/2, stand.Height - 10);

        }

        private Rectangle GetShieldRect()
        {
            if (attacking)
                return new Rectangle(0, 0, 0, 0);

            if (FacingLeft())
                return new Rectangle(location.X + 6, location.Y - 10, stand.Width / 2, stand.Height + 15);
            else
                return new Rectangle(location.X + stand.Width/2 - 6, location.Y - 10, stand.Width / 2, stand.Height + 15);

        }

        public override void DetectTileCollision(Tile tile)
        {
            //do nothing
        }
    }
}
