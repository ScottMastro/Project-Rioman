using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    abstract class AbstractEnemy
    {
        protected int type;
        protected Rectangle location;
        protected Rectangle originalLocation;
        protected Rectangle hitbox;


        protected SpriteEffects direction;
        protected Texture2D sprite;
        protected Texture2D debugSquare;

        protected Rectangle drawRect;

        protected Texture2D killExplosion;
        protected double killTime;

        protected bool isAlive;
        protected bool readyToSpawn;

        protected int health;
        protected int maxHealth;
        protected int touchDamage;

        protected Random r;


        public AbstractEnemy(int type, int r, int c)
        {
            debugSquare = EnemyAttributes.GetSprites(-1)[0];

            this.type = type;

            int y = (r+1) * Constant.TILE_SIZE;
            int x = c * Constant.TILE_SIZE;

            originalLocation = new Rectangle(x, y, 0, 0);

            maxHealth = EnemyAttributes.GetMaxHealthAttribute(type);
            touchDamage = EnemyAttributes.GetDamageAttribute(type);
            killExplosion = EnemyAttributes.GetKillSprite();


            this.r = new Random();

            Reset();
        }

        public void Reset()
        {
            location = originalLocation;
            isAlive = false;
            readyToSpawn = true;
            direction = SpriteEffects.None;
            health = maxHealth;
            killTime = 0;
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
        }

        public abstract void Move(int x, int y);

        public void Update(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            SubUpdate(player, rioBullets, deltaTime, viewport);

            if (!isAlive && health <= 0)
                killTime += deltaTime;
            
        }

        protected abstract void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport);

        public void Draw(SpriteBatch spriteBatch)
        {
            SubDraw(spriteBatch);

            if(killTime <= 0.25 && !isAlive && health <= 0)
            {
                int frame = (int) Math.Floor(killTime / 0.05);
                Rectangle killDrawRect = new Rectangle(frame * killExplosion.Width / 5, 0, killExplosion.Width / 5, killExplosion.Height);
                Rectangle killLocationRect = new Rectangle(location.X + drawRect.Width / 2 - killExplosion.Width / 10,
                    location.Y + drawRect.Height / 2 - killExplosion.Height / 2, killDrawRect.Width, killDrawRect.Height);

                spriteBatch.Draw(killExplosion, killLocationRect, killDrawRect, Color.White);

            }
        }


        protected abstract void SubDraw(SpriteBatch spriteBatch);
        public abstract Rectangle GetCollisionRect();

        public abstract void DetectTileCollision(Tile tile);


        public bool FacingLeft()
        {
            return direction == SpriteEffects.None;
        }
    }
}
