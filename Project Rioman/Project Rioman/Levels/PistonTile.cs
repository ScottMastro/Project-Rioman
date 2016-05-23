using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    //type = 10
    class PistonTile : AbstractTile
    {
        private enum State { down, up, neutral };
        private State state;
        private const int SPEED = 8;
        private Rectangle armRect;

        private int moveDist;

        private int maxMoveDist;

        //0 = arm
        //1,2,3 = base
        private Texture2D[] sprites;

        private double pistonTime;


        public PistonTile(int ID, int x, int y) : base(ID, x, y)
        {
            sprites = TileAttributes.GetSprites(-Constant.TILE_PISTON);
        }

        protected override void SubReset()
        {
            location = new Rectangle(originalLocation.X - Constant.TILE_SIZE,
                originalLocation.Y, originalLocation.Width * 3, originalLocation.Height);

            armRect = originalLocation;

            pistonTime = 0;
            moveDist = 0;
            state = State.neutral;
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            armRect.X = location.X + Constant.TILE_SIZE;
            armRect.Y = location.Y - moveDist;

            if (state == State.neutral)
            {
                pistonTime += deltaTime;
                if (pistonTime > 2)
                {
                    state = State.down;
                    pistonTime = 0;
                }
            }
            else if (state == State.down)
            {
                if (!MovePiston(SPEED))
                {
                    pistonTime += deltaTime;
                    if (pistonTime > 0.4)
                    {
                        pistonTime = 0;
                        state = State.up;
                    }
                }
            }
            else if (state == State.up)
            {
                if (!MovePiston(-SPEED))
                    state = State.neutral;
            }

        }

        private bool MovePiston(int amount)
        {
            if ((amount > 0 && moveDist < maxMoveDist) ||
                (amount < 0 && moveDist > 0))
            {
                armRect.Height += amount;
                location.Y += amount;
                moveDist += amount;

                return true;
            }

            return false;

        }

        public void SetCrushPoint(int gridY)
        {
            maxMoveDist = (gridY - this.gridY - 1) * Constant.TILE_SIZE;
        }


        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprites[0], armRect, Color.White);

            spriteBatch.Draw(sprites[1], new Rectangle(location.X, location.Y,
                Constant.TILE_SIZE, Constant.TILE_SIZE), Color.White);
            spriteBatch.Draw(sprites[2], new Rectangle(location.X + Constant.TILE_SIZE, location.Y,
                Constant.TILE_SIZE, Constant.TILE_SIZE), Color.White);
            spriteBatch.Draw(sprites[3], new Rectangle(location.X + Constant.TILE_SIZE * 2, location.Y,
                Constant.TILE_SIZE, Constant.TILE_SIZE), Color.White);

        }

        public bool IsCrushing()
        {
            return (state == State.down);
        }

        public Rectangle CrushAboveRect()
        {
            if (moveDist < 70)
                return new Rectangle(location.X + 16, location.Y - moveDist, location.Width - 32, moveDist);
            else
                return new Rectangle(0, 0, 0, 0);

        }

        public Rectangle ArmLeft()
        {
            return new Rectangle(armRect.X, armRect.Y, armRect.Width - Constant.TILE_SIZE / 2, armRect.Height);
        }
        public Rectangle ArmRight()
        {
            return new Rectangle(armRect.X + Constant.TILE_SIZE / 2, armRect.Y, armRect.Width - Constant.TILE_SIZE / 2, armRect.Height);

        }
    }
}
