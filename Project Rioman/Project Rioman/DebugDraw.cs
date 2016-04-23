using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    static class DebugDraw
    {
        private static Texture2D pixel;


        public static void LoadContent(ContentManager content)
        {
            pixel = content.Load<Texture2D>("Video\\debug\\px");
        }

        public static void DrawLine(SpriteBatch spriteBatch, int x1, int y1, int x2, int y2)
        {
            spriteBatch.Draw(pixel, new Rectangle(x1, y1, Math.Max(Math.Abs(x2 - x1), 1), Math.Max(Math.Abs(y2 - y1), 1)),
                null, Color.Magenta, 0f, new Vector2(), SpriteEffects.None, 0.0f);
        }

        public static void DrawLine(SpriteBatch spriteBatch, int x1, int y1, int x2, int y2, Color colour, float transparency, float rotation)
        {
            spriteBatch.Draw(pixel, new Rectangle(x1, y1, Math.Max(Math.Abs(x2 - x1), 1), Math.Max(Math.Abs(y2 - y1), 1)),
                null, colour * transparency, rotation, new Vector2(), SpriteEffects.None, 0.0f);
        }

        public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, float transparency)
        {
            spriteBatch.Draw(pixel, rect, null, Color.Magenta * transparency, 0f, new Vector2(), SpriteEffects.None, 0.0f);
        }

        public static void DrawRect(SpriteBatch spriteBatch, int x, int y, int width, int height)
        {
            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height),
                null, Color.Magenta, 0f, new Vector2(), SpriteEffects.None, 0.0f);
        }

        public static void DrawRect(SpriteBatch spriteBatch, int x, int y, int width, int height, Color colour, float transparency, float rotation)
        {
            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height),
                null, colour * transparency, rotation, new Vector2(), SpriteEffects.None, 0.0f);
        }

    }
}
