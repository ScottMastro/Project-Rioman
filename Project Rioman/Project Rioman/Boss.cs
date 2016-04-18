using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class Boss
    {
        public int boss;
        public int sprite;
        public int spritesheets;
        public Texture2D[] spritesheet;

        public Rectangle location;
        public Rectangle sourcerect;
        public Rectangle collisionrect;

        public int[] attacktype = new int[20];
        public int[] bullettype = new int[20];
        public Rectangle[] bulletloc = new Rectangle[20];
        public Rectangle[] bulletsource = new Rectangle[20];
        public int[] activebulletframe = new int[20];
        public bool[] bulletalive = new bool[20];
        public double[] bullettime = new double[20];
        public float[] bulletdirection = new float[20];
        public int[] bulletspeed = new int[20];
        public bool[] bulletreverse = new bool[20];
        public bool[] animatebullet = new bool[20];

        public int xoffset;
        public int[] frames;
        public int[] activeframe;
        public bool[] reverseanimation;
        public SpriteEffects direction;
        public float rotation;
        public Vector2 origin;

        public bool stopleftx;
        public bool stoprightx;
        public bool isalive;
        public bool isdead;
        public bool isfalling;
        public bool isjumping;
        public bool isrunning;
        public bool attack1;
        public bool attack2;
        public bool hit;
        public int intro;

        public double animationtime;
        public double falltime;
        public double jumptime;
        public double attack1time;
        public double attack2time;
        public double hittime;
        public double othertime;

        public bool canthurt;
        public int coldamage;
        public int atk1damage;
        public int atk2damage;

        public Texture2D explode;
        public Rectangle explodesource;
        public Vector2 explodepoint;
        public int explodeframe;
        public bool isexploding;
        public double explodetime;

        public bool pickup;
        public OldPickup weapon;
        Color healthcolour;

        Random r = new Random();

        public Boss()
        {
        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
            collisionrect.X += x;
            collisionrect.Y += y;
        }

        public void LoadBoss(ContentManager content, int bossnum)
        {
            boss = bossnum;
            string name = "";
            intro = 3;
            location.Y = 30;

            if (boss == 3)
            {
                spritesheets = 6;
                name = "AM";

                healthcolour = Color.RoyalBlue;
                frames = new int[spritesheets + 1];
                frames[1] = 2;
                frames[2] = 3;
                frames[3] = 3;
                frames[4] = 2;
                frames[5] = 2;
                frames[6] = 1;

                coldamage = 4;
                atk1damage = 5;
                atk2damage = 5;
            }
            else if (boss == 5)
            {
                spritesheets = 5;
                name = "IM";

                healthcolour = Color.Red;
                xoffset = 72;
                frames = new int[spritesheets + 1];
                frames[1] = 2;
                frames[2] = 2;
                frames[3] = 3;
                frames[4] = 2;
                frames[5] = 3;

                coldamage = 4;
                atk1damage = 4;
                atk2damage = 5;
            }

            spritesheet = new Texture2D[spritesheets + 1];

            for (int i = 1; i <= spritesheets; i++)
                spritesheet[i] = content.Load<Texture2D>("Video\\bosses\\" + name + "\\" + name + i.ToString());

            explode = content.Load<Texture2D>("Video\\bosses\\explode");
            weapon = new OldPickup();
            weapon.LoadPickupSprites(content);

            activeframe = new int[spritesheets + 1];
            reverseanimation = new bool[spritesheets + 1];
        }

        public void SetBoss(int x, int y)
        {
            sprite = 1;

            sourcerect = new Rectangle(0, 0, spritesheet[1].Width / frames[1], spritesheet[1].Height);
            location = new Rectangle(x, y, sourcerect.Width, sourcerect.Height);
            location.Y += 34;
            location.Y -= location.Height;
            collisionrect = location;

            for (int i = 0; i <= 19; i++)
            {
                activebulletframe[i] = 1;
                bulletalive[i] = false;
                bullettime[i] = 0;
                bulletreverse[i] = false;
                bulletspeed[i] = 0;

                if (i <= spritesheets)
                {
                    activeframe[i] = 1;
                    reverseanimation[i] = false;
                }
            }

            direction = SpriteEffects.None;
            rotation = 0f;
            origin = new Vector2(xoffset, 0);

            isdead = false;
            isalive = false;
            isfalling = false;
            isjumping = false;
            attack1 = false;
            attack2 = false;
            hit = false;

            animationtime = 0;
            falltime = 0;
            jumptime = 0;
            attack1time = 0;
            attack2time = 0;
            hittime = 0;
            othertime = 0;
        }

        public bool Update(double elapsedtime, Viewport viewportrect, Rioman rioman)
        {
            bool go = false;

            if (intro == 1)
                Fall(elapsedtime, false, 0);
            else if (intro == 2)
                intro = StatusBar.BossHealth();
            else if (!isdead)
            {
                if (!isexploding)
                    AliveCheck(viewportrect);

                if (isexploding)
                    Explode(elapsedtime);

                if (isalive)
                {
                    if (boss == 3)
                        AuroraManUpdate(elapsedtime, rioman, viewportrect);
                    else if (boss == 5)
                        InfernoManUpdate(elapsedtime, rioman, viewportrect);

                    if (hit)
                    {
                        hittime += elapsedtime;

                        if (hittime > 0.05)
                        {
                            hittime = 0;
                            hit = false;
                        }
                    }

                    UpdateBullets(elapsedtime, viewportrect);
                }
            }
            if (pickup)
                go = weapon.PickupUpdate(rioman, elapsedtime);

            return go;
        }

        public void Collision(AbstractBullet[] bullets, Rioman rioman)
        {
            if (isalive)
            {
                if (!rioman.IsInvincible() && rioman.Location.Intersects(collisionrect))
                {
                    StatusBar.AdjustHealth(-coldamage);
                    //rioman.Hit();

                    if (rioman.Location.Left < collisionrect.Left)
                        rioman.invincibledirection = -3;
                    else
                        rioman.invincibledirection = 3;
                }

                for (int i = 0; i <= 19; i++)
                {
                    if (bulletalive[i] && !rioman.IsInvincible() && rioman.Location.Intersects(bulletloc[i]))
                    {
                        bulletalive[i] = false;

                        if (attacktype[i] == 1)
                            StatusBar.AdjustHealth(-atk1damage);
                        else if (attacktype[i] == 2)
                            StatusBar.AdjustHealth(-atk2damage);

                        //rioman.Hit();

                        if (rioman.Location.Left < bulletloc[i].Left)
                            rioman.invincibledirection = -3;
                        else
                            rioman.invincibledirection = 3;
                    }
                }

                if (boss == 5)
                    collisionrect.X -= 18;

                foreach (AbstractBullet blt in bullets)
                {
                    if (boss == 5)
                    {
                        if (blt.Hits(collisionrect))
                        {
                            if (!canthurt)
                            {
                                hit = true;
                                StatusBar.AdjustBossHealth(-blt.TakeDamage("TODO"));
                            }
                        }
                    }
                    else
                    {
                        if (blt.Hits(location))
                        {
                            if (!canthurt)
                            {
                                hit = true;
                                StatusBar.AdjustBossHealth(-blt.TakeDamage("TODO"));
                            }
                        }
                    }
                }

                if (boss == 5)
                    collisionrect.X += 18;
            }
        }

        private void Animate(double elapsedtime, double framerate)
        {
            animationtime += elapsedtime;

            if (animationtime > framerate)
            {
                sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                   spritesheet[sprite].Height, ref reverseanimation[sprite]);
                animationtime = 0;
            }
        }

        private void Fall(double elapsedtime, bool movehorizontally, int horizontalamount)
        {
            if (isfalling)
            {
                falltime += elapsedtime;

                if (falltime * 30 > 10)
                    location.Y += 10;
                else
                    location.Y += Convert.ToInt32(falltime * 30);

                if (movehorizontally)
                {
                    if (!stopleftx && direction == SpriteEffects.None)
                        location.X -= horizontalamount;
                    else if (!stoprightx && direction == SpriteEffects.FlipHorizontally)
                        location.X += horizontalamount;
                }
            }
            else if (intro == 1)
                intro = 2;
        }

        private void Jump(double elapsedtime, double time, double height, bool movehorizontally, int horizontalamount)
        {
            if (isjumping)
            {
                jumptime += elapsedtime;

                if (jumptime < time)
                    location.Y -= Convert.ToInt32(height - jumptime * height);
                else
                {
                    jumptime = 0;
                    isjumping = false;
                    isfalling = true;
                }

                if (movehorizontally)
                {
                    if (!stopleftx && direction == SpriteEffects.None)
                        location.X -= horizontalamount;
                    else if (!stoprightx && direction == SpriteEffects.FlipHorizontally)
                        location.X += horizontalamount;
                }
            }
        }

        private void Shoot(float angle, int x, int y, int bullettyp, int atktyp, int speed, bool animate)
        {
            int number = -1;

            for (int i = 0; i <= 19; i++)
            {
                if (!bulletalive[i])
                {
                    number = i;
                    break;
                }
            }

            if (number >= 0)
            {
                bulletspeed[number] = speed;
                attacktype[number] = atktyp;
                bullettype[number] = bullettyp;
                bulletalive[number] = true;
                bulletdirection[number] = angle;
                bulletloc[number] = new Rectangle(x, y, spritesheet[bullettype[number]].Width / frames[bullettype[number]], spritesheet[bullettype[number]].Height);
                if (animate)
                    bulletsource[number] = Animation.GetSourceRect(frames[bullettype[number]], ref activebulletframe[number], spritesheet[bullettype[number]].Width,
                        spritesheet[bullettype[number]].Height, ref bulletreverse[bullettype[number]]);
                else
                    bulletsource[number] = Animation.GetSourceRect(spritesheet[bullettype[number]].Width, spritesheet[bullettype[number]].Height);
                bullettime[number] = 0;
                animatebullet[number] = animate;
            }
        }

        private void UpdateBullets(double elapsedtime, Viewport viewportrect)
        {
            for (int i = 0; i <= 19; i++)
            {
                if (bulletalive[i])
                {
                    bullettime[i] += elapsedtime;

                    if (bullettime[i] > 0.1 && animatebullet[i])
                    {
                        bullettime[i] = 0;
                        bulletsource[i] = Animation.GetSourceRect(frames[bullettype[i]], ref activebulletframe[i], spritesheet[bullettype[i]].Width,
                            spritesheet[bullettype[i]].Height, ref bulletreverse[bullettype[i]]);
                    }

                    bulletloc[i].X += Convert.ToInt32(Math.Cos(bulletdirection[i]) * Convert.ToDouble(bulletspeed[i]));
                    bulletloc[i].Y += Convert.ToInt32(Math.Sin(bulletdirection[i]) * Convert.ToDouble(bulletspeed[i]));

                    if (bulletloc[i].Right < 0 || bulletloc[i].X > viewportrect.Width || bulletloc[i].Bottom < 0 || bulletloc[i].Y > viewportrect.Width)
                        bulletalive[i] = false;
                }
            }
        }

        private void AliveCheck(Viewport viewportrect)
        {
            if (!isalive && !isexploding)
            {
                if (location.X > 0 && location.X < viewportrect.Width && location.Y > 0 && location.Y < viewportrect.Height)
                {
                    isalive = true;
                    intro = 1;
                    StatusBar.BossHealth(healthcolour);
                }
            }
            else if (isalive)
            {
                if (StatusBar.GetBossHealth() <= 0)
                {
                    for (int i = 0; i <= 19; i++)
                        bulletalive[i] = false;

                    isalive = false;
                    isexploding = true;

                    if (origin.X > 0)
                        explodepoint = new Vector2(location.X - explode.Width / 10, location.Center.Y - explode.Height / 2);
                    else
                        explodepoint = new Vector2(location.Center.X - explode.Width / 10, location.Center.Y - explode.Height / 2);

                    explodetime = 0;
                    explodeframe = 0;

                    bool fake = false;
                    explodesource = Animation.GetSourceRect(5, ref explodeframe, explode.Width, explode.Height, ref fake);
                }
            }
        }

        public void Explode(double elapsedtime)
        {
            explodetime += elapsedtime;

            if (explodetime > 0.1 && explodeframe <= 4)
            {
                explodetime = 0;

                bool fake = false;
                explodesource = Animation.GetSourceRect(5, ref explodeframe, explode.Width, explode.Height, ref fake);
            }
            else if (explodetime > 0.1 && explodeframe > 4)
            {
                isexploding = false;
                isdead = true;
                weapon.NewPickup(Convert.ToInt32(explodepoint.X), Convert.ToInt32(explodepoint.Y), boss - 1);
                pickup = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isdead)
            {
                for (int i = 0; i <= 19; i++)
                {
                    if (bulletalive[i])
                        spriteBatch.Draw(spritesheet[bullettype[i]], bulletloc[i], bulletsource[i], Color.White, bulletdirection[i],
                            new Vector2(bulletloc[i].Width / 2, bulletloc[i].Height / 2), SpriteEffects.None, 0);
                }

                if (isalive && !hit)
                    spriteBatch.Draw(spritesheet[sprite], location, sourcerect, Color.White, rotation, origin, direction, 0);

                if (isexploding)
                    spriteBatch.Draw(explode, new Rectangle(Convert.ToInt32(explodepoint.X), Convert.ToInt32(explodepoint.Y), explode.Width / 5, explode.Height),
                       explodesource, Color.White);
            }

            if (pickup)
                weapon.Draw(spriteBatch);
        }

        private void ChangeSpriteSize(int newsprite, bool changeframe)
        {
            int prevspriteframes = activeframe[sprite];
            bool prevspriterevanimation = reverseanimation[sprite];
            int ymove = spritesheet[sprite].Height - spritesheet[newsprite].Height;

            sprite = newsprite;

            if (changeframe)
            {
                activeframe[sprite] = 0;
                reverseanimation[sprite] = false;
            }
            else
            {
                activeframe[sprite] = prevspriteframes - 1;
                reverseanimation[sprite] = prevspriterevanimation;

                if (prevspriteframes - 1 <= 0)
                    reverseanimation[sprite] = false;
            }

            sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width, spritesheet[sprite].Height, ref reverseanimation[sprite]);
            location.Width = sourcerect.Width;
            location.Height = sourcerect.Height;

            location.Y += ymove;
        }

        private void AuroraManUpdate(double elapsedtime, Rioman rioman, Viewport viewportrect)
        {
            if (othertime == 0)
            {
                if (rioman.Location.X < location.Center.X)
                    direction = SpriteEffects.None;
                else
                    direction = SpriteEffects.FlipHorizontally;

                isrunning = !isrunning;
                othertime = r.Next(10, 40) / 10;
                ChangeSpriteSize(2, true);
            }

            othertime -= elapsedtime;
            attack2time += elapsedtime;

            if (othertime <= 0)
                othertime = 0;

            if (!isjumping && !isfalling && attack2time > 3 && r.Next(100) == 10)
            {
                attack2time = 0;
                isjumping = true;
            }

            if (!attack1 && r.Next(1001) % 50 == 0 && !isjumping && !isfalling)
            {
                if (direction == SpriteEffects.None)
                    Shoot(float.Parse(Math.PI.ToString()), location.X + 10, location.Center.Y, 6, 1, 10, false);
                else
                    Shoot(0, location.Right - 10, location.Center.Y, 6, 1, 10, false);
                attack1 = true;
                ChangeSpriteSize(3, false);

                if (direction == SpriteEffects.None)
                {
                    if (!isrunning)
                        location.X -= 10;
                    else if (isrunning)
                        location.X -= 8;
                }
            }

            if (attack2time > 0.5 && !attack2 && location.Y >= 200 && location.Y <= 212)
            {
                attack2 = true;
                attack2time = 0;
            }

            if (attack2)
            {
                attack1 = false;
                isfalling = false;
                isjumping = false;

                if (attack2time > 0.5 && attack2time < 1)
                {
                    attack2time = 1;
                    Shoot(0, location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse(Math.PI.ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse((Math.PI / 2).ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse((Math.PI * 3 / 2).ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse((Math.PI / 4).ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse((Math.PI * 3 / 4).ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse((Math.PI * 5 / 4).ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                    Shoot(float.Parse((Math.PI * 7 / 4).ToString()), location.Center.X, location.Center.Y, 6, 2, 10, false);
                }

                if (attack2time > 1.5)
                {
                    attack2time = 0;
                    attack2 = false;
                    isfalling = true;
                    jumptime = 0;
                }
            }
            else if (attack1)
            {
                attack1time += elapsedtime;

                if (attack1time > 0.5)
                {
                    attack1time = 0;
                    attack1 = false;

                    if (direction == SpriteEffects.None)
                    {
                        if (!isrunning)
                            location.X += 10;
                        else if (isrunning)
                            location.X += 8;
                    }
                }
            }

            if (attack2)
            {
                ChangeSpriteSize(5, true);
                if (attack2time > 0.5)
                {
                    bool fake = false;
                    sourcerect = Animation.GetSourceRect(2, ref activeframe[sprite], spritesheet[sprite].Width, spritesheet[sprite].Height, ref fake);
                }
            }
            else if (isrunning && !isjumping && !isfalling)
            {
                if (!attack1)
                    ChangeSpriteSize(2, false);

                Animate(elapsedtime, 0.15);
                location.Width = sourcerect.Width;

                if (direction == SpriteEffects.None)
                    location.X -= 2;
                else
                    location.X += 2;
            }
            else if (isjumping || isfalling)
            {
                ChangeSpriteSize(1, true);
                bool fake = false;
                sourcerect = Animation.GetSourceRect(2, ref activeframe[sprite], spritesheet[sprite].Width, spritesheet[sprite].Height, ref fake);
            }
            else if (!isrunning && attack1)
            {
                ChangeSpriteSize(4, true);
            }
            else if (!isrunning && !attack1)
            {
                ChangeSpriteSize(1, true);
            }

            collisionrect = new Rectangle(location.X + 40, location.Y + 10, location.Width - 25, location.Height - 10);

            Jump(elapsedtime, 1, 12, false, 0);
            Fall(elapsedtime, false, 0);
        }

        private void InfernoManUpdate(double elapsedtime, Rioman rioman, Viewport viewportrect)
        {
            attack1time += elapsedtime;
            attack2time += elapsedtime;
            canthurt = false;

            origin = new Vector2(xoffset, 0);

            if (location.X < 58)
                stopleftx = true;
            else
                stopleftx = false;

            if (location.X > viewportrect.Width - 36)
                stoprightx = true;
            else
                stoprightx = false;


            if (!isjumping && !isfalling && !attack1 && !attack2)
            {
                sprite = 1;
                activeframe[sprite] = 0;
                sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                    spritesheet[sprite].Height, ref reverseanimation[sprite]);

                if (r.Next(0, 50) == 10)
                {
                    SpriteEffects se = direction;

                    if (rioman.Location.X < location.Center.X)
                        direction = SpriteEffects.None;
                    else
                        direction = SpriteEffects.FlipHorizontally;

                    if (se == SpriteEffects.None && direction != se)
                        location.X += 35;
                    else if (se == SpriteEffects.FlipHorizontally && direction != se)
                        location.X -= 35;

                    isjumping = true;
                    activeframe[sprite] = 1;
                    sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                        spritesheet[sprite].Height, ref reverseanimation[sprite]);
                }
                else if (attack1time > 3 && r.Next(1, 100) % 4 == 0)
                {
                    attack1 = true;
                    sprite = 2;
                    activeframe[sprite] = 0;
                    sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                        spritesheet[sprite].Height, ref reverseanimation[sprite]);
                }
                else if (attack2time > 3 && r.Next(1, 100) % 4 == 0)
                {
                    SpriteEffects se = direction;

                    if (rioman.Location.X < location.Center.X)
                        direction = SpriteEffects.None;
                    else
                        direction = SpriteEffects.FlipHorizontally;

                    if (se == SpriteEffects.None && direction != se)
                        location.X += 35;
                    else if (se == SpriteEffects.FlipHorizontally && direction != se)
                        location.X -= 35;

                    attack2 = true;
                    sprite = 4;
                    activeframe[sprite] = 0;
                    sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                        spritesheet[sprite].Height, ref reverseanimation[sprite]);
                }
            }

            if (attack1)
            {
                canthurt = true;

                attack1time += elapsedtime;
                othertime += elapsedtime;

                if (attack1time > 0.2)
                {
                    attack1time = 0;
                    if (direction == SpriteEffects.None)
                        Shoot(float.Parse((r.Next(0, 100) / Math.PI).ToString()), location.X + 5, location.Center.Y, 3, 1, 10, true);
                    else
                        Shoot(float.Parse((r.Next(0, 100) / Math.PI).ToString()), location.X - 30, location.Center.Y, 3, 1, 10, true);

                    sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                        spritesheet[sprite].Height, ref reverseanimation[sprite]);
                }

                if (othertime > 2)
                {
                    othertime = 0;
                    attack1 = false;
                    reverseanimation[sprite] = false;
                    attack1time = 0;
                }
            }

            if (attack2)
            {
                attack2time += elapsedtime;
                othertime += elapsedtime;

                if (attack2time > 0.3)
                {

                    if (direction == SpriteEffects.None)
                        Shoot(float.Parse(Math.PI.ToString()), location.Left - 40, location.Center.Y, 5, 2, 8, true);
                    else
                        Shoot(0f, location.X + 20, location.Center.Y, 5, 2, 8, true);


                    sourcerect = Animation.GetSourceRect(frames[sprite], ref activeframe[sprite], spritesheet[sprite].Width,
                        spritesheet[sprite].Height, ref reverseanimation[sprite]);

                    attack2time = 0;
                }

                if (othertime > 0.5)
                {
                    attack2 = false;
                    othertime = 0;
                    attack2time = 0;
                    reverseanimation[sprite] = false;
                }
            }

            location.Width = sourcerect.Width;
            location.Height = sourcerect.Height;

            if (direction == SpriteEffects.None)
                collisionrect = new Rectangle(location.X - xoffset + 82, location.Y + 16, 32, location.Height - 16);
            else
                collisionrect = new Rectangle(location.X - xoffset + 46, location.Y + 16, 32, location.Height - 16);

            Jump(elapsedtime, 1, 10, true, 3);
            Fall(elapsedtime, true, 3);
        }
    }

}