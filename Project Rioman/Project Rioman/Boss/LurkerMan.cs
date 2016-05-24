using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class LurkerMan : AbstractBoss
    {
        private Texture2D defaultSprite;


        public LurkerMan(int x, int y) : base(Constant.LURKERMAN, x, y)
        {
            Texture2D[] sprites = BossAttributes.GetSprites(Constant.LURKERMAN);
            defaultSprite = sprites[1];

            sprite = defaultSprite;

            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);
            location = new Rectangle(x, y, drawRect.Width, drawRect.Height);

            Reset();
        }

        protected override void SubReset()
        {
            sprite = defaultSprite;
            drawRect = new Rectangle(0, 0, sprite.Width / 2, sprite.Height);

        }

        protected override void SubDrawBoss(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, location, drawRect, Color.White, 0f, new Vector2(), direction, 0);

        }

        protected override void SubDrawOther(SpriteBatch spriteBatch)
        {
            //do nothing
        }

        protected override void SubUpdate(Rioman player, AbstractBullet[] rioBullets, double deltaTime, Viewport viewport)
        {


        }

        protected override void SubCheckHit(Rioman player, AbstractBullet[] rioBullets)
        {
            //do nothing
        }

      
        private void Stand()
        {

        }

        public override void DetectTileCollision(AbstractTile tile)
        {

        }

        public override Rectangle GetCollisionRect()
        {
            return new Rectangle(location.X + 10, location.Y, drawRect.Width - 20, drawRect.Height);
        }

        public override Rectangle Feet()
        {
            return new Rectangle(location.X + 10, location.Y + drawRect.Height - 20, drawRect.Width - 20, 20);
        }

        public override Rectangle Head()
        {
            return new Rectangle(location.X + 16, location.Y , drawRect.Width - 32, 26);
        }

        public override Rectangle Left()
        {
            return new Rectangle();
        }

        public override Rectangle Right()
        {
            return new Rectangle();
        }
    }
}
