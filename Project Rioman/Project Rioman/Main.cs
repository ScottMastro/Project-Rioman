using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewportRect;
        KeyboardState previousKeyboardState;

        Color backgroundcolor;
        Rioman rioman;
        Bullet[] bullet = new Bullet[3];
        Level currentLevel;
        LevelLoader levelLoader = new LevelLoader();
        Level[] level = new Level[9];

        Weapons Weapons;


        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewportRect = graphics.GraphicsDevice.Viewport;
            GameState.getInstance();
            backgroundcolor = Color.Black;

            Audio.LoadAudio(Content);
            Health.LoadHealth(Content);
            Save.LoadContent(Content);
            Opening.LoadContent(Content);

            levelLoader.LoadLevelContent(Content);
            for (int i = 1; i <= 9; i++)
                level[i-1] = levelLoader.Load(i, Content);
            currentLevel = level[3];

            Weapons = new Weapons(Content);

            StageSelection.LoadStageSelection(Content, viewportRect);

            rioman = new Rioman(Content);

            for (int i = 0; i <= 2; i++)
                bullet[i] = new Bullet(Content.Load<Texture2D>("Video\\bullet"));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardstate = Keyboard.GetState();

            if (keyboardstate.IsKeyDown(Constant.END_GAME))
                this.Exit();
            if (keyboardstate.IsKeyDown(Constant.FULL_SCREEN) && !previousKeyboardState.IsKeyDown(Constant.FULL_SCREEN))
                this.graphics.ToggleFullScreen();


            if (GameState.GetState() == Constant.TITLE_SCREEN)
                Opening.Update();
            else if (GameState.GetState() == 1)
            {
                backgroundcolor = new Color(164, 11, 0);

                if (keyboardstate.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up))
                    StageSelection.MoveSelector(0, -1);
                else if (keyboardstate.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down))
                    StageSelection.MoveSelector(0, 1);

                if (keyboardstate.IsKeyDown(Keys.Right) && !previousKeyboardState.IsKeyDown(Keys.Right))
                    StageSelection.MoveSelector(1, 0);
                else if (keyboardstate.IsKeyDown(Keys.Left) && !previousKeyboardState.IsKeyDown(Keys.Left))
                    StageSelection.MoveSelector(-1, 0);

                if (keyboardstate.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
                    GameState.SetState(StageSelection.SelectLevel());
            }
            else if (GameState.GetState() == 2)
            {
                backgroundcolor = Color.Black;
                Save.Change(keyboardstate, previousKeyboardState);

                if (keyboardstate.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter) && !Save.save)
                    GameState.SetState(1);
            }
            else if (GameState.GetState() > 100)
            {
                GameState.SetState(StageSelection.DoDance(gameTime, GameState.GetState(), viewportRect));
            }
            else if (GameState.GetState() >= 10 && GameState.GetState() < 100)
            {
                if (!currentLevel.go && !rioman.iswarping)
                    ChangeLevel();

                else if (currentLevel.go && !currentLevel.dooropening && !currentLevel.closedoornow && !currentLevel.scrolling && !GameState.IsPaused() && !rioman.iswarping
                    && !Health.healthincrease && currentLevel.bosses[currentLevel.activelevel].intro >= 3)
                {
                    currentLevel.UpdateTiles(rioman, gameTime, viewportRect);
                    currentLevel.EnemyCollision(bullet, rioman);

                    rioman.BackwardScroll(currentLevel, viewportRect);

                    rioman.Moving(keyboardstate, previousKeyboardState, currentLevel, gameTime.ElapsedGameTime.TotalSeconds);
                    rioman.Update(gameTime.ElapsedGameTime.TotalSeconds);

                    currentLevel.UpdateEnemies(rioman, gameTime, viewportRect);
                    bool selectionscreen = currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportRect, rioman);

                    if (selectionscreen)
                    {
                        currentLevel.go = false;
                        GameState.SetState(1);
                    }

                    if (keyboardstate.IsKeyDown(Keys.Z) && !previousKeyboardState.IsKeyDown(Keys.Z))
                        rioman.Shooting(bullet);

                    if (!rioman.ispaused)
                    {
                        foreach (Bullet blt in bullet)
                            blt.BulletUpdate(viewportRect.Width);
                    }

                    currentLevel.CheckDeath(viewportRect, rioman);

                    if (currentLevel.lifechange != 0)
                    {
                        rioman.state.AdjustLivesByX(currentLevel.lifechange);
                        currentLevel.lifechange = 0;
                    }

                    if (!currentLevel.stoprightscreenmovement && !currentLevel.stopleftscreenmovement)
                        currentLevel.CenterRioman(viewportRect, rioman);
                }
                else if (Health.healthincrease)
                    Health.IncreaseHealth(gameTime.ElapsedGameTime.TotalSeconds);
                else if (currentLevel.dooropening)
                    currentLevel.OpenDoor();
                else if (currentLevel.scrolling)
                    currentLevel.Scroll(rioman, viewportRect);
                else if (currentLevel.closedoornow)
                    currentLevel.CloseDoor(rioman, viewportRect);
                else if (GameState.IsPaused())
                    Weapons.ChangeActiveWeapon(keyboardstate, previousKeyboardState);
                else if (rioman.iswarping)
                    rioman.Warp();
                else if (currentLevel.bosses[currentLevel.activelevel].intro < 3)
                    currentLevel.bosses[currentLevel.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportRect, rioman);

                if (currentLevel.killbullets)
                {
                    for (int i = 0; i <= 2; i++)
                        bullet[i].alive = false;

                    currentLevel.killbullets = false;
                }

                if (!currentLevel.dooropening && !currentLevel.doorclosing && !currentLevel.closedoornow)
                {
                    if (keyboardstate.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        GameState.SwitchPause();
                        if (GameState.IsPaused())
                            Audio.jump3.Play(0.5f, 0.5f, 0f);
                        else
                            Audio.jump3.Play(0.5f, -0.5f, 0f);
                    }
                }
            }

            // if (previousgamestatus != gamestatus)
            //    Audio.ChangeActiveSong(gamestatus);

            previousKeyboardState = keyboardstate;

            base.Update(gameTime);
        }

        private void ChangeLevel()
        {
            currentLevel = level[GameState.GetState() - 9];
            currentLevel.go = true;
 
                backgroundcolor = currentLevel.backgroundcolour;
                currentLevel.CenterRioman(viewportRect);
                rioman.location.X = Convert.ToInt32(currentLevel.startpos.X);
                rioman.location.Y = -100;
                currentLevel.CreateWall();
                currentLevel.Scroller();
                currentLevel.TileFader();
                currentLevel.LadderForm();
                currentLevel.TileFrames(Content);

                foreach (Bullet blt in bullet)
                    blt.alive = false;

                Health.health = 27;
                Health.drawbosshealth = false;
                rioman.iswarping = true;
                rioman.warpy = Convert.ToInt32(currentLevel.startpos.Y);
                Audio.warp.Play(0.5f, 0f, 0f);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundcolor);

            spriteBatch.Begin();

            if (GameState.GetState() == Constant.TITLE_SCREEN)
                Opening.Draw(spriteBatch, viewportRect);
            else if (GameState.GetState() == 1)
            {
                spriteBatch.Draw(StageSelection.background, new Vector2((viewportRect.Width - StageSelection.background.Width) / 2,
                    (viewportRect.Height - StageSelection.background.Height) / 2), Color.White);

                StageSelection.DrawIcons(spriteBatch);
            }
            else if (GameState.GetState() == 2)
            {
                Save.DrawSaveScreen(spriteBatch, viewportRect);
            }
            else if (GameState.GetState() > 100)
            {
                spriteBatch.Draw(StageSelection.stars, StageSelection.starsloc[0], Color.White);
                spriteBatch.Draw(StageSelection.stars, StageSelection.starsloc[1], Color.White);
                spriteBatch.Draw(StageSelection.activemiddle, StageSelection.activemiddleloc, Color.White);
                spriteBatch.Draw(StageSelection.activeintro, StageSelection.activeintroloc, Color.White);
            }
            else if (GameState.GetState() >= 10 && GameState.GetState() < 100)
            {
                if (currentLevel.go || rioman.iswarping)
                {
                    currentLevel.DrawTiles(spriteBatch, rioman.iswarping);
                    currentLevel.bosses[currentLevel.activelevel].Draw(spriteBatch);
                    currentLevel.DrawEnemies(spriteBatch);

                    foreach (Bullet blt in bullet)
                    {
                        if (blt.alive)
                            spriteBatch.Draw(blt.sprite, blt.location, Color.White);
                    }

                    spriteBatch.Draw(rioman.sprite,
                        new Rectangle(rioman.location.X, rioman.location.Y,
                        rioman.drawRect.Width, rioman.drawRect.Height),
                        rioman.drawRect, Color.White, 0f, new Vector2(rioman.drawRect.Width / 2,
                        0f), rioman.direction, 0f);

                    rioman.DrawHit(spriteBatch);

                    for (int i = 0; i <= 9; i++)
                        currentLevel.pickups[i].Draw(spriteBatch);

                    Health.DrawHealth(spriteBatch);
                }

                if (GameState.IsPaused())
                    Weapons.DrawPause(spriteBatch, rioman.state.GetLives());
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}