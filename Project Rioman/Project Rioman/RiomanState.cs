using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class RiomanState
    {
        private Rioman player;
        private enum State {running, standing, jumping, falling, climbing};
        private State state;

        private bool shooting;      
        private double shootTime;

        private double jumpTime;

        private bool climbTop;

        private int lives;
        private KeyboardState prevKeyboardState;

        public RiomanState(Rioman rioman)
        {
            player = rioman;
            Reset();
            shooting = false;
            climbTop = false;
            shootTime = 0;
        }


        public void Reset()
        {
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
            if (jumpTime > 0.4) { 
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

        public bool IsRunning() { return state == State.running; }
        public bool IsStanding() { return state == State.standing; }
        public bool IsJumping() { return state == State.jumping; }
        public bool IsFalling() { return state == State.falling; }
        public bool IsClimbing() { return state == State.climbing; }
        public bool IsShooting() { return shooting; }

        public void Run() { state = State.running; }
        public void Stand() { state = State.standing; }

        public void Jump() {
            jumpTime = 0;
            if (state == State.climbing)
                jumpTime = 0.2;

            state = State.jumping;
        }

        public void Fall() { state = State.falling; }
        public void Climb() { state = State.climbing; }

        public void Shoot() {
            shooting = true;
            shootTime = 0;  }




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


    }
}
