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
                    Audio.PlayKillEnemy();
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


            if (killTime <= 0.25 && !isAlive && health <= 0)
            {
                int frame = (int)Math.Floor(killTime / 0.05);
                Rectangle killDrawRect = new Rectangle(frame * killExplosion.Width / 5, 0, killExplosion.Width / 5, killExplosion.Height);
                Rectangle killLocationRect = new Rectangle(location.X + drawRect.Width / 2 - killExplosion.Width / 10,
                    location.Y + drawRect.Height / 2 - killExplosion.Height / 2, killDrawRect.Width, killDrawRect.Height);

                spriteBatch.Draw(killExplosion, killLocationRect, killDrawRect, Color.White);

            }
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
