﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Project_Rioman
{
    class Level
    {

        private Tile[,] tileGrid;
        private List<List<Tile>> tiles;
        private List<Tile> allTiles;

        public int levelID;

        public Color backgroundcolour;
        public int width;
        public int height;
        public bool go;
        private Vector2 startPos;

        private AbstractEnemy[] enemies;
        private List<AbstractPickup> items;
        private PowerUp bossPowerUp;
        private AbstractBoss boss;

        private int doorStopY;
        private Tile doorTop;
        private bool allowScrolling = false;
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
            AbstractEnemy[] enemies, OldPickup[] pickups, AbstractBoss boss)
        {
            backgroundcolour = bg;
            this.width = width;
            this.height = height;
            this.startPos = startpos;
            this.tileGrid = tiles;
            this.enemies = enemies;
           //TODO this.pickups = pickups;
            items = new List<AbstractPickup>();
            this.boss = boss;
            go = false;

        }

        public void StartLevel(Viewport viewport, Rioman player)
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

        public void Reset()
        {
            ResetTiles();
            OrganizeTiles();
            ResetEnemies();
            boss.Reset();

            stopLeftScreenMovement = false;
            stopRightScreenMovement = false;
            preventCenteringLeft = false;
            preventCenteringRight = false;

            items = new List<AbstractPickup>();
            bossPowerUp = null;
             
            scrollers = MakeScroller();
            allowScrolling = true;

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
                for (int y = 0; y <= height; y++)
                {
                    if (tileGrid[x, y] != null)
                        tileGrid[x, y].Reset();
                }
        }


        public bool IsBusy() { return doorOpening || doorClosing || isScrolling || boss.IsBusy(); }
        public void BusyUpdate(Rioman player, double deltaTime, Viewport viewport)
        {
            if (doorOpening)
                OpenDoor();
            if (doorClosing)
                CloseDoor(player);
            if (isScrolling)
                Scroll(player, viewport);
            if (boss.IsBusy())
                boss.BusyUpdate(deltaTime);
        }
        
        public void Update(Rioman player, GameTime gameTime, Viewport viewport)
        {

            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            bool[] enemycollision = new bool[enemies.Length];
            bool bossfalling = true;

            UpdateScrollers(player, viewport);
            InteractWithLevel(player);

            foreach (Tile tile in tileType(Constant.TILE_DISAPPEAR))
                tile.Fade(gameTime);

            foreach (Tile tile in tileType(Constant.TILE_SOLID)) {

                for (int e = 0; e <= enemies.Length - 1; e++)
                    enemies[e].DetectTileCollision(tile);

                for (int i = 0; i <= items.Count - 1; i++)
                    items[i].DetectTileCollision(tile);

                if (bossPowerUp != null)
                    bossPowerUp.DetectTileCollision(tile);

                if (boss.IsAlive())
                    boss.DetectTileCollision(tile);
            }

            for (int i = 0; i <= items.Count - 1; i++)
                items[i].Update(player, deltaTime, viewport);

            if (bossPowerUp != null)
                bossPowerUp.Update(player, deltaTime, viewport);
        }

        public void MoveStuff(Rioman player, int x, int y)
        {
            foreach (Tile tile in allTiles)
                tile.Move(x, y);

            for (int i = 0; i <= scrollers.Length - 1; i++)
                scrollers[i].Move(x, y);

            for (int i = 0; i <= items.Count - 1; i++)
                items[i].Move(x, y);

            if (bossPowerUp != null)
                bossPowerUp.Move(x, y);

            for (int i = 0; i <= enemies.Length - 1; i++)
                enemies[i].Move(x, y);

            boss.Move(x, y);

            player.MoveBullets(x, y);
        }
    


        public void UpdateEnemies(Rioman player, AbstractBullet[] bullets, double deltaTime, Viewport viewport)
        {
            AbstractPickup pickUp;

            for (int i = 0; i <= enemies.Length - 1; i++)
            {
                enemies[i].Update(player, bullets, deltaTime, viewport);
                pickUp = enemies[i].GetDroppedPickup();
                if (pickUp != null)
                    items.Add(pickUp);
            }

            boss.Update(player, player.GetBullets(), deltaTime, viewport);

            pickUp = boss.GetPowerUp();
            if (pickUp != null)
                bossPowerUp = (PowerUp)pickUp;

        }

        private void SetBossAlive(Rioman player)
        {
            allowScrolling = false;
            boss.SetAlive(player);
        }

        public int GetPowerUpState()
        {
            if (bossPowerUp == null)
                return 0;
            else
                return bossPowerUp.GetState();
        }

        // ----------------------------------------------------------------------------------------------------------------
        // Tile Logic
        // ----------------------------------------------------------------------------------------------------------------

        private void OrganizeTiles()
        {
            tiles = new List<List<Tile>>();
            allTiles = new List<Tile>();

            for (int i = 0; i <= 6; i++)
                tiles.Add(new List<Tile>());

            for (int x = 0; x <= width; x++)
                for (int y = 0; y <= height; y++)
                {
                    if (tileGrid[x, y] != null)
                    {
                        tiles[tileGrid[x, y].type].Add(tileGrid[x, y]);
                        allTiles.Add(tileGrid[x, y]);
                    }
                }
        }

        private List<Tile> tileType(int type)
        {
            return tiles[type];
        }

        public void LadderForm()
        {
            foreach (Tile tile in tileType(Constant.TILE_CLIMB))
            {
                int x = tile.GridX;
                int y = tile.GridY;

                if(y <= 0 || tileGrid[x, y - 1] == null || tileGrid[x, y - 1].type != Constant.TILE_CLIMB)
                    tile.isTop = true;
            }
        }

        public void TileFader()
        {
            int num = 0;
            foreach (Tile tile in tileType(Constant.TILE_DISAPPEAR))
            {
                tile.fadeTime = num;
                num++;
            }
        }
        // ----------------------------------------------------------------------------------------------------------------
        // Player Logic
        // ----------------------------------------------------------------------------------------------------------------

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

            player.OffLadder();
            player.AllowClimingUp();

            //limit player motion against solid tiles
            foreach (Tile tile in tileType(Constant.TILE_SOLID)){

                if (player.Right.Intersects(tile.Left))
                {
                    player.StopRightMovement();
                    player.invincibledirection = 0;
                }

                if (player.Left.Intersects(tile.Right))
                {
                    player.StopLeftMovement();
                    player.invincibledirection = 0;
                }

                if (player.Head.Intersects(tile.Bottom))
                {
                    if (!player.IsClimbing())
                        player.state.Fall();
                }

                if (player.Head.Intersects(tile.Center))
                    player.StopClimingUp();

            }

            //kill player
            foreach (Tile tile in tileType(Constant.TILE_KILL)) {

                if (player.Hitbox.Intersects(tile.Center) && !player.IsInvincible())
                {
                    lifechange = -1;
                    player.Die();
                    go = false;
                }
            }

            //open door
            foreach (Tile tile in tileType(Constant.TILE_DOOR))
            {
                if (player.Hitbox.Intersects(tile.location))
                    StartOpenDoor(tile);
            }

            foreach (Tile tile in tileType(Constant.TILE_CLIMB))
            {
                if (tile.isTop && player.IsClimbing() && player.Feet.Intersects(tile.Top))
                    climbTop = true;
                if (player.Feet.Intersects(tile.location))
                    player.OnLadder();
            }

            foreach (Tile tile in allTiles) {

                if (tile.type == 1 || (player.IsInvincible() && tile.type == 2) || 
                    tile.type == 4 || tile.type == 5 || tile.type == 3 && tile.isTop)
                {

                    if (!player.IsJumping() && player.Feet.Intersects(tile.Floor) &&
                        !player.Feet.Intersects(tile.IgnoreFloor))
                    {
                        onGround = true;
                        player.MoveToY(tile.Y - player.GetSprite().Height + 8);

                        if (player.IsFalling() || player.IsClimbing())
                            player.state.Stand();
                        
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


        public void CheckDeath(Viewport viewportrect, Rioman player)
        {
            if (player.Location.Y > viewportrect.Height + player.Location.Height + 10 || StatusBar.GetHealth() <= 0)
            {
                go = false;
                lifechange = -1;
                player.Die();

            }
        }

        public bool CheckClimb(Rioman player, int climbloc, bool up, ref Rectangle location)
        {
            bool result = false;
            Tile validTile = null;

            foreach (Tile tile in tileType(Constant.TILE_CLIMB))
            {

                if (tile != null && tile.type == 3 && (player.Hitbox.Intersects(tile.location) ||
                    !up && player.Location.Intersects(tile.location)))
                {
                    if (!up && tileGrid[tile.GridX, tile.GridY + 1] != null &&
                        tileGrid[tile.GridX, tile.GridY + 1].type == 1)
                        return false;

                    if (Math.Abs(player.Hitbox.Center.X - tile.location.Center.X) <= 20)
                    {
                        validTile = tile;
                        result = true;
                    }
                }
            }

            if (result)
            {
                if (!stopLeftScreenMovement && !stopRightScreenMovement)
                    MoveStuff(player, climbloc - validTile.location.Center.X, 0);
                else
                    location.X = validTile.location.Center.X;
            }

            return result;
        }



        // ----------------------------------------------------------------------------------------------------------------
        // Draw Logic
        // ----------------------------------------------------------------------------------------------------------------

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawTiles(spriteBatch);
            boss.Draw(spriteBatch);
            DrawEnemies(spriteBatch);
            DrawItems(spriteBatch);

            //For debugging
            for (int i = 0; i <= scrollers.Length - 1; i++)
                scrollers[i].Draw(spriteBatch);
        }

        private void DrawItems(SpriteBatch spriteBatch)
        {
            foreach (AbstractPickup item in items)
                item.Draw(spriteBatch);

            if (bossPowerUp != null)
                bossPowerUp.Draw(spriteBatch);
        }

        private void DrawTiles(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in allTiles)
                tile.Draw(spriteBatch);
        }


        private void DrawEnemies(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= enemies.Length - 1; i++)
                enemies[i].Draw(spriteBatch);
        }


        // ----------------------------------------------------------------------------------------------------------------
        // Door Logic
        // ----------------------------------------------------------------------------------------------------------------

        private void StartOpenDoor(Tile tle)
        {
            doorTop = tle;
            doorOpening = true;

            int x = tle.GridX;
            int y = tle.GridY;

            //Check for door tiles above tle
            while (true)
            {
                if (y > 0 && tileGrid[x, y - 1] != null && tileGrid[x, y - 1].type == 4)
                {
                    doorTop = tileGrid[x, y - 1];
                    y--;
                }
                else
                    break;
            }

            doorStopY = doorTop.Y - Constant.TILE_SIZE;
        }

        private void OpenDoor()
        {
            bool stillOpening = false;

            int x = doorTop.GridX;
            int y = doorTop.GridY;

            while (true)
            {
                if (y < height && tileGrid[x, y] != null && tileGrid[x, y].type == 4)
                {
                    if (tileGrid[x, y].Y > doorStopY)
                    {
                        stillOpening = true;
                        tileGrid[x, y].Move(0, -DOOR_SPEED);
                    }

                    y++;
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


        public void CloseDoor(Rioman player)
        {
            bool stillClosing = false;

            int x = doorTop.GridX;
            int y = doorTop.GridY;

            while (true)
            {
                if (y < height && tileGrid[x, y] != null && tileGrid[x, y].type == 4)
                {
                    if (y > 0 && tileGrid[x, y - 1] != null &&
                        tileGrid[x, y].Y < tileGrid[x, y - 1].location.Bottom)
                    {
                        stillClosing = true;
                        tileGrid[x, y].Move(0, DOOR_SPEED);
                    }

                    y++;
                }
                else
                    break;
            }

            if (stillClosing)
                Audio.PlayDoor();
            else
            {

                SetBossAlive(player);

                closeDoor = false;
                doorClosing = false;

                //make door a wall
                y = doorTop.GridY;

                while (true)
                {
                    if (y < height && tileGrid[x, y] != null && tileGrid[x, y].type == 4)
                    {
                        tileGrid[x, y].ChangeType(Constant.TILE_SOLID);
                        tiles[Constant.TILE_SOLID].Add(tileGrid[x, y]);
                        y++;
                    }
                    else
                        break;
                }
            }
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

            foreach (Tile tile in tileType(Constant.TILE_SCROLL))
            {
                int i = tile.GridX;
                int j = tile.GridY;
                int y = 0;
                int x = 0;

                if (tileGrid[i, j].tile == Constant.VERT_SCROLL)
                {
                    try
                    {
                        while (true)
                        {
                            y++;

                            if (tileGrid[i, j + y] != null && tileGrid[i, j + y].tile == Constant.VERT_SCROLL2)
                            {
                                s.Add(new Scroller(true, i * Constant.TILE_SIZE, j * Constant.TILE_SIZE,
                                    i * Constant.TILE_SIZE, (j + y) * Constant.TILE_SIZE));

                                counter++;

                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine("Failed to construct vertical scroller on level " + levelID.ToString());
                    }
                }

                else if (tileGrid[i, j].tile == Constant.HORIZ_SCROLL)
                {
                    try
                    {
                        while (true)
                        {
                            x++;

                            if (tileGrid[i + x, j] != null && tileGrid[i + x, j].tile == Constant.HORIZ_SCROLL2)
                            {
                                s.Add(new Scroller(false, i * Constant.TILE_SIZE, j * Constant.TILE_SIZE,
                                    (i + x) * Constant.TILE_SIZE, j * Constant.TILE_SIZE));

                                counter++;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine("Failed to construct horizontal scroller on level " + levelID.ToString());
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
            if (!allowScrolling)
            {
                stopLeftScreenMovement = true;
                stopRightScreenMovement = true;
                return;
            }

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
