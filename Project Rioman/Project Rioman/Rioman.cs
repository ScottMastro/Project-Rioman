using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class Rioman
    {
        public RiomanState state;
        public RiomanAnimation anim;

        public Texture2D sprite;
        public SpriteEffects direction;
        public Texture2D stand;
        Texture2D jumpsprite;
        Texture2D jumpgunsprite;
        Texture2D climb;
        Texture2D climbflip;
        Texture2D climbgun;
        Texture2D airhit;
        Texture2D groundhit;
        Texture2D[] warp = new Texture2D[4];
        public Texture2D climbtop;

        public Rectangle location;


        public Rectangle drawRect;

        Texture2D hit;
        bool ishit;
        Rectangle hitloc;
        Rectangle hitsource;
        int hitframe;
        double hittime;

        public bool iswarping;
        public bool isfalling;
        public bool isinvincible;
        public bool ispaused;
        public bool touchedground;

        public double climbtime;
        public double falltime;
        public double jumptime;
        public double invincibletime;
        public double pausetime;
        public double touchtime;
        public int invincibledirection;

        public bool stopx;
        public bool climbdown;
        public int warpy;

        private KeyboardState previousKeyboardState;

        public Rioman(ContentManager content)
        {
            state = new RiomanState();
            anim = new RiomanAnimation(content, state);


            stand = content.Load<Texture2D>("Video\\rioman\\riomanstand");
            jumpsprite = content.Load<Texture2D>("Video\\rioman\\riomanjump");
            jumpgunsprite = content.Load<Texture2D>("Video\\rioman\\riomanjumpgun");
            climb = content.Load<Texture2D>("Video\\rioman\\riomanclimb");
            climbgun = content.Load<Texture2D>("Video\\rioman\\riomanclimbgun");
            climbtop = content.Load<Texture2D>("Video\\rioman\\riomanclimbtop");
            climbflip = content.Load<Texture2D>("Video\\rioman\\riomanclimbflop");
            airhit = content.Load<Texture2D>("Video\\rioman\\hitair");
            groundhit = content.Load<Texture2D>("Video\\rioman\\hitground");
            hit = content.Load<Texture2D>("Video\\rioman\\hit");
            direction = SpriteEffects.None;

            for (int i = 1; i <= 4; i++)
                warp[i-1] = content.Load<Texture2D>("Video\\rioman\\warp" + i.ToString());

            sprite = stand;

            drawRect = new Rectangle(0, 0, sprite.Width, sprite.Height);
            location = new Rectangle(70, 400, sprite.Width, sprite.Height);

            Reset();
        }

        public void Reset()
        {

            isfalling = false;
            isinvincible = false;
            touchedground = false;
            stopx = false;

            direction = SpriteEffects.None;
            falltime = 0;
            jumptime = 0;
            invincibletime = 0;
            hittime = 0;
            pausetime = 0;
            touchtime = 0;
        }

        public Texture2D GetSprite() { return anim.GetSprite(); }

        public void Update(double deltaTime, Level level)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (ispaused)
                Pause(deltaTime);
            else
            {

                state.Update(deltaTime);
                anim.Update(deltaTime);

                if (!isfalling)
                    falltime = 0;

                if (state.IsClimbing())
                    Climb(deltaTime, keyboardState, level);
                else if (state.IsJumping())
                    Jump(deltaTime, keyboardState, level);
                else if (isfalling)
                    Fall(deltaTime, keyboardState, level);
               else if (state.IsRunning() || state.IsStanding())
                    Stand(keyboardState, level);

                if (!state.IsClimbing())
                    sprite = GetSprite();

                if (isinvincible)
                    Invincible(deltaTime);

                if (ishit)
                    Hit(deltaTime);
            }

            previousKeyboardState = keyboardState;
        }













        private void Stand(KeyboardState keyboardState, Level level)
        {
            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            CheckJump(keyboardState, level);

        }

        private void Climb(double deltaTime, KeyboardState keyboardState, Level level)
        {
            CheckJump(keyboardState, level);

            if (state.IsShooting() && sprite != climbtop)
                sprite = climbgun;
            else if (climbtime > 0.175)
                sprite = climbflip;
            else if (sprite != climbtop)
                sprite = climb;

            if (climbtime > 0.35)
                climbtime = 0;


            if (keyboardState.IsKeyDown(Constant.UP))
            {
                Move(0, -3);
                climbtime += deltaTime;
                climbdown = false;
            }
            else if (keyboardState.IsKeyDown(Constant.DOWN))
            {
                Move(0, 3);
                climbtime += deltaTime;
                climbdown = true;
            }

            if (keyboardState.IsKeyDown(Constant.LEFT))
                direction = SpriteEffects.FlipHorizontally;
            if (keyboardState.IsKeyDown(Constant.RIGHT))
                direction = SpriteEffects.None;

            if (keyboardState.IsKeyDown(Constant.JUMP) && !previousKeyboardState.IsKeyDown(Constant.JUMP))
            {
                state.Jump();
                jumptime = 0.2;
                climbtime = 0;
            }
        }

        private void Fall(double deltaTime, KeyboardState keyboardState, Level level)
        {

            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            falltime += deltaTime;

            if (falltime * 30 > 10)
                Move(0, 10);
            else
                Move(0, Convert.ToInt32(falltime * 30));

            touchedground = false;
        }

        private void Jump(double deltaTime, KeyboardState keyboardState, Level level)
        {

            HorizontalMovement(keyboardState, level);
            CheckClimb(keyboardState, level);

            if (!keyboardState.IsKeyDown(Constant.JUMP) && jumptime > 0.1)
            {
                jumptime = 0;
                isfalling = true;
                state.Fall();
            }

            jumptime += deltaTime;
            if (jumptime < 0.4)
            {
                Move(0, -Convert.ToInt32(10 - jumptime * 22));

            }
            else
            {
                isfalling = true;
                state.Fall();
                jumptime = 0;
            }

            touchedground = false;
        }

        public void Pause(double deltaTime)
        {
            pausetime += deltaTime;

            if (pausetime > 3)
            {
                pausetime = 0;
                ispaused = false;
            }
        }


        public void BackwardScroll(Level level, Viewport vpr)
        {
            if (level.stoprightscreenmovement)
            {
                if (location.X < vpr.Width / 2)
                    level.stoprightscreenmovement = false;
            }

            if (level.stopleftscreenmovement)
            {
                if (location.X > vpr.Width / 2)
                    level.stopleftscreenmovement = false;
            }
        }

        public void Shooting(Bullet[] bullet)
        {
            if (!stopx && !ispaused)
            {
                bool flag = false;
                int num = 0;

                for (int i = 0; i <= 2; i++)
                {
                    if (!bullet[i].alive && sprite != climbtop)
                    {
                        flag = true;
                        num = i;
                    }
                }

                if (flag)
                {
                    Audio.shoot1.Play(0.5f, 0f, 0f);
                    state.Shoot();
                    if (direction == SpriteEffects.FlipHorizontally)
                        bullet[num].BulletSpawn(location.X - 20, location.Center.Y - 8, direction);
                    else
                        bullet[num].BulletSpawn(location.X + 20, location.Center.Y - 8, direction);
                }
            }
        }

        private void HorizontalMovement(KeyboardState keyboardState, Level level)
        {
            if (keyboardState.IsKeyDown(Constant.RIGHT) && !level.stoprightxmovement)
            {
                if (!level.stopleftscreenmovement && !level.stoprightscreenmovement)
                    level.MoveStuff(-3, 0);
                else if (level.stoprightscreenmovement || level.stopleftscreenmovement)
                    Move(3, 0);

                direction = SpriteEffects.None;
            }
            else if (keyboardState.IsKeyDown(Constant.LEFT) && !level.stopleftxmovement)
            {
                if (!level.stopleftscreenmovement && !level.stoprightscreenmovement)
                    level.MoveStuff(3, 0);
                else if (level.stopleftscreenmovement || level.stoprightscreenmovement)
                    Move(-3, 0);

                direction = SpriteEffects.FlipHorizontally;
            }
        }

        private void CheckJump(KeyboardState keyboardState, Level level)
        {
            if (keyboardState.IsKeyDown(Constant.JUMP) && !previousKeyboardState.IsKeyDown(Constant.JUMP))
            {
                climbtime = 0;
                Move(0, -4);
                state.Jump();
            }

        }

        private void CheckClimb(KeyboardState keyboardState, Level level)
        {

            if (keyboardState.IsKeyDown(Constant.UP))
            {
                Rectangle result = level.CheckClimb(location, location.X, true, ref location);

                if (result.Width != 0)
                {
                    Move(0, -10);
                    state.Climb();
                    jumptime = 0;
                    falltime = 0;
                    isfalling = false;
                    climbdown = false;
                }
            }


            if (keyboardState.IsKeyDown(Constant.DOWN) && !previousKeyboardState.IsKeyDown(Constant.DOWN))
            {
                Rectangle result = level.CheckClimb(location, location.X, false, ref location);
                if (result.Width != 0)
                {
                    sprite = climbtop;
                    Move(0, 20);
                    state.Climb();
                    isfalling = false;
                    climbdown = true;
                }
            }

        }

        public void Hit()
        {
            ishit = true;
            ispaused = false;
            pausetime = 0;
            stopx = true;
            isinvincible = true;
            jumptime = 0;
            isfalling = true;
            touchedground = false;
            touchtime = 0;
            Move(0, -8);
        }

        private void Hit(double deltaTime)
        {
            hitloc = new Rectangle(location.X - hitloc.Width / 2, location.Center.Y - hitloc.Height / 2, hit.Width / 3, hit.Height);
            hittime += deltaTime;

            if (hittime > 0.05 && hitframe == 3)
            {
                hittime = 0;
                hitframe = 0;
                ishit = false;
            }

            if (hittime > 0.05)
            {
                hittime = 0;
            }
        }

        public void DrawHit(SpriteBatch spriteBatch)
        {
            if (ishit)
                spriteBatch.Draw(hit, hitloc, hitsource, new Color(1f, 1f, 1f, 0.8f), 0f, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        private void Invincible(double elapsetime)
        {
            invincibletime += elapsetime;
            stopx = false;

            if (invincibletime < 0.5 && !touchedground)
            {
                stopx = true;
                location.X += invincibledirection;
                Move(0, -4);

            }

            if (isfalling && !touchedground)
            {
                stopx = true;
                sprite = airhit;

            }
            else if (touchedground && touchtime < 0.3)
            {
                stopx = true;
                touchtime += elapsetime;
                sprite = groundhit;

            }

            for (double i = 0; i <= 2; i += 0.2)
                if (invincibletime > i && invincibletime < i + 0.1)
                    location.Width = 0;

            if (invincibletime > 2)
            {
                stopx = false;
                invincibletime = 0;
                touchtime = 0;
                isinvincible = false;
            }

            if (state.IsClimbing())
            {
                stopx = false;
                touchedground = true;
                touchtime = 10;
            }
        }


        public void Warp()
        {
            if (ishit)
            {
                ishit = false;
                hittime = 0;
                hitframe = 0;
            }

            if (warpy > 0)
            {
                if (Audio.activesoundeffect == null || Audio.activesoundeffect.State != SoundState.Playing)
                {
                    Audio.activesoundeffect = Audio.pickup.CreateInstance();
                    Audio.activesoundeffect.Volume = 0.5f;
                    Audio.activesoundeffect.Play();
                }

                if (sprite != warp[1] && sprite != warp[2] && sprite != warp[3])
                    sprite = warp[0];



                Move(0, 15);

                if (location.Y >= warpy)
                {
                    MoveToY(warpy);

                    if (sprite == warp[0])
                        sprite = warp[1];
                    else if (sprite == warp[1])
                        sprite = warp[2];
                    else if (sprite == warp[2])
                        sprite = warp[3];
                    else if (sprite == warp[3])
                    {
                        Reset();
                        iswarping = false;
                    }


                }
            }

            if (warpy < 0)
            {

                if (sprite != warp[0] && sprite != warp[1] && sprite != warp[2] && sprite != warp[3])
                    sprite = warp[3];
                else if (sprite == warp[3])
                    sprite = warp[2];
                else if (sprite == warp[2])
                    sprite = warp[1];
                else if (sprite == warp[1])
                    sprite = warp[0];



                if (sprite == warp[0])
                {
                    sprite = warp[0];


                    Move(0, -15);

                    if (location.Y <= warpy)
                    {
                        Reset();
                        iswarping = false;
                    }
                }
            }
        }

        public Rectangle Left { get { return new Rectangle(location.X, location.Y + 6, 10, 34); ; } }
        public Rectangle Head { get { return new Rectangle(location.X + 10, location.Y, 36, 32); ; } }
        public Rectangle Feet { get { return new Rectangle(location.X + 8, location.Y + 42, 40, 12); ; } }
        public Rectangle Right { get { return new Rectangle(location.X +location.Width -10, location.Y + 6, 10, 34); ; } }

        public void Move(int x, int y)
        {
            location.X += x;
            location.Y += y;
        }

        public void MoveToX(int x)
        {
            location.X = x;
        }

        public void MoveToY(int y)
        {
            location.Y = y;
        }

    }
}