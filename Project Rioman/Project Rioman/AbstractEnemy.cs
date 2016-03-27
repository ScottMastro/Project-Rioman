using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        protected bool isAlive;
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

            location = new Rectangle(x, y, 0, 0);
            originalLocation = location;

            isAlive = false;
            direction = SpriteEffects.None;

            health = EnemyAttributes.GetMaxHealthAttribute(type);
            touchDamage = EnemyAttributes.GetDamageAttribute(type);

            this.r = new Random();
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
                Die();
        }

        public void Die()
        {
            //TODO;
        }

        public abstract void Move(int x, int y);

        public abstract void Update(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract Rectangle GetCollisionRect();

        public abstract void DetectTileCollision(Tile tile);


        public bool FacingLeft()
        {
            return direction == SpriteEffects.None;
        }
    }
}
