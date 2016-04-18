using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    public static class BulletAttributes
    {

        private static Dictionary<int, int> damage;
        private static Dictionary<int, Texture2D[]> sprites;


        public static void LoadContent(ContentManager content)
        {
            DamageInit();
            SpritesInit(content);
        }


        private static void SpritesInit(ContentManager c)
        {
            sprites = new Dictionary<int, Texture2D[]>();

            sprites.Add(Constant.RIOBULLET, LoadSprites(c, new string[] { "bullet"}));
           // sprites.Add(Constant.GEOBULLET, LoadSprites(c, new string[] {}));
            sprites.Add(Constant.AURORABULLET, LoadSprites(c, new string[] {"aurorabullet" }));
           // sprites.Add(Constant.LURKERBULLET, LoadSprites(c, new string[] { }));
            sprites.Add(Constant.INFERNOBULLET, LoadSprites(c, new string[] {"infernosword", "infernobullet"}));
          //  sprites.Add(Constant.DUELBULLET, LoadSprites(c, new string[] {}));
           // sprites.Add(Constant.POSTERBULLET, LoadSprites(c, new string[] {}));
           // sprites.Add(Constant.TOXICBULLET, LoadSprites(c, new string[] {}));
           // sprites.Add(Constant.CLOVERBULLET, LoadSprites(c, new string[] {}));
           // sprites.Add(Constant.BUNNYBULLET, LoadSprites(c, new string[] {}));
          
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
        //    damage.Add(Constant.GEOBULLET, 4);
        //    damage.Add(Constant.AURORABULLET, 0);
         //   damage.Add(Constant.LURKERBULLET, 2);
            damage.Add(Constant.INFERNOBULLET, 6);
         //   damage.Add(Constant.DUELBULLET, Constant.MAX_HEALTH);
         //   damage.Add(Constant.POSTERBULLET, 6);
         //   damage.Add(Constant.TOXICBULLET, 5);
         //   damage.Add(Constant.CLOVERBULLET, 3);
         //   damage.Add(Constant.BUNNYBULLET, 4);

        }

        public static int GetDamageAttribute(int type)
        {
            int value;
            damage.TryGetValue(type, out value);

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
