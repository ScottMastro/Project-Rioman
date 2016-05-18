using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project_Rioman
{
    class PosterMan : AbstractBoss
    {

        private Texture2D defaultSprite;
        private Texture2D jumpSprite;
        private Texture2D runSprite;
        private Texture2D shootSprite;
        private Texture2D bullet;
        private double standTime;
        private double animateTime;
        private int animateDir;
        private int frame;

        private Point netMovement;
        private const int MAX_LEFT = -260;
        private const int MAX_RIGHT = 210;

        private const int MOVE_SPEED = 6;


        public PosterMan(int x, int y) : base(Constant.POSTERMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.POSTERMAN);
            defaultSprite = sprites[1];
            jumpSprite = sprites[2];
            runSprite = sprites[3];
            bullet = sprites[4];

            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);

            Reset();
        }

        protected override void SubReset()
        {
            animateDir = 1;
            frame = 0;
            animateTime = 0;
            standTime = 0;
            netMovement = new Point(0, 0);
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (IsStanding())
            {
                standTime += deltaTime;
                if (standTime > 0.6)
                    Run();
                
            }

            if (IsRunning())
            {
                if (FacingLeft())
                {
                    MoveThis(-MOVE_SPEED, 0);
                    if (netMovement.X < MAX_LEFT)
                        Stand(player);
                }
                else
                {
                    MoveThis(MOVE_SPEED, 0);

                    if (netMovement.X > MAX_RIGHT)
                    Stand(player);

                }


                animateTime += deltaTime;
                if (animateTime > 0.1)
                {
                    animateTime = 0;
                    frame += animateDir;

                    if (frame >= 2)
                        animateDir = -1;
                    if (frame <= 0)
                        animateDir = 1;

                    drawRect = new Rectangle(frame * sprite.Width / 3, 0, sprite.Width / 3, sprite.Height);
                }

            }

        }

        private void Stand(Rioman player)
        {

            FacePlayer(player);
            standTime = 0;
            state = State.standing;
            shooting = false;
            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        private void Run()
        {
            if (netMovement.X >= 0)
                direction = SpriteEffects.None;
            else
                direction = SpriteEffects.FlipHorizontally;

            animateDir = 1;
            animateTime = 0;
            frame = 0;
            state = State.running;
            shooting = false;
            sprite = runSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 3, sprite.Height);

        }


        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);
            DebugDraw.DrawRect(spriteBatch, Head(), 0.4f);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch) { }

        private void MoveThis(int x, int y)
        {
            location.X += x;
            netMovement.X += x;
            location.Y += y;
            netMovement.Y += y;
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 16, location.Y + 10, drawRect.Width - 32, drawRect.Height - 10);
        }

        public override Rectangle Feet()
        {
            return new Rectangle(location.X + 20, location.Y + drawRect.Height - 12, drawRect.Width - 40, 12);
        }

        public override Rectangle Head()
        {
            return new Rectangle(location.X + 30, location.Y + 10, drawRect.Width - 64, 16);
        }

        public override Rectangle Left()
        {
            return new Rectangle();
        }

        public override Rectangle Right()
        {
            return new Rectangle();
        }

        public override void DetectTileCollision(AbstractTile tile)
        {
            //do nothing
        }
    }
}
