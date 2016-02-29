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

        public static Keys SHOOT { get { return Keys.Z; } }
        public static Keys JUMP { get { return Keys.X; } }

        public static Keys LEFT { get { return Keys.Left; } }
        public static Keys RIGHT { get { return Keys.Right; } }
        public static Keys UP { get { return Keys.Up; } }
        public static Keys DOWN { get { return Keys.Down; } }

    }
}
