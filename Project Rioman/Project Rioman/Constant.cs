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
        public static int MAX_BULLET = 10;
        public static int MAX_AMMO = 27;

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
        public static int SMALL_AMMO = 17;

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


        public static int RIOBULLET = 0;
        public static int GEOBULLET = 1;
        public static int AURORABULLET = 2;
        public static int LURKERBULLET = 3;
        public static int INFERNOBULLET = 4;
        public static int DUELBULLET = 12;
        public static int POSTERBULLET = 6;
        public static int TOXICBULLET = 7;
        public static int CLOVERBULLET = 8;
        public static int BUNNYBULLET = 9;


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
        public static int RESPAWN = 351;

        public static int TILE_IGNORE = -1;
        public static int TILE_DECO = 0;
        public static int TILE_SOLID = 1;
        public static int TILE_KILL = 2;
        public static int TILE_CLIMB = 3;
        public static int TILE_DOOR = 4;
        public static int TILE_DISAPPEAR = 5;
        public static int TILE_FUNCTION = 6;
        public static int TILE_LASER = 7;
        public static int TILE_FALL = 8;

    }
}
