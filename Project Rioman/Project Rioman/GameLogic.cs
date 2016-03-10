using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Project_Rioman
{
    class GameLogic
    {

        private Rioman player;
        private Level level;

        private void LoadNewLevel(Rioman rioman, Level lvl)
        {
            player = rioman;
            level = lvl;
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewportRect)
        {

        }
    }
}
