using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Project_Rioman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewportrect;
        KeyboardState previouskeyboardstate;
        int gamestatus;
        int previousgamestatus;

        Color backgroundcolor;
        Rioman rioman;
        Bullet[] bullet = new Bullet[3];
        Levels Level = new Levels();
        Weapons Weapons;

        bool pause;
        int lives;


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
            previousgamestatus = -1;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewportrect = graphics.GraphicsDevice.Viewport;

            gamestatus = 18;
            backgroundcolor = Color.Black;

            Audio.LoadAudio(Content);
            Health.LoadHealth(Content);
            Save.LoadContent(Content);

            Level.LoadLevelResources(Content);

            Weapons = new Weapons(Content);
            pause = false;
            lives = 3;

            Opening.LoadOpening(Content);
            StageSelection.LoadStageSelection(Content, viewportrect);

            rioman = new Rioman(Content, new Rectangle(70, 400, 48, 48), 4);

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

            if (keyboardstate.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyboardstate.IsKeyDown(Keys.F11) && !previouskeyboardstate.IsKeyDown(Keys.F11))
                this.graphics.ToggleFullScreen();

            if (lives < 1 && !rioman.iswarping)
            {
                gamestatus = 2;
                lives = 3;
            }

            if (gamestatus == 0)
            {
                Opening.FadeIn();

                if (keyboardstate.IsKeyDown(Keys.Up) && !previouskeyboardstate.IsKeyDown(Keys.Up))
                    Opening.ChangeText(true);
                else if (keyboardstate.IsKeyDown(Keys.Down) && !previouskeyboardstate.IsKeyDown(Keys.Down))
                    Opening.ChangeText(false);

                if (keyboardstate.IsKeyDown(Keys.Enter))
                    gamestatus = Opening.ChangeGameStatus();
            }
            else if (gamestatus == 1)
            {
                backgroundcolor = new Color(164, 11, 0);

                if (keyboardstate.IsKeyDown(Keys.Up) && !previouskeyboardstate.IsKeyDown(Keys.Up))
                    StageSelection.MoveSelector(0, -1);
                else if (keyboardstate.IsKeyDown(Keys.Down) && !previouskeyboardstate.IsKeyDown(Keys.Down))
                    StageSelection.MoveSelector(0, 1);

                if (keyboardstate.IsKeyDown(Keys.Right) && !previouskeyboardstate.IsKeyDown(Keys.Right))
                    StageSelection.MoveSelector(1, 0);
                else if (keyboardstate.IsKeyDown(Keys.Left) && !previouskeyboardstate.IsKeyDown(Keys.Left))
                    StageSelection.MoveSelector(-1, 0);

                if (keyboardstate.IsKeyDown(Keys.Enter) && !previouskeyboardstate.IsKeyDown(Keys.Enter))
                    gamestatus = StageSelection.SelectLevel();
            }
            else if (gamestatus == 2)
            {
                backgroundcolor = Color.Black;
                Save.Change(keyboardstate, previouskeyboardstate);

                if (keyboardstate.IsKeyDown(Keys.Enter) && !previouskeyboardstate.IsKeyDown(Keys.Enter) && !Save.save)
                    gamestatus = 1;
            }
            else if (gamestatus > 100)
            {
                gamestatus = StageSelection.DoDance(gameTime, gamestatus, viewportrect);
            }
            else if (gamestatus >= 10 && gamestatus < 100)
            {
                if (!Level.go && !rioman.iswarping)
                    ChangeLevel();

                else if (Level.go && !Level.dooropening && !Level.closedoornow && !Level.scrolling && !pause && !rioman.iswarping
                    && !Health.healthincrease && Level.bosses[Level.activelevel].intro >= 3)
                {
                    Level.UpdateTiles(rioman, gameTime, viewportrect);
                    Level.EnemyCollision(bullet, rioman);

                    rioman.BackwardScroll(Level, viewportrect);

                    rioman.Moving(keyboardstate, previouskeyboardstate, Level, gameTime.ElapsedGameTime.TotalSeconds);
                    rioman.Update(gameTime.ElapsedGameTime.TotalSeconds);

                    Level.UpdateEnemies(rioman, gameTime, viewportrect);
                    bool selectionscreen = Level.bosses[Level.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportrect, rioman);

                    if (selectionscreen)
                    {
                        Level.go = false;
                        gamestatus = 1;
                    }

                    if (keyboardstate.IsKeyDown(Keys.Z) && !previouskeyboardstate.IsKeyDown(Keys.Z))
                        rioman.Shooting(bullet);

                    if (!rioman.ispaused)
                    {
                        foreach (Bullet blt in bullet)
                            blt.BulletUpdate(viewportrect.Width);
                    }

                    Level.CheckDeath(viewportrect, rioman);

                    if (Level.lifechange != 0)
                    {
                        lives += Level.lifechange;
                        Level.lifechange = 0;
                    }

                    if (!Level.stoprightscreenmovement && !Level.stopleftscreenmovement)
                        Level.CenterRioman(viewportrect, rioman);
                }
                else if (Health.healthincrease)
                    Health.IncreaseHealth(gameTime.ElapsedGameTime.TotalSeconds);
                else if (Level.dooropening)
                    Level.OpenDoor();
                else if (Level.scrolling)
                    Level.Scroll(rioman, viewportrect);
                else if (Level.closedoornow)
                    Level.CloseDoor(rioman, viewportrect);
                else if (pause)
                    Weapons.ChangeActiveWeapon(keyboardstate, previouskeyboardstate);
                else if (rioman.iswarping)
                    rioman.Warp();
                else if (Level.bosses[Level.activelevel].intro < 3)
                    Level.bosses[Level.activelevel].Update(gameTime.ElapsedGameTime.TotalSeconds, viewportrect, rioman);

                if (Level.killbullets)
                {
                    for (int i = 0; i <= 2; i++)
                        bullet[i].alive = false;

                    Level.killbullets = false;
                }

                if (!Level.dooropening && !Level.doorclosing && !Level.closedoornow)
                {
                    if (keyboardstate.IsKeyDown(Keys.Enter) && !previouskeyboardstate.IsKeyDown(Keys.Enter))
                    {
                        pause = !pause;
                        if (pause)
                            Audio.jump3.Play(0.5f, 0.5f, 0f);
                        else
                            Audio.jump3.Play(0.5f, -0.5f, 0f);
                    }
                }
            }

            // if (previousgamestatus != gamestatus)
            //    Audio.ChangeActiveSong(gamestatus);

            previouskeyboardstate = keyboardstate;
            previousgamestatus = gamestatus;

            base.Update(gameTime);
        }

        private void ChangeLevel()
        {
            Level.NewLevel(gamestatus, Content);

            if (Level.go)
            {
                backgroundcolor = Level.backgroundcolour;
                Level.CenterRioman(viewportrect);
                rioman.location.X = Convert.ToInt32(Level.startpos.X);
                rioman.location.Y = -100;
                Level.CreateWall();
                Level.Scroller();
                Level.TileFader();
                Level.LadderForm();
                Level.TileFrames(Content);

                foreach (Bullet blt in bullet)
                    blt.alive = false;

                Health.health = 27;
                Health.drawbosshealth = false;
                rioman.iswarping = true;
                rioman.warpy = Convert.ToInt32(Level.startpos.Y);
                Audio.warp.Play(0.5f, 0f, 0f);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundcolor);

            spriteBatch.Begin();

            if (gamestatus == 0)
            {
                spriteBatch.Draw(Opening.title, new Vector2((viewportrect.Width - Opening.title.Width) / 2, viewportrect.Height / 6), Opening.fade);
                spriteBatch.Draw(Opening.activetext, new Vector2((viewportrect.Width - Opening.activetext.Width) / 2, viewportrect.Height * 2 / 3), Opening.fade);
            }
            else if (gamestatus == 1)
            {
                spriteBatch.Draw(StageSelection.background, new Vector2((viewportrect.Width - StageSelection.background.Width) / 2,
                    (viewportrect.Height - StageSelection.background.Height) / 2), Color.White);

                StageSelection.DrawIcons(spriteBatch);
            }
            else if (gamestatus == 2)
            {
                Save.DrawSaveScreen(spriteBatch, viewportrect);
            }
            else if (gamestatus > 100)
            {
                spriteBatch.Draw(StageSelection.stars, StageSelection.starsloc[0], Color.White);
                spriteBatch.Draw(StageSelection.stars, StageSelection.starsloc[1], Color.White);
                spriteBatch.Draw(StageSelection.activemiddle, StageSelection.activemiddleloc, Color.White);
                spriteBatch.Draw(StageSelection.activeintro, StageSelection.activeintroloc, Color.White);
            }
            else if (gamestatus >= 10 && gamestatus < 100)
            {
                if (Level.go || rioman.iswarping)
                {
                    Level.DrawTiles(spriteBatch, rioman.iswarping);
                    Level.bosses[Level.activelevel].Draw(spriteBatch);
                    Level.DrawEnemies(spriteBatch);

                    foreach (Bullet blt in bullet)
                    {
                        if (blt.alive)
                            spriteBatch.Draw(blt.sprite, blt.location, Color.White);
                    }

                    spriteBatch.Draw(rioman.sprite, rioman.location, rioman.sourcerect, Color.White, 0f, new Vector2(rioman.sourcerect.Width / 2,
                        0f), rioman.direction, 0f);

                    rioman.DrawHit(spriteBatch);

                    for (int i = 0; i <= 9; i++)
                        Level.pickups[i].Draw(spriteBatch);

                    Health.DrawHealth(spriteBatch);
                }

                if (pause)
                    Weapons.DrawPause(spriteBatch, lives);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}