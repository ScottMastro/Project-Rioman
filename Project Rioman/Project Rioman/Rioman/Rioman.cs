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
        private Vector2 lastMove;

        public bool climbDown;

        private int startPosX;
        private int startPosY;

        private bool stopRightMovement;
        private bool stopLeftMovement;

        private const int WARP_SPEED = 15;
        private const int MAX_FALL_SPEED = 10;

        private AbstractBullet[] bullets = new AbstractBullet[10];

        private KeyboardState prevKeyboardState;

        private int movedByConveyor;

        public Rioman(ContentManager content)
        {
            state = new RiomanState(this);
            anim = new RiomanAnimation(content, state);

            location = new Rectangle(70, 400, GetSprite().Width, GetSprite().Height);

            Reset();
        }

        public void Reset()
        {
            state.Unfreeze();
            lastMove = new Vector2(0, 0);
            stopLeftMovement = false;
            stopRightMovement = false;
            state.SetLurking(false);

            KillBullets();
        }

        public void Draw(SpriteBatch spriteBatch, bool levelIsBusy)
        {
            foreach (AbstractBullet blt in bullets)
            {
                if (blt != null)
                    blt.Draw(spriteBatch);
            }

            anim.Draw(spriteBatch, location, this, levelIsBusy);
        }

        public void Update(double deltaTime, Level level, Viewport viewport, AbstractEnemy[] enemies)
        {
            lastMove = Vector2.Zero;
            movedByConveyor = 0;

            if (!state.IsFrozen())
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

                foreach (AbstractBullet blt in bullets)
                {
                    if (blt != null)
                        blt.Update(this, deltaTime, viewport, enemies);
                }

                stopLeftMovement = false;
                stopRightMovement = false;

                anim.Update(deltaTime);

                prevKeyboardState = keyboardState;
            }
            else
                state.UpdateFreezeTime(deltaTime);
        }

        private void CheckShoot(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Constant.SHOOT) && !prevKeyboardState.IsKeyDown(Constant.SHOOT) && state.CanShoot())
            {
                if (Weapons.GetActiveWeapon() == Constant.LURKERBULLET && Weapons.GetAmmo(Constant.LURKERBULLET) > 0)
                    state.SetLurking(!state.IsLurking());
                else {
                    int index = -1;
                    int weight = 0;

                    for (int i = 0; i <= 2; i++)
                    {
                        if (bullets[i] == null || !bullets[i].IsAlive())
                            index = i;
                        else if (bullets[i] != null && bullets[i].IsAlive())
                            weight += bullets[i].GetWeight();
                    }

                    if (index >= 0)
                    {
                        AbstractBullet newBullet = Weapons.CreateBullet(weight, location.X, location.Y, FacingRight());
                        bullets[index] = newBullet;

                        if (newBullet != null)
                            state.Shoot();

                    }
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
            if (keyboardState.IsKeyDown(Constant.UP) && !state.GetStopClimbUp())
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
                if (keyboardState.IsKeyDown(Constant.RIGHT))
                    MoveInWorld(3, 0, level);
                else if (keyboardState.IsKeyDown(Constant.LEFT))
                    MoveInWorld(-3, 0, level);

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

            if (keyboardState.IsKeyDown(Constant.UP) && !state.IsHit())
            {
                if (level.CheckClimb(this, location.X, true, ref location))
                {
                    if(!state.GetStopClimbUp())
                        Move(0, -3);
                    state.Climb();
                    climbDown = false;
                }
            }


            if (keyboardState.IsKeyDown(Constant.DOWN) && !state.IsHit())
            {
                if (level.CheckClimb(this, location.X, false, ref location))
                {
                    Move(0, 16);
                    state.Climb();
                    climbDown = true;
                }
            }

        }

        public void MoveWithGround(int x, int y, int tileType)
        {
            if (state.Grounded()) {

                if(tileType != Constant.TILE_CONVEYOR)
                    Move(x, y);

                else if (movedByConveyor == 0 || (movedByConveyor > 0 && x < 0) || (movedByConveyor < 0 && x > 0))
                {
                    Move(x, y);
                    movedByConveyor += x;
                }
            } 
        }
        
        public void Hit(int damage)
        {
            if (!IsInvincible() && !state.IsLurking())
            {
                StatusBar.AdjustHealth(-damage);
                state.Hit();

                if (state.Hit())
                    Move(0, -10);
            }
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

                if(state.IsClimbing())
                    return new Rectangle(location.X -12, location.Y + 42, 20, 12);

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
            lastMove = lastMove + new Vector2(x, y);
            location.X += x;
            location.Y += y;
        }

        public void MoveBullets(int x, int y)
        {
            for (int i = 0; i <= bullets.Length - 1; i++)
                if (bullets[i] != null)
                    bullets[i].MoveBullet(x, y);
        }

        public void MoveInWorld(int x, int y, Level level)
        {
            if ((x < 0 && stopLeftMovement) ||
                (x > 0 && stopRightMovement))
                x = 0;

            if ((x < 0 && !stopLeftMovement && level.CanMoveLeft()) ||
                (x > 0 && !stopRightMovement && level.CanMoveRight()))
            {
                level.MoveStuff(this, -x, 0);
                lastMove.X += x;
                x = 0;
            }


            Move(x, y);
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
        public void StopClimingUp() { state.SetStopClimbUp(true); }
        public void AllowClimingUp() { state.SetStopClimbUp(false); }
        public void OnLadder() { state.SetOnLadder(true); }
        public void OffLadder() { state.SetOnLadder(false); }

        public void SetStartPos(int x, int y)
        {
            startPosX = x;
            startPosY = y;
        }
        public bool IsWarping() { return state.IsWarping(); }
        public bool IsFalling() { return state.IsFalling(); }
        public bool IsRunning() { return state.IsRunning(); }
        public bool IsClimbing() { return state.IsClimbing(); }
        public bool IsGrounded() { return state.Grounded(); }
        public bool IsJumping() { return state.IsJumping(); }
        public bool IsInvincible() { return state.IsInvincible(); }
        public bool IsOnEnemy() { return state.IsOnEnemy(); }

        public bool FacingRight() { return anim.FacingRight(); }

        public void FreezeFor(double x) { state.Freeze(x); anim.SetFreezeTime(x); }
        public bool IsFrozen() { return state.IsFrozen(); }
        public bool IsLurking() { return state.IsLurking(); }
        public void StopLurking() { state.SetLurking(false); }

        public AbstractBullet[] GetBullets() { return bullets; }
        public void KillBullets()
        {
            for (int i = 0; i <= 2; i++)
            {
                if (bullets[i] != null)
                    bullets[i].Kill();
            }
        }

        public int GetLastXMovement() { return (int)lastMove.X; }
        public int GetLastYMovement() { return (int)lastMove.Y; }


        public void StartWarp() { state.Warp(); }

    }
}