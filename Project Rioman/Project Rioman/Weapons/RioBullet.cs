using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class RioBullet : AbstractBullet
    {

        public RioBullet(Texture2D bullet)
        {
            damage = 1;
            sprite = bullet;
            isAlive = false;
            location = new Rectangle(0, 0, bullet.Width, bullet.Height);
        }

        public override void BulletSpawn(int x, int y, SpriteEffects dir)
        {
            location.X = x;
            location.Y = y;

            if (dir == SpriteEffects.None)
                direction = 1;
            else
                direction = -1;

            isAlive = true;
            Audio.PlayShoot();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, location, Color.White);
        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }

        protected override void SubUpdate()
        {
            if (isAlive)
                location.X += 9 * direction;
        }

    }
}