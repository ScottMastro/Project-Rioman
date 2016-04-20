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

        private const int BULLET_SPEED = 10;
        private const int BULLET_DAMAGE = 2;

        private bool attacking;
        private double attackTime;
        private double shootTime;

        struct ZarrocBullet
        { 
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;

        }

        private ZarrocBullet[] bullets = new ZarrocBullet[6];

        public ZarrocClone(int type, int x, int y) : base(type, x, y)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            stand = sprites[0];
            bullet = sprites[1];
            attack = sprites[2];

            SubReset();
        }

        protected override void SubReset()
        {
            sprite = stand;
            drawRect = new Rectangle(0, 0, stand.Width, stand.Height);

            attacking = false;
            location.Y -= stand.Height;
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {

            if (isAlive)
            {
                if (!attacking)
                {
                    attackTime += deltaTime;
                    if (attackTime > 2)
                        Attack();

                }
                else
                {
                    attackTime += deltaTime;
                    if (attackTime > 0.35)
                        Stand();

                    shootTime += deltaTime;

                    if (shootTime > 0.15)
                    {
                        shootTime = 0;
                        int index = -1;
                        for (int i = 0; i <= bullets.Length - 1; i++)
                            if (!bullets[i].isAlive)
                                index = i;


                        if (index >= 0)
                        {
                            bullets[index].isAlive = true;
                            bullets[index].direction = (FacingLeft() ? -1 : 1);
                            bullets[index].X = GetCollisionRect().Center.X;
                            bullets[index].Y = GetCollisionRect().Center.Y - 8;

                        }
                    }

                }
            }

            for(int i = 0; i<= bullets.Length -1; i++)
            {
                if (bullets[i].isAlive)
                {
                    bullets[i].X += bullets[i].direction * BULLET_SPEED;

                    if (bullets[i].X < -20 || bullets[i].X > viewport.Width + 20)
                        bullets[i].isAlive = false;

                }
            }
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            if (isAlive)
            {
                for (int i = 0; i <= rioBullets.Length - 1; i++)
                    if (rioBullets[i] != null && rioBullets[i].Hits(GetShieldRect()))
                    {
                        rioBullets[i].TakeDamage(uniqueID);
                        Audio.PlayShieldHit();

                    }
            }

            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i].isAlive && player.Hitbox.Intersects(new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height)))
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
                    spriteBatch.Draw(bullet, new Rectangle(bullets[i].X, bullets[i].Y, bullet.Width, bullet.Height), Color.White);
            }
        }

        private void Stand()
        {
            if (FacingLeft())
                location.X += attack.Width - stand.Width;

            attacking = false;
            attackTime = 0;

            sprite = stand;
            drawRect = new Rectangle(0, 0, stand.Width, stand.Height);
        }

        private void Attack()
        {
            if (FacingLeft())
                location.X -= attack.Width - stand.Width;

            attacking = true;
            attackTime = 0;
            shootTime = 0.15;

            sprite = attack;
            drawRect = new Rectangle(0, 0, attack.Width, attack.Height);
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
            if (FacingLeft() && attacking)
                return new Rectangle(location.X + stand.Width / 2 + (attack.Width - stand.Width),
                    location.Y + 10, stand.Width/2, stand.Height - 10);
            else if(FacingLeft() && !attacking)
                return new Rectangle(location.X + stand.Width / 2, location.Y + 10, stand.Width / 2, stand.Height - 10);
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
