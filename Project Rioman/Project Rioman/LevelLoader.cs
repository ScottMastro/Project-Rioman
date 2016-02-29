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
        public int activelevel;

        Dictionary<string, int> letters = new Dictionary<string, int>();

        public Color backgroundcolour;
        public Texture2D[] tilesimg = new Texture2D[322];
        public int width;
        public int height;
        public int[,] tilepos;
        public Vector2 startpos;

        public Tile[,] tiles;
        public Enemy[] enemies = new Enemy[500];
        public Pickup[] pickups = new Pickup[10];
        public Boss[] bosses = new Boss[17];
        int numberofenemies;

        public LevelLoader() { }

        public void LoadLevelContent (ContentManager content) {

            SetLetters();

            //load level resources
            for (int i = 1; i <= 321; i++)
                tilesimg[i] = content.Load<Texture2D>("Video\\tiles\\" + i.ToString());

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

                backgroundcolour = new Color(Convert.ToByte(parts[2]), Convert.ToByte(parts[3]), Convert.ToByte(parts[4]));

                tilepos = new int[height + 1, width + 1];

                string line = "";

                for (int r = 0; r <= height - 1; r++)
                {
                    line = read.ReadLine();

                    for (int c = 0; c <= width * 2 - 2; c += 2)
                    {
                        if (line.Substring(c, 2) == "%%")
                            tilepos[r, c / 2] = 0;
                        else
                        {
                            int val = 0;
                            letters.TryGetValue(line.Substring(c, 2), out val);
                            tilepos[r, c / 2] = val;
                        }
                    }

                    line = "";
                }
                read.Close();

                //Step2: Convert string to tiles

                tiles = new Tile[height + 1, width + 1];
                numberofenemies = 0;

                for (int r = 0; r <= height; r++)
                {
                    for (int c = 0; c <= width; c++)
                    {
                        CreateTile(tilepos[r, c], r, c);

                        if (tilepos[r, c] == 284)
                            startpos = new Vector2(c * 32, r * 32 - 15);
                    }
                }

                //Step3: Convert string to enemies

                CreateEnemies(content);

                return new Level(backgroundcolour, width, height, startpos, 
                    tiles, enemies, pickups, bosses);

            }

            return null;
        }

        private void CreateTile(int tile, int r, int c)
        {
            int type = -1;

            if (tile >= 110 && tile <= 113)
                type = 0;
            if (tile >= 1 && tile <= 6 || tile == 15 || tile == 16)
                type = 1;
            if (tile >= 103 && tile <= 105 || tile == 109)
                type = 1;
            if (tile >= 39 && tile <= 45 || tile >= 50 && tile <= 55)
                type = 1;
            if (tile == 58)
                type = 4;
            if (tile == 102 || tile == 7 || tile == 8)
                type = 2;
            if (tile == 108 || tile == 9)
                type = 3;
            if (tile == 107)
                type = 4;
            if (tile == 106)
                type = 5;
            if (tile >= 318 && tile <= 321)
                type = 6;

            if (type >= 0)
                tiles[r, c] = new Tile(tilesimg[tile], tile, type, r * 32, c * 32);

            if (tile >= 297 && tile <= 317)
            {
                enemies[numberofenemies] = new Enemy(tile, c * 32, r * 32);
                numberofenemies++;
            }

            if (tile == 289)
                bosses[5].SetBoss(c * 32, r * 32);
            if (tile == 285)
                bosses[3].SetBoss(c * 32, r * 32);
        }

        private void CreateEnemies(ContentManager content)
        {
            numberofenemies--;

            if (numberofenemies >= 0)
            {
                for (int i = 0; i <= numberofenemies; i++)
                {
                    if (enemies[i].type == 297)
                        enemies[i].TotemSprites(content, "troubling-totem", "ttbullets0", "ttbullets1", "ttbullets2", "ttbullets3");
                    else if (enemies[i].type == 298)
                        enemies[i].EnemySprites(content, 4, "neo-luckystand", 2, true, "neo-luckybullet", 1, "neo-luckyattack", 1, "neo-luckyjump", 1, false, 10, 4);
                    else if (enemies[i].type == 299)
                        enemies[i].EnemySprites(content, 1, "kronos", 10, false, "", 0, "", 0, "", 0, true, 10, 0);
                    else if (enemies[i].type == 300)
                        enemies[i].EnemySprites(content, 3, "Purin", 3, true, "Purinbullets", 4, "Purinjump", 2, "", 0, false, 3, 2);
                    else if (enemies[i].type == 301)
                        enemies[i].EnemySprites(content, 4, "mousehead", 1, true, "mousebullet", 3, "mousebody", 3, "mousetail", 3, true, 25, 5);
                    else if (enemies[i].type == 302)
                        enemies[i].EnemySprites(content, 1, "flipside", 1, false, "", 0, "", 0, "", 0, true, 0, 27);
                    else if (enemies[i].type == 303)
                        enemies[i].EnemySprites(content, 3, "Mace-botball", 1, false, "Mace-botstring", 1, "Mace-bot", 1, "", 0, true, 4, 6);
                    else if (enemies[i].type == 304)
                        enemies[i].EnemySprites(content, 2, "7R06D0R", 3, true, "7R06D0Rbullet", 3, "", 0, "", 0, true, 25, 5);
                    else if (enemies[i].type == 305)
                        enemies[i].EnemySprites(content, 3, "spikebomb", 2, false, "spikebullet", 1, "spikebulletangle", 1, "", 0, true, 5, 3);
                    else if (enemies[i].type == 306)
                        enemies[i].EnemySprites(content, 2, "MaliciousMushMech", 2, false, "mmmbullet", 1, "", 0, "", 0, false, 5, 4);
                    else if (enemies[i].type == 307)
                        enemies[i].EnemySprites(content, 2, "ToxicMushMech", 2, false, "mmmbullet", 1, "", 0, "", 0, false, 6, 7);
                    else if (enemies[i].type == 308)
                        enemies[i].EnemySprites(content, 2, "ChanceBomb", 6, false, "explosion", 12, "", 0, "", 0, true, 0, 2);
                    else if (enemies[i].type == 309)
                        enemies[i].EnemySprites(content, 3, "zarroc-Clone", 1, true, "zcbullet", 1, "zarroc-Clone2", 1, "", 0, true, 3, 2);
                    else if (enemies[i].type == 310)
                        enemies[i].EnemySprites(content, 1, "deux-kama", 1, false, "", 0, "", 0, "", 0, true, 0, 5);
                    else if (enemies[i].type == 311)
                        enemies[i].EnemySprites(content, 1, "serverbot", 4, false, "", 0, "", 0, "", 0, false, 4, 4);
                    else if (enemies[i].type == 312)
                        enemies[i].EnemySprites(content, 1, "Mega-hopper", 6, false, "", 0, "", 0, "", 0, false, 6, 6);
                    else if (enemies[i].type == 313)
                        enemies[i].EnemySprites(content, 2, "dozer-bot", 2, true, "dbbullets", 1, "", 0, "", 0, false, 7, 27);
                    else if (enemies[i].type == 314)
                        enemies[i].EnemySprites(content, 2, "Blacky", 3, true, "blackbullet", 1, "", 0, "", 0, true, 5, 3);
                    else if (enemies[i].type == 315)
                        enemies[i].EnemySprites(content, 1, "hellicoptor", 2, false, "", 0, "", 0, "", 0, true, 4, 4);
                    else if (enemies[i].type == 316)
                        enemies[i].EnemySprites(content, 2, "P1-H8R", 1, true, "P1-H8Rbullets", 1, "", 0, "", 0, true, 4, 3);
                    else if (enemies[i].type == 317)
                        enemies[i].EnemySprites(content, 2, "Macks", 1, true, "mkbullets", 3, "", 0, "", 0, false, 5, 4);
                }
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
