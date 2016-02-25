using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    class RiomanState
    {
        private enum State { warping, running, jumping, falling, climbing };
        private bool shooting;
        private bool invincible;
        private bool touchedground;

        private float climbtime;
        private float shoottime;
        private float falltime;
        private float jumptime;
        private float invincibletime;
        private float pausetime;
        private float touchtime;
        private int invincibledirection;

        private int lives;

        public RiomanState()
        {
            Reset();
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


        public void Reset()
        {
            lives = 3;
        }

    }
}
