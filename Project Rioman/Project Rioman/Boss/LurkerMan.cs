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

        private Point[] standPoints;

        private const int ATTACK_SPEED = 6;
        private const int ATTACK_DISTANCE = 400;

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

            standPoints = new Point[5];
            standPoints[0] = new Point(165, 198);
            standPoints[1] = new Point(517, 198);
            standPoints[2] = new Point(165, 326);
            standPoints[3] = new Point(517, 326);
            standPoints[4] = new Point(341, 390);

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
            location.X -= drawRect.Width / 4;
        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {

            if (!IsLurking() && (!fading || fadeTime > 0.1))
                spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);

            if (IsLurking() && fading && fadeTime < 0.1)
                spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);

            if (fading && fadeTime <= 0.2)
            {
                if(IsStanding())
                    spriteBatch.Draw(defaultSpriteFade, location, null, Color.White, 0f, new Vector2(), direction, 0);

                if (IsFloating())
                    spriteBatch.Draw(floatSpriteFade, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);

                if (IsLurking())
                    spriteBatch.Draw(sprite == defaultSprite ? defaultSpriteFade : floatSpriteFade,
                        location, drawRect, Color.White, 0f, new Vector2(), direction, 0);
            }

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

                if (lurkTime > 1.2)
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
                if (fadeTime > 0.1 || !fading)
                {

                    location.X += attackDir.X;
                    location.Y += attackDir.Y;

                    floatDist += (int)Math.Sqrt(Math.Abs(attackDir.X * attackDir.X) + Math.Abs(attackDir.Y * attackDir.Y));

                    if (floatDist > ATTACK_DISTANCE)
                        Lurk();
                }
            }

        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            //do nothing
        }

      
        private void Stand(Rioman player)
        {
            if (!fading && IsLurking())
                Fade();
            state = State.standing;
            sprite = defaultSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

            Point standLoc = standPoints[r.Next(0, 5)];

            location.X = standLoc.X;
            location.Y = standLoc.Y;

            FacePlayer(player);

        }

        private void Float(Rioman player, Viewport viewport)
        {
            floatDist = 0;
            Fade();

            state = State.floating;
            sprite = floatSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

            Point spawn = GetValidSpawnPoint(player, viewport);
            location.X = spawn.X;
            location.Y = spawn.Y;

            FacePlayer(player);

            double diffY = player.Hitbox.Center.Y - GetCollisionRect().Center.Y;
            double diffX = player.Hitbox.Center.X - GetCollisionRect().Center.X;
            double dist = Math.Sqrt(diffX * diffX + diffY * diffY);

            int x = (int)(ATTACK_SPEED * diffX / dist);
            int y = (int)(ATTACK_SPEED * diffY / dist);

            if(y+1 >= Math.Abs(x))
                drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);


            attackDir = new Point(x, y);
        }

        private void Lurk()
        {
            lurkTime = 0;
            state = State.lurking;
            Fade();
        }

        private void Fade()
        {
            fading = true;
            fadeTime = 0;
        }

        public override void DetectTileCollision(AbstractTile tile)
        {

        }

        private Point GetValidSpawnPoint(Rioman player, Viewport viewport)
        {
            int x;
            int y;

            while (true)
            {
                x = r.Next(60, viewport.Width - 34);
                y = r.Next(46, viewport.Height - 34);

                if (Math.Abs(x - player.Hitbox.Center.X) < ATTACK_DISTANCE / 2 &&
                   Math.Abs(y - player.Hitbox.Center.Y) < ATTACK_DISTANCE / 2)
                    continue;
                else if (Math.Abs(x - player.Hitbox.Center.X) > ATTACK_DISTANCE &&
                   Math.Abs(y - player.Hitbox.Center.Y) > ATTACK_DISTANCE)
                    continue;
                else
                    break;
            }

            return new Point(x, y);

        }


        public override Rectangle GetCollisionRect()
        {
            if (IsLurking())
                return new Rectangle(0, 0, 0, 0);

            if (FacingLeft())
            {
                if(IsStanding())
                    return new Rectangle(location.X + 20, location.Y, drawRect.Width - 20, drawRect.Height);
                if(IsFloating())
                    return new Rectangle(location.X, location.Y + 10, drawRect.Width - 20, drawRect.Height- 30);
            }
            else
            {
                if (IsStanding())
                    return new Rectangle(location.X + 10, location.Y, drawRect.Width - 30, drawRect.Height);
                if (IsFloating())
                    return new Rectangle(location.X + 20, location.Y + 10, drawRect.Width - 20, drawRect.Height - 30);
            }
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
