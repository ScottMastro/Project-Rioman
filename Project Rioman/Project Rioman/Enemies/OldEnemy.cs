using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Enemy
    {

        struct Totem
        {
            public int type;
            public Rectangle location;
            public Rectangle sourcerect;
            public Rectangle collisionrect;

            public int bullettype;
            public Rectangle bulletloc;
            public Rectangle bulletsource;
            public Rectangle bulletcollision;
            public int bulletactiveframe;
            public bool bulletalive;
            public double bullettime;
            public bool reverseanimation;

            public bool isalive;
            public bool hit;
            public double hittime;
            public int health;
            public int damage;
        }

        struct Cart
        {
            public int number;
            public int sprite;
            public Vector2 origin;
            public float rotation;
            public Rectangle location;
            public Rectangle sourcerect;
            public Rectangle collisionrect;

            public double hittime;
            public bool isalive;
            public bool hit;
        }

        public void TotemCollision(int number, Rioman rioman)
        {
            if (!rioman.IsInvincible() && totems[number].collisionrect.Intersects(rioman.Location) && totems[number].isalive)
            {
                Health.AdjustHealth(-damage);
                //rioman.Hit();

                if (rioman.Location.Left < totems[number].location.Left)
                    rioman.invincibledirection = -3;
                else
                    rioman.invincibledirection = 3;
            }
        }

        public bool TotemCollision(Bullet bullet)
        {
            bool dead = false;
            bool alive = false;

            if (totems[0].isalive || totems[1].isalive || totems[2].isalive)
                alive = true;

            for (int i = 2; i >= 0; i--)
            {
                if (bullet.isAlive && bullet.location.Intersects(totems[i].location) && totems[i].isalive)
                {
                    totems[i].hit = true;
                    totems[i].health--;
                    if (totems[i].health <= 0)
                        totems[i].isalive = false;
                    bullet.isAlive = false;
                }
            }

            if (!totems[0].isalive && !totems[1].isalive && !totems[2].isalive && alive)
                dead = true;

            return dead;
        }

        public void TotemCollision(int number, Rioman rioman, bool bulletcollision)
        {
            if (!rioman.IsInvincible() && totems[number].bulletcollision.Intersects(rioman.Location) && totems[number].bulletalive)
            {
                Health.AdjustHealth(-totems[number].damage);
                //rioman.Hit();

                if (rioman.Location.Left < totems[number].location.Left)
                    rioman.invincibledirection = -3;
                else
                    rioman.invincibledirection = 3;
            }
        }

        public void CartCollision(int number, Rioman rioman)
        {
            if (!rioman.IsInvincible() && carts[number].collisionrect.Intersects(rioman.Location) && carts[number].isalive)
            {
                Health.AdjustHealth(-damage);
              //  rioman.Hit();

                if (rioman.Location.Left < carts[number].location.Left)
                    rioman.invincibledirection = -3;
                else
                    rioman.invincibledirection = 3;
            }
        }

        public void CartCollision(Bullet bullet)
        {
            for (int i = 0; i <= 3; i++)
                if (bullet.isAlive && bullet.location.Intersects(carts[i].location) && carts[i].isalive)
                {
                    carts[i].hit = true;
                    health--;
                    bullet.isAlive = false;
                }
        }

        public int type;

        public int sprite;
        public Rectangle originalloc;
        public Rectangle location;
        public Rectangle sourcerect;
        public Rectangle startpos;
        public Rectangle collisionrect;

        public Rectangle[] other = new Rectangle[8];
        public float[] otherrotation = new float[8];

        public Rectangle[] bulletloc = new Rectangle[3];
        public Rectangle[] bulletsource = new Rectangle[3];
        public int[] activebulletframe = new int[3];
        public bool[] bulletalive = new bool[3];
        public double[] bullettime = new double[3];
        public int[] bulletdirection = new int[3];
        public bool horizontalbullets;

        public int ss;
        public Texture2D[] spritesheet;
        public Texture2D explode;
        public Rectangle explodesource;
        public Vector2 explodepoint;
        public int explodeframe;
        public int[] frames;
        public int[] activeframe;
        public bool[] reverseanimation;
        public SpriteEffects direction;
        public float rotation;
        public Vector2 origin;
        public bool cannotkill;

        public bool isalive;
        public bool respawn;
        public bool isfalling;
        public bool isjumping;
        public bool isexploding;
        public bool throughground;
        public bool hit;

        public double animationtime;
        public double falltime;
        public double othertime;
        public double othertime2;
        public double explodetime;
        public double hittime;

        private int fullhealth;
        public int health;
        public int damage;

        public bool bool1;
        Vector2 othervector = new Vector2();

        Totem[] totems = new Totem[3];
        Cart[] carts = new Cart[4];
        Random r = new Random();

        public Enemy(int typ, int r, int c, ContentManager content)
        {
            type = typ;

            int y = r * Constant.TILE_SIZE;
            int x = c * Constant.TILE_SIZE;

            location = new Rectangle(x, y, 0, 0);
            startpos = location;

            if (type == 309)
                location.Y += 4;

            SetEnemyCharacteristics(content);
        }

        private void SetEnemyCharacteristics(ContentManager content)
        {
            if (type == 297)
                TotemSprites(content, "troubling-totem", "ttbullets0", "ttbullets1", "ttbullets2", "ttbullets3");
            else if (type == 298)
                EnemySprites(content, 4, "neo-luckystand", 2, true, "neo-luckybullet", 1, "neo-luckyattack", 1, "neo-luckyjump", 1, false, 10, 4);
            else if (type == 299)
                EnemySprites(content, 1, "kronos", 10, false, "", 0, "", 0, "", 0, true, 10, 0);
            else if (type == 300)
                EnemySprites(content, 3, "Purin", 3, true, "Purinbullets", 4, "Purinjump", 2, "", 0, false, 3, 2);
            else if (type == 301)
                EnemySprites(content, 4, "mousehead", 1, true, "mousebullet", 3, "mousebody", 3, "mousetail", 3, true, 25, 5);
            else if (type == 302)
                EnemySprites(content, 1, "flipside", 1, false, "", 0, "", 0, "", 0, true, 0, 27);
            else if (type == 303)
                EnemySprites(content, 3, "Mace-botball", 1, false, "Mace-botstring", 1, "Mace-bot", 1, "", 0, true, 4, 6);
            else if (type == 304)
                EnemySprites(content, 2, "7R06D0R", 3, true, "7R06D0Rbullet", 3, "", 0, "", 0, true, 25, 5);
            else if (type == 305)
                EnemySprites(content, 3, "spikebomb", 2, false, "spikebullet", 1, "spikebulletangle", 1, "", 0, true, 5, 3);
            else if (type == 306)
                EnemySprites(content, 2, "MaliciousMushMech", 2, false, "mmmbullet", 1, "", 0, "", 0, false, 5, 4);
            else if (type == 307)
                EnemySprites(content, 2, "ToxicMushMech", 2, false, "mmmbullet", 1, "", 0, "", 0, false, 6, 7);
            else if (type == 308)
                EnemySprites(content, 2, "ChanceBomb", 6, false, "explosion", 12, "", 0, "", 0, true, 0, 2);
            else if (type == 309)
                EnemySprites(content, 3, "zarroc-Clone", 1, true, "zcbullet", 1, "zarroc-Clone2", 1, "", 0, true, 3, 2);
            else if (type == 310)
                EnemySprites(content, 1, "deux-kama", 1, false, "", 0, "", 0, "", 0, true, 0, 5);
            else if (type == 311)
                EnemySprites(content, 1, "serverbot", 4, false, "", 0, "", 0, "", 0, false, 4, 4);
            else if (type == 312)
                EnemySprites(content, 1, "Mega-hopper", 6, false, "", 0, "", 0, "", 0, false, 6, 6);
            else if (type == 313)
                EnemySprites(content, 2, "dozer-bot", 2, true, "dbbullets", 1, "", 0, "", 0, false, 7, 27);
            else if (type == 314)
                EnemySprites(content, 2, "Blacky", 3, true, "blackbullet", 1, "", 0, "", 0, true, 5, 3);
            else if (type == 315)
                EnemySprites(content, 1, "hellicoptor", 2, false, "", 0, "", 0, "", 0, true, 4, 4);
            else if (type == 316)
                EnemySprites(content, 2, "P1-H8R", 1, true, "P1-H8Rbullets", 1, "", 0, "", 0, true, 4, 3);
            else if (type == 317)
                EnemySprites(content, 2, "Macks", 1, true, "mkbullets", 3, "", 0, "", 0, false, 5, 4);
        }

        public void EnemySprites(ContentManager content, int spritesheets, string fileloc1, int frames1, bool bullets,
            string fileloc2, int frames2, string fileloc3, int frames3, string fileloc4, int frames4, bool movethoughground,
            int hlth, int dmge)
        {
            health = hlth;
            fullhealth = health;
            damage = dmge;
            rotation = 0f;
            origin = Vector2.Zero;
            throughground = movethoughground;
            ss = spritesheets;

            spritesheet = new Texture2D[ss];
            frames = new int[ss];
            activeframe = new int[ss];
            reverseanimation = new bool[ss];

            explode = content.Load<Texture2D>("Video\\enemies\\explodingenemy");

            ss--;

            if (spritesheets > 0)
            {
                for (int i = 0; i <= ss; i++)
                {
                    frames[i] = 0;
                    activeframe[i] = 1;
                }
            }

            if (spritesheets > 0)
            {
                spritesheet[0] = content.Load<Texture2D>("Video\\enemies\\" + fileloc1);
                frames[0] = frames1;
            }

            if (spritesheets > 1)
            {
                spritesheet[1] = content.Load<Texture2D>("Video\\enemies\\" + fileloc2);
                frames[1] = frames2;
            }

            if (spritesheets > 2)
            {
                spritesheet[2] = content.Load<Texture2D>("Video\\enemies\\" + fileloc3);
                frames[2] = frames3;
            }

            if (spritesheets > 3)
            {
                spritesheet[3] = content.Load<Texture2D>("Video\\enemies\\" + fileloc4);
                frames[3] = frames4;
            }

            if (spritesheets > 0)
            {
                sprite = 0;
                sourcerect = new Rectangle(0, 0, spritesheet[0].Width / frames[0], spritesheet[0].Height);
                location.Width = sourcerect.Width;
                location.Height = sourcerect.Height;
                location.Y += 32;
                location.Y -= location.Height;
                startpos.Y = location.Y;
                collisionrect = location;
                originalloc = location;

                if (type == 303)
                {
                    location.Y -= 20;
                    location.X -= 6;
                    other[0] = new Rectangle(location.X + 12, location.Y - 4, spritesheet[2].Width, spritesheet[2].Height);
                    other[1] = new Rectangle(location.X + 14, location.Y + 12, 0, spritesheet[1].Height);
                }

                animationtime = 0;
                othertime2 = 0;
                falltime = 0;
                respawn = true;
                direction = SpriteEffects.None;

                if (bullets)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        bulletloc[i] = new Rectangle(0, 0, spritesheet[1].Width / frames[1], spritesheet[1].Height);
                        bulletsource[i] = new Rectangle(0, 0, spritesheet[1].Width / frames[1], spritesheet[1].Height);
                        bullettime[i] = 0;
                    }
                }

                if (type == 301)
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        bool fakereverse = false;
                        int fakeframe = 0;

                        carts[i] = new Cart();
                        carts[i].number = i;
                        carts[i].rotation = 0f;
                        carts[i].sprite = 2;
                        if (i == 3)
                            carts[i].sprite = 3;
                        carts[i].sourcerect = Animation.GetSourceRect(frames[carts[i].sprite], ref fakeframe, spritesheet[carts[i].sprite].Width, spritesheet[carts[i].sprite].Height, ref fakereverse);
                        carts[i].location = new Rectangle(location.Right + carts[i].sourcerect.Width * i, location.Y, carts[i].sourcerect.Width, carts[i].sourcerect.Height);
                        if (i == 3)
                            carts[i].location.X += 12;
                        carts[i].collisionrect = location;
                        carts[i].origin = new Vector2(location.Width / 2, location.Height / 2);
                        carts[i].isalive = true;
                        carts[i].hit = false;
                        carts[i].hittime = 0;
                    }
                }
            }
        }

        public void TotemSprites(ContentManager content, string totem, string bullets1, string bullets2, string bullets3,
            string bullets4)
        {
            fullhealth = 2;
            damage = 2;
            rotation = 0f;
            origin = Vector2.Zero;
            throughground = false;
            ss = 5;

            spritesheet = new Texture2D[ss];
            frames = new int[ss];
            activeframe = new int[ss];
            reverseanimation = new bool[ss];

            explode = content.Load<Texture2D>("Video\\enemies\\explodingenemy");

            spritesheet[0] = content.Load<Texture2D>("Video\\enemies\\" + totem);
            spritesheet[1] = content.Load<Texture2D>("Video\\enemies\\" + bullets1);
            spritesheet[2] = content.Load<Texture2D>("Video\\enemies\\" + bullets2);
            spritesheet[3] = content.Load<Texture2D>("Video\\enemies\\" + bullets3);
            spritesheet[4] = content.Load<Texture2D>("Video\\enemies\\" + bullets4);

            frames[0] = 5;
            frames[1] = 1;
            frames[2] = 1;
            frames[3] = 3;
            frames[4] = 3;

            sourcerect = new Rectangle(0, 0, spritesheet[0].Width / frames[0], spritesheet[0].Height);
            location.Width = sourcerect.Width;
            location.Height = sourcerect.Height;
            location.Y += 32;
            location.Y -= location.Height;
            startpos.Y = location.Y;

            animationtime = 0;
            falltime = 0;
            respawn = true;
            direction = SpriteEffects.None;

            for (int i = 0; i <= 2; i++)
            {
                totems[i] = new Totem();
                NewTotem(i);
            }
        }

        private void NewTotem(int number)
        {
            totems[number].type = r.Next(0, 5);
            totems[number].isalive = true;
            totems[number].health = fullhealth;

            totems[number].location = location;
            totems[number].location.Y -= spritesheet[0].Height * (2 - number);
            totems[number].sourcerect = new Rectangle(spritesheet[0].Width / frames[0] * totems[number].type, 0, spritesheet[0].Width / frames[0], spritesheet[0].Height);

            totems[number].collisionrect = totems[number].location;
            totems[number].collisionrect.X += 20;

            totems[number].bullettime = 0;
            totems[number].reverseanimation = false;
            totems[number].bulletactiveframe = 1;

            if (totems[number].type == 0)
                totems[number].damage = 4;
            else if (totems[number].type == 1)
                totems[number].damage = 2;
            else if (totems[number].type == 2)
                totems[number].damage = 8;
            else if (totems[number].type == 3)
                totems[number].damage = 5;
            else if (totems[number].type == 4)
                totems[number].damage = 0;
        }

        public void MoveEnemies(int x, int y)
        {
            location.X += x;
            location.Y += y;
            startpos.X += x;
            startpos.Y += y;
            collisionrect.X += x;
            collisionrect.Y += y;

            if (isexploding)
            {
                explodepoint.X += x;
                explodepoint.Y += y;
            }

            if (type == 301)
            {
                for (int k = 0; k <= 3; k++)
                {
                    carts[k].location.X += x;
                    carts[k].location.Y += y;
                }
            }
            else if (type == 305)
            {
                for (int j = 0; j <= 7; j++)
                {
                    other[j].X += x;
                    other[j].Y += y;
                }
            }

            for (int i = 0; i <= 2; i++)
            {
                if (bulletalive[i])
                {
                    bulletloc[i].X += x;
                    bulletloc[i].Y += y;
                }
                if (type == 297)
                {
                    totems[i].location.X += x;
                    totems[i].location.Y += y;
                    totems[i].collisionrect.X += x;
                    totems[i].collisionrect.Y += y;

                    if (totems[i].bulletalive)
                    {
                        totems[i].bulletloc.X += x;
                        totems[i].bulletloc.Y += y;
                        totems[i].bulletcollision.X += x;
                        totems[i].bulletcollision.Y += y;
                    }
                }
                else if (type == 303)
                {
                    other[i].X += x;
                    other[i].Y += y;
                }
            }
        }

        public void UpdateEnemy(Rioman rioman, Viewport viewportrect, double elapsedtime)
        {

            AliveCheck(viewportrect);

            if (isalive)
            {
                if (!cannotkill || type == 308)
                    DeathCheck(viewportrect);

                if (type == 297)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        totems[i].bullettime += elapsedtime;

                        if (totems[i].type < 4 && totems[i].isalive)
                        {
                            if (!totems[i].bulletalive)
                            {
                                if (r.Next(0, 250 / (5 - totems[i].type)) == 0)
                                {
                                    totems[i].bulletloc = new Rectangle(totems[i].location.Center.X - 20, totems[i].location.Center.Y - 5,
                                            spritesheet[totems[i].type + 1].Width / 3, spritesheet[totems[i].type + 1].Height);
                                    totems[i].bulletsource = new Rectangle(0, 0, totems[i].bulletloc.Width, totems[i].bulletloc.Height);
                                    totems[i].bulletcollision = totems[i].bulletloc;
                                    totems[i].bulletcollision.X += 30;
                                    totems[i].bullettime = 0;
                                    totems[i].bulletalive = true;
                                    totems[i].bullettype = totems[i].type;
                                    totems[i].bulletactiveframe = 1;
                                }
                            }
                        }
                        else if (totems[i].type == 4 && totems[i].isalive)
                        {
                        //    if (!rioman.ispaused && !rioman.IsInvincible() && r.Next(0, 300) == 10)
                          //      rioman.ispaused = true;
                        }

                        if (i < 2 && !totems[i + 1].isalive && totems[i].location.Y < totems[i + 1].location.Y)
                        {
                            totems[i].location.Y += 2;
                            if (i == 1 && totems[i - 1].isalive)
                                totems[i - 1].location.Y += 2;
                        }
                    }
                }
                else if (type == 298)
                {
                    collisionrect = new Rectangle(location.X + 30, location.Y + 10, location.Width - 10, location.Height - 10);

                    if (!isjumping && !bool1)
                    {
                        Animate(elapsedtime, 0.2);

                        if (r.Next(25) == 10)
                        {
                            bool1 = true;
                            Shoot(location.Center.X, location.Center.Y, true);
                        }
                    }

                    if (bool1)
                    {
                        othertime2 += elapsedtime;
                        if (othertime2 > 0.4)
                        {
                            bool1 = false;
                            othertime2 = 0;
                        }
                    }

                    if (r.Next(50) == 10 && !isfalling && !bool1)
                        isjumping = true;

                    if (isjumping)
                    {
                        othertime += elapsedtime;

                        if (othertime < 1.25)
                            location.Y -= Convert.ToInt32(22 - othertime * 22);
                        else
                        {
                            othertime = 0;
                            isjumping = false;
                        }

                        if (direction == SpriteEffects.None)
                            location.X -= 3;
                        else
                            location.X += 3;
                    }

                    if (isjumping || isfalling)
                    {
                        sourcerect = Animation.GetSourceRect(spritesheet[3].Width, spritesheet[3].Height);
                        sprite = 3;
                    }
                    else if (bool1)
                    {
                        sourcerect = Animation.GetSourceRect(spritesheet[2].Width, spritesheet[2].Height);
                        sprite = 2;
                    }
                    else
                    {
                        sourcerect.Height = spritesheet[0].Height;
                        sprite = 0;
                    }

                    Fall(elapsedtime);
                }
                else if (type == 299)
                {
                    collisionrect = new Rectangle(location.X + 30, location.Y, location.Width - 10, location.Height);
                    Animate(elapsedtime, 0.1);

                    if (bool1)
                        location.Y -= 5;

                    if (!bool1)
                    {
                        if (rioman.Location.Center.X < location.Center.X)
                            location.X -= 1;
                        else if (rioman.Location.Center.X > location.Center.X)
                            location.X += 1;

                        if (rioman.Location.Center.Y < location.Center.Y)
                            location.Y -= 1;
                        else if (rioman.Location.Center.Y > location.Center.Y)
                            location.Y += 1;
                    }
                    else
                        bool1 = true;
                }
                else if (type == 300)
                {
                    animationtime += elapsedtime;
                    collisionrect = new Rectangle(location.X + 26, location.Y + 8, 22, 26);

                    if (animationtime > 0.2 || isjumping && othertime < 0.05)
                    {
                        sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                            spritesheet[sprite].Height, ref reverseanimation[sprite]);
                        animationtime = 0;
                    }

                    if (r.Next(50) == 1 && !isfalling)
                        isjumping = true;

                    if (isjumping)
                    {
                        sprite = 2;
                        othertime += elapsedtime;

                        if (othertime < 0.4)
                            location.Y -= Convert.ToInt32(10 - othertime * 22);
                        else
                        {
                            sprite = 0;
                            othertime = 0;
                            isjumping = false;
                        }
                    }

                    if (othertime > 0.1 / 2 && othertime < 0.11 / 2)
                        Shoot(location.Center.X, location.Center.Y, true);

                    if (direction == SpriteEffects.None)
                        location.X -= 1;
                    else
                        location.X += 1;

                    Fall(elapsedtime);
                }
                else if (type == 301)
                {
                    origin = new Vector2(spritesheet[0].Width / 2, spritesheet[0].Height / 2);
                    collisionrect = new Rectangle(location.X + 30, location.Y + 10, location.Width - 10, location.Height - 10);

                    if (location.X - location.Width / 2 <= 32 && rotation < float.Parse((Math.PI / 2).ToString()))
                    {
                        location.X = location.Width / 2 + 32;
                        rotation += float.Parse((Math.PI / 12).ToString());
                        if (rotation > float.Parse((Math.PI / 2).ToString()))
                            rotation = float.Parse((Math.PI / 2).ToString());
                    }
                    else if (location.Y - location.Width / 2 <= 2 && rotation < float.Parse(Math.PI.ToString()))
                    {
                        location.Y = location.Width / 2 + 2;
                        rotation += float.Parse((Math.PI / 12).ToString());
                        if (rotation > float.Parse(Math.PI.ToString()))
                            rotation = float.Parse(Math.PI.ToString());
                    }
                    else if (location.X + location.Width / 2 >= viewportrect.Width - 32 && rotation < float.Parse((Math.PI * 3 / 2).ToString()))
                    {
                        location.X = viewportrect.Width - 32 - location.Width / 2;
                        rotation += float.Parse((Math.PI / 12).ToString());
                        if (rotation > float.Parse((Math.PI * 3 / 2).ToString()))
                            rotation = float.Parse((Math.PI * 3 / 2).ToString());
                    }
                    else if (location.Y + location.Width / 2 >= viewportrect.Height - 46 && rotation < float.Parse((Math.PI * 2).ToString())
                        && rotation > float.Parse(Math.PI.ToString()))
                    {
                        location.Y = viewportrect.Height - 44 - location.Width / 2;
                        rotation += float.Parse((Math.PI / 12).ToString());
                        if (rotation > float.Parse((Math.PI * 2).ToString()))
                            rotation = 0f;
                    }

                    if (rotation == 0f)
                    {
                        location.X -= 8;
                        if (!bool1)
                        {
                            Shoot(location.Left, location.Top - 16, true);
                            bool1 = true;
                        }
                    }
                    else if (rotation == float.Parse((Math.PI / 2).ToString()))
                        location.Y -= 8;
                    else if (rotation == float.Parse(Math.PI.ToString()))
                    {
                        othertime += elapsedtime;
                        location.X += 8;
                        if (bool1 && othertime > 0.5)
                        {
                            Shoot(location.X, location.Center.Y, false);
                            othertime = 0;
                        }
                    }
                    else if (rotation == float.Parse((Math.PI * 3 / 2).ToString()))
                    {
                        bool1 = false;
                        location.Y += 8;
                    }

                    animationtime += elapsedtime;

                    if (animationtime > 0.1)
                    {
                        carts[0].sourcerect = Animation.GetSourceRect(frames[2], ref activeframe[2], spritesheet[2].Width, spritesheet[2].Height, ref reverseanimation[2]);
                        carts[3].sourcerect = Animation.GetSourceRect(frames[3], ref activeframe[3], spritesheet[3].Width, spritesheet[3].Height, ref reverseanimation[3]);

                        animationtime = 0;
                    }


                    for (int i = 0; i <= 3; i++)
                    {
                        if (carts[i].isalive)
                        {
                            if (i != 0 && i != 3)
                                carts[i].sourcerect = carts[0].sourcerect;

                            if (health < (i + 1) * 5)
                                carts[i].isalive = false;

                            if (carts[i].hit)
                            {
                                carts[i].hittime += elapsedtime;
                                if (carts[i].hittime > 0.05)
                                {
                                    carts[i].hit = false;
                                    carts[i].hittime = 0;
                                }

                            }

                            carts[i].location.Width = location.Width;
                            carts[i].collisionrect = new Rectangle(carts[i].location.X + 30, carts[i].location.Y + 10, carts[i].location.Width - 10, carts[i].location.Height - 10);

                            if (carts[i].location.X - carts[i].location.Width / 2 <= 32 && carts[i].rotation < float.Parse((Math.PI / 2).ToString()))
                            {
                                carts[i].location.X = carts[i].location.Width / 2 + 32;
                                carts[i].rotation += float.Parse((Math.PI / 12).ToString());
                                if (carts[i].rotation > float.Parse((Math.PI / 2).ToString()))
                                    carts[i].rotation = float.Parse((Math.PI / 2).ToString());
                            }
                            else if (carts[i].location.Y - carts[i].location.Width / 2 <= 2 && carts[i].rotation < float.Parse(Math.PI.ToString()))
                            {
                                carts[i].location.Y = carts[i].location.Width / 2 + 2;
                                carts[i].rotation += float.Parse((Math.PI / 12).ToString());
                                if (carts[i].rotation > float.Parse(Math.PI.ToString()))
                                    carts[i].rotation = float.Parse(Math.PI.ToString());
                            }
                            else if (carts[i].location.X + carts[i].location.Width / 2 >= viewportrect.Width - 32 && carts[i].rotation < float.Parse((Math.PI * 3 / 2).ToString()))
                            {
                                carts[i].location.X = viewportrect.Width - 32 - carts[i].location.Width / 2;
                                carts[i].rotation += float.Parse((Math.PI / 12).ToString());
                                if (carts[i].rotation > float.Parse((Math.PI * 3 / 2).ToString()))
                                    carts[i].rotation = float.Parse((Math.PI * 3 / 2).ToString());
                            }
                            else if (carts[i].location.Y + carts[i].location.Width / 2 >= viewportrect.Height - 46 && carts[i].rotation < float.Parse((Math.PI * 2).ToString())
                                && carts[i].rotation > float.Parse(Math.PI.ToString()))
                            {
                                carts[i].location.Y = viewportrect.Height - 44 - location.Width / 2;
                                carts[i].rotation += float.Parse((Math.PI / 12).ToString());
                                if (carts[i].rotation > float.Parse((Math.PI * 2).ToString()))
                                    carts[i].rotation = 0f;
                            }

                            if (carts[i].rotation == 0f)
                                carts[i].location.X -= 8;
                            else if (carts[i].rotation == float.Parse((Math.PI / 2).ToString()))
                                carts[i].location.Y -= 8;
                            else if (carts[i].rotation == float.Parse(Math.PI.ToString()))
                                carts[i].location.X += 8;
                            else if (carts[i].rotation == float.Parse((Math.PI * 3 / 2).ToString()))
                                carts[i].location.Y += 8;
                        }
                        else
                            carts[i].collisionrect.Y = -1000;
                    }
                }
                else if (type == 302)
                {
                    cannotkill = true;

                    if (!bool1)
                    {
                        othertime += elapsedtime;
                        collisionrect = new Rectangle(location.X - (location.Width / 2) + 30, location.Y - (location.Height / 2),
                            location.Width / 2, location.Height / 8);
                    }
                    else if (bool1)
                    {
                        othertime2 += elapsedtime;
                        collisionrect = new Rectangle(location.X - (location.Width / 2) + 30, location.Y - (location.Height / 2),
                            location.Width / 2, location.Height / 2);
                    }

                    origin.X = location.Width / 2;
                    origin.Y = location.Height / 2;

                    if (othertime > 2)
                    {
                        rotation += 0.1f;
                        collisionrect.Y -= viewportrect.Height;

                        if (rotation > Math.PI)
                        {
                            rotation = float.Parse(Math.PI.ToString());
                            bool1 = true;
                            othertime = 0;
                        }
                    }

                    if (othertime2 > 2)
                    {
                        rotation += 0.1f;
                        collisionrect.Y -= viewportrect.Height;

                        if (rotation > Math.PI * 2)
                        {
                            rotation = 0f;
                            bool1 = false;
                            othertime2 = 0;
                        }
                    }
                }
                else if (type == 303)
                {
                    collisionrect = new Rectangle(location.X + 30, location.Y + 10, location.Width - 10, location.Height - 10);

                    if (!bool1)
                        othertime += elapsedtime;

                    if (othertime > 1)
                    {
                        bool1 = true;
                        othertime = 0;
                    }
                    else if (bool1)
                    {
                        othertime2 += elapsedtime;

                        if (othertime2 > 0 && othertime2 <= 1)
                        {
                            location.X -= 6;
                            other[1].X -= 6;
                            other[1].Width += 6;
                        }

                        if (othertime2 > 1 && other[1].Width > 0)
                        {
                            location.X += 6;
                            other[1].X += 6;
                            other[1].Width -= 6;
                        }

                        if (othertime2 > 2 && other[1].Width <= 0)
                        {
                            bool1 = false;
                            othertime2 = 0;
                        }
                    }
                }
                else if (type == 304)
                {
                    collisionrect = new Rectangle(location.X + 84, location.Y, location.Width - 110, location.Height - 6);
                    otherrotation[5] += 0.02f;

                    othertime += elapsedtime;

                    if (!bool1)
                    {
                        Animate(elapsedtime, 0.2);
                        if (activeframe[0] == 3)
                        {
                            reverseanimation[0] = true;
                            Animate(0, -1);
                        }
                    }

                    if (othertime > 3)
                    {
                        if (!bool1)
                        {
                            reverseanimation[0] = false;
                            activeframe[0] = 2;
                            Animate(0, -1);

                            float angle = 0f;

                            if (rioman.Location.X < location.Center.X)
                            {
                                direction = SpriteEffects.None;
                                angle = float.Parse(Convert.ToString(Math.Atan(((double)location.Y + 20 - (double)rioman.Location.Center.Y) / ((double)location.X - (double)rioman.Location.X))));
                                if (rioman.Location.X < location.X)
                                    angle -= float.Parse(Convert.ToString(Math.PI));
                                Shoot(location.X, location.Y + 20, float.Parse(Convert.ToString(10 * (Math.Cos(angle)))), float.Parse(Convert.ToString(10 * (Math.Sin(angle)))), angle);
                            }
                            else if (rioman.Location.Right > location.Center.X)
                            {
                                direction = SpriteEffects.FlipHorizontally;
                                angle = float.Parse(Convert.ToString(Math.Atan(((double)location.Y + 20 - (double)rioman.Location.Center.Y) / ((double)location.Right - (double)rioman.Location.X))));
                                if (rioman.Location.X < location.Right)
                                    angle -= float.Parse(Convert.ToString(Math.PI));
                                Shoot(location.Right, location.Y + 20, float.Parse(Convert.ToString(10 * (Math.Cos(angle)))), float.Parse(Convert.ToString(10 * (Math.Sin(angle)))), angle);
                            }
                        }

                        bool1 = true;

                        if (rioman.Location.X < location.Center.X)
                            direction = SpriteEffects.None;
                        else if (rioman.Location.Right > location.Center.X)
                            direction = SpriteEffects.FlipHorizontally;

                        if (othertime > 3.5)
                        {
                            bool1 = false;
                            othertime = 0;
                        }
                    }
                    else
                    {
                        location.Y += Convert.ToInt32(3 * Math.Sin(otherrotation[5]));

                        if (location.X < 0)
                            direction = SpriteEffects.FlipHorizontally;
                        else if (location.Right > viewportrect.Width)
                            direction = SpriteEffects.None;

                        if (direction == SpriteEffects.None)
                            location.X -= 3;
                        else if (direction == SpriteEffects.FlipHorizontally)
                            location.X += 3;
                    }
                }
                else if (type == 305)
                {
                    collisionrect.Y = -1000;
                    if (!bool1)
                    {
                        other[0] = new Rectangle(location.Center.X - 6, location.Y - 14, spritesheet[1].Width, spritesheet[1].Height);
                        otherrotation[0] = 0f;
                        other[1] = new Rectangle(location.X - 14, location.Center.Y + 6, spritesheet[1].Width, spritesheet[1].Height);
                        otherrotation[1] = float.Parse(Convert.ToString(Math.PI * 3 / 2));
                        other[2] = new Rectangle(location.Center.X + 36, location.Center.Y - 6, spritesheet[1].Width, spritesheet[1].Height);
                        otherrotation[2] = float.Parse(Convert.ToString(Math.PI * 1 / 2));
                        other[3] = new Rectangle(location.Center.X + 6, location.Bottom + 14, spritesheet[1].Width, spritesheet[1].Height);
                        otherrotation[3] = float.Parse(Convert.ToString(Math.PI));
                        other[4] = new Rectangle(location.X - 2, location.Y - 2, spritesheet[2].Width, spritesheet[2].Height);
                        otherrotation[4] = 0;
                        other[5] = new Rectangle(location.Right + 2, location.Y - 2, spritesheet[2].Width, spritesheet[2].Height);
                        otherrotation[5] = float.Parse(Convert.ToString(Math.PI / 2));
                        other[6] = new Rectangle(location.X - 2, location.Bottom + 2, spritesheet[2].Width, spritesheet[2].Height);
                        otherrotation[6] = float.Parse(Convert.ToString(Math.PI * 3 / 2));
                        other[7] = new Rectangle(location.Right + 2, location.Bottom + 2, spritesheet[2].Width, spritesheet[2].Height);
                        otherrotation[7] = float.Parse(Convert.ToString(Math.PI));
                    }

                    if (bool1 || Math.Abs(rioman.Location.X - location.Center.X) < 100
                        && Math.Abs(rioman.Location.Center.Y - location.Center.Y) < 100)
                    {
                        bool1 = true;
                        othertime += elapsedtime;
                        other[0].Y -= 8;
                        other[1].X -= 8;
                        other[2].X += 8;
                        other[3].Y += 8;
                        other[4].X -= 8;
                        other[4].Y -= 8;
                        other[5].X += 8;
                        other[5].Y -= 8;
                        other[6].X -= 8;
                        other[6].Y += 8;
                        other[7].X += 8;
                        other[7].Y += 8;
                    }
                    else if (Math.Abs(rioman.Location.X - location.Center.X) < 200
                         && Math.Abs(rioman.Location.Center.Y - location.Center.Y) < 200)
                    {
                        activeframe[0] = 1;
                        reverseanimation[0] = false;
                        Animate(elapsedtime, 0);
                    }
                    else
                    {
                        activeframe[0] = 0;
                        reverseanimation[0] = false;
                        Animate(elapsedtime, 0);
                    }

                    if (othertime > 0.5)
                    {
                        activeframe[0] = 0;
                        reverseanimation[0] = false;
                        Animate(elapsedtime, 0);
                    }
                }
                else if (type == 306 || type == 307)
                {
                    collisionrect = new Rectangle(location.X + 30, location.Y + 10, location.Width - 10, location.Height);

                    if (Math.Abs(rioman.Location.Y - location.Y) < 50 && Math.Abs(rioman.Location.X - location.X) < 200)
                        bool1 = true;

                    if (isfalling || isjumping)
                    {
                        activeframe[0] = 1;
                        reverseanimation[0] = false;
                        Animate(elapsedtime, 0);
                    }
                    else
                    {
                        activeframe[0] = 0;
                        reverseanimation[0] = false;
                        Animate(elapsedtime, 0);
                    }

                    if (Math.Abs(rioman.Location.X - location.X) < 200 && othertime2 > 0.5 &&
                        Math.Abs(rioman.Location.Y - location.Y) < 200 && !isfalling)
                    {
                        isjumping = true;
                        othertime2 = 0;
                    }

                    if (isjumping)
                    {
                        othertime += elapsedtime;
                        if (othertime < 1)
                            location.Y -= Convert.ToInt32(16 - othertime * 16);
                        else
                        {
                            othertime = 0;
                            isjumping = false;
                            activeframe[0] = 0;
                            reverseanimation[0] = false;
                        }
                    }
                    else
                        othertime2 += elapsedtime;

                    if (isjumping)
                    {
                        if (direction == SpriteEffects.None)
                            location.X -= 2;
                        else
                            location.X += 2;
                    }

                    Fall(elapsedtime);

                    if (!bool1)
                    {
                        other[0] = new Rectangle(location.Right - 6, location.Center.Y, spritesheet[1].Width, spritesheet[1].Height);
                        otherrotation[0] = 0f;
                        other[1] = new Rectangle(location.X + 6, location.Center.Y + 6, spritesheet[1].Width, spritesheet[1].Height);
                        otherrotation[1] = float.Parse(Math.PI.ToString());
                    }
                    else
                    {
                        other[0].X += 8;
                        other[1].X -= 8;
                    }
                }
                else if (type == 308)
                {
                    cannotkill = true;
                    collisionrect = new Rectangle(location.X + 30, location.Y, location.Width - 20, location.Height);

                    if (bool1)
                        othertime += elapsedtime;

                    if (sprite == 0 && activeframe[0] < 6)
                    {
                        if (activeframe[sprite] == 5 && othertime > 0.6)
                        {
                            reverseanimation[0] = false;
                            sprite = 1;
                        }

                        if (othertime > 0.6 && activeframe[0] < 4 || bool1 && activeframe[sprite] == 1)
                        {
                            sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                                spritesheet[sprite].Height, ref reverseanimation[sprite]);
                            othertime = 0;
                        }
                        else if (othertime > 0.6 && activeframe[0] >= 4)
                        {
                            if (location.X % 2 == 0)
                            {
                                sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                                    spritesheet[sprite].Height, ref reverseanimation[sprite]);
                                othertime = 0;
                            }
                            else
                            {
                                bool1 = false;
                                othertime = 0;
                                sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                                    spritesheet[sprite].Height, ref reverseanimation[sprite]);
                                sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                                    spritesheet[sprite].Height, ref reverseanimation[sprite]);

                                reverseanimation[0] = false;
                            }
                        }
                    }
                    else if (sprite == 1 && othertime > 0.025)
                    {
                        damage = 5;
                        sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                                spritesheet[sprite].Height, ref reverseanimation[sprite]);
                        location.X += (location.Width - sourcerect.Width) / 2;
                        location.Y += (location.Height - sourcerect.Height) / 2;
                        location.Width = sourcerect.Width;
                        location.Height = sourcerect.Height;
                        othertime = 0;

                        if (activeframe[1] == 1 && reverseanimation[1])
                        {
                            Kill();
                            damage = 2;
                        }
                    }
                }
                else if (type == 309)
                {
                    cannotkill = true;
                    othertime += elapsedtime;

                    if (othertime > 1.5 - 0.017 && othertime < 1.5 + 0.017)
                    {
                        if (direction == SpriteEffects.None)
                            direction = SpriteEffects.FlipHorizontally;
                        else
                            direction = SpriteEffects.None;

                        Shoot(location.X, location.Center.Y, true);

                        if (direction == SpriteEffects.None)
                            direction = SpriteEffects.FlipHorizontally;
                        else
                            direction = SpriteEffects.None;
                    }

                    if (othertime >= 1.4 - 0.017)
                    {
                        bulletalive[0] = false;
                        bulletalive[1] = false;

                        sprite = 2;
                        cannotkill = false;
                    }

                    if (othertime >= 2)
                    {
                        othertime = 0;
                        sprite = 0;
                    }

                    if (direction == SpriteEffects.FlipHorizontally)
                        collisionrect = new Rectangle(location.X + 20, location.Y + 10, location.Width - 30, location.Height - 10);
                    else
                        collisionrect = new Rectangle(location.X - 10, location.Y + 10, location.Width - 30, location.Height - 10);

                    location.Width = spritesheet[sprite].Width;
                    origin.X = location.Width / 2;
                    sourcerect = Animation.GetSourceRect(spritesheet[sprite].Width, spritesheet[sprite].Height);

                    if (rioman.Location.X < location.Center.X)
                        direction = SpriteEffects.FlipHorizontally;
                    else if (rioman.Location.Right > location.Center.X)
                        direction = SpriteEffects.None;
                }
                else if (type == 310)
                {
                    cannotkill = true;
                    collisionrect = new Rectangle(location.Center.X - 10, location.Y, 20, location.Height);

                    rotation -= 0.1f;
                    origin = new Vector2(spritesheet[sprite].Width / 2, spritesheet[sprite].Height / 2);

                    othertime += elapsedtime;

                    if (othertime <= 1.5 || othertime > 4.5 && othertime <= 6)
                        location.Y += 2;
                    else if (othertime <= 4.5)
                        location.Y -= 2;
                    else
                        othertime = 0;
                }
                else if (type == 311)
                {
                    collisionrect = new Rectangle(location.X + 35, location.Y + 10, location.Width - 10, location.Height - 10);
                    Animate(elapsedtime, 0.15);

                    if (animationtime == 0)
                    {
                        if (Math.Abs(rioman.Location.Y - location.Y) < 50)
                        {
                            if (rioman.Location.Left <= location.Left)
                            {
                                direction = SpriteEffects.None;
                                location.X -= 9;
                            }
                            else if (rioman.Location.Right >= location.Right)
                            {
                                direction = SpriteEffects.FlipHorizontally;
                                location.X += 9;
                            }
                        }
                        else
                        {
                            if (direction == SpriteEffects.None)
                                location.X -= 9;
                            else if (direction == SpriteEffects.FlipHorizontally)
                                location.X += 9;
                        }
                    }

                    Fall(elapsedtime);
                }
                else if (type == 312)
                {
                    collisionrect = new Rectangle(location.X + 30, location.Y + 10, location.Width - 10, location.Height);

                    if (othertime <= 0)
                        Animate(elapsedtime, 0.1);

                    if (activeframe[0] == 6 && !isjumping && !isfalling)
                        isjumping = true;

                    if (isjumping)
                    {
                        othertime += elapsedtime;

                        if (othertime < 1)
                            location.Y -= Convert.ToInt32(12 - othertime * 12);
                        else
                        {
                            othertime = 0;
                            isjumping = false;
                            activeframe[0] = 0;
                            reverseanimation[0] = false;
                        }
                    }

                    if (isjumping)
                    {
                        if (direction == SpriteEffects.None)
                            location.X -= 3;
                        else
                            location.X += 3;
                    }

                    Fall(elapsedtime);
                }

                else if (type == 313)
                {
                    collisionrect = new Rectangle(location.X + 40, location.Y + 10, 28, location.Height - 10);
                    animationtime += elapsedtime;
                    othertime += elapsedtime;

                    Animate(elapsedtime, 0.2);

                    if (othertime > 0.5)
                    {
                        direction = SpriteEffects.FlipHorizontally;
                        Shoot(location.Center.X, location.Center.Y, true);
                        direction = SpriteEffects.None;
                        othertime = 0;
                    }

                    if (direction != SpriteEffects.FlipHorizontally && !bool1)
                        location.X--;
                    else
                    {
                        bool1 = true;
                        direction = SpriteEffects.None;
                    }

                    Fall(elapsedtime);
                }
                else if (type == 314)
                {
                    othertime += elapsedtime;

                    origin.X = location.Width / 2;
                    collisionrect = new Rectangle(location.Left + 20, location.Y + 15, location.Width - 20, location.Height - 15);

                    if (othertime > 1 && othertime <= 1.3)
                    {
                        othertime2 += elapsedtime;

                        if (othertime2 > 0.1)
                        {
                            Shoot(location.X, location.Y + location.Height / 2 - 16, true);
                            activeframe[0] = 2;
                            reverseanimation[0] = false;
                            Animate(elapsedtime, 0);
                            othertime2 = 0;
                        }
                    }
                    else
                    {
                        if (othertime > 2)
                            othertime = 0;

                        if (activeframe[0] == 2)
                            reverseanimation[0] = true;
                        Animate(elapsedtime, 0.2);
                    }

                    if (rioman.Location.X < location.Center.X)
                        direction = SpriteEffects.None;
                    else if (rioman.Location.Right > location.Center.X)
                        direction = SpriteEffects.FlipHorizontally;
                }
                else if (type == 315)
                {
                    collisionrect = new Rectangle(location.Center.X - location.Width / 4 + 30, location.Center.Y - location.Height / 4,
                        location.Width / 2 - 10, location.Height / 2);
                    Animate(elapsedtime, 0.2);

                    if (rioman.Location.Left <= location.Left)
                    {
                        direction = SpriteEffects.None;
                        location.X -= 2;
                    }
                    else if (rioman.Location.Right >= location.Right)
                    {
                        direction = SpriteEffects.FlipHorizontally;
                        location.X += 2;
                    }

                    if (rioman.Location.Center.Y <= location.Center.Y)
                        location.Y -= 1;
                    else if (rioman.Location.Center.Y >= location.Center.Y)
                        location.Y += 1;

                }
                else if (type == 316)
                {
                    collisionrect = new Rectangle(location.X + 10, location.Y + 14, 41, 10);
                    sourcerect = Animation.GetSourceRect(spritesheet[0].Width, spritesheet[0].Height);

                    if (Math.Abs(location.X - rioman.Location.X) < 200)
                    {
                        if (location.Y > 0 && rioman.Location.Y < location.Bottom + 20)
                            location.Y--;
                        else if (rioman.Location.Y > location.Bottom + 20)
                            location.Y++;

                        if (location.Center.X > rioman.Location.X)
                            location.X--;
                        else if (location.Center.X < rioman.Location.X)
                            location.X++;
                    }

                    bullettime[0] += elapsedtime;

                    if (bullettime[0] > 1)
                    {
                        Shoot(location.Center.X, location.Center.Y, false);
                        bullettime[0] = 0;
                    }
                }
                else if (type == 317)
                {
                    collisionrect = new Rectangle(location.X + 30, location.Y + 10, location.Width - 10, location.Height);
                    othertime2 += elapsedtime;

                    if (isfalling)
                    {
                        if (rioman.Location.Center.Y > location.Center.Y)
                            location.Y++;
                        else if (rioman.Location.Center.Y < location.Center.Y)
                            location.Y--;
                    }
                    else
                    {
                        if (rioman.Location.Center.Y < location.Center.Y && !bool1)
                            location.Y--;
                        if (rioman.Location.Center.Y > location.Center.Y && bool1)
                            location.Y++;
                    }

                    if (rioman.Location.Center.X > location.Center.X)
                        direction = SpriteEffects.FlipHorizontally;
                    else
                        direction = SpriteEffects.None;

                    if (othertime2 > 1)
                    {
                        Shoot(location.Center.X, location.Center.Y - 4, true);
                        othertime2 = 0;
                    }

                    for (int i = 0; i <= 2; i++)
                    {
                        if (bulletalive[i])
                        {
                            bullettime[i] += elapsedtime;
                            if (bullettime[i] > 0.1)
                            {
                                bool fakereverse = false;
                                bulletsource[i] = Animation.GetSourceRect(frames[1], ref activebulletframe[i], spritesheet[1].Width, spritesheet[1].Height, ref fakereverse);
                                bullettime[i] = 0;
                            }
                        }
                    }
                }
            }
            else if (isexploding)
                Explode(elapsedtime);


            Hit(elapsedtime);
            UpdateBullets(viewportrect);
        }


        public void DeathCheck(Viewport viewportrect)
        {
            if (type != 303)
            {
                if (location.X > viewportrect.Width || location.Right < 0
                    || location.Y > viewportrect.Height || location.Bottom < 0)
                    Kill();
            }
        }

        public void Kill()
        {
            isexploding = true;
            explodepoint = new Vector2(location.Center.X, location.Center.Y);
            explodesource = new Rectangle(0, 0, explode.Width / 5, explode.Height);
            explodeframe = 1;

            bool1 = false;
            othertime = 0;
            health = fullhealth;
            isalive = false;
            sprite = 0;

            if (type != 297)
            {
                for (int i = 0; i <= ss; i++)
                {
                    activeframe[i] = 0;
                    reverseanimation[i] = false;
                }
                location = new Rectangle(startpos.X, startpos.Y, originalloc.Width, originalloc.Height);
            }

            sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                spritesheet[sprite].Height, ref reverseanimation[sprite]);
            reverseanimation[sprite] = false;

            falltime = 0;
            animationtime = 0;

            if (type == 303)
            {
                location.Y -= 20;
                location.X -= 6;
                other[0] = new Rectangle(location.X + 12, location.Y - 4, spritesheet[2].Width, spritesheet[2].Height);
                other[1] = new Rectangle(location.X + 14, location.Y + 12, 0, spritesheet[1].Height);
            }
        }

        public void Explode(double elapsedtime)
        {
            explodetime += elapsedtime;

            if (explodetime > 0.05 && explodesource.X > 123)
            {
                explodetime = 0;
                isexploding = false;
            }

            if (explodetime > 0.05)
            {
                bool fake = false;
                explodesource = Animation.GetSourceRect(5, ref explodeframe, explode.Width, explode.Height, ref fake);
                explodetime = 0;
            }
        }

        private void Hit(double elapsedtime)
        {
            if (type != 297)
            {
                if (hit)
                    hittime += elapsedtime;

                if (hittime > 0.05)
                {
                    hittime = 0;
                    hit = false;
                }
            }
            else if (type == 297)
            {
                for (int i = 0; i <= 2; i++)
                {
                    if (totems[i].hit)
                        totems[i].hittime += elapsedtime;

                    if (totems[i].hittime > 0.05)
                    {
                        totems[i].hittime = 0;
                        totems[i].hit = false;
                    }
                }
            }
        }

        private void Fall(double elapsedtime)
        {
            if (isfalling)
            {
                falltime += elapsedtime;

                if (falltime * 30 > 10)
                    location.Y += 10;
                else
                    location.Y += Convert.ToInt32(falltime * 30);
            }
        }

        public void Animate(double elapsedtime, double framerate)
        {
            animationtime += elapsedtime;
            if (animationtime > framerate)
            {
                sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                    spritesheet[sprite].Height, ref reverseanimation[sprite]);
                animationtime = 0;
            }
        }

        public void AliveCheck(Viewport viewportrect)
        {
            if (startpos.X > viewportrect.Width || startpos.X + location.Width < 0)
                respawn = true;

            if (startpos.X < viewportrect.Width && location.Right > 0 && respawn
                && startpos.Y < viewportrect.Height && location.Bottom > 0)
            {
                if (type != 303)
                    bool1 = false;
                isalive = true;
                respawn = false;

                if (type == 297)
                    for (int i = 0; i <= 2; i++)
                        NewTotem(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool scrolling)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (type == 297)
                {
                    if (totems[i].bulletalive)
                        spriteBatch.Draw(spritesheet[totems[i].bullettype + 1], totems[i].bulletloc, totems[i].bulletsource, Color.White);
                }
                else if (type == 298)
                {
                    if (bulletalive[i])
                        spriteBatch.Draw(spritesheet[1], bulletloc[i], bulletsource[i], Color.White, otherrotation[i], new Vector2(bulletloc[i].Width / 2, bulletloc[i].Height / 2), direction, 0);
                }
                else if (type == 304)
                {
                    if (bulletalive[i])
                        spriteBatch.Draw(spritesheet[1], bulletloc[i], bulletsource[i], Color.White, otherrotation[i], new Vector2(bulletloc[i].Width / 2, bulletloc[i].Height / 2), SpriteEffects.None, 0);
                }
                else
                {
                    if (bulletalive[i])
                        spriteBatch.Draw(spritesheet[1], bulletloc[i], bulletsource[i], Color.White, 0f, Vector2.Zero, direction, 0);
                }
            }

            if (isalive && !hit || type == 301 && isalive)
            {
                if (type == 303)
                {
                    spriteBatch.Draw(spritesheet[2], other[0], Color.White);
                    spriteBatch.Draw(spritesheet[1], other[1], Color.White);
                }

                if (type == 297)
                {
                    foreach (Totem tot in totems)
                    {
                        if (tot.isalive && !tot.hit)
                            spriteBatch.Draw(spritesheet[0], tot.location, tot.sourcerect, Color.White);
                    }
                }
                else if (type == 301)
                {
                    if (!hit)
                        spriteBatch.Draw(spritesheet[sprite], location, sourcerect, Color.White, rotation, origin,
                            SpriteEffects.None, 0);


                    for (int i = 0; i <= 3; i++)
                    {
                        if (!carts[i].hit && carts[i].isalive)
                            spriteBatch.Draw(spritesheet[carts[i].sprite], carts[i].location, carts[i].sourcerect, Color.White,
                                carts[i].rotation, carts[i].origin, SpriteEffects.None, 0);
                    }

                }
                else
                    spriteBatch.Draw(spritesheet[sprite], location, sourcerect, Color.White, rotation, origin,
                       direction, 0);

                if (type >= 305 && type <= 307)
                {
                    if (type == 305)
                    {
                        for (int i = 0; i <= 7; i++)
                        {
                            if (!scrolling)
                            {
                                if (i <= 3)
                                    spriteBatch.Draw(spritesheet[1], other[i], null, Color.White, otherrotation[i], new Vector2(0, 0), SpriteEffects.None, 0);
                                else
                                    spriteBatch.Draw(spritesheet[2], other[i], null, Color.White, otherrotation[i], new Vector2(0, 0), SpriteEffects.None, 0);
                            }
                            else
                                other[i].Y = -100;
                        }
                    }
                    else if (type == 306 || type == 307)
                    {
                        spriteBatch.Draw(spritesheet[1], other[0], null, Color.White, otherrotation[0], new Vector2(0, 0), SpriteEffects.None, 0);
                        spriteBatch.Draw(spritesheet[1], other[1], null, Color.White, otherrotation[1], new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }
            else if (isexploding)
                spriteBatch.Draw(explode, new Rectangle(Convert.ToInt32(explodepoint.X), Convert.ToInt32(explodepoint.Y), explodesource.Width, explodesource.Height),
                    explodesource, Color.White, 0f, new Vector2(explodesource.Width / 2, explodesource.Height / 2), SpriteEffects.None, 0);
        }

        private void Shoot(int x, int y, bool horizontal)
        {
            horizontalbullets = horizontal;

            bool flag = false;
            int num = 0;

            for (int i = 0; i <= 2; i++)
            {
                if (!bulletalive[i])
                {
                    flag = true;
                    num = i;
                }
            }

            if (flag)
            {
                bulletalive[num] = true;
                bullettime[num] = 0;
                bulletloc[num].X = x;
                bulletloc[num].Y = y;
                bulletsource[num].X = 0;
                bulletsource[num].Y = 0;

                if (direction == SpriteEffects.None && horizontal)
                    bulletdirection[num] = -10;
                else
                    bulletdirection[num] = 10;
            }
        }

        private void Shoot(int x, int y, float xmove, float ymove, float angle)
        {
            bool flag = false;
            int num = 0;

            for (int i = 0; i <= 2; i++)
            {
                if (!bulletalive[i])
                {
                    flag = true;
                    num = i;
                }
            }

            if (flag)
            {
                bulletalive[num] = true;
                bullettime[num] = 0;
                bulletloc[num].X = x;
                bulletloc[num].Y = y;
                bulletsource[num].X = 0;
                bulletsource[num].Y = 0;

                otherrotation[num] = angle;
                othervector.X = xmove;
                othervector.Y = ymove;
            }
        }

        private void UpdateBullets(Viewport viewportrect)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (bulletalive[i])
                {
                    if (bulletloc[i].Right < 0 || bulletloc[i].X > viewportrect.Width
                        || bulletloc[i].Y > viewportrect.Height || bulletloc[i].Bottom < 0)
                    {
                        bulletalive[i] = false;
                        bullettime[i] = 0;
                    }

                    if (type != 304)
                    {
                        if (horizontalbullets)
                            bulletloc[i].X += bulletdirection[i];
                        else
                            bulletloc[i].Y += bulletdirection[i];
                    }
                    else if (type == 304)
                    {
                        bulletloc[i].X += Convert.ToInt32(othervector.X);
                        bulletloc[i].Y += Convert.ToInt32(othervector.Y);
                    }

                    if (type == 301 || type == 304)
                    {
                        bullettime[i]++;

                        if (bullettime[i] > 4)
                        {
                            bool fakereverse = false;
                            bulletsource[i] = Animation.GetSourceRect(frames[1], ref activebulletframe[i], spritesheet[1].Width, spritesheet[1].Height, ref fakereverse);
                            bullettime[i] = 0;
                        }
                    }

                    if (type == 298)
                    {
                        bullettime[i]++;

                        if (bullettime[i] > 4)
                        {
                            otherrotation[i] += float.Parse((Math.PI / 2).ToString());
                            bullettime[i] = 0;
                        }
                    }
                }

                if (type == 297 && totems[i].bulletalive)
                {
                    if (totems[i].bullettime > 0.2)
                    {
                        totems[i].bulletsource = Animation.GetSourceRect(3, ref totems[i].bulletactiveframe,
                            spritesheet[totems[i].bullettype + 1].Width, spritesheet[totems[i].bullettype + 1].Height, ref totems[i].reverseanimation);
                        totems[i].bullettime = 0;
                    }

                    if (totems[i].bulletloc.X < 0)
                        totems[i].bulletalive = false;

                    totems[i].bulletloc.X -= 6;
                    totems[i].bulletcollision.X -= 6;
                }
            }
        }
    }
}