using System;
using System.Collections.Generic;

namespace Project_Rioman
{
    public static class EnemyAttributes
    {

        private static Dictionary<int, int> maxHealth;
        private static Dictionary<int, int> damage;

        static EnemyAttributes()
        {
            Init();
        }

        private static void Init()
        {
            HealthInit();
            DamageInit();
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
        private static void DamageInit()
        {
            damage.Add(Constant.TOTEM, 2);
            damage.Add(Constant.NEOLUCKY, 4);
            damage.Add(Constant.KRONOS, 0);
            damage.Add(Constant.PURIN, 2);
            damage.Add(Constant.MOUSEHEAD, 5);
            damage.Add(Constant.FLIPSIDE, 500);
            damage.Add(Constant.MACEBOT, 6);
            damage.Add(Constant.TR05D0R, 5);
            damage.Add(Constant.SPIKEBOMB, 3);
            damage.Add(Constant.MMUSHMECH, 4);
            damage.Add(Constant.TMUSHMECH, 7);
            damage.Add(Constant.CHANCEBOMB, 2);
            damage.Add(Constant.ZARROCCLONE, 2);
            damage.Add(Constant.DEUXKAMA, 5);
            damage.Add(Constant.SERVERBOT, 4);
            damage.Add(Constant.MEGAHOPPER, 6);
            damage.Add(Constant.DOZERBOT, 500);
            damage.Add(Constant.BLACKY, 3);
            damage.Add(Constant.HELLICOPTOR, 4);
            damage.Add(Constant.P1H8R, 3);
            damage.Add(Constant.MACKS, 4);
        }
       
        public int GetDamageAttribute(int type)
        {
            int value;
            damage.TryGetValue(type, out value);

            return value;
        }

        public int GetMaxHealthAttribute(int type)
        {
            int value;
            maxHealth.TryGetValue(type, out value);

            return value;
        }
    }
}
