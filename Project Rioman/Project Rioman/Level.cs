using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Project_Rioman
{
    class Level
    {

        private Tile[,] tiles;

        public int activelevel;

        public Color backgroundcolour;
        public int width;
        public int height;
        public bool go;
        private Vector2 startPos;

        public OldEnemy[] enemies;
        public Pickup[] pickups = new Pickup[10];
        public Boss[] bosses = new Boss[17];
        int numberOfEnemies;

        public bool stopleftscreenmovement;
        public bool stoprightscreenmovement;

        int fadetiles;
        private int doorStopY;
        private Tile doorTop;
        private bool doorOpening;
        private bool closeDoor;
        private bool doorClosing;
        public bool isScrolling;
        public bool killbullets;

        private const int SCROLL_SPEED = 10;
        private const int DOOR_SPEED = 2;

        int yscroll;
        int xscroll;
        public Rectangle scrollingRect;

        private Scroller[] scrollers;

        struct Scroller
        {
            public Rectangle scrollRect;
            public string direction;
            public bool active;

            public Scroller(Rectangle rect, string dir)
            {
                scrollRect = rect;
                direction = dir;
                active = true;
            }

            public void Move(int x, int y)
            {
                scrollRect.X += x;
                scrollRect.Y += y;
            }

            public bool Intersects(Rectangle intersect)
            {
                if (active)
                    return scrollRect.Intersects(intersect);
                return false;
            }

        }

        int xscrollamount;
        int yscrollamount;

        public int lifechange = 0;



        public Level(Color bg, int width, int height, Vector2 startpos, Tile[,] tiles,
            OldEnemy[] enemies, Pickup[] pickups, Boss[] bosses)
        {
            backgroundcolour = bg;
            this.width = width;
            this.height = height;
            this.startPos = startpos;
            this.tiles = tiles;
            this.enemies = enemies;
            numberOfEnemies = enemies.Length;
            this.pickups = pickups;
            this.bosses = bosses;
            go = false;



        }

        public void Reset()
        {
            ResetTiles();

            scrollingRect = new Rectangle();
            scrollers = MakeScroller();

            doorOpening = false;

        }

        public void ResetTiles()
        {
            for (int r = 0; r <= height; r++)
            {
                for (int c = 0; c <= width; c++)
                {
                    if(tiles[r,c] != null)
                    tiles[r, c].Reset();
                }
            }

        }

        public void DrawTiles(SpriteBatch spriteBatch)
        {
            for (int r = 0; r <= height; r++)
            {
                for (int c = 0; c <= width; c++)
                {
                    if (tiles[r, c] != null)
                        tiles[r, c].Draw(spriteBatch);
                }
            }

        }
        

        public void CenterRioman(Viewport viewportrect, Rioman rioman)
        {
            int middle = viewportrect.Width / 2;

            int xoffset = Convert.ToInt32(middle - rioman.Location.X);

            rioman.MoveToX(middle);

            MoveStuff(xoffset, 0);
        }

        public void InteractWithLevel(Rioman rioman)
        {
            bool climbTop = false;
            bool onGround = false;

            foreach (Tile tle in tiles)
            {
                if (tle != null)
                {
                    //limit player motion against solid tiles
                    if (tle.type == 1)
                    {
                        if (rioman.Right.Intersects(tle.Left))
                        {
                            rioman.StopRightMovement();
                            rioman.invincibledirection = 0;
                        }

                        if (rioman.Left.Intersects(tle.Right))
                        {
                            rioman.StopLeftMovement();
                            rioman.invincibledirection = 0;
                        }

                        if (rioman.Head.Intersects(tle.Bottom) && !rioman.IsClimbing())
                        {
                            rioman.state.Fall();
                        }

                    }
                    //kill player
                    else if (tle.type == 2)
                    {
                        if (rioman.Hitbox.Intersects(tle.location) && !rioman.isinvincible)
                        {
                            lifechange = -1;
                            rioman.Die();
                            go = false;
                        }
                    }
                    //open door
                    else if (tle.type == 4)
                    {
                        if (rioman.Hitbox.Intersects(tle.location))
                            StartOpenDoor(tle);

                    }

                    if (tle.type == 3 && tle.isTop && rioman.IsClimbing() && rioman.Feet.Intersects(tle.Top))
                    {
                        climbTop = true;
                    }

                    if (tle.type == 1 || tle.type == 4 || tle.type == 5 || tle.type == 3 && tle.isTop)
                    {

                        if (!rioman.IsJumping() && rioman.Feet.Intersects(tle.Floor) && !rioman.Feet.Intersects(tle.IgnoreFloor))
                        {
                            onGround = true;
                            rioman.MoveToY(tle.Y - rioman.Height + 8);

                            if (rioman.IsFalling() || rioman.IsClimbing())
                            {
                                rioman.state.Stand();
                            }
                        }

                    }
                }
            }

            if (!onGround && !rioman.IsJumping() && !rioman.IsClimbing())
                rioman.state.Fall();

            if (climbTop)
                rioman.AtLadderTop();
            else
                rioman.BelowLadderTop();
        }

        private Scroller[] MakeScroller()
        {
            Scroller[] s = new Scroller[100];
            int counter = 0;

            Rectangle scroll = new Rectangle();

            for (int r = 0; r <= height; r++)
            {
                for (int c = 1; c <= width; c++)
                {

                    //if tile is a scroller
                    if (tiles[r, c] != null && tiles[r, c].type == 6)
                    {
                        int x = 0;
                        int y = 0;

                        scroll.X = tiles[r, c].location.Right + 4;
                        scroll.Y = tiles[r, c].location.Bottom;

                        //find horizontal scroller tile paired with this one
                        while (true)
                        {
                            x++;

                            if (tiles[r, c + x] != null && tiles[r, c + x].tile == tiles[r, c].tile)
                            {
                                scroll.Width = tiles[r, c + x].location.X - tiles[r, c].location.X;
                                break;
                            }
                        }

                        //find vertical scroller tile paired with this one
                        while (true)
                        {
                            y++;

                            if (tiles[r + y, c + x] != null && tiles[r + y, c + x].tile == tiles[r, c].tile)
                            {
                                scroll.Height = tiles[r + y, c + x].location.Y - tiles[r, c + x].location.Y;
                                tiles[r + y, c + x].type = -1;
                                tiles[r, c + x].type = -1;
                                tiles[r + y, c].type = -1;
                                break;
                            }
                        }


                        if (tiles[r, c].tile == 318)
                            s[counter] = new Scroller(scroll, "up");
                        else if (tiles[r, c].tile == 319)
                            s[counter] = new Scroller(scroll, "down");
                        else if (tiles[r, c].tile == 320)
                            s[counter] = new Scroller(scroll, "right");
                        else if (tiles[r, c].tile == 321)
                            s[counter] = new Scroller(scroll, "left");

                        counter++;


                    }
                }
            }

            if (counter == 0)
                return new Scroller[1];
            else
                return new List<Scroller>(s).GetRange(0, counter).ToArray();
        }

        private void UpdateScrollers(Rioman rioman, Viewport viewportrect)
        {
            for (int i = 0; i <= scrollers.Length - 1; i++)
            {

                if (scrollers[i].Intersects(rioman.Hitbox))
                {
                    if (scrollers[i].direction == "up")
                        StartScroll(0, 1, scrollers[i], viewportrect);
                    else if (scrollers[i].direction == "down")
                        StartScroll(0, -1, scrollers[i], viewportrect);
                    else if (scrollers[i].direction == "right")
                        StartScroll(-1, 0, scrollers[i], viewportrect);
                    else if (scrollers[i].direction == "left")
                        StartScroll(1, 0, scrollers[i], viewportrect);

                    scrollers[i].active = false;
                }

                if (scrollers[i].direction == "right" && scrollers[i].scrollRect.X < viewportrect.Width)
                    stoprightscreenmovement = true;
                else if (scrollers[i].direction == "left" && scrollers[i].scrollRect.X > 0)
                    stopleftscreenmovement = true;

            }


            if (scrollingRect.Width > 0)
            {
                if (!(scrollingRect.X < 0))
                    stopleftscreenmovement = true;
                if (!(scrollingRect.Right > viewportrect.Width))
                    stoprightscreenmovement = true;
            }

        }


        private void StartScroll(int xmagnitude, int ymagnitude, Scroller scroller, Viewport viewportrect)
        {
            scrollingRect = scroller.scrollRect;
            xscrollamount = viewportrect.Width - 40;
            yscrollamount = viewportrect.Height - 32;
            isScrolling = true;
            xscroll = xmagnitude * SCROLL_SPEED;
            yscroll = ymagnitude * SCROLL_SPEED;
        }


        public void Scroll(Rioman rioman, Viewport viewportrect)
        {
            MoveStuff(xscroll, yscroll);
            killbullets = true;
            xscrollamount -= Math.Abs(xscroll);
            yscrollamount -= Math.Abs(yscroll);
            rioman.Move(xscroll, yscroll);

            if (xscroll != 0)
                rioman.Move(-(xscroll / Math.Abs(xscroll)), 0);
            if (yscroll != 0)
                rioman.Move(0, -(yscroll / Math.Abs(yscroll)));

            if (xscrollamount <= 0 || yscrollamount <= 0)
            {

                if (closeDoor)
                    doorClosing = true;
                isScrolling = false;
                stopleftscreenmovement = true;
                stoprightscreenmovement = true;
                scrollingRect.X -= 32;
            }

        }

        public bool IsBusy()
        {
            return doorOpening || doorClosing;
        }

        public void BusyUpdate()
        {
            if (doorOpening)
                OpenDoor();
            if (doorClosing)
                CloseDoor();
        }


        private void StartOpenDoor(Tile tle)
        {
            doorTop = tle;
            doorOpening = true;

            int row = tle.Row;
            int column = tle.Column;

            //Check for door tiles above tle
            while (true)
            {
                if (row > 0 && tiles[row - 1, column] != null && tiles[row - 1, column].type == 4)
                {
                    doorTop = tiles[row - 1, column];
                    row--;
                }
                else
                    break;
            }

            doorStopY = doorTop.Y - Constant.TILE_SIZE;
        }

        private void OpenDoor()
        {
            bool stillOpening = false;

            int row = doorTop.Row;
            int column = doorTop.Column;

            while (true)
            {
                if (row < height && tiles[row, column] != null && tiles[row, column].type == 4)
                {
                    if(tiles[row, column].Y > doorStopY)
                    {
                        stillOpening = true;
                        tiles[row, column].Move(0, -DOOR_SPEED);
                    }
                    
                    row++;
                }
                else
                    break;
            }

            if (stillOpening)
                Audio.PlayDoor();
            else
            {
                doorOpening = false;
                closeDoor = true;
            }
        }


        public void CloseDoor()
        {
            bool stillClosing = false;

            int row = doorTop.Row;
            int column = doorTop.Column;

            while (true)
            {
                if (row < height && tiles[row, column] != null && tiles[row, column].type == 4)
                {
                    if (row > 0 && tiles[row - 1, column] != null &&
                        tiles[row, column].Y < tiles[row - 1, column].location.Bottom)
                    {
                        stillClosing = true;
                        tiles[row, column].Move(0, DOOR_SPEED);
                    }                        

                    row++;
                }
                else
                    break;
            }

            if (stillClosing)
                Audio.PlayDoor();
            else
            {
                closeDoor = false;
                doorClosing = false;

                //make door a wall
                row = doorTop.Row;

                while (true)
                {
                    if (row < height && tiles[row, column] != null && tiles[row, column].type == 4)
                    {
                        tiles[row, column].ChangeType(1);
                        row++;
                    }
                    else
                        break;
                }
            }
        }

        public void Play(Viewport viewport, Rioman player)
        {

            Reset();
            go = true;

            Vector2 centerPos = new Vector2(viewport.Width / 2, viewport.Height - 32 * 4);

            int offsetX = Convert.ToInt32(centerPos.X - startPos.X);
            int offsetY = Convert.ToInt32(centerPos.Y - startPos.Y);

            MoveStuff(offsetX, offsetY);

            player.MoveToY(-100);
            player.MoveToX(Convert.ToInt32(centerPos.X));
            player.SetStartPos(Convert.ToInt32(centerPos.X), Convert.ToInt32(centerPos.Y));
            player.StartWarp();

        }











        public void Update(Rioman rioman, GameTime gameTime, Viewport viewportRect)
        {

            stopleftscreenmovement = false;
            stoprightscreenmovement = false;
            bool[] enemycollision = new bool[numberOfEnemies + 1];
            bool bossfalling = true;


            UpdateScrollers(rioman, viewportRect);
            InteractWithLevel(rioman);
            foreach (Tile tle in tiles)
            {
                if (tle != null)
                {


                    if (tle.type == 1 || tle.type == 4 || tle.type == 5)
                    {
                        foreach (Pickup pickup in pickups)
                            pickup.PickupUpdate(tle);

                        if (bosses[activelevel].pickup)
                            bosses[activelevel].weapon.PickupUpdate(tle);
                    }





                    if (tle.type == 5)
                        tle.Fade(gameTime);

                    if (tle.tile == 102)
                        tle.Wave(gameTime);

                    for (int i = 0; i < numberOfEnemies; i++)
                    {
                        if (enemies[i].type == 302)
                        {
                            if (enemies[i].isalive && !rioman.isinvincible && rioman.Hitbox.Intersects(enemies[i].collisionrect)
                                && Math.Abs(rioman.Location.Bottom - enemies[i].collisionrect.Y) < 10)
                            {
                                rioman.MoveToY(enemies[i].collisionrect.Y - rioman.Location.Height + 2);
                            }
                        }

                        if (enemies[i].isalive && !enemies[i].throughground)
                        {
                            if (tle.type == 1 || tle.type == 3 && tle.isTop || tle.type == 4 || tle.type == 5)
                            {
                                if (enemies[i].location.Intersects(tle.Floor) && !enemies[i].location.Intersects(tle.IgnoreFloor))
                                {
                                    if (enemies[i].type != 317)
                                        enemies[i].location.Y = tle.location.Y - enemies[i].location.Height + 2;
                                    enemycollision[i] = true;
                                    enemies[i].falltime = 0;
                                }
                                if (enemies[i].location.Intersects(tle.IgnoreFloor))
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

                                if (enemies[i].location.Intersects(tle.Top) && tle.type != 3)
                                    enemies[i].direction = SpriteEffects.None;
                                else if (enemies[i].location.Intersects(tle.Bottom) && tle.type != 3)
                                    enemies[i].direction = SpriteEffects.FlipHorizontally;
                            }
                        }
                    }

                    if (bosses[activelevel].isalive)
                    {
                        if (tle.type == 1 || tle.type == 3 && tle.isTop || tle.type == 4 || tle.type == 5)
                        {
                            if (tle.type != 4 && bosses[activelevel].location.Intersects(tle.Floor) && !bosses[activelevel].location.Intersects(tle.IgnoreFloor))
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
                            if (bosses[activelevel].location.Intersects(tle.Top) && tle.type != 3)
                                bosses[activelevel].direction = SpriteEffects.None;
                            else if (bosses[activelevel].location.Intersects(tle.Bottom) && tle.type != 3)
                                bosses[activelevel].direction = SpriteEffects.FlipHorizontally;
                        }
                    }
                }
            }


            for (int i = 0; i < numberOfEnemies; i++)
                enemies[i].isfalling = !enemycollision[i];

            

            foreach (Pickup pickup in pickups)
                pickup.PickupUpdate(rioman, viewportRect);





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




        public void MoveStuff(int x, int y)
        {
            foreach (Tile tle in tiles)
            {
                if (tle != null)
                    tle.Move(x, y);
            }


            for (int i = 0; i <= scrollers.Length - 1; i++)
                scrollers[i].Move(x, y);


            foreach (Pickup pickup in pickups)
            {
                if (pickup.isalive)
                    pickup.MovePickup(x, y);
            }

            for (int i = 0; i < numberOfEnemies; i++)
                enemies[i].MoveEnemies(x, y);

            bosses[activelevel].Move(x, y);

            scrollingRect.X += x;
            scrollingRect.Y += y;
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
                            tiles[r, c].isTop = true;
                        else if (tiles[r - 1, c] != null && tiles[r - 1, c].type != 3)
                            tiles[r, c].isTop = true;
                    }
                }
            }
        }


        public void CheckDeath(Viewport viewportrect, Rioman rioman)
        {
            if (rioman.Location.Y > viewportrect.Height + 100 || Health.GetHealth() <= 0)
            {
                go = false;
                lifechange = -1;
                rioman.Die();

            }
        }

        public Rectangle CheckClimb(Rectangle riolocation, int climbloc, bool up, ref Rectangle location)
        {
            bool okay = false;

            foreach (Tile tle in tiles)
            {
                if (up && tle != null && tle.type == 3 && riolocation.Intersects(tle.Floor) && riolocation.Intersects(tle.IgnoreFloor))
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

                if (!up && tle != null && tle.type == 3 && riolocation.Intersects(tle.Floor) && !riolocation.Intersects(tle.IgnoreFloor) && tle.isTop)
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

            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemies[i].UpdateEnemy(rioman, viewportrect, gameTime.ElapsedGameTime.TotalSeconds);
            }
                foreach (Pickup pickup in pickups)
            {
                if (pickup.isalive)
                    pickup.Animate(gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < numberOfEnemies; i++)
                enemies[i].Draw(spriteBatch, isScrolling);
        }

        public void EnemyCollision(Bullet[] bullets, Rioman rioman)
        {
            if (bosses[activelevel].isalive)
                bosses[activelevel].Collision(bullets, rioman);

            for (int i = 0; i < numberOfEnemies; i++)
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
                if (enemies[i].isalive && !rioman.isinvincible && rioman.Hitbox.Intersects(enemies[i].collisionrect))
                {
                    if (enemies[i].type == 299)
                        rioman.ispaused = true;
                    else if (enemies[i].type != 302)
                    {
                        Health.AdjustHealth(-enemies[i].damage);
                        rioman.Hit();

                        if (rioman.Location.Left < enemies[i].location.Left)
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
                            Health.AdjustHealth(-enemies[i].damage);
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
                        if (!rioman.isinvincible && rioman.Hitbox.Intersects(enemies[i].bulletloc[j]))
                        {
                            enemies[i].bulletalive[j] = false;
                            enemies[i].bullettime[j] = 0;
                            Health.AdjustHealth(-3);

                            rioman.Hit();

                            if (rioman.Location.Left < enemies[i].bulletloc[j].Left)
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
                        if (!rioman.isinvincible && rioman.Hitbox.Intersects(enemies[i].other[k]))
                        {
                            if (k == 0 || k == 4 || k == 5)
                                enemies[i].other[k].Y = -100;
                            else if (k == 6 || k == 3 || k == 7)
                                enemies[i].other[k].Y = 1000;
                            else if (k == 1)
                                enemies[i].other[k].X = -100;
                            else if (k == 2)
                                enemies[i].other[k].X = 1000;

                            Health.AdjustHealth(-enemies[i].damage);


                            rioman.Hit();

                            if (rioman.Location.Left < enemies[i].other[k].Left)
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
