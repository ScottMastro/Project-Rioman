using System;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class RiomanState
    {
        private Rioman player;
        private enum State { running, standing, jumping, falling, climbing, warping };
        private State state;

        private bool animateWarp;

        private bool onEnemy;

        private bool shooting;
        private double shootTime;

        private double jumpTime;
        private double fallTime;

        private bool hit;
        private double hitTime;

        private double freezeTime;

        private bool lurking;
        private double lurkTime;

        private bool invincible;
        private double invincibleTime;
        private const double MAX_INVINCIBLE_TIME = 2;

        private bool climbTop;
        private bool stopClimbingUp;
        private bool onLadder;

        private int lives;
        private KeyboardState prevKeyboardState;

        public RiomanState(Rioman rioman)
        {
            player = rioman;
            Reset();

        }


        public void Reset()
        {
            hit = false;
            invincible = false;
            shooting = false;
            climbTop = false;
            shootTime = 0;
            state = State.standing;
            lives = 3;
            jumpTime = 0;
            fallTime = 0;
            freezeTime = 0;
            lurking = false;
        }

        public void Update(double deltaTime)
        {
            onEnemy = false;

            KeyboardState k = Keyboard.GetState();

            if (!IsFalling())
                fallTime = 0;

            switch (state)
            {
                case State.standing:
                    CheckRun(k);
                    CheckJump(k);
                    break;
                case State.running:
                    CheckStand(k);
                    CheckJump(k);
                    break;
                case State.falling:
                    UpdateFall(deltaTime);
                    break;
                case State.jumping:
                    UpdateJump(k, deltaTime);
                    break;
                case State.climbing:
                    if (!onLadder)
                        Fall();
                    CheckJump(k);
                    break;
            }

            shootTime += deltaTime;
            if (shootTime > 0.5)
            {
                shooting = false;
                shootTime = 0;
            }

            if (lurking)
            {
                lurkTime += deltaTime;
                if (lurkTime > 0.52)
                {
                    lurkTime = 0;
                    Weapons.Lurk();
                }
                if (Weapons.GetAmmo(Constant.LURKERBULLET) <= 0)
                    SetLurking(false);
            }

            if (hit)
                UpdateHit(deltaTime);
            if (invincible)
                UpdateInvincible(deltaTime);



            prevKeyboardState = k;

        }

        private void UpdateJump(KeyboardState k, double deltaTime)
        {
            jumpTime += deltaTime;

            if (!k.IsKeyDown(Constant.JUMP) && jumpTime > 0.1 || jumpTime > 0.4)
                Fall();

        }

        private void UpdateFall( double deltaTime)
        {
            fallTime += deltaTime;
        }

        private void UpdateHit(double deltaTime)
        {
            if(Grounded())
                hitTime += deltaTime;

            if (hitTime > 0.3)
            {
                hitTime = 0;
                hit = false;
            }
        }

        private void UpdateInvincible(double deltaTime)
        {
            invincibleTime += deltaTime;

            if (invincibleTime > MAX_INVINCIBLE_TIME)
            {
                invincibleTime = 0;
                invincible = false;
            }
        }


        private void CheckJump(KeyboardState k)
        {
            if (k.IsKeyDown(Constant.JUMP) && !prevKeyboardState.IsKeyDown(Constant.JUMP))
            {
                if (IsClimbing() && stopClimbingUp)
                    Fall();
                else Jump();
            }
        }

        private void CheckRun(KeyboardState k)
        {
            if (k.IsKeyDown(Constant.LEFT) || k.IsKeyDown(Constant.RIGHT))
                Run();
        }

        private void CheckStand(KeyboardState k)
        {
            if (!k.IsKeyDown(Constant.LEFT) && !k.IsKeyDown(Constant.RIGHT))
                Stand();

        }

        public bool IsWarping() { return state == State.warping; }
        public bool IsRunning() { return state == State.running; }
        public bool IsStanding() { return state == State.standing; }
        public bool IsJumping() { return state == State.jumping; }
        public bool IsFalling() { return state == State.falling; }
        public bool IsClimbing() { return state == State.climbing; }
        public bool IsShooting() { return shooting; }
        public bool CanShoot() { return !IsWarping() && !IsHit() && !IsClimbTop(); }


        public void Warp()
        {
            state = State.warping;
            hit = false;
            invincible = false;
        }
        public void SetOnEnemy() {
            onEnemy = true;
            Stand();
        }
        public void StopWarping() { state = State.standing; }
        public void Run() { state = State.running; }
        public void Stand()
        {
            if (!IsWarping())
            {
                if (IsFalling() || IsClimbing())
                    Audio.PlayLand();

                state = State.standing;


            }
        }

        public void Jump()
        {
            if (!IsWarping() && !IsFalling() && !IsHit())
            {
                jumpTime = 0;
                if (state == State.climbing)
                    jumpTime = 0.2;

                state = State.jumping;
            }
        }

        public void Fall() {
            if(!onEnemy)
                state = State.falling;
        }
        public void Climb()
        {
            if (!IsWarping() && !IsHit())
            {
                state = State.climbing;
            }
        }
        public void StopShooting() { shooting = false; }
        public void Shoot()
        {
            if (CanShoot())
            {
                shooting = true;
                shootTime = 0;
            }
        }

        public bool Hit()
        {
            bool hitBefore = IsHit();

            if (!IsHit() && !IsInvincible() && !IsWarping() && !IsLurking())
            {
                freezeTime = 0;
                invincibleTime = 0;
                hitTime = 0;
                hit = true;
                invincible = true;
                Fall();
            }

            bool hitAfter = IsHit();

            return !hitBefore && hitAfter;
        }


        //life functions
        public void SetLives(int x) { lives = x; }
        public int GetLives() { return lives; }
        public void AdjustLivesByX(int x)
        {
            lives += x;


            if (lives <= 0)
            {
                GameState.SetState(Constant.SELECTION_SCREEN);
                Reset();
            }
        }

        public double GetJumpTime() { return jumpTime; }
        public double GetFallTime() { return fallTime; }

        public bool IsClimbTop() { return climbTop == true; }
        public void SetClimbTopState(bool x) { climbTop = x; }
        public void SetAnimateWarp(bool x) { animateWarp = x; }
        public bool IsAnimateWarp() { return animateWarp == true; }
        public bool IsHit() { return hit; }
        public bool IsLurking() { return lurking; }
        public void SetLurking(bool lurking) { this.lurking = lurking; }
        public bool IsInvincible() { return invincible; }
        public bool IsOnEnemy() { return onEnemy; }
        public bool Airborne() { return IsFalling() || IsJumping(); }
        public bool Grounded() { return IsStanding() || IsRunning() || IsOnEnemy(); }
        public bool GetStopClimbUp() { return stopClimbingUp; }
        public void SetStopClimbUp(bool value) { stopClimbingUp = value; }
        public void SetOnLadder(bool value) { onLadder = value; }

        public void Freeze(double x) {
            if (!IsWarping())
                freezeTime += x; }
        public double GetFreezeTime() { return freezeTime; }
        public void UpdateFreezeTime(double deltaTime) { freezeTime -= deltaTime; }
        public bool IsFrozen() { return freezeTime > 0; }

    }
}
