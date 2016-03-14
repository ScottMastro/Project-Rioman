using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Project_Rioman
{
    class GameLogic
    {

        private Rioman player;
        Bullet[] bullet = new Bullet[3];
        Weapons weapons;

        private Level currentLevel;
        private Level[] levels = new Level[9];

        Viewport viewportRect;     

        KeyboardState prevKeyboardState = new KeyboardState();

        public void LoadContent(ContentManager content, Viewport viewport)
        {
            player = new Rioman(content);
            Health.LoadHealth(content);

            for (int i = 0; i <= 2; i++)
                bullet[i] = new Bullet(content.Load<Texture2D>("Video\\bullet"));

            weapons = new Weapons(content);

            LevelLoader levelLoader = new LevelLoader();

            levelLoader.LoadLevelContent(content);

            for (int i = 1; i <= 9; i++)
                levels[i - 1] = levelLoader.Load(i, content);
            currentLevel = levels[3];

            viewportRect = viewport;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (!currentLevel.go && !player.iswarping)
                ChangeLevel();

            if (currentLevel.go && !currentLevel.dooropening && !currentLevel.closedoornow && !currentLevel.isScrolling && !GameState.IsPaused() && !player.iswarping
                   && !Health.HealthIncreasing() && currentLevel.bosses[currentLevel.activelevel].intro >= 3)
            {
                currentLevel.Update(player, gameTime, viewportRect);
                currentLevel.EnemyCollision(bullet, player);

                player.BackwardScroll(currentLevel, viewportRect);

                player.Moving(keyboardState, prevKeyboardState, currentLevel, gameTime.ElapsedGameTime.TotalSeconds);
                player.Update(gameTime.ElapsedGameTime.TotalSeconds);

                currentLevel.UpdateEnemies(player, gameTime, viewportRect);
                bool selectionscreen = currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportRect, player);

                if (selectionscreen)
                {
                    currentLevel.go = false;
                    GameState.SetState(Constant.SELECTION_SCREEN);
                }

                if (keyboardState.IsKeyDown(Constant.SHOOT) && !prevKeyboardState.IsKeyDown(Constant.SHOOT))
                    player.Shooting(bullet);

                if (!player.ispaused)
                {
                    foreach (Bullet blt in bullet)
                        blt.BulletUpdate(viewportRect.Width);
                }

                currentLevel.CheckDeath(viewportRect, player);

                if (currentLevel.lifechange != 0)
                {
                    player.state.AdjustLivesByX(currentLevel.lifechange);
                    currentLevel.lifechange = 0;
                }

                if (!currentLevel.stoprightscreenmovement && !currentLevel.stopleftscreenmovement)
                    currentLevel.CenterRioman(viewportRect, player);
            }
            else if (Health.HealthIncreasing())
                Health.UpdateHealth(gameTime.ElapsedGameTime.TotalSeconds);
            else if (currentLevel.dooropening)
                currentLevel.OpenDoor();
            else if (currentLevel.isScrolling)
                currentLevel.Scroll(player, viewportRect);
            else if (currentLevel.closedoornow)
                currentLevel.CloseDoor(player, viewportRect);
  //          else if (GameState.IsPaused())
      //          Weapons.ChangeActiveWeapon(keyboardState, prevKeyboardState);     //TODO: weapons
            else if (player.iswarping)
                player.Warp();
            else if (currentLevel.bosses[currentLevel.activelevel].intro < 3)
                currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportRect, player);

            if (currentLevel.killbullets)
            {
                for (int i = 0; i <= 2; i++)
                    bullet[i].alive = false;

                currentLevel.killbullets = false;
            }

            if (!currentLevel.dooropening && !currentLevel.doorclosing && !currentLevel.closedoornow)
            {
                if (keyboardState.IsKeyDown(Constant.PAUSE) && !prevKeyboardState.IsKeyDown(Constant.PAUSE))
                {
                    GameState.SwitchPause();
                    if (GameState.IsPaused())
                        Audio.jump3.Play(0.5f, 0.5f, 0f);
                    else
                        Audio.jump3.Play(0.5f, -0.5f, 0f);
                }
            }

            prevKeyboardState = keyboardState;

        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewportRect)
        {

            if (currentLevel != null)
            {
                if (currentLevel.go || player.iswarping)
                {
                    currentLevel.DrawTiles(spriteBatch, player.iswarping);
                    currentLevel.bosses[currentLevel.activelevel].Draw(spriteBatch);
                    currentLevel.DrawEnemies(spriteBatch);

                    foreach (Bullet blt in bullet)
                    {
                        if (blt.alive)
                            spriteBatch.Draw(blt.sprite, blt.location, Color.White);
                    }

                    spriteBatch.Draw(player.sprite,
                        new Rectangle(player.location.X, player.location.Y,
                        player.drawRect.Width, player.drawRect.Height),
                        player.drawRect, Color.White, 0f, new Vector2(player.drawRect.Width / 2,
                        0f), player.direction, 0f);

                    player.DrawHit(spriteBatch);

                    for (int i = 0; i <= 9; i++)
                        currentLevel.pickups[i].Draw(spriteBatch);

                    Health.DrawHealth(spriteBatch);
                }

                if (GameState.IsPaused())
                    weapons.DrawPause(spriteBatch, player.state.GetLives());
            }
            
    }

        private void ChangeLevel()
        {
            currentLevel = levels[GameState.GetLevel()];
            currentLevel.go = true;

            currentLevel.CenterRioman(viewportRect);
            player.location.X = Convert.ToInt32(currentLevel.startpos.X);
            player.location.Y = -100;
            currentLevel.CreateWall();
            currentLevel.TileFader();
            currentLevel.LadderForm();

            foreach (Bullet blt in bullet)
                blt.alive = false;

            Health.SetHealth(27);
            Health.SetDrawBossHealth(false);
            player.iswarping = true;
            player.warpy = Convert.ToInt32(currentLevel.startpos.Y);
            Audio.warp.Play(0.5f, 0f, 0f);

        }
    }
}
