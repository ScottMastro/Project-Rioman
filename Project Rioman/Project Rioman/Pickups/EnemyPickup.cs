using Microsoft.Xna.Framework;
using System;


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
                    Health.IncreaseHealth(6);
                else if (type == Constant.BIG_HEALTH)
                    Health.IncreaseHealth(27);
                else if (type == Constant.SMALL_AMM0) { }
                //TODO
                else if (type == Constant.BIG_AMMO) { }
                //TODO
                isAlive = false;
            }

        }
    }
}
