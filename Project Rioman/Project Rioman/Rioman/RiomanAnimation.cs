using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project_Rioman
{
    class RiomanAnimation
    {
        private RiomanState state;

        private Texture2D[] debug = new Texture2D[4];

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

        private Texture2D clock;

        public SpriteEffects direction;

        private int warpFrame;
        private double warpFrameTime;

        private double climbShootTime;
        private double climbTime;
        private int climbDirection;

        private int frame;
        private double frameTime;
        private const double FRAME_TIME = 0.15;

        private double hitEffectTime;
        private int blinkFrames;
        private const int BLINK_DURATION = 4;

        private double totalFreezeTime;

        public RiomanAnimation(ContentManager content, RiomanState s)
        {
            state = s;

            for (int i = 1; i <= 4; i++)
                debug[i - 1] = content.Load<Texture2D>("Video\\debug\\debug" + i.ToString());

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

            clock = content.Load<Texture2D>("Video\\rioman\\clockandhand");

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
            blinkFrames = 0;
            hitEffectTime = 0;
            sprite = stand;

            totalFreezeTime = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle location, Rioman player, bool levelIsBusy)
        {

            if (!levelIsBusy && state.IsInvincible() && blinkFrames > 0)
            { 
                //do nothing
            }
            else
            {
                Rectangle locationRect = new Rectangle(location.X, location.Y, sprite.Width, sprite.Height);
                Rectangle drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

                if (state.IsClimbing() && !state.IsShooting())
                    spriteBatch.Draw(sprite, locationRect, drawRect, Color.White, 0f, new Vector2(sprite.Width / 2, 0f), SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(sprite, locationRect, drawRect, Color.White, 0f, new Vector2(sprite.Width / 2, 0f), direction, 0f);

            }

            if (state.IsHit())
            {
                int frame = (int) Math.Floor(hitEffectTime / 0.05);
                Rectangle effectDrawRect = new Rectangle(frame * hit.Width / 3, 0, hit.Width / 3, hit.Height);
                Rectangle effectLocation = new Rectangle(location.X  - effectDrawRect.Width/2,
                    location.Y + sprite.Height/2 - effectDrawRect.Height/2, effectDrawRect.Width, effectDrawRect.Height);

                spriteBatch.Draw(hit, effectLocation, effectDrawRect, Color.White);
            }

            if (state.IsFrozen())
            {
                spriteBatch.Draw(clock, new Rectangle(player.Hitbox.Center.X, player.Hitbox.Center.Y, clock.Width / 2, clock.Height),
                    new Rectangle(0, 0, clock.Width / 2, clock.Height), Color.White * 0.6f, 0f, new Vector2(clock.Width / 4, clock.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(clock, new Rectangle(player.Hitbox.Center.X, player.Hitbox.Center.Y, clock.Width / 2, clock.Height),
                    new Rectangle(clock.Width / 2, 0, clock.Width / 2, clock.Height), Color.White ,
                    (float)(state.GetFreezeTime()/totalFreezeTime) * -MathHelper.TwoPi, new Vector2(clock.Width / 4, clock.Height / 2), SpriteEffects.None, 0);

            }
            /* debugging boxes

            spriteBatch.Draw(debug[2], r.Left, Color.White);
            spriteBatch.Draw(debug[3], r.Right, Color.White);
            spriteBatch.Draw(debug[1], r.Feet, Color.White);
            spriteBatch.Draw(debug[0], r.Hitbox, Color.White);
            spriteBatch.Draw(debug[0], r.Head, Color.White);

            */


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
            else if (state.Airborne())
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

            if (state.IsHit())
            {
                hitEffectTime += deltaTime;

                if (state.Airborne())
                    sprite = airHit;
                else if (state.Grounded())
                    sprite = groundHit;
            }
            else
                hitEffectTime = 0;

            if (state.IsInvincible())
            {
                blinkFrames--;
                if (blinkFrames <= -BLINK_DURATION * 2)
                    blinkFrames = BLINK_DURATION;
            }
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
        public void SetFreezeTime(double x) { totalFreezeTime = x; }

    }
}
