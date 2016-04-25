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

            if (currentLevel.go && !currentLevel.IsBusy() && !GameState.IsPaused()
                   && !StatusBar.IsBusy())
            {

                //update player before level
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
            else if (StatusBar.IsBusy())
                StatusBar.BusyUpdate(gameTime.ElapsedGameTime.TotalSeconds);
            else if (currentLevel.IsBusy())
                currentLevel.BusyUpdate(player, viewport);
            else if (GameState.IsPaused())
                Weapons.ChangeActiveWeapon(player);

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
                    currentLevel.Draw(spriteBatch);
                    player.Draw(spriteBatch, currentLevel.IsBusy());

                    StatusBar.Draw(spriteBatch);
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

            StatusBar.SetHealth(27);
            StatusBar.SetDrawBossHealth(false);
            player.StartWarp();

        }
    }
}
