using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    class Serverbot : AbstractEnemy
    {

        public Serverbot(int type, int r, int c) : base(type, r, c)
        {
            sprite = EnemyAttributes.GetSprites(type)[0];
        }

        public override void DetectTileCollision(Tile tile)
        {
            throw new NotImplementedException();
        }

        public override Rectangle GetCollisionRect()
        {
            throw new NotImplementedException();
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            throw new NotImplementedException();
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        protected override void SubMove(int x, int y)
        {
            throw new NotImplementedException();
        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            throw new NotImplementedException();
        }
    }
}