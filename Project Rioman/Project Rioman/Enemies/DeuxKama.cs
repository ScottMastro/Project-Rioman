using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class DeuxKama : AbstractEnemy
    {

        private float rotation;
        private Vector2 rotationOrigin;
        private int netVerticalMovement;
        private int verticalDirection;

        private const float PI = 3.14159f;

        public DeuxKama(int type, int x, int y) : base(type, x, y)
        {
            isInvincible = true;
            sprite = EnemyAttributes.GetSprites(type)[0];

            SubReset();
        }

        protected override void SubReset()
        {
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

            rotation = 0f;
            direction = SpriteEffects.FlipHorizontally;
            rotationOrigin = new Vector2((float)drawRect.Center.X, (float)drawRect.Center.Y);

            netVerticalMovement = 0;
            verticalDirection = 1;
        }


        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            rotation = (rotation + 0.075f);
            if (rotation > PI * 2f)
                rotation = rotation - PI * 2f;

            int movement = verticalDirection * 3;
            location.Y += movement;
            netVerticalMovement += movement;

            if (Math.Abs(netVerticalMovement) > 120)
                verticalDirection *= -1;
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, rotation, rotationOrigin, direction, 0);

        }

        public override Rectangle GetCollisionRect()
        {
            double r = Math.Abs(Math.Sin(rotation));

            int topLeftX = location.X - drawRect.Width / 2;
            int topLeftY = location.Y - drawRect.Height / 2;

            double x = topLeftX + (1 - r) * (drawRect.Width / 3.0);
            double y = topLeftY + r * (drawRect.Height / 3.0);
            double width = drawRect.Width - (1 - r) * (2.0 / 3.0 * drawRect.Width);
            double height = drawRect.Height - r * (2.0 / 3.0 * drawRect.Height);

            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    
        public override void DetectTileCollision(Tile tile)
        {
            //do nothing
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            //do nothing
        }
    }
}
