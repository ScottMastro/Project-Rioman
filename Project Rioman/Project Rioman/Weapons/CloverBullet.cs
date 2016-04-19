using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class CloverBullet : AbstractBullet
    {

        Clover[] clovers = new Clover[4];

        private struct Clover
        {
            private bool isAlive;
            private Rectangle location;
            private bool go;
            private bool comingOut;
            private int comingOutDist;

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
                origin = player.Hitbox.Center;

                if (isAlive && go)
                {
                    if (comingOut)
                    {
                        location.X = origin.X + (comingOutDist + speed) * direction;
                        location.Y = origin.Y;
                        comingOutDist += speed;

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
                        location.X = (int)(origin.X + radius * Math.Sin(rotation));
                        location.Y = (int)(origin.Y + radius * Math.Cos(rotation));

                    }
                }
            }

            public bool Hits(Rectangle collisionRect)
            {
                if (isAlive && collisionRect.Intersects(new Rectangle(location.X - location.Width / 2,
                    location.Y - location.Height/2, location.Width, location.Height)))
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

            public bool HasCollided() { return collision; }
            public void Kill() { isAlive = false; }
            public bool IsAlive() { return isAlive; }
            public bool ComingOut() { return comingOut; }
            public void Go() { go = true; }

        }

        public CloverBullet(int x, int y, bool facingRight) : base(Constant.CLOVERBULLET, facingRight)
        {

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

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            if (isAlive)
            {
                bool stillAlive = false;

                for (int i = 0; i <= clovers.Length - 1; i++)
                {
                    if ((!clovers[i].ComingOut() || !clovers[i].IsAlive()) && i < clovers.Length - 1)
                        clovers[i + 1].Go();
                    

                    if (clovers[i].IsAlive())
                        stillAlive = true;

                    clovers[i].Update(player);
                }

                isAlive = stillAlive;
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

