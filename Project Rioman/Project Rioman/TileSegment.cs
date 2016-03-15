using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
   abstract class TileSegment
    {

        protected List<Tile> tiles;
        protected Rectangle locationRect;
        protected int type;

        public void Move(int x, int y)
        {
        //    for (int i = 0; i <= tiles.Count - 1; i++)
      //          tiles[i].Move(x, y);

            locationRect.X += x;
            locationRect.Y += y;
        }

        protected abstract void Construct();

    }
}
