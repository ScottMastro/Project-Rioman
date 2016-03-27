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
        Bullet[] bullets = new Bullet[3];
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
                bullets[i] = new Bullet(content.Load<Texture2D>("Video\\bullet"));

            weapons = new Weapons(content);

            LevelLoader levelLoader = new LevelLoader();

            levelLoader.LoadLevelContent(content);

            for (int i = 0; i <= 8; i++)
                levels[i] = levelLoader.Load(i + 1, content);

            viewportRect = viewport;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (currentLevel == null || !currentLevel.go)
                ChangeLevel();

            if (currentLevel.go && !currentLevel.IsBusy() && !currentLevel.isScrolling && !GameState.IsPaused()
                   && !Health.HealthIncreasing() && currentLevel.bosses[currentLevel.activelevel].intro >= 3)
            {

                if(!player.IsWarping())
                    currentLevel.Update(player, gameTime, viewportRect);

                currentLevel.EnemyCollision(bullets, player);

                player.BackwardScroll(currentLevel, viewportRect);

                player.Update(gameTime.ElapsedGameTime.TotalSeconds, currentLevel);

                currentLevel.UpdateEnemies(player, bullets, gameTime.ElapsedGameTime.TotalSeconds, viewportRect);
                bool selectionscreen = currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportRect, player);

                if (selectionscreen)
                {
                    currentLevel.go = false;
                    GameState.SetState(Constant.SELECTION_SCREEN);
                }

                if (keyboardState.IsKeyDown(Constant.SHOOT) && !prevKeyboardState.IsKeyDown(Constant.SHOOT))
                    player.Shooting(bullets);

                if (!player.ispaused)
                {
                    foreach (Bullet blt in bullets)
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
            else if (currentLevel.IsBusy())
                currentLevel.BusyUpdate();
            else if (currentLevel.isScrolling)
                currentLevel.Scroll(player, viewportRect);
  //          else if (GameState.IsPaused())
      //          Weapons.ChangeActiveWeapon(keyboardState, prevKeyboardState);     //TODO: weapons
            else if (currentLevel.bosses[currentLevel.activelevel].intro < 3)
                currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportRect, player);

            if (currentLevel.killbullets)
            {
                for (int i = 0; i <= 2; i++)
                    bullets[i].alive = false;

                currentLevel.killbullets = false;
            }

            if (!currentLevel.IsBusy())
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
                if (currentLevel.go)
                {
                    currentLevel.DrawTiles(spriteBatch);
                    currentLevel.bosses[currentLevel.activelevel].Draw(spriteBatch);
                    currentLevel.DrawEnemies(spriteBatch);

                    foreach (Bullet blt in bullets)
                    {
                        if (blt.alive)
                            spriteBatch.Draw(blt.sprite, blt.location, Color.White);
                    }

                    player.Draw(spriteBatch);


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
            currentLevel = levels[GameState.GetLevel() - 1];
            currentLevel.Play(viewportRect, player);

            currentLevel.TileFader();
            currentLevel.LadderForm();

            foreach (Bullet blt in bullets)
                blt.alive = false;

            Health.SetHealth(27);
            Health.SetDrawBossHealth(false);
            player.StartWarp();

        }
    }
}
