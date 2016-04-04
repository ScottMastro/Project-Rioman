using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Flipside : AbstractEnemy
    {

        private float rotation;
        private enum State { good, evil };
        private State state;

        private bool flipping;
        private double flipTime;

        private const float FLIP_SPEED = 0.3f;
        private const double FLIP_TIME = 1.5;

        private const float PI = (float)Math.PI;
        private Rectangle nullRect = new Rectangle(0, 0, 0, 0);

        public Flipside(int type, int r, int c) : base(type, r, c)
        {
            sprite = EnemyAttributes.GetSprites(type)[0];
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

            flipping = false;
            isInvincible = true;
            rotation = 0;
            state = State.good;
        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            flipTime += deltaTime;

            if (flipTime > FLIP_TIME)
            {
                flipTime = 0;
                Flip();
            }

            if (IsGood() && rotation != 0)
            {
                rotation += FLIP_SPEED;
                if (rotation > 2 * PI)
                {
                    rotation = 0;
                    flipping = false;
                }
            }
            if (IsEvil() && rotation != PI)
            {
                rotation += FLIP_SPEED;
                if (rotation > PI)
                { 
                    rotation = PI;
                    flipping = false;
                }
            }
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            if (player.Hitbox.Intersects(EvilRect()))
                player.Die();

            if (IsGood() && player.Feet.Intersects(GoodRect()))
                player.SetOnEnemy(GoodRect().Top);
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height), drawRect,
                Color.White, rotation, new Vector2(drawRect.Width / 2, drawRect.Height / 2), direction, 0);
        }


        public override Rectangle GetCollisionRect() { return nullRect; }

        public Rectangle GoodRect()
        {

            if (IsGood() || (flipping && IsEvil()))
                return new Rectangle(location.X - drawRect.Width / 2 + 8,
                    location.Y - drawRect.Height / 2 + 2,
                    drawRect.Width - 16, 8);
            else if(flipping)
                return nullRect;
            else
                return new Rectangle(location.X - drawRect.Width / 2 + 8,
                    location.Y + 2, 
                    drawRect.Width - 16, 8);

        }
        public Rectangle EvilRect()
        {
            if (flipping)
                return nullRect;
            else if (IsEvil())
                return new Rectangle(location.X - drawRect.Width / 2 + 10,
                    location.Y - drawRect.Height / 2 + 8,
                    drawRect.Width - 20, drawRect.Height / 2 - 16);
            else
                return new Rectangle(location.X - drawRect.Width / 2 + 16,
                    location.Y + 8,
                    drawRect.Width - 32, drawRect.Height / 2 - 24);
        }


        private void Flip()
        {
            flipping = true;

            if (IsGood())
                state = State.evil;
            else
                state = State.good;
        }

        private bool IsGood() { return state == State.good; }
        private bool IsEvil() { return state == State.evil; }

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
    }
}
