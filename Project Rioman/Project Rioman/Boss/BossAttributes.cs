using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    public static class BossAttributes
    {
        private static Dictionary<int, Texture2D[]> sprites;
        private static Texture2D killExplosion;


        public static void LoadContent(ContentManager content)
        {
            SpritesInit(content);
        }


        private static void SpritesInit(ContentManager c)
        {
            killExplosion = c.Load<Texture2D>("Video\\bosses\\explode");

            sprites = new Dictionary<int, Texture2D[]>();

            sprites.Add(Constant.BUNNYMAN, LoadSprites(c, new string[]
                {"bunnymanpose", "bunnyman1", "bunnyman2", "bunnyman3", "bunnybullet1", "bunnybullet2" }));

            sprites.Add(Constant.POSTERMAN, LoadSprites(c, new string[]
                {"postermanpose", "posterman1", "posterman2", "posterman3", "bunnybullet1", "bunnybullet2" }));
        }

        private static Texture2D[] LoadSprites(ContentManager content, string[] location)
        {
            List<Texture2D> sprites = new List<Texture2D>();

            for (int i = 0; i <= location.Length - 1; i++)
                sprites.Add(content.Load<Texture2D>("Video\\bosses\\" + location[i]));

            return sprites.ToArray();
        }

        public static Texture2D[] GetSprites(int type)
        {
            Texture2D[] s;
            sprites.TryGetValue(type, out s);

            return s;
        }

        public static Texture2D GetKillSprite()
        {
            return killExplosion;
        }
    }
}
