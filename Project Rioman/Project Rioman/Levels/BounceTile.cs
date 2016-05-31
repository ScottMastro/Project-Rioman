using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Project_Rioman
{
    //type = 11
    class BounceTile : AbstractTile
    {
        private int bounceHeight;

        public BounceTile(int ID, int x, int y, int bounceHeight) : base(ID, x, y)
        {
            this.bounceHeight = bounceHeight;
        }

        protected sealed override void SubReset()
        {
        }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (player.Feet.Intersects(BounceRect()) && !player.IsBouncing())
                player.Bounce(bounceHeight);
        }

        private Rectangle BounceRect()
        {
            return new Rectangle(location.X, location.Y, Constant.TILE_SIZE * 3, Constant.TILE_SIZE / 2);
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