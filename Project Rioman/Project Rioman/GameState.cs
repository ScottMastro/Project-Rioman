using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    class GameState
    {

        //Singleton pattern so only one instance of gamestate is made
        private static volatile GameState instance;
        private GameState() { }

        public static GameState getInstance()
        {
            if (instance == null)
            {
                instance = new GameState();
            }
            return instance;
        }

        static GameState()
        {
            prevState = -1;
            state = 13;
            pause = false;

        }

        private static int state;
        private static int prevState;
        private static bool pause;


        public static int GetState() { return state; }
        public static void SetState(int value) {
            prevState = state;
            state = value; }
        public static int GetPrevState() { return prevState; }
        public static bool IsPaused() { return pause; }
        public static void Pause() { pause = true; }
        public static void Unpause() { pause = false; }
        public static void SwitchPause() { pause = !pause; }



    }
}