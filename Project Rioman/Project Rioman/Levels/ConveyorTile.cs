using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    //type = 9
    class ConveyorTile : AbstractTile
    {

        private int direction;
        private const int MOVE_SPEED = 1;


        public ConveyorTile(int ID, int x, int y, int dir) : base(ID, x, y)
        {
            direction = dir;
        }

        protected override void SubReset()
        {
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (player.Feet.Intersects(Top))
                player.MoveWithGround(MOVE_SPEED * direction, 0);          
        }


        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            //do nothing
        }
    }
}
