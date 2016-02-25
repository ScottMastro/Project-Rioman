using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Project_Rioman
{
    static class Animation
    {
        public static Rectangle GetSourceRect(int width, int height)
        {
            Rectangle rect;
            rect = new Rectangle(0, 0, width, height);
            return rect;
        }

        public static Rectangle GetSourceRect(int numberofframes, ref int currentframe, int width, int height, ref bool reverse)
        {
            Rectangle rect;

            if (numberofframes == currentframe)
                reverse = true;
            else if (currentframe == 1)
                reverse = false;

            if (reverse)
                currentframe--;
            else
                currentframe++;

            rect = new Rectangle(width / numberofframes * (currentframe - 1), 0, width / numberofframes, height);

            return rect;
        }
    }
}