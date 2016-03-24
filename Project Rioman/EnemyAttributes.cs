using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    static class EnemyAttributes
    {

        Dictionary<int, int> maxHealth;
        Dictionary<int, Texture2D> spriteSheet;

        public static EnemyAttributes()
        {

        }

        private static void Init()
        {
            HealthInit();
        }

        private static void HealthInit()
        {
            maxHealth.Add(Constant.TOTEM, 2);
            maxHealth.Add(Constant.NEOLUCKY, 10);
            maxHealth.Add(Constant.KRONOS, 10);
            maxHealth.Add(Constant.PURIN, 3);
            maxHealth.Add(Constant.MOUSEHEAD, 25);
            maxHealth.Add(Constant.FLIPSIDE, 0);
            maxHealth.Add(Constant.MACEBOT, 4);
            maxHealth.Add(Constant.TR05D0R, 25);
            maxHealth.Add(Constant.SPIKEBOMB, 5);
            maxHealth.Add(Constant.MMUSHMECH, 5);
            maxHealth.Add(Constant.TMUSHMECH, 6);
            maxHealth.Add(Constant.CHANCEBOMB, 0);
            maxHealth.Add(Constant.ZARROCCLONE, 3);
            maxHealth.Add(Constant.DEUXKAMA, 0);
            maxHealth.Add(Constant.SERVERBOT, 4);
            maxHealth.Add(Constant.MEGAHOPPER, 6);
            maxHealth.Add(Constant.DOZERBOT, 7);
            maxHealth.Add(Constant.BLACKY, 5);
            maxHealth.Add(Constant.HELLICOPTOR, 4);
            maxHealth.Add(Constant.P1H8R, 4);
            maxHealth.Add(Constant.MACKS, 5);
        }

    }
}
