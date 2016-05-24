using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Rioman
{
    class DisappearTile : AbstractTile
    {
        protected double fadeTime;

        public DisappearTile(int ID, int x, int y) : base(ID, x, y)
        {
            animate = false;
        }

        protected override void SubDraw(SpriteBatch spriteBatch) { }
        protected sealed override void SubReset() { }

        protected override void SubUpdate(Rioman player, double deltaTime)
        {
            fadeTime -= deltaTime;

            if (fadeTime <= -5)
            {
                sprite = frames[0];
                fadeTime = 1.5;
                Move(0, 100000);
            }

            if (fadeTime < 0 && location.Y > 0)
                Move(0, -100000);

            if (fadeTime <= 1)
                sprite = frames[1];

            if (fadeTime <= 0.5)
                sprite = frames[2];
        }

        public void SetFadeTime(double x) { fadeTime = x; }

        protected override void SubMove(int x, int y)
        {
            //do nothing
        }
    }
}
