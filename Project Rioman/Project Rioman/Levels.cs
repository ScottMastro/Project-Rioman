using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Project_Rioman
{
    class Levels
    {
        public int activelevel;

        public Color backgroundcolour;
        public Texture2D[] tilesimg;
        public int width;
        public int height;
        public int[,] tilepos;
        public bool go;
        public Vector2 startpos;

        public Tile[,] tiles;
        public Enemy[] enemies = new Enemy[500];
        public Pickup[] pickups = new Pickup[10];
        public Boss[] bosses = new Boss[17];
        int numberofenemies;

        public bool stopleftxmovement;
        public bool stoprightxmovement;
        public bool stopleftscreenmovement;
        public bool stoprightscreenmovement;

        int fadetiles;
        public bool dooropening;
        public bool doorclosing;
        public bool closedoornow;
        public bool scrolling;
        public bool killbullets;
        int yscroll;
        int xscroll;
        public Rectangle scrollrect;
        int xscrollamount;
        int yscrollamount;

        public int lifechange = 0;

        public Levels()
        {

        }

        public void NewLevel(int level, ContentManager content)
        {
            level -= 9;
            activelevel = 3;

            string[] letters = new string[501];
            SetLetters(letters);

            string file = string.Format("Content/" + level.ToString() + ".txt");

            if (File.Exists(file))
            {
                go = true;
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
                            for (int i = 1; i <= 500; i++)
                            {
                                if (letters[i] == line.Substring(c, 2))
                                    tilepos[r, c / 2] = i;
                            }
                        }
                    }

                    line = "";
                }
                read.Close();

                tiles = new Tile[height + 1, width + 1];
                numberofenemies = 0;

                for (int r = 0; r <= height; r++)
                {
                    for (int c = 0; c <= width; c++)
                    {
                        TileType(tilepos[r, c], r, c);

                        if (tilepos[r, c] == 284)
                            startpos = new Vector2(c * 32, r * 32 - 15);
                    }
                }

                EnemySprites(content);
            }
        }

        private void SetLetters(string[] letters)
        {
            string crypt = "ͺͻͼͽ;΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϰϪЖ";

            int gap = 1;
            int counter = 0;

            for (int i = 1; i <= 500; i++)
            {
                letters[i] = crypt.Substring(counter, 1) + crypt.Substring(counter + gap, 1);

                counter++;

                if (counter + gap > 99)
                {
                    counter = 0;
                    gap++;
                }
            }
        }

        public void LoadLevelResources(ContentManager content)
        {
            tilesimg = new Texture2D[322];

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

        public void DrawTiles(SpriteBatch spriteBatch, bool iswarping)
        {
            if (go || iswarping)
            {
                for (int r = 0; r <= height; r++)
                {
                    for (int c = 0; c <= width; c++)
                    {
                        if (tiles[r, c] != null && tiles[r, c].type != 6 && tiles[r, c].type >= 0)
                            spriteBatch.Draw(tiles[r, c].sprite, tiles[r, c].location, Color.White);
                    }
                }
            }
        }

        private void TileType(int tile, int r, int c)
        {
            int type = -1;

            if (tile >= 110 && tile <= 113)
                type = 0;
            if (tile >= 1 && tile <= 6 || tile == 15 || tile == 16)
                type = 1;
            if (tile >= 103 && tile <= 105 || tile == 109)
                type = 1;
            if (tile >= 39 && tile <= 45 || tile>=50 && tile <=55)
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

        private void EnemySprites(ContentManager content)
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

        public void TileFrames(ContentManager content)
        {
            foreach (Tile tle in tiles)
            {
                if (tle != null)
                {
                    if (tle.tile == 177 || tle.tile == 178)
                        tle.TileFrames(content, 2);
                    if (tle.tile == 106 || tle.tile == 130 || tle.tile == 160 || tle.tile == 223)
                        tle.TileFrames(content, 3);
                    if (tle.tile == 56)
                        tle.TileFrames(content, 4);
                    if (tle.tile == 102)
                        tle.TileFrames(content, 9);
                }
            }
        }

        public void CenterRioman(Viewport viewportrect)
        {
            Vector2 middle = new Vector2(viewportrect.Width / 2, viewportrect.Height - 32 * 4);

            int xoffset = Convert.ToInt32(middle.X - startpos.X);
            int yoffset = Convert.ToInt32(middle.Y - startpos.Y);

            startpos = middle;

            MoveStuff(xoffset, yoffset);
        }

        public void CenterRioman(Viewport viewportrect, Rioman rioman)
        {
            int middle = viewportrect.Width / 2;

            int xoffset = Convert.ToInt32(middle - rioman.location.X);

            rioman.location.X = middle;

            MoveStuff(xoffset, 0);
        }

        public void TileFader()
        {
            for (int c = 1; c <= width; c++)
            {
                for (int r = 0; r <= height; r++)
                {
                    if (tiles[r, c] != null && tiles[r, c].type == 5)
                    {
                        tiles[r, c].fadetime = fadetiles;
                        fadetiles++;
                    }
                }
            }
            fadetiles = 0;
        }

        public void CreateWall()
        {
            for (int r = 1; r <= height; r++)
            {
                for (int c = 0; c <= width; c++)
                {
                    if (tiles[r, c] != null && tiles[r + 1, c] != null && tiles[r + 1, c].type == tiles[r, c].type && tiles[r, c].type != 3)
                    {
                        tiles[r, c].collisionrect.X = -1000;
                        tiles[r, c].nocollisionrect.X = -1000;
                        tiles[r, c].rightside = new Rectangle(tiles[r, c].location.X + 16, tiles[r, c].location.Y + 4, 16, 32);
                        tiles[r, c].leftside = new Rectangle(tiles[r, c].location.X, tiles[r, c].location.Y + 4, 16, 32);
                    }
                }
            }

            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {
                    if (tiles[r, c] != null)
                    {
                        if (tiles[r, c + 1] != null && tiles[r, c - 1] != null &&
                            tiles[r, c + 1].type == tiles[r, c].type && tiles[r, c - 1].type == tiles[r, c].type)
                        {
                            tiles[r, c].collisionrect = new Rectangle(tiles[r, c].location.X, tiles[r, c].location.Y, 32, 16);
                            tiles[r, c].nocollisionrect = new Rectangle(tiles[r, c].location.X, tiles[r, c].location.Y + 16, 32, 16);
                            tiles[r, c].rightside.X = -100;
                            tiles[r, c].leftside.X = -100;
                        }
                        else if (tiles[r, c + 1] != null && tiles[r, c + 1].type == tiles[r, c].type)
                        {
                            tiles[r, c].collisionrect.Width = 20;
                            tiles[r, c].nocollisionrect.Width = 20;
                            tiles[r, c].rightside.X = -100;
                        }
                        else if (tiles[r, c - 1] != null && tiles[r, c - 1].type == tiles[r, c].type)
                        {
                            tiles[r, c].collisionrect.Width = 20;
                            tiles[r, c].collisionrect.X = tiles[r, c].location.X;
                            tiles[r, c].nocollisionrect.Width = 20;
                            tiles[r, c].nocollisionrect.X = tiles[r, c].location.X;
                            tiles[r, c].leftside.X = -100;
                        }
                    }
                }
            }
        }

        public void Scroller()
        {
            scrollrect = new Rectangle();

            Rectangle scroll = new Rectangle();

            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {
                    if (tiles[r, c] != null && tiles[r, c].type == 6)
                    {
                        int x = 0;
                        int y = 0;

                        scroll.X = tiles[r, c].location.Right + 4;
                        scroll.Y = tiles[r, c].location.Bottom;

                        while (true)
                        {
                            x++;

                            if (tiles[r, c + x] != null && tiles[r, c + x].tile == tiles[r, c].tile)
                            {
                                scroll.Width = tiles[r, c + x].location.X - tiles[r, c].location.X;
                                break;
                            }
                        }

                        while (true)
                        {
                            y++;

                            if (tiles[r + y, c + x] != null && tiles[r + y, c + x].tile == tiles[r, c].tile)
                            {
                                scroll.Height = tiles[r + y, c + x].location.Y - tiles[r, c + x].location.Y;
                                tiles[r + y, c + x] = null;
                                tiles[r, c + x] = null;
                                tiles[r + y, c] = null;
                                break;
                            }
                        }
                        tiles[r, c].Scroller(scroll);
                    }
                }
            }
        }


        private void StartScroll(int xmagnitude, int ymagnitude, Tile tle, Viewport viewportrect)
        {
            scrollrect = tle.scrollrect;
            xscrollamount = viewportrect.Width - 40;
            yscrollamount = viewportrect.Height - 32;
            scrolling = true;
            xscroll = xmagnitude;
            yscroll = ymagnitude;
        }

        public void Scroll(Rioman rioman, Viewport viewportrect)
        {
            MoveStuff(xscroll, yscroll);
            killbullets = true;
            xscrollamount -= Math.Abs(xscroll);
            yscrollamount -= Math.Abs(yscroll);
            rioman.location.X += xscroll;
            rioman.location.Y += yscroll;

            if (xscroll != 0)
                rioman.location.X += -(xscroll / Math.Abs(xscroll));
            if (yscroll != 0)
                rioman.location.Y += -(yscroll / Math.Abs(yscroll));

            if (xscrollamount <= 0 || yscrollamount <= 0)
            {
                if (doorclosing)
                    closedoornow = true;
                scrolling = false;
                stopleftscreenmovement = true;
                stoprightscreenmovement = true;
                scrollrect.X -= 32;
            }

        }

        public void MoveStuff(int x, int y)
        {
            foreach (Tile tle in tiles)
            {
                if (tle != null)
                    tle.MoveTiles(x, y);
            }

            foreach (Pickup pickup in pickups)
            {
                if (pickup.isalive)
                    pickup.MovePickup(x, y);
            }

            for (int i = 0; i <= numberofenemies; i++)
                enemies[i].MoveEnemies(x, y);

            bosses[activelevel].Move(x, y);

            scrollrect.X += x;
            scrollrect.Y += y;
        }

        public void OpenDoor(Tile tle)
        {
            int row = 0, column = 0;

            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {
                    if (tiles[r, c] == tle)
                    {
                        row = r;
                        column = c;
                    }
                }
            }

            int temprow = row;

            while (true)
            {
                if (row > 0 && tiles[row - 1, column] != null && tiles[row - 1, column].type == 4)
                {
                    tiles[row, column].isopening = true;
                    row--;
                }
                else
                {
                    tiles[row, column].isopening = true;
                    row = temprow;
                    break;
                }

                dooropening = true;
            }

            while (true)
            {
                if (row < height && tiles[row + 1, column] != null && tiles[row + 1, column].type == 4)
                {
                    tiles[row + 1, column].isopening = true;
                    row++;
                }
                else
                    break;
            }
        }

        public void OpenDoor()
        {
            bool open = true;
            bool playsound = false;

            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {
                    if (tiles[r, c] != null && tiles[r, c].type == 4 && tiles[r, c].isopening)
                    {
                        if (tiles[r - 1, c] == null || !tiles[r - 1, c].isopening && tiles[r, c].location.Y <= tiles[r - 1, c].location.Y)
                        {
                            tiles[r, c].isopening = false;
                            tiles[r, c].isclosing = true;
                        }
                        else
                        {
                            open = false;
                            tiles[r, c].MoveTiles(0, -2);
                            playsound = true;
                        }
                    }
                }
            }

            if (playsound)
                Audio.jump2.Play(0.5f, -1f, 0f);
            if (open)
            {
                dooropening = false;
                doorclosing = true;
            }
        }

        public void CloseDoor(Rioman rioman, Viewport viewportrect)
        {
            bool close = true;
            bool playsound = false;

            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {
                    if (tiles[r, c] != null && tiles[r, c].type == 4 && tiles[r, c].isclosing)
                    {
                        if (tiles[r - 1, c] == null || !tiles[r - 1, c].isclosing && tiles[r, c].location.Y >= tiles[r - 1, c].location.Bottom)
                        {
                            tiles[r, c].isclosing = false;
                            tiles[r, c].type = 1;
                            CreateWall();
                        }
                        else
                        {
                            close = false;
                            tiles[r, c].MoveTiles(0, 2);
                            playsound = true;
                        }
                    }
                }
            }

            if (playsound)
                Audio.jump2.Play(0.5f, -1f, 0f);
            if (close)
            {
                doorclosing = false;
                closedoornow = false;
            }
        }

        public void LadderForm()
        {
            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {
                    if (r > 0 && tiles[r, c] != null && tiles[r, c].type == 3)
                    {
                        if (tiles[r - 1, c] == null)
                            tiles[r, c].istop = true;
                        else if (tiles[r - 1, c] != null && tiles[r - 1, c].type != 3)
                            tiles[r, c].istop = true;
                    }
                }
            }
        }

        public void UpdateTiles(Rioman rioman, GameTime gameTime, Viewport viewportrect)
        {
            bool collisionflag = false;
            bool climbing = false;
            stopleftxmovement = false;
            stoprightxmovement = false;
            stopleftscreenmovement = false;
            stoprightscreenmovement = false;
            bool[] enemycollision = new bool[numberofenemies + 1];
            bool bossfalling = true;

            Rectangle temprect = rioman.location;
            rioman.location.Width = rioman.walksprite.Width / 4;
            rioman.location.Height = rioman.walksprite.Height;

            rioman.location.X -= rioman.location.Width / 2;

            foreach (Tile tle in tiles)
            {
                if (tle != null)
                {
                    if (scrollrect.Width > 0)
                    {
                        if (!(scrollrect.X < 0))
                            stopleftscreenmovement = true;
                        if (!(scrollrect.Right > viewportrect.Width))
                            stoprightscreenmovement = true;
                    }

                    if (tle.type == 6 && !rioman.iswarping)
                    {
                        if (tle.tile == 318)
                        {
                            if (rioman.location.Intersects(tle.scrollrect))
                            {
                                tle.type = -1;
                                StartScroll(0, 10, tle, viewportrect);
                            }
                        }
                        else if (tle.tile == 319)
                        {
                            if (rioman.location.Intersects(tle.scrollrect))
                            {
                                tle.type = -1;
                                StartScroll(0, -10, tle, viewportrect);
                            }
                        }
                        else if (tle.tile == 320)
                        {
                            if (tle.scrollrect.X < viewportrect.Width)
                                stoprightscreenmovement = true;

                            if (rioman.location.Intersects(tle.scrollrect))
                            {
                                tle.type = -1;
                                StartScroll(-10, 0, tle, viewportrect);
                            }

                        }
                        else if (tle.tile == 321)
                        {
                           if (tle.scrollrect.X > 0)
                                stopleftscreenmovement = true;

                            if (rioman.location.Intersects(tle.scrollrect))
                            {
                                tle.type = -1;
                                StartScroll(10, 0, tle, viewportrect);
                            }
                        }
                    }

                    if (tle.type == 2 && rioman.location.Intersects(tle.nocollisionrect) && !rioman.isinvincible)
                    {
                        go = false;
                        lifechange = -1;
                        rioman.iswarping = true;
                        rioman.warpy = -100;
                        Audio.die.Play(0.5f, 1f, 0f);
                    }

                    if (tle.type == 3 && rioman.location.Intersects(tle.nocollisionrect))
                        climbing = true;

                    if (tle.type == 3 && rioman.isclimbing && rioman.location.Intersects(new Rectangle(tle.location.X, tle.location.Y, 32, 20))
                        && tle.istop && !rioman.climbdown)
                    {
                        rioman.sprite = rioman.climbtop;
                        rioman.climbtime = 0;
                    }

                    if (tle.type == 1 || tle.type == 4 || tle.type == 5)
                    {
                        foreach (Pickup pickup in pickups)
                            pickup.PickupUpdate(tle);

                        if (bosses[activelevel].pickup)
                            bosses[activelevel].weapon.PickupUpdate(tle);

                        if (rioman.location.Intersects(tle.collisionrect) && !rioman.location.Intersects(tle.nocollisionrect))
                        {
                            collisionflag = true;
                            rioman.location.Y = tle.location.Y - rioman.location.Height + 2;
                            rioman.isclimbing = false;
                            rioman.climbtime = 0;

                            if (rioman.isfalling)
                            {
                                Audio.land.Play(0.5f, 0f, 0f);
                                rioman.touchedground = true;
                            }
                        }
                        else if (rioman.location.Intersects(tle.nocollisionrect) && !rioman.location.Intersects(tle.leftside)
                            && !rioman.location.Intersects(tle.rightside))
                        {
                            rioman.isjumping = false;
                            rioman.jumptime = 0;
                        }

                      //  if (tle.rightside.Intersects(tle.collisionrect) && tle.leftside.Intersects(tle.collisionrect))
                       // {
                            if (rioman.location.Intersects(tle.leftside))
                            {
                                stoprightxmovement = true;
                                rioman.invincibledirection = 0;
                            }

                            if (rioman.location.Intersects(tle.rightside))
                            {
                                stopleftxmovement = true;
                                rioman.invincibledirection = 0;
                            }
                       // }
                    }

                    if (tle.type == 4 && rioman.location.Intersects(tle.location))
                        OpenDoor(tle);

                    if (tle.type == 5)
                        tle.Fade(gameTime);

                    if (tle.tile == 102)
                        tle.Wave(gameTime);

                    for (int i = 0; i <= numberofenemies; i++)
                    {
                        if (enemies[i].type == 302)
                        {
                            if (enemies[i].isalive && !rioman.isinvincible && rioman.location.Intersects(enemies[i].collisionrect)
                                && Math.Abs(rioman.location.Bottom - enemies[i].collisionrect.Y) < 10)
                            {
                                collisionflag = true;
                                rioman.location.Y = enemies[i].collisionrect.Y - rioman.location.Height + 2;
                            }
                        }

                        if (enemies[i].isalive && !enemies[i].throughground)
                        {
                            if (tle.type == 1 || tle.type == 3 && tle.istop || tle.type == 4 || tle.type == 5)
                            {
                                if (enemies[i].location.Intersects(tle.collisionrect) && !enemies[i].location.Intersects(tle.nocollisionrect))
                                {
                                    if (enemies[i].type != 317)
                                        enemies[i].location.Y = tle.location.Y - enemies[i].location.Height + 2;
                                    enemycollision[i] = true;
                                    enemies[i].falltime = 0;
                                }
                                if (enemies[i].location.Intersects(tle.nocollisionrect))
                                {
                                    enemies[i].isjumping = false;
                                    enemies[i].othertime = 0;
                                    if (enemies[i].type == 317)
                                    {
                                        enemies[i].location.Y = tle.location.Bottom - 1;
                                        enemies[i].isfalling = false;
                                        enemies[i].bool1 = true;
                                    }
                                }

                                if (enemies[i].location.Intersects(tle.leftside) && tle.type != 3)
                                    enemies[i].direction = SpriteEffects.None;
                                else if (enemies[i].location.Intersects(tle.rightside) && tle.type != 3)
                                    enemies[i].direction = SpriteEffects.FlipHorizontally;
                            }
                        }
                    }

                    if (bosses[activelevel].isalive)
                    {
                        if (tle.type == 1 || tle.type == 3 && tle.istop || tle.type == 4 || tle.type == 5)
                        {
                            if (tle.type != 4 && bosses[activelevel].location.Intersects(tle.collisionrect) && !bosses[activelevel].location.Intersects(tle.nocollisionrect))
                            {
                                bosses[activelevel].location.Y = tle.location.Y - bosses[activelevel].location.Height + 2;
                                bossfalling = false;
                                bosses[activelevel].falltime = 0;
                            }
                        }

                        if (!bosses[activelevel].isjumping)
                            bosses[activelevel].isfalling = bossfalling;

                        if (bosses[activelevel].boss == 3)
                        {
                            if (bosses[activelevel].location.Intersects(tle.leftside) && tle.type != 3)
                                bosses[activelevel].direction = SpriteEffects.None;
                            else if (bosses[activelevel].location.Intersects(tle.rightside) && tle.type != 3)
                                bosses[activelevel].direction = SpriteEffects.FlipHorizontally;
                        }
                    }
                }
            }

            for (int i = 0; i <= numberofenemies; i++)
                enemies[i].isfalling = !enemycollision[i];

            if (!climbing)
            {
                foreach (Tile tle in tiles)
                {
                    if (tle != null && tle.type == 3 && rioman.location.Intersects(tle.collisionrect) && !rioman.location.Intersects(tle.nocollisionrect))
                    {
                        rioman.isclimbing = false;
                        rioman.location.Y = tle.location.Y - rioman.location.Height + 2;
                        rioman.climbtime = 0;
                        collisionflag = true;

                        if (rioman.isfalling)
                            Audio.land.Play(0.5f, 0f, 0f);
                    }
                }
            }

            foreach (Pickup pickup in pickups)
                pickup.PickupUpdate(rioman, viewportrect);

            rioman.location.X += rioman.location.Width / 2;

            if (!collisionflag)
                rioman.location = temprect;

            rioman.isfalling = !collisionflag;
        }

        public void CheckDeath(Viewport viewportrect, Rioman rioman)
        {
            if (rioman.location.Y > viewportrect.Height + 100 || Health.health <= 0)
            {
                go = false;
                lifechange = -1;
                rioman.iswarping = true;
                rioman.warpy = -100;
                Audio.die.Play(0.5f, 1f, 0f);
            }
        }

        public Rectangle CheckClimb(Rectangle riolocation, int climbloc, bool up, ref Rectangle location)
        {
            bool okay = false;

            foreach (Tile tle in tiles)
            {
                if (up && tle != null && tle.type == 3 && riolocation.Intersects(tle.collisionrect) && riolocation.Intersects(tle.nocollisionrect))
                {
                    if (Math.Abs(riolocation.X - tle.location.Center.X) <= 16 && !okay)
                    {
                        if (!stoprightscreenmovement && !stopleftscreenmovement)
                            MoveStuff(climbloc - tle.location.Center.X, 0);
                        else
                            location.X = tle.location.Center.X;
                        okay = true;
                    }
                }

                if (!up && tle != null && tle.type == 3 && riolocation.Intersects(tle.collisionrect) && !riolocation.Intersects(tle.nocollisionrect) && tle.istop)
                {
                    if (Math.Abs(riolocation.X - tle.location.Center.X) <= 16 && !okay)
                    {
                        if (!stoprightscreenmovement && !stopleftscreenmovement)
                            MoveStuff(climbloc - tle.location.Center.X, 0);
                        else
                            location.X = tle.location.Center.X;
                        okay = true;
                    }
                }
            }

            if (!okay)
                riolocation.Width = 0;

            return riolocation;
        }

        public void UpdateEnemies(Rioman rioman, GameTime gameTime, Viewport viewportrect)
        {
            for (int i = 0; i <= numberofenemies; i++)
                enemies[i].UpdateEnemy(rioman, viewportrect, gameTime.ElapsedGameTime.TotalSeconds);

            foreach (Pickup pickup in pickups)
            {
                if (pickup.isalive)
                    pickup.Animate(gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= numberofenemies; i++)
                enemies[i].Draw(spriteBatch, scrolling);
        }

        public void EnemyCollision(Bullet[] bullets, Rioman rioman)
        {
            if (bosses[activelevel].isalive)
                bosses[activelevel].Collision(bullets, rioman);

            for (int i = 0; i <= numberofenemies; i++)
            {
                if (enemies[i].isalive)
                {
                    if (!rioman.ispaused)
                    {
                        foreach (Bullet bullet in bullets)
                        {
                            if (enemies[i].type == 297)
                            {
                                bool dead = enemies[i].TotemCollision(bullet);
                                if (dead)
                                    pickups[GetPickupIndex()].NewPickup(enemies[i].location.Center.X, enemies[i].location.Center.Y);
                            }
                            else
                            {
                                if (bullet.alive && enemies[i].location.Intersects(bullet.location))
                                {
                                    if (!enemies[i].cannotkill)
                                    {
                                        enemies[i].hit = true;
                                        enemies[i].health--;
                                        if (enemies[i].health <= 0)
                                        {
                                            pickups[GetPickupIndex()].NewPickup(enemies[i].location.Center.X, enemies[i].location.Center.Y);
                                            enemies[i].Kill();
                                        }
                                    }
                                    bullet.alive = false;

                                    if (enemies[i].type == 308)
                                        enemies[i].bool1 = true;
                                }
                                else if (enemies[i].type == 301)
                                    enemies[i].CartCollision(bullet);
                            }
                        }
                    }
                }
                if (enemies[i].isalive && !rioman.isinvincible && rioman.location.Intersects(enemies[i].collisionrect))
                {
                    if (enemies[i].type == 299)
                        rioman.ispaused = true;
                    else if (enemies[i].type != 302)
                    {
                        Health.health -= enemies[i].damage;
                        rioman.Hit();

                        if (rioman.location.Left < enemies[i].location.Left)
                            rioman.invincibledirection = -3;
                        else
                            rioman.invincibledirection = 3;

                        if (enemies[i].type == 308)
                            enemies[i].bool1 = true;
                    }
                    else if (enemies[i].type == 302)
                    {
                        if (enemies[i].othertime <= 2 && enemies[i].othertime <= 2 && enemies[i].bool1)
                        {
                            Health.health -= enemies[i].damage;
                            rioman.Hit();
                        }
                    }
                }

                if (enemies[i].type == 297)
                {
                    for (int l = 0; l <= 2; l++)
                        enemies[i].TotemCollision(l, rioman);
                }
                else if (enemies[i].type == 301)
                {
                    for (int l = 0; l <= 3; l++)
                        enemies[i].CartCollision(l, rioman);
                }

                for (int j = 0; j <= 2; j++)
                {
                    if (enemies[i].type == 297)
                        enemies[i].TotemCollision(j, rioman, true);

                    if (enemies[i].bulletalive[j])
                    {
                        if (!rioman.isinvincible && rioman.location.Intersects(enemies[i].bulletloc[j]))
                        {
                            enemies[i].bulletalive[j] = false;
                            enemies[i].bullettime[j] = 0;
                            Health.health -= 3;
                            stopleftxmovement = false;
                            stoprightxmovement = false;

                            rioman.Hit();

                            if (rioman.location.Left < enemies[i].bulletloc[j].Left)
                                rioman.invincibledirection = -3;
                            else
                                rioman.invincibledirection = 3;
                        }
                    }
                }

                if (enemies[i].isalive && enemies[i].type >= 305 && enemies[i].type <= 307)
                {
                    for (int k = 0; k <= 7; k++)
                    {
                        if (!rioman.isinvincible && rioman.location.Intersects(enemies[i].other[k]))
                        {
                            if (k == 0 || k == 4 || k == 5)
                                enemies[i].other[k].Y = -100;
                            else if (k == 6 || k == 3 || k == 7)
                                enemies[i].other[k].Y = 1000;
                            else if (k == 1)
                                enemies[i].other[k].X = -100;
                            else if (k == 2)
                                enemies[i].other[k].X = 1000;

                            Health.health -= enemies[i].damage;
                            stopleftxmovement = false;
                            stoprightxmovement = false;

                            rioman.Hit();

                            if (rioman.location.Left < enemies[i].other[k].Left)
                                rioman.invincibledirection = -3;
                            else
                                rioman.invincibledirection = 3;
                        }
                    }
                }
            }
        }

        private int GetPickupIndex()
        {
            int number = 0;

            for (int i = 0; i <= 9; i++)
            {
                if (!pickups[i].isalive)
                {
                    number = i;
                    break;
                }
            }

            return number;

        }
    }
}
