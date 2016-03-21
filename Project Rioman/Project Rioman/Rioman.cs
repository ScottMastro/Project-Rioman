using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class Rioman
    {
        public RiomanState state;
        public RiomanAnimation anim;

        public Texture2D sprite;
        public Texture2D stand;

        Texture2D airhit;
        Texture2D groundhit;

        private Rectangle location;

        Texture2D hit;
        bool ishit;
        Rectangle hitloc;
        Rectangle hitsource;
        int hitframe;
        double hittime;

        public bool isfalling;
        public bool isinvincible;
        public bool ispaused;
        public bool touchedground;

        public double falltime;
        public double invincibletime;
        public double pausetime;
        public double touchtime;
        public int invincibledirection;

        public bool stopx;
        public bool climbDown;
        private int warpY;

        private bool stopRightMovement;
        private bool stopLeftMovement;

        private const int WARP_SPEED = 15;

        private KeyboardState previousKeyboardState;

        public Rioman(ContentManager content)
        {
            state = new RiomanState(this);
            anim = new RiomanAnimation(content, state);


            stand = content.Load<Texture2D>("Video\\rioman\\riomanstand");
            airhit = content.Load<Texture2D>("Video\\rioman\\hitair");
            groundhit = content.Load<Texture2D>("Video\\rioman\\hitground");
            hit = content.Load<Texture2D>("Video\\rioman\\hit");

            sprite = stand;

            location = new Rectangle(70, 400, sprite.Width / 2, sprite.Height);

            Reset();
        }

        public void Reset()
        {

            isfalling = false;
            isinvincible = false;
            touchedground = false;
            stopx = false;

            stopLeftMovement = false;
            stopRightMovement = false;

            falltime = 0;
            invincibletime = 0;
            hittime = 0;
            pausetime = 0;
            touchtime = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            anim.Draw(spriteBatch, location);
        }


        public void Update(double deltaTime, Level level)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (ispaused)
                Pause(deltaTime);
            else
            {

                state.Update(deltaTime);

                if (!isfalling)
                    falltime = 0;

                if (state.IsClimbing())
                    Climb(deltaTime, keyboardState, level);
                else if (state.IsJumping())
                    Jump(keyboardState, level);
                else if (isfalling)
                    Fall(deltaTime, keyboardState, level);
                else if (state.IsRunning() || state.IsStanding())
                    Stand(keyboardState, level);
                else if (state.IsWarping())
                    Warp();

                sprite = GetSprite();

                if (isinvincible)
                    Invincible(deltaTime);

                if (ishit)
                    Hit(deltaTime);


                stopLeftMovement = false;
                stopRightMovement = false;

                anim.Update(deltaTime);

            }


            previousKeyboardState = keyboardState;
        }

        private void Stand(KeyboardState keyboardState, Level level)
        {
            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

        }

        private void Climb(double deltaTime, KeyboardState keyboardState, Level level)
        {
            if (keyboardState.IsKeyDown(Constant.UP))
            {
                Move(0, -3);
                climbDown = false;
            }
            else if (keyboardState.IsKeyDown(Constant.DOWN))
            {
                Move(0, 3);
                climbDown = true;
            }

        }

        private void Fall(double deltaTime, KeyboardState keyboardState, Level level)
        {

            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            falltime += deltaTime;

            if (falltime * 30 > 10)
                Move(0, 10);
            else
                Move(0, Convert.ToInt32(falltime * 30));

            touchedground = false;
        }

        private void Jump(KeyboardState keyboardState, Level level)
        {

            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            Move(0, -Convert.ToInt32(10 - state.GetJumpTime() * 22));

        }


        private void HorizontalMovement(KeyboardState keyboardState, Level level)
        {
            if (keyboardState.IsKeyDown(Constant.RIGHT) && !stopRightMovement)
            {
                if (!level.stopleftscreenmovement && !level.stoprightscreenmovement)
                    level.MoveStuff(-3, 0);
                else if (level.stoprightscreenmovement || level.stopleftscreenmovement)
                    Move(3, 0);
            }
            else if (keyboardState.IsKeyDown(Constant.LEFT) && !stopLeftMovement)
            {
                if (!level.stopleftscreenmovement && !level.stoprightscreenmovement)
                    level.MoveStuff(3, 0);
                else if (level.stopleftscreenmovement || level.stoprightscreenmovement)
                    Move(-3, 0);

            }
        }



        private void CheckClimb(KeyboardState keyboardState, Level level)
        {

            if (keyboardState.IsKeyDown(Constant.UP))
            {
                Rectangle result = level.CheckClimb(location, location.X, true, ref location);

                if (result.Width != 0)
                {
                    Move(0, -10);
                    state.Climb();
                    falltime = 0;
                    isfalling = false;
                    climbDown = false;
                }
            }


            if (keyboardState.IsKeyDown(Constant.DOWN) && !previousKeyboardState.IsKeyDown(Constant.DOWN))
            {
                Rectangle result = level.CheckClimb(location, location.X, false, ref location);
                if (result.Width != 0)
                {
                    Move(0, 20);
                    state.Climb();
                    isfalling = false;
                    climbDown = true;
                }
            }

        }

        private void Warp()
        {
            if (warpY > 0)
            {
                Audio.PlayWarp();

                Move(0, WARP_SPEED);

                if (location.Y >= warpY)
                {
                    MoveToY(warpY);
                    state.SetAnimateWarp(true);
                }
            }
        }

        public void Die()
        {
            state.Warp();
            Reset();
            Audio.die.Play(0.5f, 1f, 0f);

        }

        public Rectangle Left { get { return new Rectangle(location.X, location.Y + 6, 10, 34); } }
        public Rectangle Head { get { return new Rectangle(location.X + 10, location.Y, 36, 32); } }
        public Rectangle Feet { get { return new Rectangle(location.X + 8, location.Y + 42, 40, 12); } }
        public Rectangle Right { get { return new Rectangle(location.X + location.Width - 10, location.Y + 6, 10, 34); } }
        public Rectangle Location { get { return location; } }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void MoveToX(int x)
        {
            location.X = x;
        }

        public void MoveToY(int y)
        {
            location.Y = y;
        }

        public void StopRightMovement() { stopRightMovement = true; }
        public void StopLeftMovement() { stopLeftMovement = true; }
        public Texture2D GetSprite() { return anim.GetSprite(); }
        public void AtLadderTop() { state.SetClimbTopState(true); }
        public void BelowLadderTop() { state.SetClimbTopState(false); }
        public void SetStartYPos(int y) { warpY = y; }
        public bool IsWarping() { return state.IsWarping(); }
        public void StartWarp() { state.Warp(); }

















        //TODO: Refactor this stuff V V V V V V V

        public void Pause(double deltaTime)
        {
            pausetime += deltaTime;

            if (pausetime > 3)
            {
                pausetime = 0;
                ispaused = false;
            }
        }


        public void BackwardScroll(Level level, Viewport vpr)
        {
            if (level.stoprightscreenmovement)
            {
                if (location.X < vpr.Width / 2)
                    level.stoprightscreenmovement = false;
            }

            if (level.stopleftscreenmovement)
            {
                if (location.X > vpr.Width / 2)
                    level.stopleftscreenmovement = false;
            }
        }

        public void Shooting(Bullet[] bullet)
        {
            if (!stopx && !ispaused)
            {
                bool flag = false;
                int num = 0;

                for (int i = 0; i <= 2; i++)
                {
                    if (!bullet[i].alive && !state.IsClimbTop())
                    {
                        flag = true;
                        num = i;
                    }
                }

                if (flag)
                {
                    Audio.shoot1.Play(0.5f, 0f, 0f);
                    state.Shoot();
                    if (!anim.FacingRight())
                        bullet[num].BulletSpawn(location.X - 20, location.Center.Y - 8, SpriteEffects.FlipHorizontally);
                    else
                        bullet[num].BulletSpawn(location.X + 20, location.Center.Y - 8, SpriteEffects.None);
                }
            }
        }

        public void Hit()
        {
            ishit = true;
            ispaused = false;
            pausetime = 0;
            stopx = true;
            isinvincible = true;
            isfalling = true;
            touchedground = false;
            touchtime = 0;
            Move(0, -8);
        }

        private void Hit(double deltaTime)
        {
            hitloc = new Rectangle(location.X - hitloc.Width / 2, location.Center.Y - hitloc.Height / 2, hit.Width / 3, hit.Height);
            hittime += deltaTime;

            if (hittime > 0.05 && hitframe == 3)
            {
                hittime = 0;
                hitframe = 0;
                ishit = false;
            }

            if (hittime > 0.05)
            {
                hittime = 0;
            }
        }

        public void DrawHit(SpriteBatch spriteBatch)
        {
            if (ishit)
                spriteBatch.Draw(hit, hitloc, hitsource, new Color(1f, 1f, 1f, 0.8f), 0f, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        private void Invincible(double elapsetime)
        {
            invincibletime += elapsetime;
            stopx = false;

            if (invincibletime < 0.5 && !touchedground)
            {
                stopx = true;
                location.X += invincibledirection;
                Move(0, -4);

            }

            if (isfalling && !touchedground)
            {
                stopx = true;
                sprite = airhit;

            }
            else if (touchedground && touchtime < 0.3)
            {
                stopx = true;
                touchtime += elapsetime;
                sprite = groundhit;

            }

            for (double i = 0; i <= 2; i += 0.2)
                if (invincibletime > i && invincibletime < i + 0.1)
                    location.Width = 0;

            if (invincibletime > 2)
            {
                stopx = false;
                invincibletime = 0;
                touchtime = 0;
                isinvincible = false;
            }

            if (state.IsClimbing())
            {
                stopx = false;
                touchedground = true;
                touchtime = 10;
            }
        }


    }
}