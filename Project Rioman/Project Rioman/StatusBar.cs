﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Project_Rioman
{
    static class StatusBar
    {

        private static Texture2D bar;
        private static Texture2D healthpoint;
        private static int health;
        private static double increasetime;
        private static int healthIncreaseAmount;
        private static int ammoIncreaseAmount;

        private static bool drawBossHealth;
        private static int bossHealth;
        private static Color bosshealthcolour;

        public static void LoadHealth(ContentManager content)
        {
            bar = content.Load<Texture2D>("Video\\pause\\bar");
            healthpoint = content.Load<Texture2D>("Video\\pause\\health");
            health = Constant.MAX_HEALTH;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bar, new Vector2(50, 52), Color.White);

            for (int i = 1; i <= health; i++)
                spriteBatch.Draw(healthpoint, new Vector2(50, 50 + bar.Height -
                    (healthpoint.Height - 2) * i), Color.White);

            if (drawBossHealth)
            {
                spriteBatch.Draw(bar, new Vector2(75, 52), Color.White);

                for (int i = 1; i <= bossHealth; i++)
                    spriteBatch.Draw(healthpoint, new Vector2(75, 50 + bar.Height -
                        (healthpoint.Height - 2) * i), bosshealthcolour);
            }


            spriteBatch.Draw(bar, new Vector2(72, 52), Color.White);

            Texture2D ammoPoint = Weapons.GetAmmoPoint();

            for (int i = 1; i <= Weapons.GetAmmo(); i++)
                spriteBatch.Draw(ammoPoint, new Rectangle(88, 50 + bar.Height - (ammoPoint.Width - 2) * i, ammoPoint.Width, ammoPoint.Height), 
                    new Rectangle(0,0, ammoPoint.Width, ammoPoint.Height), Color.White, MathHelper.PiOver2, new Vector2(), SpriteEffects.None, 0);
        }

        public static void BusyUpdate(double deltaTime)
        {
            increasetime += deltaTime;

            if (increasetime > 0.05)
            {
                increasetime = 0;

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
                else if (ammoIncreaseAmount > 0)
                {
                    ammoIncreaseAmount--;

                    if(Weapons.GetAmmo() < Constant.MAX_AMMO)
                    {
                        Weapons.AddAmmo(1);
                        Audio.PlayRestorePoint();
                    }
                }
                
            }
        }

        public static void BossHealth(Color healthcolour)
        {
            bosshealthcolour = healthcolour;
            drawBossHealth = true;
        }

        public static int BossHealth()
        {
            int done = 2;
            bossHealth++;

            if (bossHealth >= Constant.MAX_HEALTH)
                done = 3;

            return done;

        }

        public static void SetDrawBossHealth(bool b) { drawBossHealth = b; }
        public static bool GetDrawBossHealth() { return drawBossHealth; }
        public static void AdjustBossHealth(int x) { bossHealth += x; }
        public static int GetBossHealth() { return bossHealth; }


        public static void IncreaseHealth(int amount) {
            if (health < Constant.MAX_HEALTH)
            {
                healthIncreaseAmount = amount;
                increasetime = 0;
            }
        }

        public static void IncreaseAmmo(int amount)
        {
            if (Weapons.GetAmmo() < Constant.MAX_AMMO)
            {
                ammoIncreaseAmount = amount;
                increasetime = 0;

            }
        }
        public static void AdjustHealth(int amount) { health += amount; }
        public static int GetHealth() { return health; }
        public static void SetHealth(int x) { health = x; }

        public static bool IsBusy() { return healthIncreaseAmount > 0 || ammoIncreaseAmount > 0; }
    }
}