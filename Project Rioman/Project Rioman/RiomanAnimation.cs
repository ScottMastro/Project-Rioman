using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class RiomanAnimation
    {
        private RiomanState state;

        private Texture2D sprite;
        private Texture2D stand;
        private Texture2D standShoot;
        private Texture2D jump;
        private Texture2D jumpShoot;
        private Texture2D climb;
        private Texture2D climbFlip;
        private Texture2D climbShoot;
        private Texture2D airHit;
        private Texture2D groundHit;
        private Texture2D hit;
        private Texture2D[] warp = new Texture2D[4];
        private Texture2D[] run = new Texture2D[3];
        private Texture2D[] runShoot = new Texture2D[4];
        private Texture2D climbTop;

        public SpriteEffects direction;

        private int warpFrame;
        private double warpFrameTime;

        private double climbShootTime;
        private double climbTime;
        private int climbDirection;

        private int frame;
        private double frameTime;
        private const double FRAME_TIME = 0.15;

        public RiomanAnimation(ContentManager content, RiomanState s)
        {
            state = s;

            stand = content.Load<Texture2D>("Video\\rioman\\riomanstand");
            standShoot = content.Load<Texture2D>("Video\\rioman\\riomanstandgun");
            jump = content.Load<Texture2D>("Video\\rioman\\riomanjump");
            jumpShoot = content.Load<Texture2D>("Video\\rioman\\riomanjumpgun");
            climb = content.Load<Texture2D>("Video\\rioman\\riomanclimb");
            climbShoot = content.Load<Texture2D>("Video\\rioman\\riomanclimbgun");
            climbTop = content.Load<Texture2D>("Video\\rioman\\riomanclimbtop");
            climbFlip = content.Load<Texture2D>("Video\\rioman\\riomanclimbflop");
            airHit = content.Load<Texture2D>("Video\\rioman\\hitair");
            groundHit = content.Load<Texture2D>("Video\\rioman\\hitground");
            hit = content.Load<Texture2D>("Video\\rioman\\hit");
            climb = content.Load<Texture2D>("Video\\rioman\\riomanclimb");
            climbShoot = content.Load<Texture2D>("Video\\rioman\\riomanclimbgun");
            climbTop = content.Load<Texture2D>("Video\\rioman\\riomanclimbtop");
            climbFlip = content.Load<Texture2D>("Video\\rioman\\riomanclimbflop");

            for (int i = 1; i <= 4; i++)
                warp[i - 1] = content.Load<Texture2D>("Video\\rioman\\warp" + i.ToString());
            for (int i = 1; i <= 3; i++)
                run[i - 1] = content.Load<Texture2D>("Video\\rioman\\riomanrun" + i.ToString());
            for (int i = 1; i <= 4; i++)
                runShoot[i - 1] = content.Load<Texture2D>("Video\\rioman\\riomanrungun" + i.ToString());

            sprite = stand;
            Reset();
        }

        private void Reset()
        {
            direction = SpriteEffects.None;

            climbDirection = 0;
            climbTime = 0;
            climbShootTime = 0;
            frame = 0;
            warpFrame = 0;
            sprite = stand;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle location)
        {
            Rectangle locationRect = new Rectangle(location.X, location.Y, sprite.Width, sprite.Height);
            Rectangle drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

            if(state.IsClimbing() && !state.IsShooting())
                spriteBatch.Draw(sprite, locationRect, drawRect, Color.White, 0f, new Vector2(sprite.Width / 2, 0f), SpriteEffects.None, 0f);
            else
               spriteBatch.Draw(sprite, locationRect, drawRect, Color.White, 0f, new Vector2(sprite.Width / 2, 0f), direction, 0f);

        }

        public void Update(double deltaTime)
        {

            if (state.IsStanding())
            {
                sprite = stand;
                if (state.IsShooting())
                    sprite = standShoot;
            }
            else if (state.IsRunning())
            {
                if (state.IsShooting())
                {
                    UpdateFrame(deltaTime, 4);
                    sprite = runShoot[frame];
                }
                else
                {
                    UpdateFrame(deltaTime, 3);
                    sprite = run[frame];
                }
            }
            else if (state.IsJumping() || state.IsFalling())
            {
                if (state.IsShooting())
                    sprite = jumpShoot;
                else
                    sprite = jump;
            }
            else if (state.IsClimbing())
            {
                AnimateClimbing(deltaTime);
            }

            GetDirection();
            AnimateWarping(deltaTime);
        }

        private void GetDirection()
        {
            if(!state.IsWarping())
            {
                KeyboardState k = Keyboard.GetState();
                if (k.IsKeyDown(Constant.RIGHT))
                    direction = SpriteEffects.None;
                if (k.IsKeyDown(Constant.LEFT))
                    direction = SpriteEffects.FlipHorizontally;

            }
        }

        private void AnimateWarping(double deltaTime)
        {
            if (state.IsWarping())
                sprite = warp[0];
            
            if (state.IsWarping() && state.IsAnimateWarp())
            {
                warpFrameTime += deltaTime;
                if (warpFrameTime > 0.06)
                {
                    warpFrameTime = 0;
                    warpFrame++;

                    if (warpFrame > 3)
                    {
                        warpFrame = 0;
                        state.SetAnimateWarp(false);
                        state.StopWarping();
                    }
                }

                sprite = warp[warpFrame];
            }
        }

        private void AnimateClimbing(double deltaTime)
        {
            climbTime += deltaTime;
            KeyboardState k = Keyboard.GetState();
            bool upDownPress = k.IsKeyDown(Constant.UP) || k.IsKeyDown(Constant.DOWN);

            if (climbDirection == 0)
                sprite = climb;
            else
                sprite = climbFlip;

            if (state.IsShooting() && !state.IsClimbTop())
            {
                sprite = climbShoot;

                if (upDownPress)
                    climbShootTime += deltaTime;

                if (climbShootTime > 0.175)
                {
                    state.StopShooting();

                    if (climbDirection == 0)
                        sprite = climb;
                    else
                        sprite = climbFlip;

                    climbShootTime = 0;
                }

            }

            else if (upDownPress && climbTime > 0.175)
            {
                climbShootTime = 0;
                climbTime = 0;
                climbDirection++;
                if (climbDirection > 1)
                    climbDirection = 0;
            }

            if (state.IsClimbTop())
                sprite = climbTop;
        }

        private void UpdateFrame(double deltaTime, int nFrames)
        {
            frameTime += deltaTime;

            if (frameTime > FRAME_TIME)
            {
                frame++;
                frameTime = 0;
            }

            if (frame >= nFrames)
                frame = 0;
        }

        public Texture2D GetSprite() { return sprite; }
        public bool FacingRight() { return direction == SpriteEffects.None; }
    }
}
