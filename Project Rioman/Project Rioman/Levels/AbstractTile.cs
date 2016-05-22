using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Project_Rioman
{
    //type -1 = ignore
    //type 0 = decoration
    //type 1 = walkable
    //type 2 = death
    //type 3 = climb
    //type 4 = door
    //type 5 = disappearing
    //type 6 = functional
    //type 7 = laser
    //type 8 = falling
    //type 9 = conveyer


    abstract class AbstractTile
    {
        protected Texture2D sprite;
        protected int gridX;
        protected int gridY;

        protected int tileID;
        protected int type;
        protected int originalType;

        protected bool animate;
        protected Texture2D[] frames;
        protected int numFrames;
        protected int currentFrame;
        protected int animateDir;
        protected double animationTime;

        protected Rectangle location;
        protected Rectangle originalLocation;

        protected bool isTop;

        public AbstractTile(int ID, int x, int y)
        {
            frames = TileAttributes.GetSprites(ID);

            sprite = frames[0];

            type = TileAttributes.TileIDToType(ID);
            originalType = type;

            tileID = ID;

            gridX = x;
            gridY = y;

            location = new Rectangle(x * Constant.TILE_SIZE, y * Constant.TILE_SIZE, Constant.TILE_SIZE, Constant.TILE_SIZE);
            originalLocation = location;

            isTop = false;

            animate = true;
            animateDir = 1;
            currentFrame = 0;
            numFrames = frames.Length;

            Reset();

        }

        public void Reset()
        {
            type = originalType;
            location = originalLocation;

            SubReset();
        }

        protected abstract void SubReset();

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }



        public void Update(Rioman player, double deltaTime)
        {
            SubUpdate(player, deltaTime);

            if (animate)
                Animate(deltaTime);
        }

        protected abstract void SubUpdate(Rioman player, double deltaTime);

        public void Animate(double deltaTime)
        {
            if (numFrames == 1 || type == Constant.TILE_DISAPPEAR || type == Constant.TILE_LASER)
                return;

            animationTime += deltaTime;

            if (animationTime > 0.25)
            {
                animationTime = 0;
                currentFrame += animateDir;

                if (currentFrame == 0)
                    animateDir = 1;
                else if (currentFrame == numFrames - 1)
                    animateDir = -1;

                sprite = frames[currentFrame];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (type != Constant.TILE_FUNCTION && type != Constant.TILE_IGNORE)
                spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, sprite.Width, sprite.Height), Color.White);

            SubDraw(spriteBatch);
        }

        protected abstract void SubDraw(SpriteBatch spriteBatch);

        public int X { get { return location.X; } }
        public int Y { get { return location.Y; } }
        public int GridX { get { return gridX; } }
        public int GridY { get { return gridY; } }
        public int Width { get { return location.Width; } }
        public void ChangeType(int newtype) { type = newtype; }

        public Rectangle Top { get { return new Rectangle(location.X, location.Y, location.Width, location.Height / 2); } }
        public Rectangle Floor { get { return new Rectangle(location.X + 12, location.Y, 16, 4); } }
        public Rectangle IgnoreFloor { get { return new Rectangle(location.X, location.Y + 16, 32, 16); } }
        public Rectangle Left { get { return new Rectangle(location.X, location.Y, location.Width / 2, location.Height); } }
        public Rectangle Right { get { return new Rectangle(location.X + location.Width / 2, location.Y, location.Width / 2, location.Height); } }
        public Rectangle Bottom { get { return new Rectangle(location.X, location.Y + location.Height / 2, location.Width, location.Height / 2); } }
        public Rectangle Center { get { return new Rectangle(location.X + 8, location.Y + 8, location.Width - 16, location.Height - 16); } }

        public int TileID { get { return tileID; } }
        public int Type { get { return type; } }
        public Rectangle Location { get { return location; } }
        public bool IsTop { get { return isTop; } }
        public void SetTop() { isTop = true; }

    }
}