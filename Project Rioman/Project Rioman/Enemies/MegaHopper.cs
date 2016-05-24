using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class MegaHopper : AbstractEnemy
    {

        private int frame;
        private double frameTime;
        private bool jumping;

        private double jumpTime;
        private double fallTime;

        private bool stopUpMovement;

        public MegaHopper(int type, int x, int y) : base(type, x, y)
        {
            sprite = EnemyAttributes.GetSprites(type)[0];

            SubReset();
        }

        protected sealed override void SubReset()
        {
            frame = 0;
            fallTime = 0;
            jumpTime = 0;
            location.Y -= sprite.Height;
            drawRect = new Rectangle(0, 0, sprite.Width / 6, sprite.Height);
        }


        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {

            if (isAlive)
            {
                if (!jumping)
                {
                    frameTime += deltaTime;

                    if (frameTime > 0.1 && frame < 5)
                    {
                        frameTime = 0;
                        frame++;
                        if (frame == 5 && fallTime <= 0.1)
                            jumping = true;
                    }


                    if (player.Hitbox.Center.X > GetCollisionRect().Center.X)
                        direction = SpriteEffects.FlipHorizontally;
                    else if (player.Hitbox.Center.Y < GetCollisionRect().Center.Y)
                        direction = SpriteEffects.None;
                }


                if (jumping)
                {

                    jumpTime += deltaTime;
                    fallTime += deltaTime;

                    if (FacingLeft())
                        Move(-3, 0);
                    else
                        Move(3, 0);


                    if (jumpTime < 1)
                    {
                        if (!stopUpMovement) 
                            Move(0, -Convert.ToInt32(14 - jumpTime * 14));
                    }
                    else {
                        jumpTime = 0;
                        jumping = false;
                        frame = 0;
                    }
                }

                Move(0, Convert.ToInt32(fallTime * 22));

            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            drawRect = new Rectangle(frame * sprite.Width / 6, 0, sprite.Width / 6, sprite.Height);

            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height), drawRect,
                Color.White, 0f, new Vector2(), direction, 0);
        }

        public override void DetectTileCollision(AbstractTile tile)
        {

            if (tile.Type == 1 || tile.Type == 3 && tile.IsTop)
            {
                if (GetCollisionRect().Intersects(tile.Floor))
                    GroundCollision(tile.Location.Y);
            }

            if (tile.Type == 1 || tile.Type == 4)
            {
                if (Head().Intersects(tile.Bottom))
                    BottomCollision();
                if (Right().Intersects(tile.Left))
                    LeftCollision();
                if (Left().Intersects(tile.Right))
                    RightCollision();
            }
        }

        private void GroundCollision(int groundTop)
        {
            if (jumping)
            {
                fallTime = 0;
                stopUpMovement = false;
                frame = 0;
                location.Y = groundTop - drawRect.Height;
            }
        }

        private void BottomCollision()
        {
            stopUpMovement = true;
        }

        private void LeftCollision()
        {
            direction = SpriteEffects.None;
        }

        private void RightCollision()
        {
            direction = SpriteEffects.FlipHorizontally;
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 6, location.Y + 12, drawRect.Width - 12, drawRect.Height - 12);
        }

        private Rectangle Left() { return new Rectangle(location.X, location.Y + 20, 12, drawRect.Height * 2 / 3); }
        private Rectangle Right() { return new Rectangle(location.X + drawRect.Width - 14, location.Y + 20, 12, drawRect.Height * 2 / 3); }
        private Rectangle Head() { return new Rectangle(location.X + 6, location.Y, drawRect.Width - 12, 12); }
        private Rectangle Feet() { return new Rectangle(location.X + 12, location.Y + drawRect.Height - 12, drawRect.Width - 24, 12); }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            //do nothing
        }
    }
}
