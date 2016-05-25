using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class LurkerMan : AbstractBoss
    {
        private Texture2D defaultSprite;
        private Texture2D defaultSpriteFade;
        private Texture2D floatSprite;
        private Texture2D floatSpriteFade;

        private double lurkTime;
        private double fadeTime;
        private double standTime;

        private int floatDist;


        private const int ATTACK_SPEED = 5;
        private Point attackDir;

        private bool fading;

        private Random r;

        int screenWidth;
        int screenHeight;


        public LurkerMan(int x, int y) : base(Constant.LURKERMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.LURKERMAN);
            defaultSprite = sprites[1];
            defaultSpriteFade = sprites[4];
            floatSprite = sprites[2];
            floatSpriteFade = sprites[3];

            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);

            Reset();
        }

        protected override void SubReset()
        {
            r = new Random();
            floatDist = 0;
            attackDir = new Point(0, 0);
            fading = false;
            state = State.lurking;
            sprite = defaultSprite;

            lurkTime = 0;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {
            if(!IsLurking() && (!fading || fadeTime > 0.1))
                spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);

            if (fading && fadeTime <= 0.2)
            {
                if(IsStanding())
                    spriteBatch.Draw(defaultSpriteFade, location, null, Color.White, 0f, new Vector2(), direction, 0);

                if (IsFloating())
                    spriteBatch.Draw(floatSpriteFade, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);

            }

            //DebugDraw.DrawRect(spriteBatch, new Rectangle(80, 80, screenWidth - 160, screenHeight - 160), 0.15f);

            DebugDraw.DrawRect(spriteBatch, new Rectangle(54, 40, screenWidth - 40 - 54, screenHeight - 40 - 40), 0.4f);

        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            screenWidth = viewport.Width;
            screenHeight = viewport.Height; 

            if (fading)
            {
                fadeTime += deltaTime;
                if (fadeTime > 1)
                    fading = false;
            }

            if (IsLurking())
            {
                lurkTime += deltaTime;

                if (lurkTime > 1.5)
                {
                    if (r.Next(100) < 25)
                        Stand(player);
                    else
                        Float(player, viewport);

                }
            }

            if (IsStanding())
            {
                if (!fading)
                    Lurk();

            }

            if (IsFloating())
            {
                location.X += attackDir.X;
                location.Y += attackDir.Y;

                floatDist += attackDir.X + attackDir.Y;

                if (floatDist > 50)
                    Lurk();
            }

        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            //do nothing
        }

      
        private void Stand(Rioman player)
        {
            FacePlayer(player);
            if (!fading && IsLurking())
                Fade();
            state = State.standing;
            sprite = defaultSprite;
        }

        private void Float(Rioman player, Viewport viewport)
        {
            floatDist = 0;

            if (!fading && IsLurking())
                Fade();
            state = State.floating;
            sprite = floatSprite;

            location.X = r.Next(80, viewport.Width - 80);
            location.Y = r.Next(80, viewport.Height - 80);

            FacePlayer(player);

            double diffY = player.Hitbox.Center.Y - GetCollisionRect().Center.Y;
            double diffX = player.Hitbox.Center.X - GetCollisionRect().Center.X;
            double dist = Math.Sqrt(diffX * diffX + diffY * diffY);

            int x = (int)(ATTACK_SPEED * diffX / dist);
            int y = (int)(ATTACK_SPEED * diffY / dist);

            attackDir = new Point(x, y);
        }

        private void Lurk()
        {
            lurkTime = 0;
            state = State.lurking;
        }

        private void Fade()
        {
            fading = true;
            fadeTime = 0;
        }

        public override void DetectTileCollision(AbstractTile tile)
        {

        }

        public override Rectangle GetCollisionRect()
        {
            if (IsLurking())
                return new Rectangle(0, 0, 0, 0);

            return new Rectangle(location.X + 10, location.Y, drawRect.Width - 20, drawRect.Height);
        }

        public override Rectangle Feet()
        {
            return new Rectangle(location.X + 10, location.Y + drawRect.Height - 20, drawRect.Width - 20, 20);
        }

        public override Rectangle Head()
        {
            return new Rectangle(location.X + 16, location.Y , drawRect.Width - 32, 26);
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
