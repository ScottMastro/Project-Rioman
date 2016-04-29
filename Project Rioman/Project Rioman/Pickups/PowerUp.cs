using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Project_Rioman
{
    class PowerUp : AbstractPickup
    {
        private int type;

        private Texture2D textBackground;
        private Texture2D textWeapon;
        private Texture2D textGet;
        private Texture2D rioman;
        private Texture2D riomanClothes;

        private bool slideOut;
        private double getTime;
        private double resetTime;

        private int flickerReset;
        private int currentFlicker;
        private bool flicker;


        private double aliveTime;
        private bool canPickup;

        public PowerUp(int type, int x, int y) : base(type, x, y)
        {
            this.type = type;
                
            sprite = PickupAttributes.GetSprite(type);
            textBackground = PickupAttributes.GetSprite(99);
            textGet = PickupAttributes.GetSprite(100);
            textWeapon = PickupAttributes.GetSprite(100 + type);
            rioman = PickupAttributes.GetRiomanSprite();
            riomanClothes = PickupAttributes.GetRiomanClothesSprite();

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location.Y -= 100;
            location.Width = drawRect.Width;
            location.Height = drawRect.Height;

            Console.WriteLine(location.X);
            Console.WriteLine(location.Y);

            canPickup = false;
            aliveTime = 0;
            resetTime = 0;
            slideOut = false;

            flicker = false;
            flickerReset = 16;
            currentFlicker = 16;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, location, drawRect, Color.White);
            else
            {
                int slideX = Math.Min(700, (int)(getTime * 400));
                if (slideX >= 700)
                    slideOut = true;

                Rectangle backgroundRect = new Rectangle(slideX - textBackground.Width, 200, textBackground.Width, textBackground.Height);
                Rectangle weaponRect = new Rectangle(slideX - textWeapon.Width - 160 + textWeapon.Width / 2,
                     220, textWeapon.Width, textWeapon.Height);
                Rectangle getRect = new Rectangle(slideX - textGet.Width - 100, 270, textGet.Width, textGet.Height);
                Rectangle rioRect = new Rectangle(190, 275, rioman.Width, rioman.Height);


                Color rioColour = BulletAttributes.GetRiomanColour(type);

                spriteBatch.Draw(textBackground, backgroundRect, Color.White);
                spriteBatch.Draw(textWeapon, weaponRect, new Rectangle(0, 0, weaponRect.Width, weaponRect.Height), Color.White, 0f,
                    new Vector2(weaponRect.Width / 2, 0), SpriteEffects.None, 0);
                spriteBatch.Draw(textGet, getRect, Color.White);
                if (slideOut)
                {
                    spriteBatch.Draw(rioman, rioRect, Color.White);
                    if (flicker)
                    {
                        spriteBatch.Draw(riomanClothes, rioRect, rioColour);
                    }
                }

            }
        }

        protected override void PickedUp()
        {
            if(canPickup)
                isAlive = false;

        }

        protected override void SubUpdate(double deltaTime)
        {
            if (isAlive && !onGround)
                location.Y += 3;

            if (isAlive && onGround)
                aliveTime += deltaTime;

            if (aliveTime > 2)
                canPickup = true;

            if (!isAlive)
            {
                getTime += deltaTime;
                if (slideOut)
                {
                    if (flickerReset <= 0)
                    {
                        flicker = true;
                        resetTime += deltaTime;
                    }
                    else {
                        currentFlicker--;
                        if (currentFlicker <= 0)
                        {
                            flickerReset--;
                            currentFlicker = flickerReset;
                            flicker = !flicker;
                        }
                    }
                }
            }
        }

        protected override Rectangle GetCollisionRect()
        {
            return location;
        }

        // 0 = not picked up
        // 1 = picked up displaying text
        // 2 = ready to return to selection screen
        public int GetState()
        {
            if (resetTime > 3)
                return 2;
            else if (!isAlive)
                return 1;
            else
                return 0;
        }

    }
}
