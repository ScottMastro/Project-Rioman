﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    public static class BulletAttributes
    {
        private static Dictionary<int, Color> riomanColour;
        private static Dictionary<int, int> weight;
        private static Dictionary<int, int> ammoUse;
        private static Dictionary<int, int> speed;
        private static Dictionary<int, int> damage;
        private static Dictionary<int, Texture2D[]> sprites;


        public static void LoadContent(ContentManager content)
        {
            WeightInit();
            ColourInit();
            AmmoUseInit();
            SpeedInit();
            DamageInit();
            SpritesInit(content);
        }


        private static void SpritesInit(ContentManager c)
        {
            sprites = new Dictionary<int, Texture2D[]>();

            sprites.Add(Constant.RIOBULLET, LoadSprites(c, new string[] { "bullet"}));
            sprites.Add(Constant.GEOBULLET, LoadSprites(c, new string[] {"geobullet"}));
            sprites.Add(Constant.AURORABULLET, LoadSprites(c, new string[] {"aurorabullet" }));
           // sprites.Add(Constant.LURKERBULLET, LoadSprites(c, new string[] { }));
            sprites.Add(Constant.INFERNOBULLET, LoadSprites(c, new string[] {"infernosword", "infernobullet"}));
          //  sprites.Add(Constant.DUELBULLET, LoadSprites(c, new string[] {}));
            sprites.Add(Constant.POSTERBULLET, LoadSprites(c, new string[] {"posterbullet"}));
            sprites.Add(Constant.TOXICBULLET, LoadSprites(c, new string[] {"toxbullet1", "toxbullet2", "toxbullet3", "toxbullet4"}));
            sprites.Add(Constant.CLOVERBULLET, LoadSprites(c, new string[] {"cloverbullet"}));
            sprites.Add(Constant.BUNNYBULLET, LoadSprites(c, new string[] {"bunnybullet"}));

        }

        private static Texture2D[] LoadSprites(ContentManager content, string[] location)
        {
            List<Texture2D> sprites = new List<Texture2D>();

            for (int i = 0; i <= location.Length - 1; i++)
                sprites.Add(content.Load<Texture2D>("Video\\weapons\\" + location[i]));

            return sprites.ToArray();
        }

        private static void DamageInit()
        {
            damage = new Dictionary<int, int>();

            damage.Add(Constant.RIOBULLET, 1);
            damage.Add(Constant.GEOBULLET, 5);
            damage.Add(Constant.AURORABULLET, 2);
            damage.Add(Constant.LURKERBULLET, 0);
            damage.Add(Constant.INFERNOBULLET, 4);
         //   damage.Add(Constant.DUELBULLET, 0);
            damage.Add(Constant.POSTERBULLET, 1);
            damage.Add(Constant.TOXICBULLET, 3);
            damage.Add(Constant.CLOVERBULLET, 4);
            damage.Add(Constant.BUNNYBULLET, 4);

        }

        private static void SpeedInit()
        {
            speed = new Dictionary<int, int>();

            speed.Add(Constant.RIOBULLET, 9);
            speed.Add(Constant.GEOBULLET, 9);
            speed.Add(Constant.AURORABULLET, 12);
            speed.Add(Constant.LURKERBULLET, 0);
            speed.Add(Constant.INFERNOBULLET, 4);
            //   speed.Add(Constant.DUELBULLET, Constant.MAX_HEALTH);
            speed.Add(Constant.POSTERBULLET, 12);
            speed.Add(Constant.TOXICBULLET, 2);
            speed.Add(Constant.CLOVERBULLET, 7);
            speed.Add(Constant.BUNNYBULLET, 4);

        }

        private static void AmmoUseInit()
        {
            ammoUse = new Dictionary<int, int>();

            ammoUse.Add(Constant.RIOBULLET, 0);
            ammoUse.Add(Constant.GEOBULLET, 3);
            ammoUse.Add(Constant.AURORABULLET, 1);
            ammoUse.Add(Constant.LURKERBULLET, 1);
            ammoUse.Add(Constant.INFERNOBULLET, 2);
            //   ammoUse.Add(Constant.DUELBULLET, Constant.MAX_HEALTH);
            ammoUse.Add(Constant.POSTERBULLET, 1);
            ammoUse.Add(Constant.TOXICBULLET, 3);
               ammoUse.Add(Constant.CLOVERBULLET, 4);
            ammoUse.Add(Constant.BUNNYBULLET, 2);

        }

        private static void WeightInit()
        {
            weight = new Dictionary<int, int>();

            weight.Add(Constant.RIOBULLET, 3);
            weight.Add(Constant.GEOBULLET, 6);
            weight.Add(Constant.AURORABULLET, 3);
            weight.Add(Constant.LURKERBULLET, 0);
            weight.Add(Constant.INFERNOBULLET, 3);
            //   weight.Add(Constant.DUELBULLET, Constant.MAX_HEALTH);
            weight.Add(Constant.POSTERBULLET, 2);
            weight.Add(Constant.TOXICBULLET, 3);
            weight.Add(Constant.CLOVERBULLET, 6);
            weight.Add(Constant.BUNNYBULLET, 3);

        }


        private static void ColourInit()
        {
            riomanColour = new Dictionary<int, Color>();

            riomanColour.Add(Constant.RIOBULLET, Color.White * 0f);
            riomanColour.Add(Constant.GEOBULLET, Color.Peru);
            riomanColour.Add(Constant.AURORABULLET, Color.CornflowerBlue);
            riomanColour.Add(Constant.LURKERBULLET, Color.Gray);
            riomanColour.Add(Constant.INFERNOBULLET, Color.Red);
            //   riomanColour.Add(Constant.DUELBULLET, Constant.MAX_HEALTH);
            riomanColour.Add(Constant.POSTERBULLET, Color.CadetBlue);
            riomanColour.Add(Constant.TOXICBULLET, Color.DarkOrchid);
            riomanColour.Add(Constant.CLOVERBULLET, Color.LimeGreen);
            riomanColour.Add(Constant.BUNNYBULLET, Color.HotPink);

        }

        public static int GetDamageAttribute(int type)
        {
            int value;
            damage.TryGetValue(type, out value);

            return value;
        }

        public static int GetSpeedAttribute(int type)
        {
            int value;
            speed.TryGetValue(type, out value);

            return value;
        }

        public static int GetAmmoUse(int type)
        {
            int value;
            ammoUse.TryGetValue(type, out value);

            return value;
        }

        public static int GetWeight(int type)
        {
            int value;
            weight.TryGetValue(type, out value);

            return value;
        }

    public static Texture2D[] GetSprites(int type)
        {
            Texture2D[] s;
            sprites.TryGetValue(type, out s);

            return s;
        }

        public static Color GetRiomanColour(int type)
        {
            Color value;
            riomanColour.TryGetValue(type, out value);

            return value;
        }
    }
}
