using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Project_Rioman
{
    class Level
    {

        private AbstractTile[,] tileGrid;
        private List<List<AbstractTile>> tiles;
        private List<AbstractTile> allTiles;

        public int levelID;

        public Color backgroundcolour;
        public int width;
        public int height;
        public bool active;

        public bool respawn;
        private int respawnViewportY;
        private Point originalStartPos;
        private Point startPos;

        private AbstractEnemy[] enemies;
        private List<AbstractPickup> items;
        private PowerUp bossPowerUp;
        private AbstractBoss boss;

        private int doorStopY;
        private AbstractTile doorTop;
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
        private Respawn[] respawns;

        private int scrollAmountX;
        private int scrollAmountY;

        public int lifechange = 0;


        public Level(Color bg, int width, int height, Point startpos, AbstractTile[,] tiles,
            AbstractEnemy[] enemies, OldPickup[] pickups, AbstractBoss boss)
        {
            backgroundcolour = bg;
            this.width = width;
            this.height = height;
            this.startPos = startpos;
            originalStartPos = startPos;
            this.tileGrid = tiles;
            this.enemies = enemies;
           //TODO this.pickups = pickups;
            items = new List<AbstractPickup>();
            this.boss = boss;
            active = false;
            respawn = false;

        }

        public void StartLevel(Viewport viewport, Rioman player)
        {
            startPos = originalStartPos;
            respawnViewportY = viewport.Height - Constant.TILE_SIZE * 4;
            Reset();
            SetRespawnsAlive();

            Start(player, new Point(viewport.Width / 2, viewport.Height - Constant.TILE_SIZE * 4));
        }

        public void RestartLevel(Viewport viewport, Rioman player)
        {

            Reset();

            int centerY = viewport.Height - Constant.TILE_SIZE * 4;
            Start(player, new Point(viewport.Width / 2, centerY + (respawnViewportY - centerY)));
        }

        private void Start(Rioman player, Point centerPos)
        {
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
            active = true;
            respawn = false;

            ResetTiles();
            OrganizeTiles();
            TileFader();
            LadderForm();
            LaserForm();
            ResetEnemies();
            boss.Reset();

            stopLeftScreenMovement = false;
            stopRightScreenMovement = false;
            preventCenteringLeft = false;
            preventCenteringRight = false;

            items = new List<AbstractPickup>();
            bossPowerUp = null;
             
            scrollers = MakeScrollers();
            respawns = MakeRespawns();

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
            UpdateRespawns(player, viewport);
            InteractWithLevel(player);

            foreach (AbstractTile tile in allTiles)
                tile.Update(player, deltaTime);

            foreach (AbstractTile tile in tileType(Constant.TILE_SOLID)) {

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
            foreach (AbstractTile tile in allTiles)
                tile.Move(x, y);

            for (int i = 0; i <= scrollers.Length - 1; i++)
                scrollers[i].Move(x, y);

            for (int i = 0; i <= respawns.Length - 1; i++)
                respawns[i].Move(x, y);

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
            tiles = new List<List<AbstractTile>>();
            allTiles = new List<AbstractTile>();

            for (int i = 0; i <= 9; i++)
                tiles.Add(new List<AbstractTile>());

            for (int x = 0; x <= width; x++)
                for (int y = 0; y <= height; y++)
                {

                    AbstractTile t = tileGrid[x, y];

                    if (t != null)
                    {

                        if (t.Type == 8 || t.Type == 9)
                            tiles[1].Add(t);

                        tiles[t.Type].Add(t);
                        allTiles.Add(t);
                    }
                }
        }

        private List<AbstractTile> tileType(int type)
        {
            return tiles[type];
        }

        public void LadderForm()
        {
            foreach (AbstractTile tile in tileType(Constant.TILE_CLIMB))
            {
                int x = tile.GridX;
                int y = tile.GridY;

                if(y <= 0 || tileGrid[x, y - 1] == null || tileGrid[x, y - 1].Type != Constant.TILE_CLIMB)
                    tile.SetTop();
            }
        }

        public void LaserForm()
        {
            foreach (LaserTile tile in tileType(Constant.TILE_LASER))
            {
                int x = tile.GridX;
                int y = tile.GridY;

                int shift = 0;

                while (true)
                {
                    if (tile.FacingUp())
                    {
                        if (y - shift < 0 || !(tileGrid[x, y - shift] == null) && tileGrid[x, y - shift].Type == Constant.TILE_SOLID)
                            break;
                    }
                    else if (tile.FacingDown())
                    {
                        if (y + shift >= height || !(tileGrid[x, y + shift] == null) && tileGrid[x, y + shift].Type == Constant.TILE_SOLID)
                            break;
                    }
                    else if (tile.FacingLeft())
                    {
                        if (x - shift < 0 || !(tileGrid[x - shift, y] == null) && tileGrid[x - shift, y].Type == Constant.TILE_SOLID)
                            break;
                    }
                    else if (tile.FacingRight())
                    {
                        if (x + shift >= width || !(tileGrid[x + shift, y] == null) && tileGrid[x + shift, y].Type == Constant.TILE_SOLID)
                            break;
                    }

                    shift++;
                }

                tile.SetRange((shift-1) * Constant.TILE_SIZE);
            }
        }

        public void TileFader()
        {
            int num = 0;
            foreach (DisappearTile tile in tileType(Constant.TILE_DISAPPEAR))
            {
                tile.SetFadeTime(num);
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
            foreach (AbstractTile tile in tileType(Constant.TILE_SOLID)){

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
            foreach (AbstractTile tile in tileType(Constant.TILE_KILL)) {

                if (player.Hitbox.Intersects(tile.Center) && !player.IsInvincible())
                {
                    lifechange = -1;
                    player.Die();
                    respawn = true;
                }
            }

            foreach (AbstractTile tile in tileType(Constant.TILE_KILL))
            {
                if (player.Hitbox.Intersects(tile.Center) && !player.IsInvincible())
                    KillPlayer(player);
            }

            foreach (LaserTile tile in tileType(Constant.TILE_LASER))
            {
                if (player.Hitbox.Intersects(tile.GetLaserRect()))
                    KillPlayer(player);
            }

            //open door
            foreach (AbstractTile tile in tileType(Constant.TILE_DOOR))
            {
                if (player.Hitbox.Intersects(tile.Location))
                    StartOpenDoor(tile);
            }

            foreach (AbstractTile tile in tileType(Constant.TILE_CLIMB))
            {
                if (tile.IsTop && player.IsClimbing() && player.Feet.Intersects(tile.Top))
                    climbTop = true;
                if (player.Feet.Intersects(tile.Location))
                    player.OnLadder();
            }

            foreach (AbstractTile tile in allTiles) {

                if (tile.Type == 1 || (player.IsInvincible() && tile.Type == 2) ||
                    tile.Type == 4 || tile.Type == 5 || tile.Type == 3 && tile.IsTop
                    || tile.Type == 8 || tile.Type == 9)
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
                KillPlayer(player);
        }

        private void KillPlayer(Rioman player)
        {
            lifechange = -1;
            player.Die();
            respawn = true;
        }

        public bool CheckClimb(Rioman player, int climbloc, bool up, ref Rectangle location)
        {
            bool result = false;
            AbstractTile validTile = null;

            foreach (AbstractTile tile in tileType(Constant.TILE_CLIMB))
            {

                if (tile != null && tile.Type == 3 && (player.Hitbox.Intersects(tile.Location) ||
                    !up && player.Location.Intersects(tile.Location)))
                {
                    if (!up && tileGrid[tile.GridX, tile.GridY + 1] != null &&
                        tileGrid[tile.GridX, tile.GridY + 1].Type == 1)
                        return false;

                    if (Math.Abs(player.Hitbox.Center.X - tile.Location.Center.X) <= 20)
                    {
                        validTile = tile;
                        result = true;
                    }
                }
            }

            if (result)
            {
                if (!stopLeftScreenMovement && !stopRightScreenMovement)
                    MoveStuff(player, climbloc - validTile.Location.Center.X, 0);
                else
                    location.X = validTile.Location.Center.X;
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

            for (int i = 0; i <= respawns.Length - 1; i++)
                respawns[i].Draw(spriteBatch);
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
            foreach (AbstractTile tile in allTiles)
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

        private void StartOpenDoor(AbstractTile tle)
        {
            doorTop = tle;
            doorOpening = true;

            int x = tle.GridX;
            int y = tle.GridY;

            //Check for door tiles above tle
            while (true)
            {
                if (y > 0 && tileGrid[x, y - 1] != null && tileGrid[x, y - 1].Type == 4)
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
                if (y < height && tileGrid[x, y] != null && tileGrid[x, y].Type == 4)
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
                if (y < height && tileGrid[x, y] != null && tileGrid[x, y].Type == 4)
                {
                    if (y > 0 && tileGrid[x, y - 1] != null &&
                        tileGrid[x, y].Y < tileGrid[x, y - 1].Location.Bottom)
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
                    if (y < height && tileGrid[x, y] != null && tileGrid[x, y].Type == 4)
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
        //  Respawn Logic
        // ----------------------------------------------------------------------------------------------------------------


        private struct Respawn
        {
            private Rectangle respawnRect;
            private Point startPos;
            private Point startViewportPos;

            private bool isAlive;

            public Respawn(int x, int y)
            {
                isAlive = true;
                startPos = new Point(x, y - Constant.TILE_SIZE);
                startViewportPos = new Point(x, y - Constant.TILE_SIZE);

                respawnRect = new Rectangle(x - Constant.TILE_SIZE * 4, y - Constant.TILE_SIZE * 5,
                    Constant.TILE_SIZE * 9, Constant.TILE_SIZE * 11);
            }

            public void Move(int x, int y)
            {
                respawnRect.X += x;
                respawnRect.Y += y;
                startViewportPos.X += x;
                startViewportPos.Y += y;
            }

            public bool InsideRespawnZone(Rectangle playerRect)
            {
                return isAlive && playerRect.Intersects(respawnRect);
            }

            public Point GetRespawnPoint()
            {
                isAlive = false;
                return startPos;
            }

            public int GetViewportY()
            {
                return startViewportPos.Y;
            }


            public void SetAlive()
            {
                isAlive = true;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                DebugDraw.DrawRect(spriteBatch, respawnRect, 0.1f);

            }
        }

        private Respawn[] MakeRespawns()
        {
            List<Respawn> r = new List<Respawn>();

            foreach (AbstractTile tile in tileType(Constant.TILE_FUNCTION))
            {
                if (tile.TileID == Constant.RESPAWN)
                    r.Add(new Respawn(tile.X, tile.Y));
            }

            return new List<Respawn>(r).GetRange(0, r.Count).ToArray();
        }

        private void UpdateRespawns(Rioman player, Viewport viewport)
        {
            for(int i = 0; i<= respawns.Length-1; i++)
            {
                if (respawns[i].InsideRespawnZone(player.Hitbox))
                {
                    startPos = respawns[i].GetRespawnPoint();
                    Console.WriteLine(startPos.Y);
                    respawnViewportY = respawns[i].GetViewportY();
                }
            }
        }

        private void SetRespawnsAlive()
        {
            for (int i = 0; i <= respawns.Length - 1; i++)
                respawns[i].SetAlive();
        }

        // ----------------------------------------------------------------------------------------------------------------
        // Scroller Logic
        // ----------------------------------------------------------------------------------------------------------------


        private struct Scroller
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

        private Scroller[] MakeScrollers()
        {
            List<Scroller> s = new List<Scroller>();
            int counter = 0;

            Rectangle scroll = new Rectangle();

            foreach (AbstractTile tile in tileType(Constant.TILE_FUNCTION))
            {
                int i = tile.GridX;
                int j = tile.GridY;
                int y = 0;
                int x = 0;

                if (tileGrid[i, j].TileID == Constant.VERT_SCROLL)
                {
                    try
                    {
                        while (true)
                        {
                            y++;

                            if (tileGrid[i, j + y] != null && tileGrid[i, j + y].TileID == Constant.VERT_SCROLL2)
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

                else if (tileGrid[i, j].TileID == Constant.HORIZ_SCROLL)
                {
                    try
                    {
                        while (true)
                        {
                            x++;

                            if (tileGrid[i + x, j] != null && tileGrid[i + x, j].TileID == Constant.HORIZ_SCROLL2)
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
            scrollAmountY = (viewport.Height - Constant.TILE_SIZE / 2) * Math.Abs(magnitudeY) + 14;
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
