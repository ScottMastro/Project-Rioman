using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace Project_Rioman
{
    static class Save
    {
        static Texture2D sprite;
        static Texture2D save1;
        static Texture2D save2;

        public static bool save;

        public static void LoadContent(ContentManager content)
        {
            save1 = content.Load<Texture2D>("Video\\save\\Save1");
            save2 = content.Load<Texture2D>("Video\\save\\Save2");

            sprite = save1;
        }

        public static void Change(KeyboardState kbs, KeyboardState pkbs)
        {
            if (kbs.IsKeyDown(Keys.Left) && !pkbs.IsKeyDown(Keys.Left) && sprite != save1)
            {
                Yes();
                Audio.selection.Play(0.5f, 0f, 0f);
                save = true;
            }

            if (kbs.IsKeyDown(Keys.Right) && !pkbs.IsKeyDown(Keys.Right) && sprite != save2)
            {
                No();
                Audio.selection.Play(0.5f, 0f, 0f);
                save = false;
            }
        }

        private static void Yes()
        {
            sprite = save1;
        }

        private static void No()
        {
            sprite = save2;
        }

        public static void DrawSaveScreen(SpriteBatch spriteBatch, Viewport viewportrect)
        {
            spriteBatch.Draw(sprite, new Vector2((viewportrect.Width - sprite.Width) / 2,
                (viewportrect.Height - sprite.Height) / 2), Color.White);

        }

    }
}
