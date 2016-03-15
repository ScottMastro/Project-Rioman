using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    class Wall : TileSegment
    {

        private Rectangle leftRect;
        private Rectangle rightRect;
        private Rectangle topRect;
        private Rectangle bottomRect;

        public Wall(List<Tile> t)
        {
            tiles = t;
            Construct();
        }

        protected override void Construct()
        {
            Tile bottom = tiles[0];
            Tile top = tiles[0];

            foreach (Tile t in tiles) {
                if (t.Y > bottom.Y)
                    bottom = t;
                else if (t.Y < top.Y)
                    top = t;
            }

            locationRect = new Rectangle(top.X, top.Y, top.Width, bottom.Y - top.Y);
            leftRect = new Rectangle(top.X, top.Y, top.Width/2, bottom.Y - top.Y);
            rightRect = new Rectangle(top.X + top.Width / 2, top.Y, top.Width / 2, bottom.Y - top.Y);

        }

        public void MoveWall(int x, int y)
        {
            Move(x,y);

            leftRect.X += x;
            leftRect.Y += y;
            rightRect.X += x;
            rightRect.Y += y;

        }

        public Rectangle LeftRect { get { return leftRect; } }
        public Rectangle RightRect { get { return rightRect; } }

    }
}
