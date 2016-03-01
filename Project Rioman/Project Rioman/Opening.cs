using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    public static class Opening
    {
        private static Texture2D title;
        private static Texture2D newGame;
        private static Texture2D loadGame;
        private enum State { newGame, loadGame };
        private static State state;

        private static Color fade;
        private static KeyboardState prevKeyboardState;

        static public void LoadContent(ContentManager content)
        {
            title = content.Load<Texture2D>("Video\\opening\\RioManlogo");
            newGame = content.Load<Texture2D>("Video\\opening\\text1");
            loadGame = content.Load<Texture2D>("Video\\opening\\text2");

            state = State.newGame;

            fade = new Color(255, 255, 255, 0);
            prevKeyboardState = new KeyboardState();
        }

        static public void Update()
        {
            FadeIn();
            HandleInput();

        }

        static public void Draw(SpriteBatch spriteBatch, Viewport viewportRect)
        {
            spriteBatch.Draw(title, new Vector2((viewportRect.Width - Opening.title.Width) / 2, viewportRect.Height / 6), fade);

            Texture2D sprite = (state == State.newGame ? newGame : loadGame);

            spriteBatch.Draw(sprite, new Vector2((viewportRect.Width - sprite.Width) / 2, viewportRect.Height * 2 / 3), fade);
        }

        static public void FadeIn()
        {
            if (fade.A <= 253)
                fade.A += 2;
        }

        static public void HandleInput() { 

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Constant.UP) && !prevKeyboardState.IsKeyDown(Constant.UP) ||
                keyboardState.IsKeyDown(Constant.DOWN) && !prevKeyboardState.IsKeyDown(Constant.DOWN))
            {
                Audio.selection.Play(0.5f, 0f, 0f);
                if (state == State.newGame)
                    state = State.loadGame;
                else
                    state = State.newGame;
            }

            if (keyboardState.IsKeyDown(Constant.CONFIRM))
                GameState.SetState(ChangeGameStatus());


            prevKeyboardState = keyboardState;
        }

        static public int ChangeGameStatus()
        {

            if (state == State.newGame)
                return Constant.SELECTION_SCREEN;
            else
                return Constant.LOAD_SCREEN;

        }


    }
}