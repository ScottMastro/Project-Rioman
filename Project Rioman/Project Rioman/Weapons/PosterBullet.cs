using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class PosterBullet : AbstractBullet
    {
        private const double TIME_BETWEEN_BULLETS = 0.1;
        private double weightTime;

        private Post[] posts = new Post[4];

        private struct Post
        {
            private bool isAlive;
            private bool isBorn;

            private Rectangle location;

            private int direction;
            private int speed;
            private double birthTime;

            private bool collision;

            public void MakePost(Rectangle loc, int dir, int spd, Texture2D sprite, double birthTime)
            {
                isAlive = true;
                isBorn = false;
                this.birthTime = birthTime;

                location = new Rectangle(loc.X, loc.Y, sprite.Width, sprite.Height);

                direction = dir;
                speed = spd;

                collision = false;
            }

            public void Draw(SpriteBatch spriteBatch, Texture2D sprite)
            {
                if (isAlive && isBorn)
                    spriteBatch.Draw(sprite, location, Color.White);
            }

            public void Update(Rioman player, double deltaTime)
            {
                if (isAlive && isBorn)
                    location.X += speed * direction;

                if (isAlive && !isBorn)
                {
                    location.X = player.Hitbox.Center.X + (direction < 0 ? -10 : 2);
                    location.Y = player.Hitbox.Center.Y - 6;

                    birthTime -= deltaTime;
                    if (birthTime < 0)
                        isBorn = true;
                }

            }

            public bool Hits(Rectangle collisionRect)
            {
                if (isAlive && collisionRect.Intersects(location) && isBorn)
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
            public bool IsBorn() { return isBorn; }
            public Rectangle Location() { return location; }

        }


        public PosterBullet(int x, int y, bool facingRight) : base(Constant.POSTERBULLET, facingRight)
        {

            sprite = BulletAttributes.GetSprites(Constant.POSTERBULLET)[0];

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x ;

            location.Y = y + 24;

            for (int i = 0; i <= posts.Length - 1; i++)
                posts[i].MakePost(location, direction, speed, sprite, TIME_BETWEEN_BULLETS * i);

            weightTime = 0;

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                for (int i = 0; i <= posts.Length - 1; i++)
                    posts[i].Draw(spriteBatch, sprite);
        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }

        public override bool Hits(Rectangle collisionRect)
        {
            if (isAlive)
            {
                for (int i = 0; i <= posts.Length - 1; i++)
                    if (posts[i].Hits(collisionRect))
                        return true;
            }

            return false;
        }

        public override int TakeDamage(string enemyID)
        {
            if (isAlive)
            {
                for (int i = 0; i <= posts.Length - 1; i++)
                {
                    if (posts[i].IsAlive() && posts[i].HasCollided())
                    {
                        posts[i].Kill();
                        return damage;
                    }
                }
            }

            return 0;
        }

        protected override void SubMove(int x, int y)
        {
            for (int i = 0; i <= posts.Length - 1; i++)
                posts[i].Move(x, y);
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies)
        {
            if (isAlive)
            {
                for (int i = 0; i <= posts.Length - 1; i++)
                    posts[i].Update(player, deltaTime);

                bool stillAlive = false;
                bool outOfBounds = true;
                bool allBorn = true;

                for (int i = 0; i <= posts.Length - 1; i++)
                {
                    if (posts[i].IsAlive())
                        stillAlive = true;

                    Rectangle loc = posts[i].Location();
                    if (!posts[i].IsAlive() || loc.X < -loc.Width || loc.X > viewport.Width ||
                        loc.Y < -loc.Height || loc.Y > viewport.Height) { }
                    else outOfBounds = false;

                    if (!posts[i].IsBorn())
                        allBorn = false;
                }

                if (outOfBounds)
                    stillAlive = false;
                if (allBorn && weightTime > TIME_BETWEEN_BULLETS)
                    weight = 0;
                else if (allBorn)
                    weightTime += deltaTime;
                else 
                    weight = 10;
                
                isAlive = stillAlive;

                

            }
        }
    }
}
