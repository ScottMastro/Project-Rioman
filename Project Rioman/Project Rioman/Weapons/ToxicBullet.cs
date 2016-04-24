using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class ToxicBullet : AbstractBullet
    {
        private Texture2D bubbleInside;
        private Texture2D bubbleBorder;
        private Texture2D cloud1;
        private Texture2D cloud2;

        private const int CLOUD_DAMAGE = 1;
        private const double CLOUD_DAMAGE_TIME = 0.2;


        private int frame;
        private double frameTime;

        private double cloudGrowTime;
        private double cloudFadeTime;
        private Dictionary<string, double> hitTimes;


        private bool hit;

        public ToxicBullet(int x, int y, bool facingRight) : base(Constant.TOXICBULLET, facingRight)
        {

            bubbleBorder = BulletAttributes.GetSprites(Constant.TOXICBULLET)[0];
            bubbleInside = BulletAttributes.GetSprites(Constant.TOXICBULLET)[1];
            cloud1 = BulletAttributes.GetSprites(Constant.TOXICBULLET)[2];
            cloud2 = BulletAttributes.GetSprites(Constant.TOXICBULLET)[3];

            drawRect = new Rectangle(0, 0, bubbleBorder.Width / 3, bubbleBorder.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);


            //TODO: adjust location

            if (facingRight)
                location.X = x - 10;
            else
                location.X = x - 24;

            location.Y = y + 20;

            frame = 0;
            frameTime = 0;
            cloudGrowTime = 0;
            cloudFadeTime = 1;
            hit = false;

            hitTimes = new Dictionary<string, double>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                if (!hit)
                {
                    drawRect = new Rectangle(bubbleBorder.Width / 3 * frame, 0, bubbleBorder.Width / 3, bubbleBorder.Height);

                    spriteBatch.Draw(bubbleBorder, location, drawRect, Color.White);
                    spriteBatch.Draw(bubbleInside, location, drawRect, Color.White * 0.8f);
                }
                else
                {
                    drawRect = new Rectangle(0, 0, cloud1.Width, cloud1.Height);


                    DrawCloud(spriteBatch, 0, 0);
                    DrawCloud(spriteBatch, 30, 50);
                    DrawCloud(spriteBatch, -40, -30);
                    DrawCloud(spriteBatch, -50, 40);
                    DrawCloud(spriteBatch, 50, -50);

                }
            }
        }

        private void DrawCloud(SpriteBatch spriteBatch, int x, int y)
        {
            float scale = (float)cloudGrowTime;
            float fade = (float)cloudFadeTime;

            Texture2D img = (frame == 0 ? cloud1 : cloud2);

            spriteBatch.Draw(img, new Vector2(location.X + scale * x,  location.Y + scale * y), drawRect, Color.White * fade, 0f,
                new Vector2(img.Width / 2, img.Height / 2), scale, SpriteEffects.None, 0);
        }


        public override Rectangle GetCollisionRect()
        {
            if (!hit)
                return location;
            else
                return new Rectangle(location.X - cloud1.Width / 2 - 50,
                    location.Y - cloud1.Height / 2 - 50, cloud1.Width + 100, cloud1.Height + 100);
        }

        public override bool Hits(Rectangle collisionRect)
        {
            return GetCollisionRect().Intersects(collisionRect);
        }

        public override int TakeDamage(string enemyID)
        {
            if (isAlive)
            {
                if (!hit)
                {
                    hit = true;
                    return damage;
                }
                else
                {
                    if (hitList.Contains(enemyID))
                        return 0;

                    hitList.Add(enemyID);
                    hitTimes.Add(enemyID, 0);

                    if (isAlive)
                        return 1;

                }
            }
            return 0;
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies)
        {
            if (isAlive) {
                if (hit)
                {

                    for (int i = 0; i <= hitList.Count - 1; i++)
                    {
                        hitTimes[hitList[i]] += deltaTime;
                        if (hitTimes[hitList[i]] > CLOUD_DAMAGE_TIME)
                        {
                            hitTimes.Remove(hitList[i]);
                            hitList.Remove(hitList[i]);
                        }
                    }

                    frameTime += deltaTime;
                    if (frameTime > 0.2)
                    {
                        frameTime = 0;
                        if (frame <= 0)
                            frame = 1;
                        else
                            frame = 0;
                    }

                    cloudGrowTime = Math.Min(cloudGrowTime + deltaTime * 5, 1);
                    cloudFadeTime = Math.Max(cloudFadeTime - deltaTime / 2, 0);

                    if (cloudFadeTime <= 0.03)
                        isAlive = false;
                }

                if (!hit)
                {
                    location.X += direction * speed;

                    double advanceFrame = 0.3;

                    frameTime += deltaTime;
                    if (frameTime > advanceFrame * 4)
                    {
                        frameTime = 0;
                        frame = 0;
                    }
                    else if (frameTime > advanceFrame * 3)
                        frame = 2;
                    else if (frameTime > advanceFrame * 2)
                        frame = 0;
                    else if (frameTime > advanceFrame)
                        frame = 1;
                    else
                        frame = 0;
                }
            }
        }
    }
}