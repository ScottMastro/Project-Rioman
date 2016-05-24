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

        private double shootTime;
        private double pauseTime;
        private const double PAUSE_TIME = 0.5;

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
        protected sealed override void SubReset()
        {
            direction = GetDirection(Type, tileID);
            shooting = false;
            shootTime = 0;
            currentFrame = 1;
            pauseTime = 0;
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (!shooting)
            {
                int buffer = 10;

                if (player.Hitbox.Top - buffer < location.Center.Y &&
                    player.Hitbox.Bottom + buffer > location.Center.Y)
                {
                    if (FacingRight() && player.Hitbox.Right > location.Center.X &&
                        player.Hitbox.Left < location.Center.X + range)
                        shooting = true;

                    else if (FacingLeft() && player.Hitbox.Left < location.Center.X &&
                        player.Hitbox.Right > location.Center.X - range)
                        shooting = true;
                }
                else if (player.Hitbox.Left - buffer < location.Center.X &&
                        player.Hitbox.Right + buffer > location.Center.X)
                {
                    if (FacingUp() && player.Hitbox.Top < location.Center.Y &&
                        player.Hitbox.Bottom > location.Center.Y - range)
                        shooting = true;

                    else if (FacingDown() && player.Hitbox.Bottom > location.Center.Y &&
                        player.Hitbox.Top < location.Center.Y + range)
                        shooting = true;
                }
            }
            else
            {
                if (pauseTime < PAUSE_TIME)
                {
                    pauseTime += deltaTime;
                }
                else {

                    animationTime += deltaTime;
                    if (animationTime > 0.05)
                    {
                        animationTime = 0;
                        currentFrame++;
                        if (currentFrame > numFrames - 1)
                            currentFrame = 1;
                    }

                    shootTime = Math.Min(shootTime + deltaTime * 500, range);
                }
            }
        }
        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            if (shooting)
            {
                Rectangle rect = GetLaserRect();
                SpriteEffects flip = SpriteEffects.None;
                if (FacingLeft()) flip = SpriteEffects.FlipHorizontally;
                if (FacingUp()) flip = SpriteEffects.FlipVertically;

                spriteBatch.Draw(frames[currentFrame], rect, new Rectangle(0, 0, rect.Width, rect.Height),
                    Color.White, 0f, new Vector2(), flip, 1f);
                    
            }
        }

        public bool FacingUp() { return direction == Direction.up; }
        public bool FacingDown() { return direction == Direction.down; }
        public bool FacingRight() { return direction == Direction.right; }
        public bool FacingLeft() { return direction == Direction.left; }
        public void SetRange(int x) { range = x; }

        public Rectangle GetLaserRect()
        {
            if (!shooting)
                return new Rectangle(0, 0, 0, 0);
            else
            {
                int size = (int)shootTime;

                if (FacingLeft())
                    return new Rectangle(location.X - size, location.Y, size, Constant.TILE_SIZE);
                if (FacingRight())
                    return new Rectangle(location.X + Constant.TILE_SIZE, location.Y, size, Constant.TILE_SIZE);
                if (FacingUp())
                    return new Rectangle(location.X, location.Y - size, Constant.TILE_SIZE, size);
                if (FacingDown())
                    return new Rectangle(location.X, location.Y + Constant.TILE_SIZE, Constant.TILE_SIZE, size);

            }

            return new Rectangle(0, 0, 0, 0);
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}
