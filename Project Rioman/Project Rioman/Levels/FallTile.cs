using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project_Rioman
{
    //type = 8
    class FallTile : AbstractTile
    {

        private bool falling;
        private const int FALL_SPEED = 1;

        public FallTile(int ID, int x, int y) : base(ID, x, y)
        {
        }

        protected sealed override void SubReset()
        {
            falling = false;
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (player.Feet.Intersects(Top))
                falling = true;
            else
                falling = false;


            if (type == 8 && falling)
            {
                location.Y += FALL_SPEED;
                player.MoveWithGround(0, FALL_SPEED, type);
            }
          
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
