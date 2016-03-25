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

        protected Texture2D sprite;
        protected Rectangle drawRect;

        protected bool isAlive;
        protected int health;
        protected int maxHealth;
        protected int touchDamage;

        public AbstractEnemy(int type, int r, int c)
        {

            this.type = type;

            int y = r * Constant.TILE_SIZE;
            int x = c * Constant.TILE_SIZE;

            location = new Rectangle(x, y, 0, 0);
            originalLocation = location;

            health = EnemyAttributes.GetMaxHealthAttribute(type);
            touchDamage = EnemyAttributes.GetDamageAttribute(type);

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


    }
}
