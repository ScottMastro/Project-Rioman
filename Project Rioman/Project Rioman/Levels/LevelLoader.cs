using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Project_Rioman
{
    class LevelLoader
    {

        Dictionary<string, int> letters = new Dictionary<string, int>();

        private Color backgroundColour;
        private Texture2D[] tileSprites = new Texture2D[500];
        private int width;
        private int height;
        private int[,] tilePos;
        private Vector2 startPos;

        private Tile[,] tiles;
        private List<AbstractEnemy> enemies;
        private OldPickup[] pickups = new OldPickup[10];
        private Boss[] bosses = new Boss[17];

        public LevelLoader() { }

        public void LoadLevelContent(ContentManager content)
        {

            SetLetters();

            //load level resources
            for (int i = 1; i <= 350; i++)
                tileSprites[i] = content.Load<Texture2D>("Video\\tiles\\" + i.ToString());

            for (int j = 0; j <= 9; j++)
            {
                pickups[j] = new OldPickup();
                pickups[j].LoadPickupSprites(content);

                bosses[j] = new Boss();
                bosses[j].LoadBoss(content, j);
            }
        }

        public Level Load(int level, ContentManager content)
        {
            string file = string.Format("Content/" + level.ToString() + ".txt");

            //Step1: Read in level

            if (File.Exists(file))
            {
                StreamReader read = new StreamReader(file);

                string rc = read.ReadLine();
                string[] parts = rc.Split('&');
                width = Convert.ToInt32(parts[0]);
                height = Convert.ToInt32(parts[1]);

                backgroundColour = new Color(Convert.ToByte(parts[2]), Convert.ToByte(parts[3]), Convert.ToByte(parts[4]));

                tilePos = new int[width + 1, height + 1];

                string line = "";

                for (int y = 0; y <= height - 1; y++)
                {
                    line = read.ReadLine();

                    for (int x = 0; x <= width * 2 - 2; x += 2)
                    {
                        if (line.Substring(x, 2) == "%%")
                            tilePos[x / 2, y] = 0;
                        else
                        {
                            int val = 0;
                            letters.TryGetValue(line.Substring(x, 2), out val);
                            tilePos[x / 2, y] = val;
                        }
                    }

                    line = "";
                }
                read.Close();

                //Step2: Convert string to tiles

                tiles = new Tile[width + 1, height + 1];
                enemies = new List<AbstractEnemy>();


                for (int x = 0; x <= width; x++)
                {
                    for (int y = 0; y <= height; y++)
                    {
                        CreateLevelElement(tilePos[x, y], x, y, content);

                        if (tilePos[x, y] == 284)
                            startPos = new Vector2(x * Constant.TILE_SIZE, y * Constant.TILE_SIZE - Constant.TILE_SIZE);
                    }
                }


                return new Level(backgroundColour, width, height, startPos, tiles,
                    enemies.ToArray(), pickups, bosses);

            }

            return null;
        }

        private void CreateLevelElement(int value, int x, int y, ContentManager content)
        {
            int type = Constant.TileNumberToType(value);

            //tile is a tile
            if (type >= 0)
                tiles[x, y] = new Tile(tileSprites[value], value, type, x, y, content);

            //tile is an enemy
            if (value >= 297 && value <= 317)
                CreateEnemy(value, x, y, content);

            //tile is a boss
            if (value == 289)
                bosses[5].SetBoss(x * Constant.TILE_SIZE, y * Constant.TILE_SIZE);
            if (value == 285)
                bosses[3].SetBoss(x * Constant.TILE_SIZE, y * Constant.TILE_SIZE);
        }

        private void CreateEnemy(int value, int x, int y, ContentManager content)
        {
            if (value == Constant.NEOLUCKY)
                enemies.Add(new Neolucky(value, x, y));
            else if (value == Constant.DOZERBOT)
                enemies.Add(new DozerBot(value, x, y));
            else if (value == Constant.DEUXKAMA)
                enemies.Add(new DeuxKama(value, x, y));
            else if (value == Constant.MACKS)
                enemies.Add(new Macks(value, x, y));
            else if (value == Constant.MACEBOT)
                enemies.Add(new Macebot(value, x, y));
            else if (value == Constant.PURIN)
                enemies.Add(new Purin(value, x, y));
            else if (value == Constant.FLIPSIDE)
                enemies.Add(new Flipside(value, x, y));
            else if (value == Constant.SPIKEBOMB)
                enemies.Add(new SpikeBomb(value, x, y));
            else if (value == Constant.CHANCEBOMB)
                enemies.Add(new ChanceBomb(value, x, y));
            else if (value == Constant.MEGAHOPPER)
                enemies.Add(new MegaHopper(value, x, y));
            else if (value == Constant.SERVERBOT)
                enemies.Add(new Serverbot(value, x, y));
            else if (value == Constant.TOTEM)
                enemies.Add(new Totem(value, x, y));
            else if (value == Constant.ZARROCCLONE)
                enemies.Add(new ZarrocClone(value, x, y));
            else if (value == Constant.MMUSHMECH)
                enemies.Add(new MushMech(value, x, y));
            else if (value == Constant.TMUSHMECH)
                enemies.Add(new MushMech(value, x, y));
            else if (value == Constant.P1H8R)
                enemies.Add(new P1H8R(value, x, y));
            else if (value == Constant.HELLICOPTOR)
                enemies.Add(new Hellicoptor(value, x, y));
            else if (value == Constant.KRONOS)
                enemies.Add(new Kronos(value, x, y));
        }

        private void SetLetters()
        {
            string crypt = "ͺͻͼͽ;΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϰϪЖ";

            int gap = 1;
            int counter = 0;

            for (int i = 1; i <= 500; i++)
            {
                letters.Add(crypt.Substring(counter, 1) + crypt.Substring(counter + gap, 1), i);

                counter++;

                if (counter + gap > 99)
                {
                    counter = 0;
                    gap++;
                }
            }
        }
    }
}
