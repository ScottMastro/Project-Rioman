using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Project_Rioman
{
    static class Health
    {
        private static Texture2D healthbar;
        private static Texture2D healthpoint;
        private static int health;
        private static double increasetime;
        private static int increaseAmount;

        private static bool drawBossHealth;
        private static int bossHealth;
        private static Color bosshealthcolour;

        public static void LoadHealth(ContentManager content)
        {
            healthbar = content.Load<Texture2D>("Video\\pause\\bar");
            healthpoint = content.Load<Texture2D>("Video\\pause\\health");
            health = Constant.MAX_HEALTH;
        }

        public static void DrawHealth(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthbar, new Vector2(50, 52), Color.White);

            for (int i = 1; i <= health; i++)
                spriteBatch.Draw(healthpoint, new Vector2(50, 50 + healthbar.Height -
                    (healthpoint.Height - 2) * i), Color.White);

            if (drawBossHealth)
            {
                spriteBatch.Draw(healthbar, new Vector2(75, 52), Color.White);

                for (int i = 1; i <= bossHealth; i++)
                    spriteBatch.Draw(healthpoint, new Vector2(75, 50 + healthbar.Height -
                        (healthpoint.Height - 2) * i), bosshealthcolour);
            }
        }

        public static void UpdateHealth(double deltaTime)
        {
            increasetime += deltaTime;

            if (increasetime > 0.05)
            {
                increasetime = 0;
                increaseAmount--;

                if (health < Constant.MAX_HEALTH)
                {
                    health++;
                    Audio.heal.Play();
                }
                else
                    increaseAmount = 0;
                
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
                increaseAmount = amount;
                increasetime = 0;
            }
        }
        public static void AdjustHealth(int amount) { health += amount; }
        public static int GetHealth() { return health; }
        public static void SetHealth(int x) { health = x; }
        public static bool HealthIncreasing() { return increaseAmount > 0; }
    }
}