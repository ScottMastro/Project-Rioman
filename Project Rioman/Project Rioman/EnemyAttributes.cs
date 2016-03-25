using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    public static class EnemyAttributes
    {

        private static Dictionary<int, int> maxHealth;
        private static Dictionary<int, int> damage;
        private static Dictionary<int, Texture2D[]> sprites;


     
        public static void LoadContent(ContentManager content)
        {
            HealthInit();
            DamageInit();
            SpritesInit(content);
        }


        private static void SpritesInit(ContentManager c)
        {
            sprites = new Dictionary<int, Texture2D[]>();

            sprites.Add(Constant.TOTEM, LoadSprites(c, new string[]
                {"troubling-totem", "ttbullets0", "ttbullets1", "ttbullets2", "ttbullets3"}));
            sprites.Add(Constant.NEOLUCKY, LoadSprites(c, new string[]
                {"neo-luckystand", "neo-luckybullet", "neo-luckyattack", "neo-luckyjump"}));
            sprites.Add(Constant.KRONOS, LoadSprites(c, new string[]
                {"kronos" }));
            sprites.Add(Constant.PURIN, LoadSprites(c, new string[]
                {"Purin", "Purinbullets", "Purinjump" }));
            sprites.Add(Constant.MOUSEHEAD, LoadSprites(c, new string[]
                {"mousehead", "mousebullet", "mousebody", "mousetail"}));
            sprites.Add(Constant.FLIPSIDE, LoadSprites(c, new string[]
                {"flipside"}));
            sprites.Add(Constant.MACEBOT, LoadSprites(c, new string[]
                {"Mace-botball", "Mace-botstring", "Mace-bot"}));
            sprites.Add(Constant.TR05D0R, LoadSprites(c, new string[]
                {"7R06D0R", "7R06D0Rbullet"}));
            sprites.Add(Constant.SPIKEBOMB, LoadSprites(c, new string[]
                {"spikebomb", "spikebullet", "spikebulletangle"}));
            sprites.Add(Constant.MMUSHMECH, LoadSprites(c, new string[]
                {"MaliciousMushMech", "mmmbullet"}));
            sprites.Add(Constant.TMUSHMECH, LoadSprites(c, new string[]
                {"ToxicMushMech", "mmmbullet"}));
            sprites.Add(Constant.CHANCEBOMB, LoadSprites(c, new string[]
                {"ChanceBomb", "explosion"}));
            sprites.Add(Constant.ZARROCCLONE, LoadSprites(c, new string[]
                {"zarroc-Clone", "zcbullet", "zarroc-Clone2"}));
            sprites.Add(Constant.DEUXKAMA, LoadSprites(c, new string[]
                {"deux-kama"}));
            sprites.Add(Constant.SERVERBOT, LoadSprites(c, new string[]
                {"serverbot"}));
            sprites.Add(Constant.MEGAHOPPER, LoadSprites(c, new string[]
                {"Mega-hopper"}));
            sprites.Add(Constant.DOZERBOT, LoadSprites(c, new string[]
                {"dozer-bot", "dbbullets"}));
            sprites.Add(Constant.BLACKY, LoadSprites(c, new string[]
                {"Blacky", "blackbullet"}));
            sprites.Add(Constant.HELLICOPTOR, LoadSprites(c, new string[]
                {"hellicoptor"}));
            sprites.Add(Constant.P1H8R, LoadSprites(c, new string[]
                {"P1-H8R", "P1-H8Rbullets"}));
            sprites.Add(Constant.MACKS, LoadSprites(c, new string[]
                { "Macks", "mkbullets"}));
        }

        private static Texture2D[] LoadSprites(ContentManager content, string[] location)
        {
            List<Texture2D> sprites = new List<Texture2D>();

            for (int i = 0; i <= location.Length - 1; i++)
                sprites.Add(content.Load<Texture2D>("Video\\enemies\\" + location[i]));
            
            return sprites.ToArray();
        }

        private static void HealthInit()
        {
            maxHealth = new Dictionary<int, int>();

            maxHealth.Add(Constant.TOTEM, 2);
            maxHealth.Add(Constant.NEOLUCKY, 10);
            maxHealth.Add(Constant.KRONOS, 10);
            maxHealth.Add(Constant.PURIN, 3);
            maxHealth.Add(Constant.MOUSEHEAD, 25);
            maxHealth.Add(Constant.FLIPSIDE, 0);
            maxHealth.Add(Constant.MACEBOT, 4);
            maxHealth.Add(Constant.TR05D0R, 25);
            maxHealth.Add(Constant.SPIKEBOMB, 5);
            maxHealth.Add(Constant.MMUSHMECH, 5);
            maxHealth.Add(Constant.TMUSHMECH, 6);
            maxHealth.Add(Constant.CHANCEBOMB, 0);
            maxHealth.Add(Constant.ZARROCCLONE, 3);
            maxHealth.Add(Constant.DEUXKAMA, 0);
            maxHealth.Add(Constant.SERVERBOT, 4);
            maxHealth.Add(Constant.MEGAHOPPER, 6);
            maxHealth.Add(Constant.DOZERBOT, 7);
            maxHealth.Add(Constant.BLACKY, 5);
            maxHealth.Add(Constant.HELLICOPTOR, 4);
            maxHealth.Add(Constant.P1H8R, 4);
            maxHealth.Add(Constant.MACKS, 5);
        }
        private static void DamageInit()
        {
            damage = new Dictionary<int, int>();

            damage.Add(Constant.TOTEM, 2);
            damage.Add(Constant.NEOLUCKY, 4);
            damage.Add(Constant.KRONOS, 0);
            damage.Add(Constant.PURIN, 2);
            damage.Add(Constant.MOUSEHEAD, 5);
            damage.Add(Constant.FLIPSIDE, 500);
            damage.Add(Constant.MACEBOT, 6);
            damage.Add(Constant.TR05D0R, 5);
            damage.Add(Constant.SPIKEBOMB, 3);
            damage.Add(Constant.MMUSHMECH, 4);
            damage.Add(Constant.TMUSHMECH, 7);
            damage.Add(Constant.CHANCEBOMB, 2);
            damage.Add(Constant.ZARROCCLONE, 2);
            damage.Add(Constant.DEUXKAMA, 5);
            damage.Add(Constant.SERVERBOT, 4);
            damage.Add(Constant.MEGAHOPPER, 6);
            damage.Add(Constant.DOZERBOT, 500);
            damage.Add(Constant.BLACKY, 3);
            damage.Add(Constant.HELLICOPTOR, 4);
            damage.Add(Constant.P1H8R, 3);
            damage.Add(Constant.MACKS, 4);
        }

        public static int GetDamageAttribute(int type)
        {
            int value;
            damage.TryGetValue(type, out value);

            return value;
        }

        public static int GetMaxHealthAttribute(int type)
        {
            int value;
            maxHealth.TryGetValue(type, out value);

            return value;
        }

        public static Texture2D[] GetSprites(int type)
        {
            Texture2D[] s;
            sprites.TryGetValue(type, out s);

            return s;
        }
    }
}
