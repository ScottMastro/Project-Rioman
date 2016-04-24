using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    abstract class AbstractEnemy
    {

        protected string uniqueID;
        private Level level;
        
        protected int type;
        protected Rectangle location;
        protected Rectangle originalLocation;
        protected Rectangle netMovement;
        protected Rectangle hitbox;

        protected AbstractPickup droppedItem;

        protected SpriteEffects direction;
        protected Texture2D sprite;
        protected Texture2D debugSquare;

        protected Rectangle drawRect;

        protected int blinkFrames;

        protected Texture2D killExplosion;
        protected double killTime;

        protected bool isAlive;
        protected bool wasAlive;
        protected bool readyToSpawn;
        protected bool isInvincible = false;

        protected int health;
        protected int maxHealth;
        protected int touchDamage;


        public AbstractEnemy(int type, int x, int y)
        {
            debugSquare = EnemyAttributes.GetSprites(-1)[0];

            this.type = type;

            originalLocation = new Rectangle(x * Constant.TILE_SIZE, (y + 1) * Constant.TILE_SIZE, 0, 0);

            maxHealth = EnemyAttributes.GetMaxHealthAttribute(type);
            touchDamage = EnemyAttributes.GetDamageAttribute(type);
            killExplosion = EnemyAttributes.GetKillSprite();

            blinkFrames = 0;

            AbstractReset();
        }

        private void AbstractReset()
        {
            location = originalLocation;
            isAlive = false;
            readyToSpawn = false;
            direction = SpriteEffects.None;
            health = maxHealth;
            killTime = 0;
            droppedItem = null;

            Guid guid = Guid.NewGuid();
            uniqueID = guid.ToString();
        }


        public void Reset(bool fullReset)
        {
            AbstractReset();

            if (fullReset)
            {
                netMovement = new Rectangle(0, 0, 0, 0);
                readyToSpawn = true;
            }
            else
            {
                location.X = originalLocation.X + netMovement.X;
                location.Y = originalLocation.Y + netMovement.Y;
            }

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

            int n = new Random().Next(0, 100) + 1;
            int x = GetCollisionRect().Center.X;
            int y = GetCollisionRect().Center.Y;


            //drop small health
            int dropProb = Constant.HEALTH_DROP_PERCENT_SMALL;
            if (n < dropProb) {
                droppedItem = new EnemyPickup(Constant.SMALL_HEALTH, x, y);
                return;
            }
            
            //drop big health
            dropProb += Constant.HEALTH_DROP_PERCENT_BIG;
            if (n < dropProb) {
                droppedItem = new EnemyPickup(Constant.BIG_HEALTH, x, y);
                return;
            }

            //drop small ammo
            dropProb += Constant.AMMO_DROP_PERCENT_SMALL;
            if (n < dropProb) {
                droppedItem = new EnemyPickup(Constant.SMALL_AMMO, x, y);
                return;
            }

            //drop big ammo
            dropProb += Constant.AMMO_DROP_PERCENT_BIG;
            if (n < dropProb) {
                droppedItem = new EnemyPickup(Constant.BIG_AMMO, x, y);
                return;
            }


        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;

            netMovement.X += x;
            netMovement.Y += y;

            SubMove(x, y);
        }


        public void Update(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (!isAlive && readyToSpawn && !Offscreen(viewport))
            {
                Reset(false);
                isAlive = true;

                if (player.Hitbox.Center.X < location.X)
                    direction = SpriteEffects.None;
                else
                    direction = SpriteEffects.FlipHorizontally;
            }
            else if (isAlive && Offscreen(viewport) && !isInvincible)
                Reset(false);
            else if (!readyToSpawn)
                readyToSpawn = SpawnPointOffscreen(viewport);

            SubUpdate(player, rioBullets, deltaTime, viewport);
            CheckHit(player, rioBullets);

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
                if (GetCollisionRect().Intersects(player.Hitbox) && touchDamage > 0)
                    player.Hit(touchDamage);

                if (!isInvincible)
                {
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
            }

            SubCheckHit(player, rioBullets);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            SubDrawOther(spriteBatch);
            if (isAlive && blinkFrames <= 0)
                SubDrawEnemy(spriteBatch);
            else if(blinkFrames > 0)
                blinkFrames--;


            if (killTime <= 0.25 && !isAlive && health <= 0)
            {
                int frame = (int) Math.Floor(killTime / 0.05);
                Rectangle killDrawRect = new Rectangle(frame * killExplosion.Width / 5, 0, killExplosion.Width / 5, killExplosion.Height);
                Rectangle killLocationRect = new Rectangle(location.X + drawRect.Width / 2 - killExplosion.Width / 10,
                    location.Y + drawRect.Height / 2 - killExplosion.Height / 2, killDrawRect.Width, killDrawRect.Height);

                spriteBatch.Draw(killExplosion, killLocationRect, killDrawRect, Color.White);

            }
        }

        protected abstract void SubReset();
        protected abstract void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport);
        protected abstract void SubCheckHit(Rioman player, AbstractBullet[] rioBullets);
        protected abstract void SubDrawEnemy(SpriteBatch spriteBatch);
        protected abstract void SubDrawOther(SpriteBatch spriteBatch);
        protected abstract void SubMove(int x, int y);

        public abstract Rectangle GetCollisionRect();

        public abstract void DetectTileCollision(Tile tile);

        public bool Offscreen(Viewport viewport)
        {
            int buffer = 50;

            return (location.Right < -buffer || location.Left > viewport.Width + buffer ||
                    location.Bottom < -buffer || location.Top > viewport.Height + buffer);
        }

        private bool SpawnPointOffscreen(Viewport viewport)
        {
            int buffer = 50;

            return (originalLocation.X + netMovement.X < -buffer || originalLocation.X + netMovement.X > viewport.Width + buffer ||
                    originalLocation.Y + netMovement.Y < -buffer || originalLocation.Y + netMovement.Y > viewport.Height + buffer);
        }


        public bool FacingLeft()
        {
            return direction == SpriteEffects.None;
        }

        public AbstractPickup GetDroppedPickup()
        {
            AbstractPickup temp = droppedItem;
            droppedItem = null;

            return temp;
        }

        public string GetID() { return uniqueID; }
        public bool IsAlive() { return isAlive; }
        public bool IsInvincible() { return isInvincible; }

    }
}
