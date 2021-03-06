﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class ChanceBomb : AbstractEnemy
    {
        private Texture2D explosion;
        private Texture2D bomb;

        private bool hasExploded;
        private bool exploding;
        private int explosionFrame;
        private double explosionTime;
        private const int EXPLOSION_DAMAGE = 6;

        private int counter;
        private double countTime;



        public ChanceBomb(int type, int x, int y) : base(type, x, y)
        {
            Texture2D[] sprites = EnemyAttributes.GetSprites(type);

            bomb = sprites[0];
            explosion = sprites[1];
            canDropItem = false;

            SubReset();
        }


        protected sealed override void SubReset()
        {
            sprite = bomb;
            counter = 0;
            drawRect = new Rectangle(0, 0, sprite.Width / 6, sprite.Height);
            exploding = false;

            location.Y -= sprite.Height;

            isInvincible = true;
            hasExploded = false;
            explosionFrame = 0;
            explosionTime = 0;
            countTime = 0;
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive && !hasExploded)
            {
                if (!exploding)
                {
                    foreach (AbstractBullet b in rioBullets)
                    {
                        if (b!= null && b.Hits(GetCollisionRect()))
                        {
                            b.TakeDamage(uniqueID);
                            if (counter == 0)
                                counter = 1;
                        }
                    }

                    if (counter > 0)
                    {
                        countTime += deltaTime;
                        if (countTime > 1)
                        {
                            countTime = 0;
                            if (counter < 3)
                                counter++;
                            else if (counter == 3)
                            {
                                if (new Random().Next(0, 2) == 0)
                                    counter = 4;
                                else
                                    counter = 5;
                            }
                        }

                        if (countTime > 0.5)
                        {
                            if (counter == 4)
                            {
                                Die();

                                if (Math.Abs(location.Center.X - player.Hitbox.Center.X) < 100 &&
                                    (Math.Abs(location.Center.Y - player.Hitbox.Center.Y) < 100))
                                    droppedItem = new EnemyPickup(Constant.BIG_HEALTH, location.Center.X + 16, location.Center.Y);
                            }
                            else if (counter == 5)
                                exploding = true;
                        }
                    }
                }
                else
                {
                    sprite = explosion;
                    explosionTime += deltaTime;

                    if (explosionTime > 0.1)
                    {
                        explosionFrame++;
                        explosionTime = 0;
                        if (explosionFrame > 11)
                            hasExploded = true;
                    }
                }
            }
        }

        protected override void SubDrawEnemy(SpriteBatch spriteBatch)
        {
            if (!hasExploded)
            {
                if (!exploding)
                {
                    drawRect = new Rectangle(counter * sprite.Width / 6, 0, sprite.Width / 6, sprite.Height);
                    spriteBatch.Draw(sprite, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height), drawRect, Color.White);
                }
                else
                {
                    drawRect = new Rectangle(explosionFrame * sprite.Width / 12, 0, sprite.Width / 12, sprite.Height);
                    spriteBatch.Draw(explosion, new Rectangle(location.X, location.Y, drawRect.Width, drawRect.Height), drawRect, Color.White,
                        0f, new Vector2(drawRect.Width / 2, drawRect.Height / 2), SpriteEffects.None, 0);
                }
            }
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            if (isAlive && !hasExploded)
            {
                Rectangle explosionRect = new Rectangle(location.X - drawRect.Width / 2 + 12,
                    location.Y - drawRect.Height / 2 + 10, drawRect.Width - 24, drawRect.Height - 20);

                if (player.Hitbox.Intersects(explosionRect))
                    player.Hit(EXPLOSION_DAMAGE);
            }
        }

        public override Rectangle GetCollisionRect()
        {
            if (exploding || hasExploded)
                return new Rectangle(0, 0, 0, 0);
            else
                return new Rectangle(location.X + 8, location.Y + 5, drawRect.Width - 16, drawRect.Height - 5);

        }


        public override void DetectTileCollision(AbstractTile tile)
        {
            //do nothing
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }


    }
}
