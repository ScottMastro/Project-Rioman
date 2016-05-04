using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    //type = 7
    class LaserTile : AbstractTile
    {

        private enum Direction { na, up, down, left, right };
        private Direction direction;
        private bool shooting;
        private int range;


        public LaserTile(int ID, int x, int y) : base(ID, x, y)
        {
            animate = false;
        }


        private Direction GetDirection(int tileType, int tileNumber)
        {
            if (tileType != Constant.TILE_LASER)
                return Direction.na;

            if (tileNumber == 152)
                return Direction.right;
            else if (tileNumber == 330)
                return Direction.down;
            else if (tileNumber == 331)
                return Direction.left;
            else if (tileNumber == 332)
                return Direction.up;

            return Direction.na;

        }

        public bool FacingUp() { return direction == Direction.up; }
        public bool FacingDown() { return direction == Direction.down; }
        public bool FacingRight() { return direction == Direction.right; }
        public bool FacingLeft() { return direction == Direction.left; }
        public void SetRange(int x) { range = x; }

        protected override void SubReset()
        {
            direction = GetDirection(Type, tileID);
            shooting = false;
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (!shooting)
            {
                int buffer = 10;

                if (player.Hitbox.Top - buffer < Location.Center.Y &&
                    player.Hitbox.Bottom + buffer > Location.Center.Y)
                {
                    if (FacingRight() && player.Hitbox.Right > Location.Center.X &&
                        player.Hitbox.Left < Location.Center.X + range)
                        shooting = true;

                    else if (FacingLeft() && player.Hitbox.Left < Location.Center.X &&
                        player.Hitbox.Right > Location.Center.X - range)
                        shooting = true;
                }
                else if (player.Hitbox.Left - buffer < Location.Center.X &&
                        player.Hitbox.Right + buffer > Location.Center.X)
                {
                    if (FacingUp() && player.Hitbox.Top < Location.Center.Y &&
                        player.Hitbox.Bottom > Location.Center.Y - range)
                        shooting = true;
                    else if (FacingDown() && player.Hitbox.Bottom > Location.Center.Y &&
                        player.Hitbox.Top < Location.Center.Y + range)
                        shooting = true;
                }
            }
            else
            {
                animationTime = Math.Min(animationTime + deltaTime * 500, range);
            }
        }

        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            if (shooting)
            {
                int dist = (int)animationTime;
                if (FacingLeft())
                {
                    spriteBatch.Draw(frames[1], new Vector2(Location.X - dist, Location.Y), new Rectangle(0, 0,
                        dist, frames[1].Height), Color.White);

                }
            }
        }
    }
}
