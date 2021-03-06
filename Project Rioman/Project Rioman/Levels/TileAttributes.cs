﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    static class TileAttributes
    {
        private static int[] tileKey;
        public static int NUMBER_OF_TILES = 371;

        private static Dictionary<int, Texture2D[]> tileSprites;

        public static bool IsSolid(AbstractTile tile)
        {
            if (tile == null)
                return false;

            if (tile.Type == Constant.TILE_SOLID || tile.Type == Constant.TILE_DISAPPEAR ||
                tile.Type == Constant.TILE_DOOR || tile.Type == Constant.TILE_CLIMB && tile.IsTop
                || tile.Type == Constant.TILE_CONVEYOR || tile.Type == Constant.TILE_FALL || tile.Type == Constant.TILE_PISTON)
                return true;

            return false;
        }

        public static AbstractTile CreateTile(int ID, int x, int y)
        {

            if (tileKey[ID] == Constant.TILE_IGNORE)
                return null;

            else if (tileKey[ID] == Constant.TILE_FALL)
                return new FallTile(ID, x, y);

            else if (tileKey[ID] == Constant.TILE_CONVEYOR)
            {
                int dir = 1;

                if (ID >= 365 && ID <= 367)
                    dir = -1;

                return new ConveyorTile(ID, x, y, dir);
            }

            else if (tileKey[ID] == Constant.TILE_LASER)
                return new LaserTile(ID, x, y);

            else if (tileKey[ID] == Constant.TILE_DISAPPEAR)
                return new DisappearTile(ID, x, y);

            else if (tileKey[ID] == Constant.TILE_PISTON)
                return new PistonTile(ID, x, y);

            else if (tileKey[ID] == Constant.TILE_BOUNCE)
            {
                if (ID == 88)
                    return new BounceTile(ID, x, y, 15);

                return new BounceTile(ID, x, y, 10);

            }

            else if (tileKey[ID] == Constant.TILE_MOVING)
            {
                if (ID == 77)
                    return new MovingTile(ID, x, y, true);
                else if (ID == 78)
                    return new MovingTile(ID, x, y, false);

                return new MovingTile(ID, x, y, true);
            }

            return new Tile(ID, x, y);

        }

        public static void LoadContent(ContentManager content)
        {
            MakeKey();

            tileSprites = new Dictionary<int, Texture2D[]>();

            for (int i = 1; i <= NUMBER_OF_TILES; i++)
                LoadFrames(content, i);

            LoadPiston(content);

        }

        private static void LoadPiston(ContentManager content)
        {
            List<Texture2D> textures = new List<Texture2D>();
            for (int i = 134; i <= 137; i++)
                textures.Add(content.Load<Texture2D>("Video\\tiles\\" + i.ToString() ));

            tileSprites.Add(-Constant.TILE_PISTON, textures.ToArray());

        }

        private static void LoadFrames(ContentManager content, int tileID)
        {

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("Video\\tiles\\" + tileID.ToString()));
            int counter = 1;

            while (true)
            {
                try
                {
                    Texture2D t = content.Load<Texture2D>("Video\\tiles\\" + tileID.ToString() + "-" + counter.ToString());
                    textures.Add(t);
                }
                catch (ContentLoadException e)
                {
                    break;
                }

                counter++;
            }

            tileSprites.Add(tileID, textures.ToArray());
        }

        public static Texture2D[] GetSprites(int tileID)
        {
            Texture2D[] result;
            tileSprites.TryGetValue(tileID, out result);
            return result;
        }

        public static int TileIDToType(int tileID)
        {
            if (tileID > NUMBER_OF_TILES)
                return Constant.TILE_IGNORE;

            return tileKey[tileID];
        }


        public static void MakeKey()
        {
            tileKey = new int[NUMBER_OF_TILES + 1];

            tileKey[0] = Constant.TILE_IGNORE;

            tileKey[1] = 1;
            tileKey[2] = 1;
            tileKey[3] = 1;
            tileKey[4] = 1;
            tileKey[5] = 1;
            tileKey[6] = 1;
            tileKey[7] = 2;
            tileKey[8] = 2;
            tileKey[9] = 3;
            tileKey[10] = 4;
            tileKey[11] = 1;
            tileKey[12] = 1;
            tileKey[13] = 1;
            tileKey[14] = 1;
            tileKey[15] = 1;
            tileKey[16] = 1;
            tileKey[17] = 0;
            tileKey[18] = -1;
            tileKey[19] = -1;
            tileKey[20] = -1;
            tileKey[21] = -1;
            tileKey[22] = -1;
            tileKey[23] = -1;
            tileKey[24] = -1;
            tileKey[25] = -1;
            tileKey[26] = -1;
            tileKey[27] = -1;
            tileKey[28] = -1;
            tileKey[29] = -1;
            tileKey[30] = -1;
            tileKey[31] = -1;
            tileKey[32] = -1;
            tileKey[33] = -1;
            tileKey[34] = -1;
            tileKey[35] = -1;
            tileKey[36] = -1;
            tileKey[37] = -1;
            tileKey[38] = -1;
            tileKey[39] = 1;
            tileKey[40] = 1;
            tileKey[41] = 1;
            tileKey[42] = 1;
            tileKey[43] = 1;
            tileKey[44] = 1;
            tileKey[45] = 1;
            tileKey[46] = -1;
            tileKey[47] = -1;
            tileKey[48] = -1;
            tileKey[49] = -1;
            tileKey[50] = 1;
            tileKey[51] = 1;
            tileKey[52] = 1;
            tileKey[53] = 1;
            tileKey[54] = 1;
            tileKey[55] = 1;
            tileKey[56] = -1;
            tileKey[57] = -1;
            tileKey[58] = 4;
            tileKey[59] = 1;
            tileKey[60] = 1;
            tileKey[61] = 1;
            tileKey[62] = 1;
            tileKey[63] = 1;
            tileKey[64] = 1;
            tileKey[65] = 1;
            tileKey[66] = 1;
            tileKey[67] = 1;
            tileKey[68] = 1;
            tileKey[69] = 1;
            tileKey[70] = 1;
            tileKey[71] = 1;
            tileKey[72] = 1;
            tileKey[73] = 1;
            tileKey[74] = 1;
            tileKey[75] = 4;
            tileKey[76] = 3;
            tileKey[77] = 12;
            tileKey[78] = 12;
            tileKey[79] = 6;
            tileKey[80] = 6;
            tileKey[81] = 6;
            tileKey[82] = 6;
            tileKey[83] = 2;
            tileKey[84] = 2;
            tileKey[85] = 1;
            tileKey[86] = 1;
            tileKey[87] = 1;
            tileKey[88] = 11;
            tileKey[89] = 1;
            tileKey[90] = 1;
            tileKey[91] = 1;
            tileKey[92] = 1;
            tileKey[93] = 1;
            tileKey[94] = -1;
            tileKey[95] = -1;
            tileKey[96] = -1;
            tileKey[97] = -1;
            tileKey[98] = -1;
            tileKey[99] = -1;
            tileKey[100] = -1;
            tileKey[101] = -1;
            tileKey[102] = 2;
            tileKey[103] = 1;
            tileKey[104] = 1;
            tileKey[105] = 1;
            tileKey[106] = 5;
            tileKey[107] = 4;
            tileKey[108] = 3;
            tileKey[109] = 1;
            tileKey[110] = 0;
            tileKey[111] = 0;
            tileKey[112] = 0;
            tileKey[113] = 0;
            tileKey[114] = 1;
            tileKey[115] = 1;
            tileKey[116] = 1;
            tileKey[117] = 1;
            tileKey[118] = 1;
            tileKey[119] = 1;
            tileKey[120] = 4;
            tileKey[121] = 3;
            tileKey[122] = 1;
            tileKey[123] = 2;
            tileKey[124] = 0;
            tileKey[125] = 0;
            tileKey[126] = 0;
            tileKey[127] = 0;
            tileKey[128] = 0;
            tileKey[129] = 0;
            tileKey[130] = 5;
            tileKey[131] = 1;
            tileKey[132] = 1;
            tileKey[133] = 8;
            tileKey[134] = 10;
            tileKey[135] = 1;
            tileKey[136] = 1;
            tileKey[137] = 1;
            tileKey[138] = 1;
            tileKey[139] = 1;
            tileKey[140] = 1;
            tileKey[141] = 0;
            tileKey[142] = 1;
            tileKey[143] = 0;
            tileKey[144] = 0;
            tileKey[145] = 0;
            tileKey[146] = 0;
            tileKey[147] = 0;
            tileKey[148] = 0;
            tileKey[149] = 2;
            tileKey[150] = 4;
            tileKey[151] = 3;
            tileKey[152] = 7;
            tileKey[153] = 1;
            tileKey[154] = -1;
            tileKey[155] = -1;
            tileKey[156] = -1;
            tileKey[157] = -1;
            tileKey[158] = -1;
            tileKey[159] = -1;
            tileKey[160] = -1;
            tileKey[161] = -1;
            tileKey[162] = -1;
            tileKey[163] = -1;
            tileKey[164] = -1;
            tileKey[165] = -1;
            tileKey[166] = -1;
            tileKey[167] = -1;
            tileKey[168] = -1;
            tileKey[169] = -1;
            tileKey[170] = -1;
            tileKey[171] = -1;
            tileKey[172] = -1;
            tileKey[173] = -1;
            tileKey[174] = -1;
            tileKey[175] = -1;
            tileKey[176] = -1;
            tileKey[177] = -1;
            tileKey[178] = -1;
            tileKey[179] = -1;
            tileKey[180] = -1;
            tileKey[181] = -1;
            tileKey[182] = -1;
            tileKey[183] = -1;
            tileKey[184] = -1;
            tileKey[185] = -1;
            tileKey[186] = -1;
            tileKey[187] = -1;
            tileKey[188] = -1;
            tileKey[189] = -1;
            tileKey[190] = -1;
            tileKey[191] = -1;
            tileKey[192] = -1;
            tileKey[193] = -1;
            tileKey[194] = -1;
            tileKey[195] = -1;
            tileKey[196] = -1;
            tileKey[197] = -1;
            tileKey[198] = -1;
            tileKey[199] = -1;
            tileKey[200] = -1;
            tileKey[201] = -1;
            tileKey[202] = -1;
            tileKey[203] = -1;
            tileKey[204] = -1;
            tileKey[205] = -1;
            tileKey[206] = -1;
            tileKey[207] = -1;
            tileKey[208] = -1;
            tileKey[209] = -1;
            tileKey[210] = -1;
            tileKey[211] = -1;
            tileKey[212] = -1;
            tileKey[213] = -1;
            tileKey[214] = -1;
            tileKey[215] = -1;
            tileKey[216] = -1;
            tileKey[217] = -1;
            tileKey[218] = -1;
            tileKey[219] = -1;
            tileKey[220] = -1;
            tileKey[221] = -1;
            tileKey[222] = -1;
            tileKey[223] = -1;
            tileKey[224] = -1;
            tileKey[225] = -1;
            tileKey[226] = -1;
            tileKey[227] = -1;
            tileKey[228] = -1;
            tileKey[229] = -1;
            tileKey[230] = -1;
            tileKey[231] = -1;
            tileKey[232] = -1;
            tileKey[233] = -1;
            tileKey[234] = -1;
            tileKey[235] = -1;
            tileKey[236] = -1;
            tileKey[237] = -1;
            tileKey[238] = -1;
            tileKey[239] = -1;
            tileKey[240] = -1;
            tileKey[241] = -1;
            tileKey[242] = -1;
            tileKey[243] = -1;
            tileKey[244] = -1;
            tileKey[245] = -1;
            tileKey[246] = -1;
            tileKey[247] = -1;
            tileKey[248] = -1;
            tileKey[249] = -1;
            tileKey[250] = -1;
            tileKey[251] = -1;
            tileKey[252] = -1;
            tileKey[253] = -1;
            tileKey[254] = -1;
            tileKey[255] = -1;
            tileKey[256] = -1;
            tileKey[257] = -1;
            tileKey[258] = -1;
            tileKey[259] = -1;
            tileKey[260] = -1;
            tileKey[261] = -1;
            tileKey[262] = -1;
            tileKey[263] = -1;
            tileKey[264] = -1;
            tileKey[265] = -1;
            tileKey[266] = -1;
            tileKey[267] = -1;
            tileKey[268] = -1;
            tileKey[269] = -1;
            tileKey[270] = -1;
            tileKey[271] = -1;
            tileKey[272] = -1;
            tileKey[273] = -1;
            tileKey[274] = -1;
            tileKey[275] = -1;
            tileKey[276] = -1;
            tileKey[277] = -1;
            tileKey[278] = -1;
            tileKey[279] = -1;
            tileKey[280] = -1;
            tileKey[281] = -1;
            tileKey[282] = -1;
            tileKey[283] = -1;
            tileKey[284] = -1;
            tileKey[285] = -1;
            tileKey[286] = -1;
            tileKey[287] = -1;
            tileKey[288] = -1;
            tileKey[289] = -1;
            tileKey[290] = -1;
            tileKey[291] = -1;
            tileKey[292] = -1;
            tileKey[293] = -1;
            tileKey[294] = -1;
            tileKey[295] = -1;
            tileKey[296] = -1;
            tileKey[297] = -1;
            tileKey[298] = -1;
            tileKey[299] = -1;
            tileKey[300] = -1;
            tileKey[301] = -1;
            tileKey[302] = -1;
            tileKey[303] = -1;
            tileKey[304] = -1;
            tileKey[305] = -1;
            tileKey[306] = -1;
            tileKey[307] = -1;
            tileKey[308] = -1;
            tileKey[309] = -1;
            tileKey[310] = -1;
            tileKey[311] = -1;
            tileKey[312] = -1;
            tileKey[313] = -1;
            tileKey[314] = -1;
            tileKey[315] = -1;
            tileKey[316] = -1;
            tileKey[317] = -1;
            tileKey[318] = 6;
            tileKey[319] = 6;
            tileKey[320] = 6;
            tileKey[321] = 6;
            tileKey[322] = -1;
            tileKey[323] = 2;
            tileKey[324] = 2;
            tileKey[325] = 2;
            tileKey[326] = 2;
            tileKey[327] = 2;
            tileKey[328] = 2;
            tileKey[329] = 2;
            tileKey[330] = 7;
            tileKey[331] = 7;
            tileKey[332] = 7;
            tileKey[333] = -1;
            tileKey[334] = 2;
            tileKey[335] = 2;
            tileKey[336] = -1;
            tileKey[337] = -1;
            tileKey[338] = -1;
            tileKey[339] = -1;
            tileKey[340] = -1;
            tileKey[341] = 1;
            tileKey[342] = 1;
            tileKey[343] = -1;
            tileKey[344] = -1;
            tileKey[345] = -1;
            tileKey[346] = -1;
            tileKey[347] = -1;
            tileKey[348] = -1;
            tileKey[349] = -1;
            tileKey[350] = -1;
            tileKey[351] = 6;
            tileKey[352] = 1;
            tileKey[353] = 1;
            tileKey[354] = 1;
            tileKey[355] = 0;
            tileKey[356] = 0;
            tileKey[357] = 0;
            tileKey[358] = 0;
            tileKey[359] = 0;
            tileKey[360] = 0;
            tileKey[361] = 1;
            tileKey[362] = 1;
            tileKey[363] = 1;
            tileKey[364] = 1;
            tileKey[365] = 9;
            tileKey[366] = 9;
            tileKey[367] = 9;
            tileKey[368] = 1;
            tileKey[369] = 9;
            tileKey[370] = 9;
            tileKey[371] = 9;
        }
    }
}
