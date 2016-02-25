using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Project_Rioman
{
    public static class Opening
    {
        public static Texture2D title;
        static Texture2D text1;
        static Texture2D text2;
        public static Texture2D activetext;
        public static Color fade;

        static public void LoadOpening(ContentManager content)
        {
            title = content.Load<Texture2D>("Video\\opening\\RioManlogo");
            text1 = content.Load<Texture2D>("Video\\opening\\text1");
            text2 = content.Load<Texture2D>("Video\\opening\\text2");

            activetext = text1;

            fade = new Color(255, 255, 255, 0);
        }

        static public void FadeIn()
        {
            if (fade.A < 254)
                fade.A += 2;
        }

        static public void ChangeText(bool one)
        {
            if (!one && activetext == text1 || one && activetext == text2)
                Audio.selection.Play(0.5f, 0f, 0f);

            activetext = text2;

            if (one)
                 activetext = text1;    

        }

        static public int ChangeGameStatus()
        {
            int status;

            if (activetext == text1)
                status = 1;
            else
                status = 2;

            return status;
        }
    }
}