using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project_Rioman
{
    //type = 12
    class MovingTile : AbstractTile
    {
        private enum Direction { horizontal, vertical};
        Direction direction;

        private const int MOVE_SPEED = 3;
        private int leftUpLimit;
        private int rightDownLimit;
        private int moveAmount;
        private int moveDir;

        public MovingTile(int ID, int x, int y, bool horizontal) : base(ID, x, y)
        {

            if (horizontal)
                direction = Direction.horizontal;
            else
                direction = Direction.vertical;

        }

        protected sealed override void SubReset()
        {
            moveAmount = 0;
            moveDir = 1;
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {

            int move = moveDir * MOVE_SPEED;
            moveAmount += move;

            if (IsHorizontal())
                location.X += move;
            else
                location.Y += move;

            if (player.Feet.Intersects(SubFloor()))
            {
                if(IsHorizontal())
                    player.MoveWithGround(move, 0, type);
                else
                    player.MoveWithGround(0, move, type);
            }

            if (moveAmount < leftUpLimit || moveAmount > rightDownLimit)
                moveDir = -moveDir;

        }

        public Rectangle SubFloor() { return new Rectangle(location.X + 12, location.Y, Constant.TILE_SIZE * 3 - 24, 4); }
        public Rectangle SubIgnoreFloor() { return new Rectangle(location.X, location.Y + 16, Constant.TILE_SIZE * 3, 16); }

        public void SetLimits(int limit1, int limit2)
        {
            leftUpLimit = limit1 * Constant.TILE_SIZE;
            rightDownLimit = (limit2 -2) * Constant.TILE_SIZE;
        }

        public bool IsHorizontal()
        {
            return direction == Direction.horizontal;
        }

        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}
