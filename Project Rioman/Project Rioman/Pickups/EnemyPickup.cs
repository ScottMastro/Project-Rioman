using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class EnemyPickup : AbstractPickup
    {

        private int frame;
        private double frameTime;

        public EnemyPickup(int type, int x, int y) : base(type, x, y)
        {
            sprite = PickupAttributes.GetSprite(type);
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        private void Reset()
        {
            frame = 0;
            frameTime = 0;
        }

        protected override void SubUpdate(double deltaTime)
        {
            frameTime += deltaTime;
            if (frameTime > 0.2)
            {
                frameTime = 0;

                if (frame == 0)
                    frame = 1;
                else frame = 0;
            }

            drawRect = new Rectangle(frame * sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
        }

        protected override void PickedUp()
        {
            if (isAlive)
            {
                if (type == Constant.SMALL_HEALTH)
                    StatusBar.IncreaseHealth(6);
                else if (type == Constant.BIG_HEALTH)
                    StatusBar.IncreaseHealth(Constant.MAX_HEALTH);
                else if (type == Constant.SMALL_AMMO)
                    StatusBar.IncreaseAmmo(6);
                else if (type == Constant.BIG_AMMO)
                    StatusBar.IncreaseAmmo(Constant.MAX_AMMO);
            
                isAlive = false;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color colour = Color.White;

            if (type == Constant.BIG_AMMO || type == Constant.SMALL_AMMO)
            {
                colour = BulletAttributes.GetRiomanColour(Weapons.GetActiveWeapon());
                if (Weapons.GetActiveWeapon() == Constant.RIOBULLET)
                    colour = Color.Gold;
            }


            if (isAlive)
                spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                    drawRect, colour, 0f, new Vector2(drawRect.Width / 2, drawRect.Height / 2), SpriteEffects.None, 0);
        }
    }
}
