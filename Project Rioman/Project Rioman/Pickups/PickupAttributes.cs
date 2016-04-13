using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    public static class PickupAttributes
    {

        private static Dictionary<int, Texture2D> sprites;

        public static void LoadContent(ContentManager content)
        {
            SpritesInit(content);
        }

        private static void SpritesInit(ContentManager content)
        {
            sprites = new Dictionary<int, Texture2D>();

            for (int i = 1; i <= 9; i++)
                sprites.Add(i, content.Load<Texture2D>("Video\\pickups\\wpickup" + i.ToString());

            sprites.Add(10, content.Load<Texture2D>("Video\\pickups\\etank"));
            sprites.Add(11, content.Load<Texture2D>("Video\\pickups\\mtank"));
            sprites.Add(12, content.Load<Texture2D>("Video\\pickups\\wtank"));

            sprites.Add(13, content.Load<Texture2D>("Video\\pickups\\life"));
            sprites.Add(14, content.Load<Texture2D>("Video\\pickups\\bighealth"));
            sprites.Add(15, content.Load<Texture2D>("Video\\pickups\\smallhealth"));
            sprites.Add(16, content.Load<Texture2D>("Video\\pickups\\bigweapon"));
            sprites.Add(17, content.Load<Texture2D>("Video\\pickups\\smallweapon"));
        }

        public static Texture2D GetSprite(int type)
        {
            Texture2D s;
            sprites.TryGetValue(type, out s);

            return s;
        }

    }
