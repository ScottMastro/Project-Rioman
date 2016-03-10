using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    public static class Constant
    {
        public static Keys END_GAME { get { return Keys.Escape; } }
        public static Keys FULL_SCREEN { get { return Keys.F11; } }

        public static Keys PAUSE { get { return Keys.Enter; } }
        public static Keys CONFIRM { get { return Keys.Enter; } }

        public static Keys SHOOT { get { return Keys.Z; } }
        public static Keys JUMP { get { return Keys.X; } }

        public static Keys LEFT { get { return Keys.Left; } }
        public static Keys RIGHT { get { return Keys.Right; } }
        public static Keys UP { get { return Keys.Up; } }
        public static Keys DOWN { get { return Keys.Down; } }



        public static int TITLE_SCREEN = 0;
        public static int SELECTION_SCREEN = 1;
        public static int LOAD_SCREEN = 2;

        public static int AURORAMAN = 11;
        public static int BUNNYMAN = 18;
        public static int CLOVERMAN = 17;
        public static int GEOGIRL = 10;
        public static int INFERNOMAN = 13;
        public static int LURKERMAN = 12;
        public static int POSTERMAN = 15;
        public static int TOXICMAN = 16;
        public static int MUSH = 14;

    }
}
