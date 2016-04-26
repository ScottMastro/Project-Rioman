using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class BunnyMan : AbstractBoss
    {
        private Texture2D defaultSprite;
        private Texture2D crouchSprite;
        private Texture2D shootSprite;

        private double crouchTime;
        private double airTime;

        public BunnyMan(int x, int y) : base(Constant.BUNNYMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.BUNNYMAN);
            defaultSprite = sprites[0];
            shootSprite = sprites[1];
            crouchSprite = sprites[2];


            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);
        }

        protected override void SubReset()
        {
            crouchTime = 0;
            airTime = 0;
        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), SpriteEffects.None, 0);

            DebugDraw.DrawRect(spriteBatch, GetCollisionRect(), 0.2f);
            DebugDraw.DrawRect(spriteBatch, Head(), 0.3f);
            DebugDraw.DrawRect(spriteBatch, Feet(), 0.4f);


        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (IsStanding())
                Crouch();


            if (IsCrouching())
            {
                crouchTime += deltaTime;
                if (crouchTime > 2)
                    Jump();
            }

            if (IsJumping())
            {
                if (location.Y > -drawRect.Height)
                    location.Y -= 10;
                else {
                    airTime += deltaTime;
                    Console.WriteLine(airTime);
                }

                if (airTime > 3)
                    Fall();
            }

            else if (IsFalling())
            {
                Console.WriteLine("falling");

                location.Y += 6;
            }
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
        }

        private void Crouch()
        {
            if (!IsJumping())
            {
                crouchTime = 0;
                sprite = crouchSprite;
                state = State.crouching;
                drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            }
        }
        private void Jump()
        {
            airTime = 0;
            if (!shooting)
                sprite = defaultSprite;
            else
                sprite = shootSprite;

            state = State.jumping;
            drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width/2, sprite.Height);
        }

        private void Fall()
        {
            if (!shooting)
                sprite = defaultSprite;
            else
                sprite = shootSprite;

            state = State.falling;
            drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
        }

        private void Shoot()
        {

        }

        private void Stand(int groundTop)
        {
            location.Y = groundTop - drawRect.Height;
            Stand();
        }

        private void Stand()
        {
            state = State.standing;
            if (!shooting)
                sprite = defaultSprite;
            else
                sprite = shootSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        public override void DetectTileCollision(Tile tile)
        {
            if (tile.type == 1 || tile.type == 3 && tile.isTop)
            {
                if (Feet().Intersects(tile.Floor))
                    Stand(tile.location.Y);
            }
        }

        public override Rectangle GetCollisionRect()
        {
            int yShift = IsCrouching() ? 28 : 14;
            return new Rectangle(location.X + 10, location.Y + yShift, drawRect.Width - 20, drawRect.Height - yShift);
        }

        public override Rectangle Feet()
        {
            return new Rectangle(location.X + 10, location.Y + drawRect.Height - 20, drawRect.Width - 20, 20);
        }

        public override Rectangle Head()
        {
            int yShift = IsCrouching() ? 22 : 10;
            return new Rectangle(location.X + 16, location.Y + yShift, drawRect.Width - 32, 26);
        }

        public override Rectangle Left()
        {
            return new Rectangle();
        }

        public override Rectangle Right()
        {
            return new Rectangle();
        }
    }
}
