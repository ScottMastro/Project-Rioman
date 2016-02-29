using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class RiomanState
    {
        private enum State { running, standing};
        private State state;

        private bool shooting;      
        private double shootTime;

        private int lives;

        public RiomanState()
        {
            Reset();
            shooting = false;
            shootTime = 0;
        }


        public void Reset()
        {
            state = State.standing;
            lives = 3;
        }

        public void Update(double deltaTime)
        {

            KeyboardState k = Keyboard.GetState();

            switch (state)
            {
                case State.standing:
                    if (k.IsKeyDown(Constant.LEFT) || k.IsKeyDown(Constant.RIGHT))
                        Run();
                    break;
                case State.running:
                    if (!k.IsKeyDown(Constant.LEFT) && !k.IsKeyDown(Constant.RIGHT))
                        Stand();
                    break;
            }

            shootTime += deltaTime;
            if (shootTime > 0.5)
            {
                shooting = false;
                shootTime = 0;
            }

        }


        public bool IsRunning() { return state == State.running; }
        public bool IsStanding() { return state == State.standing; }
        public bool IsShooting() { return shooting; }

        public void Run() { state = State.running; }
        public void Stand() { state = State.standing; }
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
    }
}
