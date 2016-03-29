using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Rioman
{
    class Macks : AbstractEnemy
    {

        private Texture2D bullet;
        private bool stopUpMovement;
        private bool stopDownMovement;
        private bool collideWithTile;

        struct MackBullet
        {
            public bool isAlive;
            public int X;
            public int Y;
            public int direction;

            public void Reset()
            {
                isAlive = false;
                X = 0;
                Y = 0;
                direction = -1;
            }

        }

        private MackBullet[] bullets = new MackBullet[3];


        public Macks(int type, int r, int c) : base(type, r, c)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);
            sprite = sprites[0];
            bullet = sprites[1];

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location.Y -= sprite.Height;

            stopUpMovement = false;
            stopDownMovement = false;
            collideWithTile = false;
        }

        protected override void SubUpdate(Rioman player, Bullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                if (player.Hitbox.Left > GetCollisionRect().Right)
                    direction = SpriteEffects.FlipHorizontally;
                else if (player.Hitbox.Right < GetCollisionRect().Left)
                    direction = SpriteEffects.None;


                int distance = GetCollisionRect().Center.Y - player.Hitbox.Center.Y;
                int speed = Math.Min(Math.Abs(distance) * 20 / viewport.Height, 12);
                speed = Math.Max(speed, 1);

                if (distance < 0 && !stopDownMovement)
                {
                    location.Y += speed;
                }
                else if (distance > 0 && !stopUpMovement)
                {
                    location.Y -= speed;

                }


                if (!collideWithTile)
                {
                    stopUpMovement = false;
                    stopDownMovement = false;
                }
                collideWithTile = false;
            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height),
               drawRect, Color.White, 0f, new Vector2(), direction, 0);

        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 5, location.Y + 3, drawRect.Width - 10, drawRect.Height - 6);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }

        public override void DetectTileCollision(Tile tile)
        {
            if (tile.type == 1)
            {
                if (GetCollisionRect().Intersects(tile.Bottom))
                {
                    stopUpMovement = true;
                    collideWithTile = true;
                }
                if (GetCollisionRect().Intersects(tile.Top))
                {
                    stopDownMovement = true;
                    collideWithTile = true;
                }
            }
        }

        protected override void SubCheckHit(Rioman player, Bullet[] rioBullets)
        {
            //do nothing
        }
    }
}
