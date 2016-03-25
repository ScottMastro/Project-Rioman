using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    class Neolucky : AbstractEnemy
    {

        private Texture2D stand;
        private Texture2D jump;
        private Texture2D attack;
        private Texture2D bullet;

        private int frame;

        private double standTime;
        private double jumpTime;
        private double fallTime;

        private enum State { standing, jumping, falling };
        private State state;
        private bool shooting;

        struct NeoBullet
        {
            bool isAlive;
            int X;
            int Y;

            void Reset()
            {
                isAlive = false;
            }
        }

        private NeoBullet[] bullets = new NeoBullet[3];

        public Neolucky(int type, int r, int c) : base(type, r, c)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            stand = sprites[0];
            bullet = sprites[1];
            attack = sprites[2];
            jump = sprites[3];

            shooting = false;
            location.Y -= stand.Height;
            Stand();
        }

        public override void Update(double deltaTime)
        {

            isAlive = true;
            //TODO: CHANGE THIS


            if (isAlive)
            {

                if (IsStanding())
                {
                    UpdateStand(deltaTime);
                }
                if (IsJumping())
                {
                    UpdateJump(deltaTime);
                }
                if(IsFalling())
                {
                    UpdateFall(deltaTime);
                }
            }
        }

        private void UpdateStand(double deltaTime)
        {
            standTime += deltaTime;
            if (standTime > 0.2)
            {
                if(frame == 1)
                {
                    frame = 0;
                    drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
                }
                else if(frame == 0)
                {
                    frame++;
                    drawRect = new Rectangle(sprite.Width / 2, 0, sprite.Width / 2, sprite.Height);
                }
            }

            if (r.Next(50) == 10 && !shooting)
                Jump();
        }

        private void UpdateJump(double deltaTime)
        {
            jumpTime += deltaTime;

            if (FacingLeft())
                location.X -= 1;
            else
                location.X += 1;

            location.Y -= Convert.ToInt32(2.2 - jumpTime * 2.2);

            if (jumpTime > 1.25)
                Fall();
        }

        private void UpdateFall(double deltaTime)
        {
            fallTime += deltaTime;

            if (fallTime * 30 > 10)
                location.Y += 10;
            else
                location.Y += Convert.ToInt32(fallTime * 30);
        }

        private void Stand()
        {
            frame = 0;
            standTime = 0;
            sprite = stand;
            state = State.standing;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
        }

        private void Jump()
        {
            sprite = jump;
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            state = State.jumping;
            jumpTime = 0;
        }


        private void Fall()
        {
            sprite = jump;
            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            state = State.falling;
            fallTime = 0;
        }


        private void Shoot()
        {
            if (IsStanding())
                sprite = attack;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                drawRect, Color.White, 0f, new Vector2(), direction, 0);
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 30, location.Y + 10, drawRect.Width - 10, drawRect.Height - 10);
        }

        public override void GroundCollision(int groundTop)
        {
            if (IsFalling())
            {
                Stand();
                location.Y = groundTop;
            }

        }

        private bool IsJumping() { return state == State.jumping; }
        private bool IsFalling() { return state == State.falling; }
        private bool IsStanding() { return state == State.standing; }


    }
}
