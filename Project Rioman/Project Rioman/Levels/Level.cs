﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

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

        private AbstractEnemy[] enemies;
        public OldPickup[] pickups = new OldPickup[10];
        private List<AbstractPickup> items;

        public Boss[] bosses = new Boss[17];


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
            AbstractEnemy[] enemies, OldPickup[] pickups, Boss[] bosses)
        {
            backgroundcolour = bg;
            this.width = width;
            this.height = height;
            this.startPos = startpos;
            this.tiles = tiles;
            this.enemies = enemies;
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
            for (int i = 0; i <= enemies.Length - 1; i++)
                enemies[i].Reset(true);
        }

        public void ResetTiles()
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    if (tiles[x, y] != null)
                        tiles[x, y].Reset();
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
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    if (tiles[x, y] != null)
                        tiles[x, y].Draw(spriteBatch);
                }
            }


            //For debugging
            for (int i = 0; i <= scrollers.Length - 1; i++)
                scrollers[i].Draw(spriteBatch);

        }


        public void CenterRioman(Viewport viewport, Rioman player)
        {
            if (!preventCenteringRight && !preventCenteringLeft)
            {
                int middle = viewport.Width / 2;
                int xoffset = Convert.ToInt32(middle - player.Location.X);

                player.MoveToX(middle);
                MoveStuff(player, xoffset, 0);
            }
        }

        public void InteractWithLevel(Rioman player)
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
                        if (player.Right.Intersects(tle.Left))
                        {
                            player.StopRightMovement();
                            player.invincibledirection = 0;
                        }

                        if (player.Left.Intersects(tle.Right))
                        {
                            player.StopLeftMovement();
                            player.invincibledirection = 0;
                        }

                        if (player.Head.Intersects(tle.Bottom) && !player.IsClimbing())
                        {
                            player.state.Fall();
                        }

                    }
                    //kill player
                    else if (tle.type == 2)
                    {
                        if (player.Hitbox.Intersects(tle.location) && !player.IsInvincible())
                        {
                            lifechange = -1;
                            player.Die();
                            go = false;
                        }
                    }
                    //open door
                    else if (tle.type == 4)
                    {
                        if (player.Hitbox.Intersects(tle.location))
                            StartOpenDoor(tle);

                    }

                    if (tle.type == 3 && tle.isTop && player.IsClimbing() && player.Feet.Intersects(tle.Top))
                    {
                        climbTop = true;
                    }

                    if (tle.type == 1 || (player.IsInvincible() && tle.type == 2) || tle.type == 4 || tle.type == 5 || tle.type == 3 && tle.isTop)
                    {

                        if (!player.IsJumping() && player.Feet.Intersects(tle.Floor) && !player.Feet.Intersects(tle.IgnoreFloor))
                        {
                            onGround = true;
                            player.MoveToY(tle.Y - player.GetSprite().Height + 8);

                            if (player.IsFalling() || player.IsClimbing())
                            {
                                player.state.Stand();
                            }
                        }

                    }
                }
            }

            if (!onGround && !player.IsJumping() && !player.IsClimbing() && !player.IsOnEnemy())
                player.state.Fall();

            if (climbTop)
                player.AtLadderTop();
            else
                player.BelowLadderTop();
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

            MoveStuff(player, offsetX, offsetY);

            player.MoveToY(-100);
            player.MoveToX(Convert.ToInt32(centerPos.X));
            player.SetStartPos(Convert.ToInt32(centerPos.X), Convert.ToInt32(centerPos.Y));
            player.StartWarp();

        }











        public void Update(Rioman player, GameTime gameTime, Viewport viewport)
        {

            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            bool[] enemycollision = new bool[enemies.Length];
            bool bossfalling = true;


            UpdateScrollers(player, viewport);
            InteractWithLevel(player);
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

                    for (int e = 0; e <= enemies.Length-1; e++)
                        enemies[e].DetectTileCollision(tle);


                    for (int i = 0; i <= items.Count - 1; i++)
                        items[i].DetectTileCollision(tle);
                }

            }

            foreach (OldPickup pickup in pickups)
                pickup.PickupUpdate(player, viewport);


            for (int i = 0; i <= items.Count - 1; i++)
                items[i].Update(player, deltaTime, viewport);





        }



        public void TileFader()
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    if (tiles[x, y] != null && tiles[x, y].type == 5)
                    {
                        tiles[x, y].fadetime = fadetiles;
                        fadetiles++;
                    }
                }
            }
            fadetiles = 0;
        }




        public void MoveStuff(Rioman player, int x, int y)
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

            for (int i = 0; i <= enemies.Length -1; i++)
                enemies[i].Move(x, y);

            bosses[activelevel].Move(x, y);

            player.MoveBullets(x, y);
        }




        public void LadderForm()
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 1; y <= height; y++)
                {
                    if (x > 0 && tiles[x, y] != null && tiles[x, y].type == 3)
                    {
                        if (tiles[x, y - 1] == null)
                            tiles[x, y].isTop = true;
                        else if (tiles[x, y - 1] != null && tiles[x, y - 1].type != 3)
                            tiles[x, y].isTop = true;
                    }
                }
            }
        }


        public void CheckDeath(Viewport viewportrect, Rioman player)
        {
            if (player.Location.Y > viewportrect.Height + 100 || StatusBar.GetHealth() <= 0)
            {
                go = false;
                lifechange = -1;
                player.Die();

            }
        }

        public Rectangle CheckClimb(Rioman player, int climbloc, bool up, ref Rectangle location)
        {
            Rectangle rioLocation = player.Location;
            bool okay = false;

            foreach (Tile tle in tiles)
            {
                if (up && tle != null && tle.type == 3 && rioLocation.Intersects(tle.Floor) && rioLocation.Intersects(tle.IgnoreFloor))
                {
                    if (Math.Abs(rioLocation.X - tle.location.Center.X) <= 16 && !okay)
                    {
                        if (!stopLeftScreenMovement && !stopRightScreenMovement)
                            MoveStuff(player, climbloc - tle.location.Center.X, 0);
                        else
                            location.X = tle.location.Center.X;
                        okay = true;
                    }
                }

                if (!up && tle != null && tle.type == 3 && rioLocation.Intersects(tle.Floor) && !rioLocation.Intersects(tle.IgnoreFloor) && tle.isTop)
                {
                    if (Math.Abs(rioLocation.X - tle.location.Center.X) <= 16 && !okay)
                    {
                        if (!stopLeftScreenMovement && !stopRightScreenMovement)
                            MoveStuff(player, climbloc - tle.location.Center.X, 0);
                        else
                            location.X = tle.location.Center.X;
                        okay = true;
                    }
                }
            }

            if (!okay)
                rioLocation.Width = 0;

            return rioLocation;
        }

        public void UpdateEnemies(Rioman player, AbstractBullet[] bullets, double deltaTime, Viewport viewport)
        {

            for (int i = 0; i <= enemies.Length - 1; i++)
            {
                enemies[i].Update(player, bullets, deltaTime, viewport);
                AbstractPickup p = enemies[i].GetDroppedPickup();
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
            for (int i = 0; i <= enemies.Length - 1; i++)
                enemies[i].Draw(spriteBatch);
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


            public void Draw(SpriteBatch spriteBatch)
            {
                DebugDraw.DrawLine(spriteBatch, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

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
                        intersect.Left < endPoint.X && intersect.Right > startPoint.X)
                        return true;
                }

                return false;

            }

            public bool IntersectsProjection(Rectangle intersect)
            {
                if (IsVertical())
                {
                    if (intersect.Bottom > startPoint.Y && intersect.Top < endPoint.Y)
                        return true;
                }
                else {
                    if (intersect.Left < endPoint.X && intersect.Right > startPoint.X)
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

            for (int i = 0; i <= width; i++)
            {
                for (int j = 1; j <= height; j++)
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

                                    if (tiles[i, j + y] != null && tiles[i, j + y].tile == Constant.VERT_SCROLL2)
                                    {
                                        s.Add(new Scroller(true, i * Constant.TILE_SIZE, j * Constant.TILE_SIZE,
                                            i * Constant.TILE_SIZE, (j + y) * Constant.TILE_SIZE));

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

                                    if (tiles[i + x, j] != null && tiles[i + x, j].tile == Constant.HORIZ_SCROLL2)
                                    {
                                        s.Add(new Scroller(false, i * Constant.TILE_SIZE, j * Constant.TILE_SIZE,
                                            (i + x) * Constant.TILE_SIZE, j * Constant.TILE_SIZE));

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


        private void UpdateScrollers(Rioman player, Viewport viewport)
        {
            Rectangle vp = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            bool atTop = player.Location.Top < 3;
            bool atBottom = player.Location.Bottom > viewport.Height - 3;

            for (int i = 0; i <= scrollers.Length - 1; i++)
            {

                if (scrollers[i].IsVertical() && scrollers[i].Intersects(player.Hitbox))
                {

                    if (scrollers[i].X < viewport.Width && scrollers[i].X > viewport.Width * 2 / 3 &&
                        player.GetLastXMovement() > 0)
                        StartScroll(1, 0, viewport);

                    if (scrollers[i].X > 0 && scrollers[i].X < viewport.Width * 1 / 3 &&
                            player.GetLastXMovement() < 0)
                        StartScroll(-1, 0, viewport);

                }

                else if (scrollers[i].IsHorizontal() && scrollers[i].IntersectsProjection(player.Hitbox))
                {
                    if (atTop && scrollers[i].Y > - viewport.Height * 1 / 3 && scrollers[i].Y < viewport.Height * 1 / 3 &&
                            player.GetLastYMovement() < 0)
                        StartScroll(0, -1, viewport);

                    if (atBottom && scrollers[i].Y <  viewport.Height * 4 / 3 && scrollers[i].Y > viewport.Height * 2 / 3 &&
                            player.GetLastYMovement() > 0)
                        StartScroll(0, 1, viewport);
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


            if (preventCenteringRight && player.Location.X < viewport.Width / 2)
                preventCenteringRight = false;
            if (preventCenteringLeft && player.Location.X > viewport.Width / 2)
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
            MoveStuff(player, -scrollSpeedX, -scrollSpeedY);
            scrollAmountX -= Math.Abs(scrollSpeedX);
            scrollAmountY -= Math.Abs(scrollSpeedY);

            int x = 0, y = 0;

            if (moveNow)
            {
                if (scrollSpeedX != 0)
                    x = scrollSpeedX / Math.Abs(scrollSpeedX);
            }

            moveNow = !moveNow;

            if (scrollSpeedY != 0)
                y = scrollSpeedY / Math.Abs(scrollSpeedY);

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
                if (scrollSpeedX < 0)
                    preventCenteringRight = true;

            }

        }

        public bool CanMoveLeft() { return !stopLeftScreenMovement && !preventCenteringRight; }
        public bool CanMoveRight() { return !stopRightScreenMovement && !preventCenteringLeft; }

        public AbstractEnemy[] GetEnemies() { return enemies; }
    }
}
