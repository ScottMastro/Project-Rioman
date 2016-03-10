using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project_Rioman
{
    static class StageSelection
    {

        private static Texture2D selectBackground;
        private static Texture2D AuroraMan;
        private static Texture2D BunnyMan;
        private static Texture2D CloverMan;
        private static Texture2D GeoGirl;
        private static Texture2D InfernoMan;
        private static Texture2D LurkerMan;
        private static Texture2D PosterMan;
        private static Texture2D ToxicMan;
        private static Texture2D Mush;
        private static Texture2D selector;
        private static Texture2D static1;
        private static Texture2D static2;
        private static Texture2D static3;

        private static Vector2[,] location = new Vector2[3, 3];
        private static int selectorX, selectorY;

        private static Texture2D stars;
        private static Vector2[] starsloc = new Vector2[2];

        private static Texture2D[] band = new Texture2D[5];
        private static int bandFrame;
        private static Vector2 bandLocation;

        private static Texture2D[] characterIntro = new Texture2D[10];
        private static Texture2D currentCharacter;
        private static Vector2 currentCharacterLocation;
        private static double elapsedtime;

        private static bool confirmLock;
        private static KeyboardState prevKeyboardState;

        static public void LoadContent(ContentManager content, Viewport viewportrect)
        {
            selectBackground = content.Load<Texture2D>("Video\\stageselection\\ScreenSelectBackground");
            AuroraMan = content.Load<Texture2D>("Video\\stageselection\\ssAuroraMan");
            BunnyMan = content.Load<Texture2D>("Video\\stageselection\\ssBunnyMan");
            CloverMan = content.Load<Texture2D>("Video\\stageselection\\ssCloverMan");
            GeoGirl = content.Load<Texture2D>("Video\\stageselection\\ssGeoGirl");
            InfernoMan = content.Load<Texture2D>("Video\\stageselection\\ssInfernoMan");
            LurkerMan = content.Load<Texture2D>("Video\\stageselection\\ssLurkerMan");
            PosterMan = content.Load<Texture2D>("Video\\stageselection\\ssPosterMan");
            ToxicMan = content.Load<Texture2D>("Video\\stageselection\\ssToxicMan");
            Mush = content.Load<Texture2D>("Video\\stageselection\\ssMush");
            selector = content.Load<Texture2D>("Video\\stageselection\\selector");
            static1 = content.Load<Texture2D>("Video\\stageselection\\static1");
            static2 = content.Load<Texture2D>("Video\\stageselection\\static2");
            static3 = content.Load<Texture2D>("Video\\stageselection\\static3");

            stars = content.Load<Texture2D>("Video\\stageselection\\stars");
            starsloc[0] = new Vector2(-stars.Width, 0);
            starsloc[1] = new Vector2(0, 0);

            for (int i = 0; i <= 4; i++)
            {
                band[i] = content.Load<Texture2D>("Video\\stageselection\\middlepart" + (i + 1).ToString());
            }
            
            for (int j = 1; j <= 9; j++)
            {
                characterIntro[j] = content.Load<Texture2D>("Video\\stageselection\\intro" + j.ToString());
            }
          

            for (int r = 0; r <= 2; r++)
            {
                for (int c = 0; c <= 2; c++)
                {
                    location[r, c] = new Vector2(((viewportrect.Width - selectBackground.Width) / 2) + r * 130 + 84,
                    ((viewportrect.Height - selectBackground.Height) / 2) + c * 128 + 36);
                }
            }

            Reset();

        }

        static private void Reset()
        {
            prevKeyboardState = new KeyboardState();
            confirmLock = true;
            selectorX = 0; selectorY = 0;

            elapsedtime = 0;
            bandFrame = 0;
            bandLocation = new Vector2(0, 0);

            currentCharacter = characterIntro[1];
            currentCharacterLocation = new Vector2(-200, 0);
        }

        static public void Update()
        {
            HandleInput();
        }

        static public void Draw(SpriteBatch spriteBatch, Viewport viewportRect)
        {

            spriteBatch.Draw(selectBackground, new Vector2((viewportRect.Width - selectBackground.Width) / 2,
                (viewportRect.Height - selectBackground.Height) / 2), Color.White);

            DrawIcons(spriteBatch);
        }

        static public void DrawIcons(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GeoGirl, location[0, 0], Color.White);
            spriteBatch.Draw(InfernoMan, location[1, 0], Color.White);
            spriteBatch.Draw(ToxicMan, location[2, 0], Color.White);
            spriteBatch.Draw(AuroraMan, location[0, 1], Color.White);
            spriteBatch.Draw(Mush, location[1, 1], Color.White);
            spriteBatch.Draw(CloverMan, location[2, 1], Color.White);
            spriteBatch.Draw(LurkerMan, location[0, 2], Color.White);
            spriteBatch.Draw(PosterMan, location[1, 2], Color.White);
            spriteBatch.Draw(BunnyMan, location[2, 2], Color.White);

            spriteBatch.Draw(selector, location[selectorX, selectorY], Color.White);
        }

        static public void HandleInput()
        {

            KeyboardState keyboardState = Keyboard.GetState();

            //check to see if confirm button has been let go since the opening screen
            if (!keyboardState.IsKeyDown(Constant.CONFIRM))
                confirmLock = false;

            int initSelectorX = selectorX;
            int initSelectorY = selectorY;

            if (keyboardState.IsKeyDown(Constant.UP) && !prevKeyboardState.IsKeyDown(Constant.UP))
                selectorY = Math.Max(0, selectorY - 1);
            else if (keyboardState.IsKeyDown(Constant.DOWN) && !prevKeyboardState.IsKeyDown(Constant.DOWN))
                selectorY = Math.Min(2, selectorY + 1);
            if (keyboardState.IsKeyDown(Constant.LEFT) && !prevKeyboardState.IsKeyDown(Constant.LEFT))
                selectorX = Math.Max(0, selectorX - 1);
            else if (keyboardState.IsKeyDown(Constant.RIGHT) && !prevKeyboardState.IsKeyDown(Constant.RIGHT))
                selectorX = Math.Min(2, selectorX + 1);

            if (initSelectorX != selectorX || initSelectorY != selectorY)
                Audio.selection.Play(0.5f, 0f, 0f);

            if (keyboardState.IsKeyDown(Constant.CONFIRM) && !prevKeyboardState.IsKeyDown(Constant.CONFIRM) &&
                !confirmLock)
                GameState.SetState(SelectLevel());

            prevKeyboardState = keyboardState;
        }

        static public int SelectLevel()
        {
            if (selectorX == 0)
            {
                if (selectorY == 0)
                    return Constant.GEOGIRL + 100;
                else if (selectorY == 1)
                    return Constant.AURORAMAN + 100;
                else if (selectorY == 2)
                    return Constant.LURKERMAN + 100;
            }
            else if (selectorX == 1)
            {
                if (selectorY == 0)
                    return Constant.INFERNOMAN + 100;
                else if (selectorY == 1)
                    return Constant.MUSH + 100;
                else if (selectorY == 2)
                    return Constant.POSTERMAN + 100;
            }
            else if (selectorX == 2)
            {
                if (selectorY == 0)
                    return Constant.TOXICMAN + 100;
                else if (selectorY == 1)
                    return Constant.CLOVERMAN + 100;
                else if (selectorY == 2)
                    return Constant.BUNNYMAN + 100;
            }

            return 1;
        }

        static public void DoDance(GameTime gameTime, Viewport viewportrect)
        {
            int gameState = GameState.GetState();

            elapsedtime += gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedtime > 0.4)
                bandFrame = 4;
            else if (elapsedtime > 0.3)
                bandFrame = 3;
            else if (elapsedtime > 0.2)
                bandFrame = 2;
            else if (elapsedtime > 0.1)
                bandFrame = 1;

            bandLocation = new Vector2(0, (viewportrect.Height - band[bandFrame].Height) / 2);
            currentCharacterLocation.Y = (band[bandFrame].Height - currentCharacter.Height) / 2 + bandLocation.Y;

            if (elapsedtime > 0.4 && currentCharacterLocation.X + currentCharacter.Width / 2 < viewportrect.Width / 2)
            {
                currentCharacter = characterIntro[gameState - 109];
                currentCharacterLocation.X += 4;
            }

            starsloc[0].X += 25;
            starsloc[1].X += 25;

            if (starsloc[0].X > 0)
            {
                starsloc[0] = new Vector2(-stars.Width, 0);
                starsloc[1] = new Vector2(0, 0);
            }

            if (elapsedtime > 5)
            {
                GameState.SetState(gameState - 100);
                currentCharacterLocation.X = -200;
                Reset();
            }

        }

        public static void DrawDance(SpriteBatch spriteBatch, Viewport viewportRect)
        {
            spriteBatch.Draw(stars, starsloc[0], Color.White);
            spriteBatch.Draw(stars, starsloc[1], Color.White);
            spriteBatch.Draw(band[bandFrame], bandLocation, Color.White);
            spriteBatch.Draw(currentCharacter, currentCharacterLocation, Color.White);

        }
    }
}