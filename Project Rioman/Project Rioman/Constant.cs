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

        public static float VOLUME = 0.1f;

        public static int TILE_SIZE = 32;
        public static int MAX_HEALTH = 27;

        public static int HEALTH_DROP_PERCENT_SMALL = 6;
        public static int HEALTH_DROP_PERCENT_BIG = 3;
        public static int AMMO_DROP_PERCENT_SMALL = 6;
        public static int AMMO_DROP_PERCENT_BIG = 3;

        public static int ETANK = 10;
        public static int MTANK = 11;
        public static int WTANK = 12;
        public static int LIFE = 13;
        public static int BIG_HEALTH = 14;
        public static int SMALL_HEALTH = 15;
        public static int BIG_AMMO = 16;
        public static int SMALL_AMM0 = 17;

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

        public static int TOTEM = 297;
        public static int NEOLUCKY = 298;
        public static int KRONOS = 299;
        public static int PURIN = 300;
        public static int MOUSEHEAD = 301;
        public static int FLIPSIDE = 302;
        public static int MACEBOT = 303;
        public static int TR05D0R = 304;
        public static int SPIKEBOMB = 305;
        public static int MMUSHMECH = 306;
        public static int TMUSHMECH = 307;
        public static int CHANCEBOMB = 308;
        public static int ZARROCCLONE = 309;
        public static int DEUXKAMA = 310;
        public static int SERVERBOT = 311;
        public static int MEGAHOPPER = 312;
        public static int DOZERBOT = 313;
        public static int BLACKY = 314;
        public static int HELLICOPTOR = 315;
        public static int P1H8R = 316;
        public static int MACKS = 317;

        public static int VERT_SCROLL2 = 318;
        public static int VERT_SCROLL = 319;
        public static int HORIZ_SCROLL = 320;
        public static int HORIZ_SCROLL2 = 321;

        public static int TileNumberToType(int tileNumber)
        {
            int type = -1;

            if (tileNumber >= 110 && tileNumber <= 113 || tileNumber == 17)
                type = 0;
            if (tileNumber >= 1 && tileNumber <= 6 || tileNumber == 15 || tileNumber == 16)
                type = 1;
            if (tileNumber >= 103 && tileNumber <= 105 || tileNumber == 109)
                type = 1;
            if (tileNumber >= 39 && tileNumber <= 45 || tileNumber >= 50 && tileNumber <= 55)
                type = 1;
            if (tileNumber == 58)
                type = 4;
            if (tileNumber == 102 || tileNumber == 7 || tileNumber == 8)
                type = 2;
            if (tileNumber == 108 || tileNumber == 9)
                type = 3;
            if (tileNumber == 107)
                type = 4;
            if (tileNumber == 106)
                type = 5;
            if (tileNumber >= 318 && tileNumber <= 331)
                type = 6;

            return type;
        }
    }
}
