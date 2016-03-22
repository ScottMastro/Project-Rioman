using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    //type 0 = decoration
    //type 1 = walkable
    //type 2 = death
    //type 3 = climb
    //type 4 = door
    //type 5 = disappearing
    //type 6 = scroll

    class Tile
    {
        private Texture2D sprite;
        public Texture2D[] frames;

        public int tile;
        public int type;

        public Rectangle location;
        public Rectangle floor;
        public Rectangle ignoreFloor;
        public Rectangle left;
        public Rectangle right;
        public Rectangle top;
        public Rectangle bottom;

        public double fadetime;
        public double animationtime;

        public bool isOpening;
        public bool isclosing;
        public bool isTop;

        public Tile(Texture2D texture, int tileNumber, int tileType, int y, int x, ContentManager content)
        {
            sprite = texture;
            tile = tileNumber;
            type = tileType;

            location = new Rectangle(x, y, 32, 32);

            floor = new Rectangle(x + 12, y, 16, 4);
            ignoreFloor = new Rectangle(x, y + 16, 32, 16);

            top = new Rectangle(x, y, location.Width, location.Height/2);
            bottom = new Rectangle(x, y + location.Height / 2, location.Width, location.Height / 2);
            left = new Rectangle(x, y, location.Width/2, location.Height);
            right = new Rectangle(x + location.Width / 2, y, location.Width/2, location.Height);


            isOpening = false;
            isTop = false;

            CheckLoadFrames(content);
        }

        private void CheckLoadFrames(ContentManager content)
        {
            if (tile == 177 || tile == 178)
                LoadFrames(content, 2);
            if (tile == 106 || tile == 130 || tile == 160 || tile == 223)
                LoadFrames(content, 3);
            if (tile == 56)
                LoadFrames(content, 4);
            if (tile == 102)
                LoadFrames(content, 9);
        }

        public void LoadFrames(ContentManager content, int frm)
        {
            frames = new Texture2D[frm];
            frames[0] = sprite;
            frames[1] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "b");
            if (frm > 2)
                frames[2] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "c");
            if (frm > 3)
                frames[3] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "d");
            if (frm > 4)
                frames[4] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "e");
            if (frm > 5)
            {
                frames[5] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "f");
                frames[6] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "g");
                frames[7] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "h");
                frames[8] = content.Load<Texture2D>("Video\\tiles\\" + tile.ToString() + "i");
            }
        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
            floor.X += x;
            floor.Y += y;
            ignoreFloor.X += x;
            ignoreFloor.Y += y;
            top.X += x;
            top.Y += y;
            bottom.X += x;
            bottom.Y += y;
            left.X += x;
            left.Y += y;
            right.X += x;
            right.Y += y;
        }

        public void Fade(GameTime gameTime)
        {
            fadetime -= gameTime.ElapsedGameTime.TotalSeconds;

            if (fadetime <= -5)
            {
                sprite = frames[0];
                fadetime = 1.5;
                Move(0, 100000);
            }

            if (fadetime < 0 && location.Y > 0)
                Move(0, -100000);

            if (fadetime <= 1)
                sprite = frames[1];

            if (fadetime <= 0.5)
                sprite = frames[2];
        }

        public void Wave(GameTime gameTime)
        {
            animationtime += gameTime.ElapsedGameTime.TotalSeconds;

            if (animationtime > 2.25)
            {
                animationtime = 0;
                sprite = frames[0];
            }

            if (animationtime > 2)
                sprite = frames[8];
            else if (animationtime > 1.75)
                sprite = frames[7];
            else if (animationtime > 1.5)
                sprite = frames[6];
            else if (animationtime > 1.25)
                sprite = frames[5];
            else if (animationtime > 1)
                sprite = frames[4];
            else if (animationtime > 0.75)
                sprite = frames[3];
            else if (animationtime > 0.5)
                sprite = frames[2];
            else if (animationtime > 0.25)
                sprite = frames[1];
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            if (type != 6 && type >= 0)
                spriteBatch.Draw(sprite, location, Color.White);

        }

        public int X { get { return location.X; } }
        public int Y { get { return location.Y; } }
        public int Width  { get { return location.Width; } }


    }
}