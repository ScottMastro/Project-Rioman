using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Project_Rioman
{
    static class StageSelection
    {
        public static Texture2D background;
        public static Texture2D AuroraMan;
        public static Texture2D BunnyMan;
        public static Texture2D CloverMan;
        public static Texture2D GeoGirl;
        public static Texture2D InfernoMan;
        public static Texture2D LurkerMan;
        public static Texture2D PosterMan;
        public static Texture2D ToxicMan;
        public static Texture2D Mush;
        public static Texture2D selector;
        public static Texture2D static1;
        public static Texture2D static2;
        public static Texture2D static3;
        public static Vector2[,] location = new Vector2[3, 3];
        static int selectorx, selectory = 0;

        public static Texture2D stars;
        public static Vector2[] starsloc = new Vector2[2];
        public static Texture2D[] middle = new Texture2D[5];
        public static Texture2D activemiddle;
        public static Vector2 activemiddleloc;
        public static Texture2D[] intro = new Texture2D[10];
        public static Texture2D activeintro;
        public static Vector2 activeintroloc;
        public static double elapsedtime;


        static public void LoadStageSelection(ContentManager content, Viewport viewportrect)
        {
            background = content.Load<Texture2D>("Video\\stageselection\\ScreenSelectBackground");
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
                middle[i] = content.Load<Texture2D>("Video\\stageselection\\middlepart" + (i + 1).ToString());
            }
            activemiddle = middle[0];
            activemiddleloc = new Vector2(0, 0);

            for (int j = 1; j <= 9; j++)
            {
                intro[j] = content.Load<Texture2D>("Video\\stageselection\\intro" + j.ToString());
            }
            activeintro = intro[1];
            activeintroloc = new Vector2(-200, 0);

            for (int r = 0; r <= 2; r++)
            {
                for (int c = 0; c <= 2; c++)
                {
                    location[r, c] = new Vector2(((viewportrect.Width - background.Width) / 2) + r * 130 + 84,
                    ((viewportrect.Height - background.Height) / 2) + c * 128 + 36);
                }
            }
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

            spriteBatch.Draw(selector, location[selectorx, selectory], Color.White);
        }

        static public void MoveSelector(int xdir, int ydir)
        {
            if (selectorx > 0 && xdir < 0 || selectorx < 2 && xdir > 0)
            {
                selectorx += xdir;
                Audio.selection.Play(0.5f, 0f, 0f);
            }

            if (selectory > 0 && ydir < 0 || selectory < 2 && ydir > 0)
            {
                selectory += ydir;
                Audio.selection.Play(0.5f, 0f, 0f);
            }
        }

        static public int SelectLevel()
        {
            int gamestatus = 1;

            if (selectorx == 0)
            {
                if (selectory == 0)
                    gamestatus = 10;
                else if (selectory == 1)
                    gamestatus = 11;
                else if (selectory == 2)
                    gamestatus = 12;
            }
            else if (selectorx == 1)
            {
                if (selectory == 0)
                    gamestatus = 13;
                else if (selectory == 1)
                    gamestatus = 14;
                else if (selectory == 2)
                    gamestatus = 15;
            }
            else if (selectorx == 2)
            {
                if (selectory == 0)
                    gamestatus = 16;
                else if (selectory == 1)
                    gamestatus = 17;
                else if (selectory == 2)
                    gamestatus = 18;
            }

            if (gamestatus != 1)
                gamestatus += 100;

            return gamestatus;
        }

        static public int DoDance(GameTime gameTime, int gamestatus, Viewport viewportrect)
        {
            elapsedtime += gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedtime > 0.4)
                activemiddle = middle[4];
            else if (elapsedtime > 0.3)
                activemiddle = middle[3];
            else if (elapsedtime > 0.2)
                activemiddle = middle[2];
            else if (elapsedtime > 0.1)
                activemiddle = middle[1];

            activemiddleloc = new Vector2(0, (viewportrect.Height - activemiddle.Height) / 2);
            activeintroloc.Y = (activemiddle.Height - activeintro.Height) / 2 + activemiddleloc.Y;

            if (elapsedtime > 0.4 && activeintroloc.X + activeintro.Width / 2 < viewportrect.Width / 2)
            {
                activeintro = intro[gamestatus - 109];
                activeintroloc.X += 4;
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
                gamestatus -= 100;
                activemiddle = middle[0];
                activeintroloc.X = -200;
                elapsedtime = 0;
            }



            return gamestatus;
        }
    }
}