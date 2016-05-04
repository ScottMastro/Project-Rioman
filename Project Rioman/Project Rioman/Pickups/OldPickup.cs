using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class OldPickup
    {
        public int type;
        public bool isalive;
        public Texture2D[] sprites = new Texture2D[19];
        public Rectangle location;
        public Rectangle sourcerect;
        public int activeframe;
        public bool reverseanimation;
        public bool onground;
        public double animationtime;
        public double nextlevel;


        public OldPickup()
        {
        }

        public void LoadPickupSprites(ContentManager content)
        {
            for (int i = 1; i <= 9; i++)
                sprites[i] = content.Load<Texture2D>("Video\\pickups\\wpickup" + i.ToString());

            sprites[10] = content.Load<Texture2D>("Video\\pickups\\etank");
            sprites[11] = content.Load<Texture2D>("Video\\pickups\\mtank");
            sprites[12] = content.Load<Texture2D>("Video\\pickups\\wtank");

            sprites[13] = content.Load<Texture2D>("Video\\pickups\\life");
            sprites[14] = content.Load<Texture2D>("Video\\pickups\\bighealth");
            sprites[15] = content.Load<Texture2D>("Video\\pickups\\smallhealth");
            sprites[16] = content.Load<Texture2D>("Video\\pickups\\bigweapon");
            sprites[17] = content.Load<Texture2D>("Video\\pickups\\smallweapon");
        }

        public void NewPickup(int xpos, int ypos)
        {
            Random r = new Random();
            int number = r.Next(0, 401);

            if (number < 101)
            {
                isalive = true;
                activeframe = 0;
                reverseanimation = false;

                if (number > 0 && number <= 30)
                    type = 15;
                else if (number > 30 && number <= 50)
                    type = 14;
                else if (number > 50 && number <= 80)
                    type = 17;
                else
                    type = 16;

                if (number == 0)
                {
                    type = 13;
                    sourcerect = Animation.GetSourceRect(sprites[type].Width, sprites[type].Height);
                }
                else
                    sourcerect = Animation.GetSourceRect(2, ref activeframe, sprites[type].Width, sprites[type].Height, ref reverseanimation);

                location = new Rectangle(xpos, ypos, sourcerect.Width, sourcerect.Height);
            }
        }

        public void NewPickup(int xpos, int ypos, int gunnumber)
        {
            type = gunnumber;
            isalive = true;
            activeframe = 0;
            reverseanimation = false;
            sourcerect = Animation.GetSourceRect(sprites[type].Width, sprites[type].Height);
            location = new Rectangle(xpos, ypos, sourcerect.Width, sourcerect.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isalive)
                spriteBatch.Draw(sprites[type], location, sourcerect, Color.White, 0f, new Vector2(location.Width / 2, location.Height / 2), SpriteEffects.None, 0);
        }

        public bool PickupUpdate(Rioman rioman, double elapsedtime)
        {
            bool backtoselection = false;

            if (isalive)
            {
                if (!onground)
                    location.Y += 4;

                onground = false;

                if (rioman.Location.Intersects(location))
                    isalive = false;
            }
            else
            {
                nextlevel += elapsedtime;

                if (nextlevel > 3)
                {
                    backtoselection = true;
                }
            }

            return backtoselection;
        }


        public void PickupUpdate(Rioman rioman, Viewport viewportrect)
        {
            if (isalive)
            {
                Kill(true, viewportrect);

                if (!onground)
                    location.Y += 4;

                onground = false;

                if (rioman.Location.Intersects(location))
                {
                    if (type == 13)
                    {                     
                        //add life
                    }
                    else if (type == 14)
                        StatusBar.IncreaseHealth(10);
                    else if (type == 15)
                        StatusBar.IncreaseHealth(4);

                    Kill(false, viewportrect);
                }
            }
        }

        public void PickupUpdate(AbstractTile tile)
        {
            if (tile.Floor.Intersects(location))
            {
                onground = true;
                location.Y = tile.Location.Y - location.Height + 6;

                if (type <= 9)
                    location.Y += 12;
            }
        }

        public void Animate(double elapsedtime)
        {
            animationtime += elapsedtime;

            if (animationtime > 0.1)
            {
                sourcerect = Animation.GetSourceRect(2, ref activeframe, sprites[type].Width, sprites[type].Height, ref reverseanimation);
                animationtime = 0;
            }
        }

        public void MovePickup(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void Kill(bool check, Viewport viewportrect)
        {
            if (check)
            {
                if (location.Right < 0 || location.X > viewportrect.Width ||
                    location.Bottom < 0 || location.Y > viewportrect.Height)
                    check = false;
            }

            if (!check)
            {
                isalive = false;
                activeframe = 0;
                animationtime = 0;
                reverseanimation = false;
            }
        }
    }
}
