using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Macebot : AbstractEnemy
    {

        private Texture2D ball;
        private Texture2D ballString;
        private Texture2D macebot;

        private int ballDistance;

        private Rectangle ballDrawRect;

        private bool attacking;
        private double ballTime;
        private int ballSpeed;
        private bool ballSpeedSkipFrame;
        private const int MAX_BALL_SPEED = 16;
        private const int BALL_DAMAGE = 5;

        private Vector2 vectorToPlayer;
        private float angleToPlayer;

        public Macebot(int type, int x, int y) : base(type, x, y)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(Constant.MACEBOT);
            ball = sprites[0];
            ballString = sprites[1];
            macebot = sprites[2];

            SubReset();
        }

        protected override void SubReset()
        {
            sprite = macebot;
            location.Y -= sprite.Height;
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

            angleToPlayer = 0;
            vectorToPlayer = new Vector2();

            ballTime = 0;
            ballDistance = 0;
            ballSpeed = MAX_BALL_SPEED;
            ballDrawRect = new Rectangle(0, 0, ball.Width, ball.Height);
            ballSpeedSkipFrame = true;

            attacking = false;

        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                if (!attacking)
                {
                    Point p = player.Hitbox.Center;
                    Point m = GetCollisionRect().Center;

                    vectorToPlayer = new Vector2(p.X - m.X, p.Y - m.Y);
                    vectorToPlayer = Vector2.Normalize(vectorToPlayer);
                    angleToPlayer = RoundToFifteen((float)Math.Atan2(p.Y - m.Y, p.X - m.X));


                    if (player.Hitbox.Center.X > GetCollisionRect().Center.X)
                        direction = SpriteEffects.FlipHorizontally;
                    else if (player.Hitbox.Center.X < GetCollisionRect().Center.X)
                        direction = SpriteEffects.None;

                    if (ballTime > 1.5)
                    {
                        attacking = true;
                        ballTime = 0;
                    }
                    else
                        ballTime += deltaTime;
                }
                else {
                    ballDistance += ballSpeed;
                    if (ballSpeedSkipFrame)
                        ballSpeedSkipFrame = false;
                    else {
                        ballSpeedSkipFrame = true;
                        ballSpeed--;
                        if (ballSpeed < -MAX_BALL_SPEED)
                            ballSpeed = MAX_BALL_SPEED;
                    }

                    if (ballDistance <= 0)
                        attacking = false;
                }
            }
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            if (isAlive && attacking && GetBallCollisionRect().Intersects(player.Hitbox))
                player.Hit(BALL_DAMAGE);

        }


        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {

            float angle = 0f;
            if (attacking)
                angle = angleToPlayer;

            spriteBatch.Draw(ballString, GetStringRect(), null, Color.White, angle,
                  new Vector2(0, ballString.Height / 2), SpriteEffects.None, 0);

            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, 0f, new Vector2(), direction, 0);

            spriteBatch.Draw(ball, GetBallRect(), null, Color.White, angle,
                new Vector2(-ballDistance, ballDrawRect.Height / 2), SpriteEffects.None, 0);

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

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 5, location.Y + 5, drawRect.Width - 10, drawRect.Height - 10);
        }

        private Rectangle GetBallRect() {

            int x = location.X;
            int y = location.Y + ballDrawRect.Height/2;

            if (!FacingLeft())
                x += drawRect.Width;
            else if(!attacking)
                x -= 12;


            if (!attacking)
                x -= 8;


            return new Rectangle(x, y, ballDrawRect.Width, ballDrawRect.Height);

        }

        private Rectangle GetBallCollisionRect()
        {
            if (!attacking)
                return new Rectangle(0,0,0,0);

            if (FacingLeft())
            {
                return new Rectangle(location.X + (int)(Math.Cos(angleToPlayer) * (ballDistance + 10) - ballDrawRect.Width / 2),
                     location.Y + (int)(Math.Sin(angleToPlayer) * (ballDistance + 10)) + ballDrawRect.Height / 4,
                     ballDrawRect.Width / 2, ballDrawRect.Height / 2);

            }
            else
            {

                return new Rectangle(location.X + drawRect.Width + (int)(Math.Cos(angleToPlayer) * (ballDistance + 10)),
                     location.Y + (int)(Math.Sin(angleToPlayer) * (ballDistance+10)) + ballDrawRect.Height / 4,
                     ballDrawRect.Width / 2, ballDrawRect.Height / 2);
            }
        }

        private Rectangle GetStringRect()
        {

            if (FacingLeft())
                return new Rectangle(location.X, location.Y + 16, ballString.Width + ballDistance, ballString.Height);
            else
                return new Rectangle(location.X + drawRect.Width - 6, location.Y + 16, ballString.Width + ballDistance, ballString.Height);

        }


        private float RoundToFifteen(float value)
        {
            int sign = 1;
            if (value < 0)
                sign = -1;

            value = Math.Abs(value);

            const float fifteen = 0.261799f;
            float floor = (float)Math.Floor(value / fifteen);

            //round down
            if (value % fifteen < fifteen / 2f)
                return fifteen * floor * sign;
            //round up
            else
                return fifteen * (floor + 1) * sign;

        }

    }
}
