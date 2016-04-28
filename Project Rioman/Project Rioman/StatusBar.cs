using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Project_Rioman
{
    static class StatusBar
    {

        private static Texture2D bar;
        private static Texture2D healthPoint;
        private static int health;
        private static double increaseTime;
        private static int healthIncreaseAmount;
        private static int ammoIncreaseAmount;
        private static int bossIncreaseAmount;

        private static bool drawBossHealth;
        private static int bossHealth;

        public static void LoadHealth(ContentManager content)
        {
            bar = content.Load<Texture2D>("Video\\pause\\bar");
            healthPoint = content.Load<Texture2D>("Video\\pause\\health");
            health = Constant.MAX_HEALTH;
        }

        public static void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            spriteBatch.Draw(bar, new Vector2(50, 52), Color.White);

            for (int i = 1; i <= health; i++)
                spriteBatch.Draw(healthPoint, new Vector2(50, 50 + bar.Height -
                    (healthPoint.Height - 2) * i), Color.White);

            if (drawBossHealth)
            {
                int x = viewport.Width - 50 - bar.Width;
                spriteBatch.Draw(bar, new Vector2(x, 52), Color.White);

                for (int i = 1; i <= bossHealth; i++)
                    spriteBatch.Draw(healthPoint, new Vector2(x, 50 + bar.Height -
                        (healthPoint.Height - 2) * i), Color.LightCoral);
            }


            spriteBatch.Draw(bar, new Vector2(72, 52), Color.White);

            Texture2D ammoPoint = Weapons.GetAmmoPoint();

            for (int i = 1; i <= Weapons.GetAmmo(); i++)
                spriteBatch.Draw(ammoPoint, new Rectangle(88, 50 + bar.Height - (ammoPoint.Width - 2) * i, ammoPoint.Width, ammoPoint.Height), 
                    new Rectangle(0,0, ammoPoint.Width, ammoPoint.Height), Color.White, MathHelper.PiOver2, new Vector2(), SpriteEffects.None, 0);
        }

        public static void BusyUpdate(double deltaTime)
        {
            increaseTime += deltaTime;

            if (increaseTime > 0.05)
            {
                increaseTime = 0;

                if (healthIncreaseAmount > 0)
                {
                    healthIncreaseAmount--;

                    if (health < Constant.MAX_HEALTH)
                    {
                        health++;
                        Audio.PlayRestorePoint();
                    }
                    else
                        healthIncreaseAmount = 0;
                }
                if (ammoIncreaseAmount > 0)
                {
                    ammoIncreaseAmount--;

                    if (Weapons.GetAmmo() < Constant.MAX_AMMO)
                    {
                        Weapons.AddAmmo(1);
                        Audio.PlayRestorePoint();
                    }
                    else
                        ammoIncreaseAmount = 0;
                }
                if (bossIncreaseAmount > 0)
                {
                    bossIncreaseAmount--;

                    if (bossHealth < Constant.MAX_HEALTH)
                    {
                        bossHealth++;
                        Audio.PlayRestorePoint();
                    }
                    else
                        bossIncreaseAmount = 0;
                }

            }
        }

        public static void StopDrawBossHealth() { drawBossHealth = false; }
        public static void DrawBossHealth() { drawBossHealth = true; }
        public static bool GetDrawBossHealth() { return drawBossHealth; }
        public static void SetBossHealth(int x) { bossHealth = x; }
        public static void IncreaseBossHealth(int amount) { bossIncreaseAmount = amount; }



        public static void IncreaseHealth(int amount) {
            if (health < Constant.MAX_HEALTH)
            {
                healthIncreaseAmount = amount;
                increaseTime = 0;
            }
        }

        public static void IncreaseAmmo(int amount)
        {
            if (Weapons.GetAmmo() < Constant.MAX_AMMO)
            {
                ammoIncreaseAmount = amount;
                increaseTime = 0;

            }
        }
        public static void AdjustHealth(int amount) { health += amount; }

        public static int GetHealth() { return health; }
        public static void SetHealth(int x) { health = x; }

        public static bool IsBusy() { return healthIncreaseAmount > 0 || ammoIncreaseAmount > 0 ||
                bossIncreaseAmount > 0; }
    }
}