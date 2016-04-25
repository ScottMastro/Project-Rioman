using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class BunnyMan : AbstractBoss
    {
        private Texture2D defaultSprite;
        private Texture2D shootSprite;

        public BunnyMan(int x, int y) : base(Constant.BUNNYMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.BUNNYMAN);
            defaultSprite = sprites[0];
            shootSprite = sprites[1];

            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);
        }

        protected override void SubReset()
        {
        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), SpriteEffects.None, 0);
            DebugDraw.DrawRect(spriteBatch, new Rectangle(location.X, location.Y, 100, 100), 0.5f);
        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {
        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
        }


        public override void DetectTileCollision(Tile tile)
        {
            //todo
        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }



    }
}
