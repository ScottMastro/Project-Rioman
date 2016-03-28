using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class DozerBot : AbstractEnemy
    {
        public DozerBot(int type, int r, int c) : base(type, r, c)
        {
        }

        public override void DetectTileCollision(Tile tile)
        {
            throw new NotImplementedException();
        }

        public override Rectangle GetCollisionRect()
        {
            throw new NotImplementedException();
        }

        public override void Move(int x, int y)
        {
            throw new NotImplementedException();
        }

        protected override void SubDraw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            throw new NotImplementedException();
        }
    }
}
