using Microsoft.Xna.Framework.Input;
using System;

namespace Project_Rioman
{
    class RiomanState
    {
        private Rioman player;
        private enum State { running, standing, jumping, falling, climbing, warping };
        private State state;

        private bool animateWarp;

        private bool shooting;
        private double shootTime;

        private double jumpTime;

        private bool hit;

        private bool climbTop;

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
            shooting = false;
            climbTop = false;
            shootTime = 0;
            state = State.standing;
            lives = 3;
            jumpTime = 0;
        }

        public void Update(double deltaTime)
        {

            KeyboardState k = Keyboard.GetState();

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
                case State.jumping:
                    UpdateJump(k, deltaTime);
                    break;
                case State.climbing:
                    CheckJump(k);
                    break;
            }

            shootTime += deltaTime;
            if (shootTime > 0.5)
            {
                shooting = false;
                shootTime = 0;
            }

            prevKeyboardState = k;

        }

        private void UpdateJump(KeyboardState k, double deltaTime)
        {
            if (!k.IsKeyDown(Constant.JUMP) && jumpTime > 0.1)
            {
                player.isfalling = true;
                Fall();
            }

            jumpTime += deltaTime;
            if (jumpTime > 0.4)
            {
                player.isfalling = true;
                Fall();
            }

            player.touchedground = false;

        }


        private void CheckJump(KeyboardState k)
        {
            if (k.IsKeyDown(Constant.JUMP) && !prevKeyboardState.IsKeyDown(Constant.JUMP))
                Jump();

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

        public void Warp()
        {
            state = State.warping;
            hit = false;
        }
        public void StopWarping() { state = State.standing; }
        public void Run() { state = State.running; }
        public void Stand()
        {
            if (!IsWarping())
                state = State.standing;
        }

        public void Jump()
        {
            if (!IsWarping() && !IsFalling())
            {
                jumpTime = 0;
                if (state == State.climbing)
                    jumpTime = 0.2;

                state = State.jumping;
            }
        }

        public void Fall() { state = State.falling; }
        public void Climb()
        {
            if (!IsWarping())
            {
                state = State.climbing;
            }
        }
        public void StopShooting() { shooting = false; }
        public void Shoot()
        {
            shooting = true;
            shootTime = 0;
        }

        public void Hit()
        {
            if (!IsWarping())
                hit = true;
        }


        //life functions
        public void SetLives(int x) { lives = x; }
        public int GetLives() { return lives; }
        public void AdjustLivesByX(int x)
        {
            lives += x;


            if (lives <= 0)
            {
                GameState.SetState(2);
                Reset();
            }
        }

        public double GetJumpTime() { return jumpTime; }
        public bool IsClimbTop() { return climbTop == true; }
        public void SetClimbTopState(bool x) { climbTop = x; }
        public void SetAnimateWarp(bool x) { animateWarp = x; }
        public bool IsAnimateWarp() { return animateWarp == true; }

    }
}
