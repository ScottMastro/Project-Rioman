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
        private bool hideRioman;

        private Level currentLevel;
        private Level[] levels = new Level[9];

        Viewport viewport;     
        KeyboardState prevKeyboardState = new KeyboardState();

        public void LoadContent(ContentManager content, Viewport viewport)
        {
            player = new Rioman(content);
            StatusBar.LoadHealth(content);

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

            if (currentLevel.go && !currentLevel.IsBusy() && !GameState.IsPaused() && !StatusBar.IsBusy())
            {

                int state = currentLevel.GetPowerUpState();
                if (state == 1)
                    hideRioman = true;
                else if (state == 2)
                    GameState.SetState(Constant.SELECTION_SCREEN);

                //update player before level
                if(!hideRioman)
                    player.Update(gameTime.ElapsedGameTime.TotalSeconds, currentLevel, viewport, currentLevel.GetEnemies());

                if (!player.IsWarping())
                {

                    currentLevel.UpdateEnemies(player, player.GetBullets(), gameTime.ElapsedGameTime.TotalSeconds, viewport);

                    currentLevel.Update(player, gameTime, viewport);

                }

                currentLevel.CheckDeath(viewport, player);

                if (currentLevel.lifechange != 0)
                {
                    player.state.AdjustLivesByX(currentLevel.lifechange);
                    currentLevel.lifechange = 0;
                }

                currentLevel.CenterRioman(viewport, player);
            }
            else if (GameState.IsPaused())
                Weapons.ChangeActiveWeapon(player);
            else {

                if (StatusBar.IsBusy())
                    StatusBar.BusyUpdate(gameTime.ElapsedGameTime.TotalSeconds);
                if (currentLevel.IsBusy())
                    currentLevel.BusyUpdate(player, gameTime.ElapsedGameTime.TotalSeconds, viewport);
            }

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

        public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, Viewport viewport)
        {
            graphics.Clear(currentLevel.backgroundcolour);


            if (currentLevel != null)
            {
                if (currentLevel.go)
                {
                    currentLevel.Draw(spriteBatch);

                    if(!hideRioman)
                        player.Draw(spriteBatch, currentLevel.IsBusy());

                    StatusBar.Draw(spriteBatch, viewport);
                }

                if (GameState.IsPaused())
                    Weapons.DrawPause(spriteBatch, player.state.GetLives());
            }
            
    }

        private void ChangeLevel()
        {
            currentLevel = levels[GameState.GetLevel() - 1];
            currentLevel.StartLevel(viewport, player);

            currentLevel.TileFader();
            currentLevel.LadderForm();

            player.Reset();
            hideRioman = false;

            StatusBar.SetHealth(Constant.MAX_HEALTH);
            StatusBar.StopDrawBossHealth();
            player.StartWarp();

        }
    }
}
