using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class GeoBullet : AbstractBullet
    {

        private Point origin;
        private bool shoot;
        private float rotation;
        private const float ROTATION_SPEED = 0.2f;

        private KeyboardState prevKeyboardState;

        public GeoBullet(int x, int y, bool facingRight) : base(Constant.GEOBULLET, facingRight)
        {

            sprite = BulletAttributes.GetSprites(Constant.GEOBULLET)[0];

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location = new Rectangle(0, 0, drawRect.Width, drawRect.Height);

            if (facingRight)
                origin = new Point(x + 36, - sprite.Height/2);
            else
                origin = new Point(x - 18 - sprite.Width/2, -sprite.Height/2);

            location.X = origin.X;
            location.Y = origin.Y;

            shoot = false;
            rotation = 0;
            prevKeyboardState = Keyboard.GetState();
        }

        protected override void SubUpdate(Rioman player, double deltaTime, Viewport viewport, AbstractEnemy[] enemies)
        {
            if (!shoot && Keyboard.GetState().IsKeyDown(Constant.SHOOT) && !prevKeyboardState.IsKeyDown(Constant.SHOOT))
            {
                shoot = true;
                direction = player.FacingRight() ? 1 : -1;
            }

            if (!shoot)
                location.Y += speed;
            else {
                location.X += speed * direction;
                rotation = MathHelper.WrapAngle(rotation + ROTATION_SPEED);
            }

            prevKeyboardState = Keyboard.GetState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, location, drawRect, Color.White, rotation, new Vector2(drawRect.Width/2, drawRect.Height/2), SpriteEffects.None, 0);
        }

        public override Rectangle GetCollisionRect()
        {
            return location;
        }

        public override bool Hits(Rectangle collisionRect)
        {
            return GetCollisionRect().Intersects(collisionRect);
        }

        public override int TakeDamage(string enemyID)
        {
            if (hitList.Contains(enemyID))
                return 0;

            hitList.Add(enemyID);

            if (isAlive)
                return damage;
            else return 0;
        }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}
