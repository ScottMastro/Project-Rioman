using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    class Serverbot : AbstractEnemy
    {

        private int frame;
        private double frameTime;
        private bool falling;
        private double fallTime;

        private bool groundBelow;
        private bool stopLeft;
        private bool stopRight;

        private const int MOVE_SPEED = 16;


        public Serverbot(int type, int x, int y) : base(type, x, y)
        {
            sprite = EnemyAttributes.GetSprites(type)[0];

            SubReset();

        }

        protected override void SubReset()
        {
            frame = 1;

            location.Y -= sprite.Height;
            drawRect = new Rectangle(sprite.Width / 4, 0, sprite.Width / 4, sprite.Height);

            groundBelow = false;
            stopLeft = false;
            stopRight = false;
        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {

            if (isAlive)
            {
                if (!falling)
                {
                    if (Math.Abs(player.Hitbox.Center.X - GetCollisionRect().Center.X) < MOVE_SPEED + 4)
                        Stand();
                    else if (player.Hitbox.Center.X > GetCollisionRect().Center.X)
                        direction = SpriteEffects.FlipHorizontally;
                    else if (player.Hitbox.Center.X < GetCollisionRect().Center.X)
                        direction = SpriteEffects.None;
                }

                frameTime += deltaTime;

                if (frameTime > 0.15 && !falling)
                {
                    frameTime = 0;
                    frame++;
                    if (frame > 3)
                        frame = 1;

                    if (FacingLeft() && !stopLeft)
                        Move(-MOVE_SPEED, 0);
                    else if (!FacingLeft() && !stopRight)
                        Move(MOVE_SPEED, 0);
                    else
                        Stand();
                }


                if (falling)
                {

                    fallTime += deltaTime;
                    if (fallTime * 30 > 10)
                        Move(0, 10);
                    else
                        Move(0, Math.Max(Convert.ToInt32(fallTime * 30), 2));
                }

                if (!groundBelow)
                    falling = true;
                groundBelow = false;
                stopLeft = false;
                stopRight = false;
            }
        }

        private void Stand()
        {
            frame = 0;
            frameTime = 0;
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            drawRect = new Rectangle(frame * sprite.Width / 4, 0, sprite.Width / 4, sprite.Height);

            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height), drawRect,
                Color.White, 0f, new Vector2(), direction, 0);

        }

        public override void DetectTileCollision(Tile tile)
        {

            if (tile.type == 1 || tile.type == 3 && tile.isTop)
            {
                if (GetCollisionRect().Intersects(tile.Floor))
                    GroundCollision(tile.location.Y);
            }

            if (tile.type == 1 || tile.type == 4)
            {
                if (Right().Intersects(tile.Left))
                    LeftCollision(tile.location.Left);
                if (Left().Intersects(tile.Right))
                    RightCollision(tile.location.Right);
            }
        }

        private void GroundCollision(int groundTop)
        {
            groundBelow = true;
            if (falling)
            {
                fallTime = 0;
                location.Y = groundTop - drawRect.Height;
                falling = false;
            }
        }


        private void LeftCollision(int leftX)
        {
            stopRight = true;
            location.X = leftX - drawRect.Width + 5;
        }

        private void RightCollision(int rightX)
        {
            stopLeft = true;
            location.X = rightX - 6;

        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 6, location.Y + 12, drawRect.Width - 12, drawRect.Height - 12);
        }

        private Rectangle Left() { return new Rectangle(location.X- 4, location.Y + 8, 10, drawRect.Height * 2 / 3); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width - 14, location.Y + 8, 10, drawRect.Height * 2 / 3); }
        private Rectangle Feet() { return new Rectangle(location.X + 12, location.Y + drawRect.Height - 10, drawRect.Width - 24, 10); }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            //do nothing
        }
    }
}