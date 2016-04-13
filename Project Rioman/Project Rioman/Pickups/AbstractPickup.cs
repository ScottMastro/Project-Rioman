using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Project_Rioman
{
    class AbstractPickup
    {
        private int type;
        private bool isAlive;
        private bool onGround;
        private Texture2D sprite;
        private Rectangle drawRect;
        private Rectangle location;
        private Rectangle originalLocation;

        private bool canDie;

        public AbstractPickup(int type)
        {
            sprite = PickupAttributes.GetSprite(type);
            isAlive = true;
            onGround = false;
        }

        public void Update(double deltaTime)
        {

        }

        public void MovePickup(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                    drawRect, Color.White, 0f, new Vector2(), SpriteEffects.None, 0);
        }

        public void Kill(bool check, Viewport viewportrect)
        {
            if (canDie) { 
                if (location.Right < 50 || location.X > viewportrect.Width ||
                    location.Bottom < 50 || location.Y > viewportrect.Height)
                        isAlive = false;
        }
    }
}
 