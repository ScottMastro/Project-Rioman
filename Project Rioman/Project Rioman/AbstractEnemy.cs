using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    abstract class AbstractEnemy
    {
        private int type;
        private Rectangle location;
        private Rectangle originalLocation;
        private Rectangle hitbox;

        private Texture2D sprite;
        private Rectangle drawRect;

        private bool isAlive;
        private int health;
        private int maxHealth;

        public AbstractEnemy(int typ, int r, int c, ContentManager content)
        {

            type = typ;

            int y = r * Constant.TILE_SIZE;
            int x = c * Constant.TILE_SIZE;

            location = new Rectangle(x, y, 0, 0);
            originalLocation = location;

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
