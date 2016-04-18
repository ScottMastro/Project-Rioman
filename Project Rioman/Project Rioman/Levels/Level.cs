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

        public Enemy[] enemies;
        private AbstractEnemy[] aenemy;
        public OldPickup[] pickups = new OldPickup[10];
        private List<AbstractPickup> items; 

        public Boss[] bosses = new Boss[17];
        int numberOfEnemies;



        int fadetiles;
        private int doorStopY;
        private Tile doorTop;
        private bool doorOpening;
        private bool closeDoor;
        private bool doorClosing;
        private bool isScrolling;
        public bool killBullets;

        private const int DOOR_SPEED = 2;

        int scrollSpeedY;
        int scrollSpeedX;
        private const int SCROLL_SPEED = 10;

        private bool stopRightScreenMovement;
        private bool stopLeftScreenMovement;
        private bool preventCenteringLeft;
        private bool preventCenteringRight;

        private Scroller[] scrollers;

        private int scrollAmountX;
        private int scrollAmountY;

        public int lifechange = 0;



        public Level(Color bg, int width, int height, Vector2 startpos, Tile[,] tiles,
            Enemy[] enemies, AbstractEnemy[] ae, OldPickup[] pickups, Boss[] bosses)
        {
            backgroundcolour = bg;
            this.width = width;
            this.height = height;
            this.startPos = startpos;
            this.tiles = tiles;
            this.enemies = enemies;
            aenemy = ae;
            numberOfEnemies = enemies.Length;
            this.pickups = pickups;
            items = new List<AbstractPickup>();
            this.bosses = bosses;
            go = false;


        }

        public void Reset() 
        {
            ResetTiles();
            ResetEnemies();

            stopLeftScreenMovement = false;
            stopRightScreenMovement = false;
            preventCenteringLeft = false;
            preventCenteringRight = false;

            items = new List<AbstractPickup>();

            scrollers = MakeScroller();

            doorOpening = false;

        }


        public void ResetEnemies()
        {
            for (int i = 0; i <= aenemy.Length - 1; i++)
                aenemy[i].Reset(true);
            

        }

        public void ResetTiles()
        {
            for (int r = 0; r <= height; r++)
            {
                for (int c = 0; c <= width; c++)
                {
                    if (tiles[r, c] != null)
                        tiles[r, c].Reset();
                }
            }

        }

        public void BusyUpdate(Rioman player, Viewport viewport)
        {
            if (doorOpening)
                OpenDoor();
            if (doorClosing)
                CloseDoor();
            if (isScrolling)
                Scroll(player, viewport);
        }

        public void DrawItems(SpriteBatch spriteBatch)
        {
            foreach (AbstractPickup item in items)
            {
                item.Draw(spriteBatch);
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
            if (!preventCenteringRight && !preventCenteringLeft)
            {
                int middle = viewportrect.Width / 2;
                int xoffset = Convert.ToInt32(middle - rioman.Location.X);

                rioman.MoveToX(middle);
                MoveStuff(xoffset, 0);
            }
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
                        if (rioman.Hitbox.Intersects(tle.location) && !rioman.IsInvincible())
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

                    if (tle.type == 1 || (rioman.IsInvincible() && tle.type == 2) || tle.type == 4 || tle.type == 5 || tle.type == 3 && tle.isTop)
                    {

                        if (!rioman.IsJumping() && rioman.Feet.Intersects(tle.Floor) && !rioman.Feet.Intersects(tle.IgnoreFloor))
                        {
                            onGround = true;
                            rioman.MoveToY(tle.Y - rioman.GetSprite().Height + 8);

                            if (rioman.IsFalling() || rioman.IsClimbing())
                            {
                                rioman.state.Stand();
                            }
                        }

                    }
                }
            }

            if (!onGround && !rioman.IsJumping() && !rioman.IsClimbing() && !rioman.IsOnEnemy())
                rioman.state.Fall();

            if (climbTop)
                rioman.AtLadderTop();
            else
                rioman.BelowLadderTop();
        }

       

        public bool IsBusy()
        {
            return doorOpening || doorClosing || isScrolling;
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
                    if (tiles[row, column].Y > doorStopY)
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











        public void Update(Rioman rioman, GameTime gameTime, Viewport viewport)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            bool[] enemycollision = new bool[numberOfEnemies + 1];
            bool bossfalling = true;


            UpdateScrollers(rioman, viewport);
            InteractWithLevel(rioman);
            foreach (Tile tle in tiles)
            {
                if (tle != null)
                {


                    if (tle.type == 1 || tle.type == 4 || tle.type == 5)
                    {
                        foreach (OldPickup pickup in pickups)
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
                            if (enemies[i].isalive && !rioman.IsInvincible() && rioman.Hitbox.Intersects(enemies[i].collisionrect)
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


                if (tle != null)
                {

                    for (int e = 0; e <= aenemy.Length - 1; e++)
                        aenemy[e].DetectTileCollision(tle);


                    for (int i = 0; i <= items.Count - 1; i++)
                        items[i].DetectTileCollision(tle);
                }

            }


            for (int i = 0; i < numberOfEnemies; i++)
                enemies[i].isfalling = !enemycollision[i];



            foreach (OldPickup pickup in pickups)
                pickup.PickupUpdate(rioman, viewport);


            for (int i = 0; i<= items.Count -1; i++) 
                items[i].Update(rioman, deltaTime, viewport);





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


            foreach (OldPickup pickup in pickups)
            {
                if (pickup.isalive)
                    pickup.MovePickup(x, y);
            }

            for (int i = 0; i <= items.Count - 1; i++)
                items[i].Move(x, y);

            for (int i = 0; i < numberOfEnemies; i++)
                enemies[i].MoveEnemies(x, y);

            for (int i = 0; i <= aenemy.Length - 1; i++)
                aenemy[i].Move(x, y);

            bosses[activelevel].Move(x, y);
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
                        if (!stopLeftScreenMovement && !stopRightScreenMovement)
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
                        if (!stopLeftScreenMovement && !stopRightScreenMovement)
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

        public void UpdateEnemies(Rioman rioman, Bullet[] bullets, double deltaTime, Viewport viewport)
        {

            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemies[i].UpdateEnemy(rioman, viewport, deltaTime);
            }

            for (int i = 0; i < aenemy.Length; i++)
            {
                aenemy[i].Update(rioman, bullets, deltaTime, viewport);
                AbstractPickup p = aenemy[i].GetDroppedPickup();
                if (p != null)
                    items.Add(p);
            }

            foreach (OldPickup pickup in pickups)
            {
                if (pickup.isalive)
                    pickup.Animate(deltaTime);
            }
        }

        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < numberOfEnemies; i++)
                enemies[i].Draw(spriteBatch, isScrolling);

            for (int i = 0; i < aenemy.Length - 1; i++)
                aenemy[i].Draw(spriteBatch);
        }

        public void EnemyCollision(Bullet[] bullets, Rioman rioman)
        {
            if (bosses[activelevel].isalive)
                bosses[activelevel].Collision(bullets, rioman);

            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (enemies[i].isalive)
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
                            if (bullet.isAlive && enemies[i].location.Intersects(bullet.location))
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
                                bullet.isAlive = false;

                                if (enemies[i].type == 308)
                                    enemies[i].bool1 = true;
                            }
                            else if (enemies[i].type == 301)
                                enemies[i].CartCollision(bullet);
                        }
                    }

                }
                if (enemies[i].isalive && !rioman.IsInvincible() && rioman.Hitbox.Intersects(enemies[i].collisionrect))
                {
                    //      if (enemies[i].type == 299)
                    //        rioman.ispaused = true;
                    // else
                    if (enemies[i].type != 302)
                    {
                        Health.AdjustHealth(-enemies[i].damage);
                       // rioman.Hit();

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
                         //   rioman.Hit();
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
                        if (!rioman.IsInvincible() && rioman.Hitbox.Intersects(enemies[i].bulletloc[j]))
                        {
                            enemies[i].bulletalive[j] = false;
                            enemies[i].bullettime[j] = 0;
                            Health.AdjustHealth(-3);

//                            rioman.Hit();

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
                        if (!rioman.IsInvincible() && rioman.Hitbox.Intersects(enemies[i].other[k]))
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


                         //   rioman.Hit();

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

        // ----------------------------------------------------------------------------------------------------------------
        // Scroller Logic
        // ----------------------------------------------------------------------------------------------------------------


        struct Scroller
        {
            private Point startPoint;
            private Point endPoint;

            private enum Direction { horizontal, vertical };
            private Direction dir;

            public Scroller(bool isVertical, int x1, int y1, int x2, int y2)
            {
                if (isVertical)
                    dir = Direction.vertical;
                else
                    dir = Direction.horizontal;

                startPoint = new Point(x1, y1);
                endPoint = new Point(x2, y2);

            }

            public void Move(int x, int y)
            {
                startPoint.X += x;
                startPoint.Y += y;
                endPoint.X += x;
                endPoint.Y += y;
            }


            public bool Intersects(Rectangle intersect)
            {
                if (IsVertical())
                {
                    if (intersect.Left < startPoint.X && intersect.Right > startPoint.X &&
                        intersect.Bottom > startPoint.Y && intersect.Top < endPoint.Y)
                        return true;
                }
                else {
                    if (intersect.Bottom > startPoint.Y && intersect.Top < startPoint.Y &&
                        intersect.Left < startPoint.X && intersect.Right > endPoint.X)
                        return true;
                }

                return false;

            }

            public bool IsVertical()
            {
                return dir == Direction.vertical;
            }
            public bool IsHorizontal()
            {
                return dir == Direction.horizontal;
            }

            public int X { get { return (startPoint.X + endPoint.X) / 2; } }
            public int Y { get { return (startPoint.Y + endPoint.Y) / 2; } }
            public int Top { get { return startPoint.Y; } }
            public int Bottom { get { return endPoint.Y; } }
            public int Left { get { return startPoint.X; } }
            public int Right { get { return endPoint.X; } }
        }

        private Scroller[] MakeScroller()
        {
            List<Scroller> s = new List<Scroller>();
            int counter = 0;

            Rectangle scroll = new Rectangle();

            for (int i = 0; i <= height; i++)
            {
                for (int j = 1; j <= width; j++)
                {
                    if (tiles[i, j] != null)
                    {
                        if (tiles[i, j].tile == Constant.VERT_SCROLL)
                        {
                            int y = 0;
                            try
                            {
                                while (true)
                                {
                                    y++;

                                    if (tiles[i + y, j] != null && tiles[i + y, j].tile == Constant.VERT_SCROLL2)
                                    {
                                        s.Add(new Scroller(true, j * Constant.TILE_SIZE, i * Constant.TILE_SIZE,
                                            j * Constant.TILE_SIZE, (i + y) * Constant.TILE_SIZE));

                                        counter++;

                                    }
                                }
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                Console.WriteLine("Failed to construct vertical scroller on level " + activelevel.ToString());
                            }
                        }

                        else if (tiles[i, j].tile == Constant.HORIZ_SCROLL)
                        {
                            int x = 0;
                            try
                            {
                                while (true)
                                {
                                    x++;

                                    if (tiles[i, j + x] != null && tiles[i, j + x].tile == Constant.HORIZ_SCROLL2)
                                    {
                                        s.Add(new Scroller(true, j * Constant.TILE_SIZE, i * Constant.TILE_SIZE,
                                            (j + x) * Constant.TILE_SIZE, i * Constant.TILE_SIZE));

                                        counter++;
                                    }
                                }
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                Console.WriteLine("Failed to construct horizontal scroller on level " + activelevel.ToString());
                            }
                        }

                    }
                }
            }

            if (counter == 0)
                return new Scroller[0];
            else
                return new List<Scroller>(s).GetRange(0, counter).ToArray();
        }


        private void UpdateScrollers(Rioman rioman, Viewport viewport)
        {
            Rectangle vp = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            for (int i = 0; i <= scrollers.Length - 1; i++)
            {

                if (scrollers[i].Intersects(rioman.Hitbox))
                {
                    if (scrollers[i].X < viewport.Width && scrollers[i].X > viewport.Width * 2 / 3 &&
                        scrollers[i].IsVertical() && rioman.GetLastXMovement() > 0)
                        StartScroll(1, 0, viewport);

                    if (scrollers[i].X > 0 && scrollers[i].X < viewport.Width * 1 / 3 &&
                            scrollers[i].IsVertical() && rioman.GetLastXMovement() < 0)
                        StartScroll(-1, 0, viewport);

                }

                if (scrollers[i].IsVertical())
                {
                    if (scrollers[i].X < viewport.Width && scrollers[i].X > viewport.Width * 2 / 3 &&
                        scrollers[i].Bottom > 0 && scrollers[i].Top < viewport.Height)
                    {
                        stopRightScreenMovement = true;
                        preventCenteringRight = true;
                    }
                    else if (scrollers[i].X > 0 && scrollers[i].X < viewport.Width * 1 / 3 &&
                        scrollers[i].Bottom > 0 && scrollers[i].Top < viewport.Height)
                    {
                        stopLeftScreenMovement = true;
                        preventCenteringLeft = true;
                    }

                }
            }

            if (preventCenteringRight && rioman.Location.X < viewport.Width / 2)
                preventCenteringRight = false;
            if (preventCenteringLeft && rioman.Location.X > viewport.Width / 2)
                preventCenteringLeft = false;
        }


        private void StartScroll(int magnitudeX, int magnitudeY, Viewport viewport)
        {
            scrollAmountX = (viewport.Width - Constant.TILE_SIZE / 2) * Math.Abs(magnitudeX);
            scrollAmountY = (viewport.Height - Constant.TILE_SIZE / 2) * Math.Abs(magnitudeY);
            isScrolling = true;
            scrollSpeedX = magnitudeX * SCROLL_SPEED;
            scrollSpeedY = magnitudeY * SCROLL_SPEED;
            killBullets = true;
        }

        private bool moveNow = false;

        public void Scroll(Rioman player, Viewport viewport)
        {
            MoveStuff(-scrollSpeedX, -scrollSpeedY);
            scrollAmountX -= Math.Abs(scrollSpeedX);
            scrollAmountY -= Math.Abs(scrollSpeedY);

            int x = 0, y = 0;

            if (moveNow)
            {
                if (scrollSpeedX != 0)
                    x = scrollSpeedX / Math.Abs(scrollSpeedX);
                if (scrollSpeedY != 0)
                    y = scrollSpeedY / Math.Abs(scrollSpeedY);
            }

            moveNow = !moveNow;

            player.Move(-scrollSpeedX + x, -scrollSpeedY + y);



            if (scrollAmountX <= 0 && scrollAmountY <= 0)
            {

                if (closeDoor)
                    doorClosing = true;
                isScrolling = false;
                stopRightScreenMovement = true;
                stopLeftScreenMovement = true;

                if (scrollSpeedX > 0)
                    preventCenteringLeft = true;
            }

        }

        public bool CanMoveLeft() { return !stopLeftScreenMovement && !preventCenteringRight; }
        public bool CanMoveRight() { return !stopRightScreenMovement && !preventCenteringLeft; }

    }
}
