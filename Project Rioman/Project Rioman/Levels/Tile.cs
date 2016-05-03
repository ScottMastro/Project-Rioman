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

    class Tile
    {
        private Texture2D sprite;
        private int x;
        private int y;

        private Texture2D[] frames;
        private int numFrames;
        private int currentFrame;
        private int animateDir;

        public int tile;
        public int type;
        private int originalType;

        public Rectangle location;
        private Rectangle originalLocation;

        public double fadeTime;
        public double animationtime;

        public bool isTop;

        public Tile(Texture2D texture, int tileNumber, int tileType, int x, int y, ContentManager content)
        {
            sprite = texture;
            tile = tileNumber;
            type = tileType;
            originalType = type;

            this.x = x;
            this.y = y;

            location = new Rectangle(x * Constant.TILE_SIZE, y * Constant.TILE_SIZE, Constant.TILE_SIZE, Constant.TILE_SIZE);
            originalLocation = location;

            isTop = false;

            CheckLoadFrames(content);
            animateDir = 1;
            currentFrame = 0;
        }

        private void CheckLoadFrames(ContentManager content)
        {
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(sprite);
            int counter = 1;

            while (true)
            {
                try
                {
                    Texture2D t = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "-" + counter.ToString());
                    textures.Add(t);
                }
                catch
                {
                    break;
                }

                counter++;
            }

            frames = textures.ToArray();
            numFrames = counter;
        }

        public void Reset()
        {
            type = originalType;
            location = originalLocation;
        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void Fade(GameTime gameTime)
        {
            fadeTime -= gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeTime <= -5)
            {
                sprite = frames[0];
                fadeTime = 1.5;
                Move(0, 100000);
            }

            if (fadeTime < 0 && location.Y > 0)
                Move(0, -100000);

            if (fadeTime <= 1)
                sprite = frames[1];

            if (fadeTime <= 0.5)
                sprite = frames[2];
        }


        public void Animate(double deltaTime)
        {

            if (numFrames == 1 || type == 5)
                return;

            animationtime += deltaTime;

            if (animationtime > 0.25)
            {
                animationtime = 0;
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

        }

        public int X { get { return location.X; } }
        public int Y { get { return location.Y; } }
        public int GridX { get { return x; } }
        public int GridY { get { return y; } }
        public int Width { get { return location.Width; } }
        public void ChangeType(int newType) { type = newType; }

        public Rectangle Top { get { return new Rectangle(location.X, location.Y, location.Width, location.Height / 2); } }
        public Rectangle Floor { get { return new Rectangle(location.X + 12, location.Y, 16, 4); } }
        public Rectangle IgnoreFloor { get { return new Rectangle(location.X, location.Y + 16, 32, 16); } }
        public Rectangle Left { get { return new Rectangle(location.X, location.Y, location.Width / 2, location.Height); } }
        public Rectangle Right { get { return new Rectangle(location.X + location.Width / 2, location.Y, location.Width / 2, location.Height); } }
        public Rectangle Bottom { get { return new Rectangle(location.X, location.Y + location.Height / 2, location.Width, location.Height / 2); } }
        public Rectangle Center { get { return new Rectangle(location.X + 8, location.Y + 8, location.Width - 16, location.Height - 16); } }

    }
}