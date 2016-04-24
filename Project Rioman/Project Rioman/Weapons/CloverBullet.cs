using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class CloverBullet : AbstractBullet
    {

        private Clover[] clovers = new Clover[4];
        private bool release;
        private KeyboardState prevKeyboardState;

        private struct Clover
        {
            private bool isAlive;
            private Rectangle location;
            private bool go;
            private bool comingOut;
            private int comingOutDist;
            private int releaseDist;

            private int direction;
            private int speed;
            private int radius;
            private float rotation;
            private Point origin;
            private bool collision;

            public void MakeClover(Rectangle loc, int dir, int spd, Texture2D sprite)
            {
                isAlive = true;
                location = new Rectangle(loc.X, loc.Y, sprite.Width, sprite.Height);
                origin = new Point(loc.X, loc.Y);
                go = false;
                comingOut = true;
                comingOutDist = 0;
                releaseDist = 0;
                direction = dir;
                speed = spd;

                radius = 46;
                rotation = 0f;
                collision = false;
            }

            public void Draw(SpriteBatch spriteBatch, Texture2D sprite)
            {
                if (go && isAlive)
                    spriteBatch.Draw(sprite, location, new Rectangle(0, 0, location.Width, location.Height),
                        Color.White, rotation, new Vector2(location.Width / 2, location.Height / 2), SpriteEffects.None, 0);

            }

            public void Update(Rioman player)
            {
                if (releaseDist == 0)
                    origin = player.Hitbox.Center;

                if (isAlive && go)
                {
                    if (comingOut)
                    {
                        location.X = origin.X + (comingOutDist + 3) * direction;
                        location.Y = origin.Y;
                        comingOutDist += 3;

                        rotation = direction * MathHelper.PiOver2 * comingOutDist / radius;

                        if (comingOutDist >= radius)
                        {
                            comingOut = false;
                            rotation = direction * MathHelper.PiOver2;
                        }
                    }
                    else
                    {
                        rotation += 0.1f;
                        rotation = MathHelper.WrapAngle(rotation);
                        location.X = (int)(origin.X + radius * Math.Sin(rotation)) + releaseDist;
                        location.Y = (int)(origin.Y + radius * Math.Cos(rotation));

                    }
                }
            }

            public bool Hits(Rectangle collisionRect)
            {
                if (isAlive && collisionRect.Intersects(new Rectangle(location.X - location.Width / 2,
                    location.Y - location.Height / 2, location.Width, location.Height)))
                {
                    collision = true;
                    return true;
                }

                return false;
            }

            public void Move(int x, int y)
            {
                location.X += x;
                location.Y += y;
            }

            public void ReleaseMove()
            {
                releaseDist += speed * direction;
            }

            public bool HasCollided() { return collision; }
            public void Kill() { isAlive = false; }
            public bool IsAlive() { return isAlive; }
            public bool ComingOut() { return comingOut; }
            public void SetDirection(int dir) { direction = dir; }
            public void Go() { go = true; }
            public Rectangle Location() { return location; }

        }

        public CloverBullet(int x, int y, bool facingRight) : base(Constant.CLOVERBULLET, facingRight)
        {
            release = false;
            sprite = BulletAttributes.GetSprites(Constant.CLOVERBULLET)[0];

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x - 24;

            location.Y = y + 20;

            for (int i = 0; i <= clovers.Length - 1; i++)
                clovers[i].MakeClover(location, direction, speed, sprite);

            clovers[0].Go();

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                for (int i = 0; i <= clovers.Length - 1; i++)
                    clovers[i].Draw(spriteBatch, sprite);
            }
        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle();
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies)
        {
            canDie = false;

            if (isAlive)
            {
                bool stillAlive = false;

                //Update bullets
                for (int i = 0; i <= clovers.Length - 1; i++)
                {
                    if ((!clovers[i].ComingOut() || !clovers[i].IsAlive()) && i < clovers.Length - 1)
                        clovers[i + 1].Go();
                    
                    if (clovers[i].IsAlive())
                        stillAlive = true;

                    if (release)
                    {
                        clovers[i].ReleaseMove();
                        weight = 0;
                    }

                    clovers[i].Update(player);
                }


                //check if offscreen
                if (release)
                {
                    canDie = true;
                    bool outOfBounds = true;

                    for (int i = 0; i <= clovers.Length - 1; i++)
                    {
                        Rectangle loc = clovers[i].Location();
                        if (!clovers[i].IsAlive() || loc.X < -loc.Width || loc.X > viewport.Width ||
                            loc.Y < -loc.Height || loc.Y > viewport.Height) { }
                        else outOfBounds = false;
                    }
                    if (outOfBounds)
                        stillAlive = false;
                }

                //check if can be released
                if (!release)
                {
                    if (Keyboard.GetState().IsKeyDown(Constant.SHOOT) && !prevKeyboardState.IsKeyDown(Constant.SHOOT))
                    {
                        bool readyToRelease = true;

                        for (int i = 0; i <= clovers.Length - 1; i++)
                            if (clovers[i].ComingOut() && (clovers[i].IsAlive()))
                                readyToRelease = false;

                        release = readyToRelease;

                        if (release)
                        {
                            for (int i = 0; i <= clovers.Length - 1; i++)
                                clovers[i].SetDirection(player.FacingRight() ? 1 : -1);
                        }
                    }
                }
                
                isAlive = stillAlive;
                prevKeyboardState = Keyboard.GetState();
            }

        }

        public override int TakeDamage(string enemyID)
        {
            if (isAlive)
            {
                for (int i = 0; i <= clovers.Length - 1; i++)
                {
                    if (clovers[i].IsAlive() && clovers[i].HasCollided())
                    {
                        clovers[i].Kill();
                        return damage;
                    }
                }

            }

            return 0;
        }

        public override bool Hits(Rectangle collisionRect)
        {
            if (isAlive)
            {
                for (int i = 0; i <= clovers.Length - 1; i++)
                    if (clovers[i].Hits(collisionRect))
                        return true;
            }

            return false;
        }

        protected override void SubMove(int x, int y)
        {
            for (int i = 0; i <= clovers.Length - 1; i++)
                clovers[i].Move(x, y);
        }
    }
}

