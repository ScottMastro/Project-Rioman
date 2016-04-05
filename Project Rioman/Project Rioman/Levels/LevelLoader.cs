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
        private Texture2D[] tileSprites = new Texture2D[322];
        private int width;
        private int height;
        private int[,] tilePos;
        private Vector2 startPos;

        private Tile[,] tiles;
        private List<Enemy> enemies;
        private List<AbstractEnemy> aenemy;
        private Pickup[] pickups = new Pickup[10];
        private Boss[] bosses = new Boss[17];

        public LevelLoader() { }

        public void LoadLevelContent(ContentManager content) {

            SetLetters();

            //load level resources
            for (int i = 1; i <= 321; i++)
                tileSprites[i] = content.Load<Texture2D>("Video\\tiles\\" + i.ToString());

            for (int j = 0; j <= 9; j++)
            {
                pickups[j] = new Pickup();
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
                height = Convert.ToInt32(parts[0]);
                width = Convert.ToInt32(parts[1]);

                backgroundColour = new Color(Convert.ToByte(parts[2]), Convert.ToByte(parts[3]), Convert.ToByte(parts[4]));

                tilePos = new int[height + 1, width + 1];

                string line = "";

                for (int r = 0; r <= height - 1; r++)
                {
                    line = read.ReadLine();

                    for (int c = 0; c <= width * 2 - 2; c += 2)
                    {
                        if (line.Substring(c, 2) == "%%")
                            tilePos[r, c / 2] = 0;
                        else
                        {
                            int val = 0;
                            letters.TryGetValue(line.Substring(c, 2), out val);
                            tilePos[r, c / 2] = val;
                        }
                    }

                    line = "";
                }
                read.Close();

                //Step2: Convert string to tiles

                tiles = new Tile[height + 1, width + 1];
                enemies = new List<Enemy>();
                aenemy = new List<AbstractEnemy>();


                for (int r = 0; r <= height; r++)
                {
                    for (int c = 0; c <= width; c++)
                    {
                        CreateLevelElement(tilePos[r, c], r, c, content);

                        if (tilePos[r, c] == 284)
                            startPos = new Vector2(c * 32, r * 32 - 15);
                    }
                }


                return new Level(backgroundColour, width, height, startPos, tiles, 
                    enemies.ToArray(), aenemy.ToArray(), pickups, bosses);

            }

            return null;
        }

        private void CreateLevelElement(int value, int r, int c, ContentManager content)
        {
           int type = Constant.TileNumberToType(value);

            //tile is a tile
            if (type >= 0)
                tiles[r, c] = new Tile(tileSprites[value], value, type, r, c, content);

            //tile is an enemy
            if (value >= 297 && value <= 317)
                CreateEnemy(value, r, c, content);

            //tile is a boss
            if (value == 289)
                bosses[5].SetBoss(c * 32, r * 32);
            if (value == 285)
                bosses[3].SetBoss(c * 32, r * 32);
        }

        private void CreateEnemy(int value, int r, int c, ContentManager content)
        {
            if (value == Constant.NEOLUCKY)
                aenemy.Add(new Neolucky(value, r, c));
            else if (value == Constant.DOZERBOT)
                aenemy.Add(new DozerBot(value, r, c));
            else if (value == Constant.DEUXKAMA)
                aenemy.Add(new DeuxKama(value, r, c));
            else if (value == Constant.MACKS)
                aenemy.Add(new Macks(value, r, c));
            else if (value == Constant.MACEBOT)
                aenemy.Add(new Macebot(value, r, c));
            else if (value == Constant.PURIN)
                aenemy.Add(new Purin(value, r, c));
            else if (value == Constant.FLIPSIDE)
                aenemy.Add(new Flipside(value, r, c));
            else if (value == Constant.SPIKEBOMB)
                aenemy.Add(new SpikeBomb(value, r, c));
            else if (value == Constant.CHANCEBOMB)
                aenemy.Add(new ChanceBomb(value, r, c));
            else if (value == Constant.MEGAHOPPER)
                aenemy.Add(new MegaHopper(value, r, c));
            else if (value == Constant.SERVERBOT)
                aenemy.Add(new Serverbot(value, r, c));
            else if (value == Constant.TOTEM)
                aenemy.Add(new Totem(value, r, c));
            else if (value >= 297 && value <= 317)
            {
                enemies.Add(new Enemy(value, r, c, content));
            }

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
