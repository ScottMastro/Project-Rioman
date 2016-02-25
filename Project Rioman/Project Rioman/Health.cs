using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    static class Health
    {
        static Texture2D healthbar;
        static Texture2D healthpoint;
        public static int health;
        public static bool healthincrease;
        static double increasetime;
        static int increaseamount;

        public static bool drawbosshealth;
        public static int bosshealth;
        public static Color bosshealthcolour;

        public static int lifeplus;

        public static void LoadHealth(ContentManager content)
        {
            healthbar = content.Load<Texture2D>("Video\\pause\\bar");
            healthpoint = content.Load<Texture2D>("Video\\pause\\health");
            health = 27;
            lifeplus = 0;
        }

        public static void DrawHealth(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthbar, new Vector2(50, 52), Color.White);

            for (int i = 1; i <= health; i++)
                spriteBatch.Draw(healthpoint, new Vector2(50, 50 + healthbar.Height -
                    (healthpoint.Height - 2) * i), Color.White);

            if (drawbosshealth)
            {
                spriteBatch.Draw(healthbar, new Vector2(75, 52), Color.White);

                for (int i = 1; i <= bosshealth; i++)
                    spriteBatch.Draw(healthpoint, new Vector2(75, 50 + healthbar.Height -
                        (healthpoint.Height - 2) * i), bosshealthcolour);
            }
        }

        public static void StartIncreaseHealth(int amount)
        {
            increaseamount = amount;
            healthincrease = true;
            increasetime = 0;
        }

        public static void IncreaseHealth(double elapsedtime)
        {
            increasetime += elapsedtime;

            if (increasetime > 0.05)
            {
                increasetime = 0;
                increaseamount--;

                if (health < 27)
                    health++;

                if (health >= 27 || increaseamount <= 0)
                {
                    healthincrease = false;
                    increaseamount = 0;
                }
            }
        }

        public static void BossHealth(Color healthcolour)
        {
            bosshealthcolour = healthcolour;
            drawbosshealth = true;
        }

        public static int BossHealth()
        {
            int done = 2;
            bosshealth++;

            if (bosshealth >= 27)
                done = 3;

            return done;

        }
    }
}
