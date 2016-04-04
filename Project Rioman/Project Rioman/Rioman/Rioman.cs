using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class Rioman
    {
        public RiomanState state;
        public RiomanAnimation anim;

        private Rectangle location;

        public int invincibledirection;

        public bool climbDown;
        private int startPosX;
        private int startPosY;

        private bool stopRightMovement;
        private bool stopLeftMovement;

        private const int WARP_SPEED = 15;
        private const int MAX_FALL_SPEED = 10;

        private Bullet[] bullets = new Bullet[3];

        private KeyboardState prevKeyboardState;

        public Rioman(ContentManager content)
        {
            state = new RiomanState(this);
            anim = new RiomanAnimation(content, state);

            for (int i = 0; i <= 2; i++)
                bullets[i] = new Bullet(content.Load<Texture2D>("Video\\bullet"));

            location = new Rectangle(70, 400, GetSprite().Width, GetSprite().Height);

            Reset();
        }

        public void Reset()
        {
            stopLeftMovement = false;
            stopRightMovement = false;

            KillBullets();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet blt in bullets)
                blt.Draw(spriteBatch);

            anim.Draw(spriteBatch, location, this);

        }

        public void Update(double deltaTime, Level level, Viewport viewport)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            state.Update(deltaTime);

            if (IsClimbing())
                UpdateClimb(deltaTime, keyboardState, level);
            else if (IsJumping())
                UpdateJump(keyboardState, level);
            else if (IsFalling())
                UpdateFall(deltaTime, keyboardState, level);
            else if (IsRunning() || state.IsStanding())
                UpdateStand(keyboardState, level);
            else if (IsWarping())
                UpdateWarp();

            if (state.IsHit())
                UpdateHit();

            CheckShoot(keyboardState);

            foreach (Bullet blt in bullets)
                blt.BulletUpdate(viewport.Width);
            

            stopLeftMovement = false;
            stopRightMovement = false;

            anim.Update(deltaTime);

            prevKeyboardState = keyboardState;
        }

        private void CheckShoot(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Constant.SHOOT) && !prevKeyboardState.IsKeyDown(Constant.SHOOT) && state.CanShoot())
            {
                int index = -1;

                for (int i = 0; i <= 2; i++)
                {
                    if (!bullets[i].isAlive)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    Audio.PlayShoot();
                    state.Shoot();
                    if (!FacingRight())
                        bullets[index].BulletSpawn(location.X - 20, location.Center.Y - 8, SpriteEffects.FlipHorizontally);
                    else
                        bullets[index].BulletSpawn(location.X + 20, location.Center.Y - 8, SpriteEffects.None);
                }
            }
        }


        private void UpdateStand(KeyboardState keyboardState, Level level)
        {
            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

        }

        private void UpdateClimb(double deltaTime, KeyboardState keyboardState, Level level)
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

        private void UpdateFall(double deltaTime, KeyboardState keyboardState, Level level)
        {

            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            if (state.GetFallTime() * 30 > MAX_FALL_SPEED)
                Move(0, MAX_FALL_SPEED);
            else
                Move(0, Convert.ToInt32(state.GetFallTime() * 30));

        }

        private void UpdateJump(KeyboardState keyboardState, Level level)
        {

            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            Move(0, -Convert.ToInt32(10 - state.GetJumpTime() * 22));

        }


        private void HorizontalMovement(KeyboardState keyboardState, Level level)
        {

            if (!state.IsHit())
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
        }

        private void UpdateWarp()
        {
            Audio.PlayWarp();

            Move(0, WARP_SPEED);

            if (location.Y >= startPosY)
            {
                MoveToY(startPosY);
                state.SetAnimateWarp(true);
            }

        }

        private void UpdateHit()
        {
            if (state.Airborne()) {
                if (FacingRight() && !stopLeftMovement)
                    location.X -= 3;
                else if (!FacingRight() && !stopRightMovement)
                    location.X += 3;

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
                    climbDown = false;
                }
            }


            if (keyboardState.IsKeyDown(Constant.DOWN) && !prevKeyboardState.IsKeyDown(Constant.DOWN))
            {
                Rectangle result = level.CheckClimb(location, location.X, false, ref location);
                if (result.Width != 0)
                {
                    Move(0, 20);
                    state.Climb();
                    climbDown = true;
                }
            }

        }

        public void Hit()
        {
            if (state.Hit())
                Move(0, -10);
        }

        public void Hit(int damage)
        {
            //TODO
            Hit();
        }

        public void Die()
        {
            Reset();
            MoveToY(-100);
            Audio.die.Play(Constant.VOLUME, 1f, 0f);

        }


        public Rectangle Hitbox
        {
            get
            {

                int shiftX = 0;

                if (FacingRight())
                    shiftX = -24;
                else
                    shiftX = -8;

                return new Rectangle(location.X + shiftX, location.Y + 10, 32, 42);
            }
        }
        public Rectangle Left
        {
            get
            {

                int shiftX = 0;

                if (FacingRight())
                    shiftX = -30;
                else
                    shiftX = -20;

                return new Rectangle(location.X + shiftX, location.Y + 6, 10, 34);
            }
        }

        public Rectangle Right {
            get
            {

                int shiftX = 0;

                if (FacingRight())
                    shiftX = -25;
                else
                    shiftX = -15;

                return new Rectangle(location.X + 38 + shiftX, location.Y + 6, 10, 34);
            }
        }

        public Rectangle Head
        {
            get
            {
                int shiftX = 0;

                if (FacingRight())
                    shiftX = -22;
                else
                    shiftX = -8;

                return new Rectangle(location.X + shiftX, location.Y, 30, 20);

            }
        }
        public Rectangle Feet
        {
            get
            {
                int shiftX = 0;

                if (FacingRight())
                    shiftX = -22;
                else
                    shiftX = -8;
                return new Rectangle(location.X + shiftX, location.Y + 42, 30, 12);
            }
        }
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

        public void SetOnEnemy(int posY) {
            state.SetOnEnemy();
            MoveToY(posY - GetSprite().Height + 8);
        }

        public void StopRightMovement() { stopRightMovement = true; }
        public void StopLeftMovement() { stopLeftMovement = true; }
        public Texture2D GetSprite() { return anim.GetSprite(); }
        public void AtLadderTop() { state.SetClimbTopState(true); }
        public void BelowLadderTop() { state.SetClimbTopState(false); }
        public void SetStartPos(int x, int y)
        {
            startPosX = x;
            startPosY = y;
        }
        public bool IsWarping() { return state.IsWarping(); }
        public bool IsFalling() { return state.IsFalling(); }
        public bool IsRunning() { return state.IsRunning(); }
        public bool IsClimbing() { return state.IsClimbing(); }
        public bool IsJumping() { return state.IsJumping(); }
        public bool IsInvincible() { return state.IsInvincible(); }
        public bool IsOnEnemy() { return state.IsOnEnemy(); }

        public bool FacingRight() { return anim.FacingRight(); }

        public Bullet[] GetBullets() { return bullets; }
        public void KillBullets()
        {
            for (int i = 0; i <= 2; i++)
                bullets[i].isAlive = false;

        }


        public void StartWarp() { state.Warp(); }

    }
}