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
        Weapons weapons;

        private Level currentLevel;
        private Level[] levels = new Level[9];

        Viewport viewport;     

        KeyboardState prevKeyboardState = new KeyboardState();

        public void LoadContent(ContentManager content, Viewport viewport)
        {
            player = new Rioman(content);
            Health.LoadHealth(content);

            weapons = new Weapons(content);

            LevelLoader levelLoader = new LevelLoader();

            levelLoader.LoadLevelContent(content);

            for (int i = 0; i <= 8; i++)
                levels[i] = levelLoader.Load(i + 1, content);

            this.viewport = viewport;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (currentLevel == null || !currentLevel.go)
                ChangeLevel();

            if (currentLevel.go && !currentLevel.IsBusy() && !GameState.IsPaused()
                   && !Health.HealthIncreasing() && currentLevel.bosses[currentLevel.activelevel].intro >= 3)
            {

                //updater player before level
                player.Update(gameTime.ElapsedGameTime.TotalSeconds, currentLevel, viewport);

                if (!player.IsWarping())
                {

                    currentLevel.EnemyCollision(player.GetBullets(), player);

                    currentLevel.UpdateEnemies(player, player.GetBullets(), gameTime.ElapsedGameTime.TotalSeconds, viewport);

                    currentLevel.Update(player, gameTime, viewport);

                }
                    bool selectionscreen = currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewport, player);

                    if (selectionscreen)
                {
                    currentLevel.go = false;
                    GameState.SetState(Constant.SELECTION_SCREEN);
                }

                currentLevel.CheckDeath(viewport, player);

                if (currentLevel.lifechange != 0)
                {
                    player.state.AdjustLivesByX(currentLevel.lifechange);
                    currentLevel.lifechange = 0;
                }

                currentLevel.CenterRioman(viewport, player);
            }
            else if (Health.HealthIncreasing())
                Health.UpdateHealth(gameTime.ElapsedGameTime.TotalSeconds);
            else if (currentLevel.IsBusy())
                currentLevel.BusyUpdate(player, viewport);
  //          else if (GameState.IsPaused())
      //          Weapons.ChangeActiveWeapon(keyboardState, prevKeyboardState);     //TODO: weapons
            else if (currentLevel.bosses[currentLevel.activelevel].intro < 3)
                currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewport, player);

            if (currentLevel.killBullets)
            {
                player.KillBullets();

                currentLevel.killBullets = false;
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

        public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, Viewport viewportRect)
        {
            graphics.Clear(currentLevel.backgroundcolour);


            if (currentLevel != null)
            {
                if (currentLevel.go)
                {
                    currentLevel.DrawTiles(spriteBatch);
                    currentLevel.bosses[currentLevel.activelevel].Draw(spriteBatch);
                    currentLevel.DrawEnemies(spriteBatch);
                    currentLevel.DrawItems(spriteBatch);

                    player.Draw(spriteBatch, currentLevel.IsBusy());


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
            currentLevel.Play(viewport, player);

            currentLevel.TileFader();
            currentLevel.LadderForm();

            player.Reset();

            Health.SetHealth(27);
            Health.SetDrawBossHealth(false);
            player.StartWarp();

        }
    }
}
