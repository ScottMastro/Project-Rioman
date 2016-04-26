using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    abstract class AbstractBoss
    {

        protected string uniqueID;
        private Level level;

        protected int type;
        protected Rectangle location;
        protected Rectangle originalLocation;

        protected AbstractPickup powerUp;

        protected SpriteEffects direction;
        protected Texture2D sprite;
        protected Rectangle drawRect;

        protected int blinkFrames;

        protected Texture2D killExplosion;
        protected const int EXPLOSION_SPEED = 35;

        protected double killTime;

        protected bool isAlive;
        protected bool wasAlive;

        protected int health;
        protected const int TOUCH_DAMAGE = 4;


        public AbstractBoss(int type, int x, int y)
        {
            this.type = type;

            originalLocation = new Rectangle(x * Constant.TILE_SIZE, (y + 1) * Constant.TILE_SIZE, 0, 0);
            killExplosion = BossAttributes.GetKillSprite();
            blinkFrames = 0;

            Reset();
        }

        public void Reset()
        {
            location.X = originalLocation.X;
            location.Y = originalLocation.Y;
            isAlive = true;
            direction = SpriteEffects.None;
            health = Constant.MAX_HEALTH;
            killTime = 0;

            Guid guid = Guid.NewGuid();
            uniqueID = guid.ToString();

            SubReset();
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
                Die();
        }

        public void Die()
        {
            isAlive = false;

            int x = GetCollisionRect().Center.X;
            int y = GetCollisionRect().Center.Y;

        }

        public void SetAlive(Rioman player)
        {
            Reset();
            isAlive = true;

            if (player.Hitbox.Center.X < location.X)
                direction = SpriteEffects.None;
            else
                direction = SpriteEffects.FlipHorizontally;

        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }


        public void Update(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                SubUpdate(player, rioBullets, deltaTime, viewport);
                CheckHit(player, rioBullets);
            }

            if (!isAlive && health <= 0)
            {
                killTime += deltaTime;
                if (wasAlive)
                    Audio.PlayKillBoss();
            }

            wasAlive = isAlive;
        }

        protected void CheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            if (isAlive)
            {
                if (GetCollisionRect().Intersects(player.Hitbox))
                    player.Hit(TOUCH_DAMAGE);

                for (int i = 0; i <= rioBullets.Length - 1; i++)
                    if (rioBullets[i] != null)
                    {
                        if (rioBullets[i].Hits(GetCollisionRect()))
                        {
                            int damage = rioBullets[i].TakeDamage(uniqueID);
                            if (damage > 0)
                            {
                                TakeDamage(damage);
                                blinkFrames = 2;
                            }

                        }
                    }
            }

            SubCheckHit(player, rioBullets);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            SubDrawOther(spriteBatch);
            if (isAlive && blinkFrames <= 0)
                SubDrawBoss(spriteBatch);
            else if (blinkFrames > 0)
                blinkFrames--;


            if (killTime < 5 && !isAlive && health <= 0)
            {

                for (int i = 0; i < 4; i++)
                    DrawExplosion(spriteBatch, EXPLOSION_SPEED / 8, MathHelper.TwoPi / 4f * i);

                for (int i = 0; i < 6; i++)
                    DrawExplosion(spriteBatch, EXPLOSION_SPEED / 2, MathHelper.TwoPi / 6f * i);

                for (int i = 0; i < 8; i++)
                    DrawExplosion(spriteBatch, EXPLOSION_SPEED, MathHelper.TwoPi / 8f * i);

            }
        }

        private void DrawExplosion(SpriteBatch spriteBatch, int speed, float angle)
        {
            int frame = (int)Math.Floor(killTime);

            int x = GetCollisionRect().Center.X;
            int y = GetCollisionRect().Center.Y;
            int width = killExplosion.Width / 5;
            int height = killExplosion.Height;
            int move = (int)(killTime * speed);

            Rectangle killDrawRect = new Rectangle(frame * width, 0, width, height);
            Rectangle killRect = new Rectangle(x, y - move, width, height);
            Vector2 origin = new Vector2(width / 2, height / 2 + move * 3);

            float alpha = Math.Min(1f, 5f - (float)killTime);

            spriteBatch.Draw(killExplosion, killRect, killDrawRect, Color.White * alpha,
                angle, origin, SpriteEffects.None, 0);

        }

        protected abstract void SubReset();
        protected abstract void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport);
        protected abstract void SubCheckHit(Rioman player, AbstractBullet[] rioBullets);
        protected abstract void SubDrawBoss(SpriteBatch spriteBatch);
        protected abstract void SubDrawOther(SpriteBatch spriteBatch);

        public abstract Rectangle GetCollisionRect();

        public abstract void DetectTileCollision(Tile tile);

        public bool FacingLeft()
        {
            return direction == SpriteEffects.None;
        }

        public AbstractPickup GetPowerUp()
        {
            return powerUp;
        }

        public string GetID() { return uniqueID; }
        public bool IsAlive() { return isAlive; }

    }
}
