using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Project_Rioman
{
    abstract class AbstractPickup
    {
        protected int type;
        protected bool isAlive;
        protected bool onGround;
        protected Texture2D sprite;
        protected Texture2D debugSquare;

        protected Rectangle drawRect;
        protected Rectangle location;
        protected Rectangle originalLocation;

        protected bool canDie;

        public AbstractPickup(int type, int x, int y)
        {
            this.type = type;
            debugSquare = PickupAttributes.GetSprite(-1);
            isAlive = true;
            onGround = false;
            location.X = x;
            location.Y = y - 10;
        }

        public void Update(Rioman player, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                if (player.Hitbox.Intersects(GetCollisionRect()))
                    PickedUp();

                if (!onGround)
                    location.Y += 2;
            }

            SubUpdate(deltaTime);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
                    drawRect, Color.White, 0f, new Vector2(drawRect.Width / 2, drawRect.Height / 2), SpriteEffects.None, 0);
        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void DetectTileCollision(Tile tile)
        {
            if (!onGround)
            {
                if (tile.type == 1 || tile.type == 3 && tile.isTop)
                    if (tile.Top.Intersects(GetCollisionRect()))
                        onGround = true;
                    
            }

        }

        private void Kill(bool check, Viewport viewportrect)
        {
            if (canDie)
                if (location.X < -50 || location.X > viewportrect.Width + 50 ||
                    location.Y < -50 || location.Y > viewportrect.Height + 50)
                    isAlive = false;
        }

        public Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X - drawRect.Width / 2, location.Y - drawRect.Height / 2, drawRect.Width, drawRect.Height);
        }


        protected abstract void SubUpdate(double deltaTime);
        protected abstract void PickedUp();

    }
}
 