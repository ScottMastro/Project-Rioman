﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Project_Rioman
{
    abstract class AbstractBoss
    {
        private Texture2D pose;
        private double poseTime;
        private int poseFrame;
        private int poseAnimateDir;
        protected bool posing;

        protected string uniqueID;
        private Level level;

        protected int type;
        protected Rectangle location;
        protected Rectangle originalLocation;
        protected Rectangle netMovement;

        protected AbstractPickup powerUp;
        protected bool madePowerUp;

        protected SpriteEffects direction;
        protected Texture2D sprite;
        protected Rectangle drawRect;

        protected int blinkFrames;

        protected enum State { standing, jumping, falling, running, crouching, lurking, floating }
        protected bool shooting;
        protected State state;

        protected Texture2D killExplosion;
        protected const int EXPLOSION_SPEED = 35;

        protected double killTime;

        protected bool isAlive;
        protected bool wasAlive;

        protected int health;
        protected const int TOUCH_DAMAGE = 6;

        public AbstractBoss(int type, int x, int y)
        {

            pose = BossAttributes.GetSprites(type)[0];
            this.type = type;

            originalLocation = new Rectangle(x * Constant.TILE_SIZE, (y + 1) * Constant.TILE_SIZE, 0, 0);
            killExplosion = BossAttributes.GetKillSprite();
            blinkFrames = 0;

        }

        public void Reset()
        {
            state = State.standing;
            shooting = false;
            location.X = originalLocation.X;
            location.Y = originalLocation.Y - drawRect.Height;
            netMovement = new Rectangle(0, 0, 0, 0);

            isAlive = false;
            direction = SpriteEffects.None;
            health = Constant.MAX_HEALTH;
            killTime = 0;
            powerUp = null;
            madePowerUp = false;

            poseFrame = 0;
            poseTime = 0;
            poseAnimateDir = 1;

            Guid guid = Guid.NewGuid();
            uniqueID = guid.ToString();

            SubReset();
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
                Die();
        }

        public void Die()
        {
            isAlive = false;

            int x = GetCollisionRect().Center.X;
            int y = GetCollisionRect().Center.Y;

        }

        public void SetAlive(Rioman player)
        {
            isAlive = true;
            FacePlayer(player);

            posing = true;
            poseTime = 0;
            StatusBar.SetBossHealth(0);
            StatusBar.DrawBossHealth();
            StatusBar.IncreaseBossHealth(Constant.MAX_HEALTH);
        }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
            netMovement.X += x;
            netMovement.Y += y;
        }

        public void BusyUpdate(double deltaTime)
        {
            if (posing && isAlive)
            {
                poseTime += deltaTime;
                if (poseTime > 0.08 && poseFrame == 0 && poseAnimateDir == -1)
                {
                    posing = false;
                    poseTime = 0;
                    poseAnimateDir = 1;
                }
                else if (poseTime > 0.08 && poseFrame != 4)
                {
                    poseTime = 0;
                    poseFrame += poseAnimateDir;
                }
                else if (poseTime > 1 && poseFrame == 4)
                {
                    poseTime = 0;
                    poseAnimateDir = -1;
                    poseFrame += poseAnimateDir;
                }
            }
        }


        public void Update(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
            if (isAlive)
            {
                StatusBar.SetBossHealth(health);

                SubUpdate(player, rioBullets, deltaTime, viewport);
                CheckHit(player, rioBullets);
            }

            if (!isAlive && health <= 0)
            {
                StatusBar.SetBossHealth(health);

                killTime += deltaTime;

                if (killTime > 5 && !madePowerUp)
                {
                    madePowerUp = true;
                    powerUp = new PowerUp(type - 9, 
                        originalLocation.X + netMovement.X,
                        originalLocation.Y + netMovement.Y);
                }

                if (wasAlive)
                    Audio.PlayKillBoss();
            }


            wasAlive = isAlive;
        }

        protected void CheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            if (isAlive && !player.IsLurking())
            {
                if (GetCollisionRect().Intersects(player.Hitbox))
                    player.Hit(TOUCH_DAMAGE);

                for (int i = 0; i <= rioBullets.Length - 1; i++)
                    if (rioBullets[i] != null)
                    {
                        if (rioBullets[i].Hits(GetCollisionRect()))
                        {
                            int damage = rioBullets[i].TakeDamage(uniqueID);
                            if (damage > 0)
                            {
                                TakeDamage(damage);
                                blinkFrames = 2;
                            }

                        }
                    }
            }

            SubCheckHit(player, rioBullets);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive && posing)
            {
                spriteBatch.Draw(pose, location, new Rectangle(pose.Width/5 * poseFrame, 0, pose.Width/5, pose.Height),
                    Color.White, 0f, new Vector2(), direction, 0);
            }
            else {
                SubDrawOther(spriteBatch);
                if (health > 0 && blinkFrames <= 0)
                    SubDrawBoss(spriteBatch);
                else if (blinkFrames > 0)
                    blinkFrames--;


                if (killTime < 5 && !isAlive && health <= 0)
                {

                    for (int i = 0; i < 4; i++)
                        DrawExplosion(spriteBatch, EXPLOSION_SPEED / 8, MathHelper.TwoPi / 4f * i);

                    for (int i = 0; i < 6; i++)
                        DrawExplosion(spriteBatch, EXPLOSION_SPEED / 2, MathHelper.TwoPi / 6f * i);

                    for (int i = 0; i < 8; i++)
                        DrawExplosion(spriteBatch, EXPLOSION_SPEED, MathHelper.TwoPi / 8f * i);

                }
            }
        }

        private void DrawExplosion(SpriteBatch spriteBatch, int speed, float angle)
        {
            int frame = (int)Math.Floor(killTime);

            int x = GetCollisionRect().Center.X;
            int y = GetCollisionRect().Center.Y;
            int width = killExplosion.Width / 5;
            int height = killExplosion.Height;
            int move = (int)(killTime * speed);

            Rectangle killDrawRect = new Rectangle(frame * width, 0, width, height);
            Rectangle killRect = new Rectangle(x, y - move, width, height);
            Vector2 origin = new Vector2(width / 2, height / 2 + move * 3);

            float alpha = Math.Min(1f, 5f - (float)killTime);

            spriteBatch.Draw(killExplosion, killRect, killDrawRect, Color.White * alpha,
                angle, origin, SpriteEffects.None, 0);

        }

        protected abstract void SubReset();
        protected abstract void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport);
        protected abstract void SubCheckHit(Rioman player, AbstractBullet[] rioBullets);
        protected abstract void SubDrawBoss(SpriteBatch spriteBatch);
        protected abstract void SubDrawOther(SpriteBatch spriteBatch);

        public abstract Rectangle GetCollisionRect();
        public abstract Rectangle Feet();
        public abstract Rectangle Head();
        public abstract Rectangle Left();
        public abstract Rectangle Right();


        public abstract void DetectTileCollision(AbstractTile tile);

        public bool FacingLeft()
        {
            return direction == SpriteEffects.None;
        }

        protected void FacePlayer(Rioman player)
        {
            if (player.Hitbox.Center.X < location.X)
                direction = SpriteEffects.None;
            else
                direction = SpriteEffects.FlipHorizontally;
        }

        public AbstractPickup GetPowerUp()
        {
            AbstractPickup p = powerUp;
            powerUp = null;
            return p;
        }

        public string GetID() { return uniqueID; }
        public bool IsAlive() { return isAlive; }
        public bool IsBusy() { return posing; }
        public int Type() { return type; }

        protected bool IsStanding() { return state == State.standing; }
        protected bool IsRunning() { return state == State.running; }
        protected bool IsJumping() { return state == State.jumping; }
        protected bool IsFalling() { return state == State.falling; }
        protected bool IsCrouching() { return state == State.crouching; }
        protected bool IsLurking() { return state == State.lurking; }
        protected bool IsFloating() { return state == State.floating; }

    }
}
