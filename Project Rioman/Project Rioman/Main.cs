using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Command line code
//if exist "$(TargetPath).locked" del "$(TargetPath).locked"
//if not exist "$(TargetPath).locked" move "$(TargetPath)" "$(TargetPath).locked"


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
        GameLogic gameLogic = new GameLogic();

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

            EnemyAttributes.LoadContent(Content);
            PickupAttributes.LoadContent(Content);
            BulletAttributes.LoadContent(Content);

            DebugDraw.LoadContent(Content);
            Audio.LoadAudio(Content);
            Save.LoadContent(Content);
            Opening.LoadContent(Content);
            StageSelection.LoadContent(Content, viewportRect);
            Weapons.LoadContent(Content);
            gameLogic.LoadContent(Content, viewportRect);

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

            backgroundcolor = Color.Black;

            if (GameState.GetState() == Constant.TITLE_SCREEN)
                Opening.Update();
            else if (GameState.GetState() == Constant.SELECTION_SCREEN)
            {
                StageSelection.Update();
                backgroundcolor = new Color(164, 11, 0);

            }
            else if (GameState.GetState() == 2)
            {
                //TODO: save logic
                Save.Change(keyboardstate, previousKeyboardState);

                if (keyboardstate.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter) && !Save.save)
                    GameState.SetState(Constant.SELECTION_SCREEN);
            }
            else if (GameState.GetState() > 100)
                StageSelection.DoDance(gameTime, viewportRect);
            else if (GameState.GetState() >= 10 && GameState.GetState() < 100)
                gameLogic.Update(gameTime);

            previousKeyboardState = keyboardstate;

            base.Update(gameTime);
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
            else if (GameState.GetState() == Constant.SELECTION_SCREEN)
                StageSelection.Draw(spriteBatch, viewportRect);
            else if (GameState.GetState() == 2)
                Save.DrawSaveScreen(spriteBatch, viewportRect);
            else if (GameState.GetState() > 100)
                StageSelection.DrawDance(spriteBatch, viewportRect);
            else if (GameState.GetState() >= 10 && GameState.GetState() < 100)
                gameLogic.Draw(GraphicsDevice, spriteBatch, viewportRect);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}