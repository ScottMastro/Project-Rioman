using System;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Tile : AbstractTile
    {
        public Tile(int ID, int x, int y) : base(ID, x, y) { }

        protected override void SubDraw(SpriteBatch spriteBatch) { }

        protected override void SubMove(int x, int y) { }
        protected override void SubReset() { }
        protected override void SubUpdate(Rioman player, double deltaTime) { }
    }
}
