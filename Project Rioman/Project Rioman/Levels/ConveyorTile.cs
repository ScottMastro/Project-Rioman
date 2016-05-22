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
        private const int MOVE_SPEED = 2;


        public ConveyorTile(int ID, int x, int y, int dir) : base(ID, x, y)
        {
            direction = dir;
        }

        protected override void SubReset()
        {
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (player.Feet.Intersects(ConveyorTop()))
                player.MoveWithGround(MOVE_SPEED * direction, 0, type);          
        }

        private Rectangle ConveyorTop()
        {
            return new Rectangle(location.X - 16, location.Y, location.Width + 32, location.Height / 2); 
        }

        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            //do nothing
        }
    }
}
