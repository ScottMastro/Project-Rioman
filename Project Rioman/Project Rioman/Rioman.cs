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
        public Texture2D sprite;
        public SpriteEffects direction;
        public Texture2D walksprite;
        Texture2D standgunsprite;
        Texture2D walkgunsprite;
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
        public Rectangle sourcerect;

        int frame;
        double animationtime;
        bool reverseanimation;
        int walkframes;

        Texture2D hit;
        bool ishit;
        Rectangle hitloc;
        Rectangle hitsource;
        int hitframe;
        double hittime;

        public bool iswarping;
        public bool isrunning;
        public bool isshooting;
        public bool isstanding;
        public bool isfalling;
        public bool isjumping;
        public bool isclimbing;
        public bool isinvincible;
        public bool ispaused;
        public bool touchedground;

        public double climbtime;
        public double shoottime;
        public double falltime;
        public double jumptime;
        public double invincibletime;
        public double pausetime;
        public double touchtime;
        public int invincibledirection;

        public bool stopx;
        public bool climbdown;
        public int warpy;

        public Rioman(ContentManager content, Rectangle loc, int wlkframes)
        {
            state = new RiomanState();

            sprite = content.Load<Texture2D>("Video\\rioman\\riomanrun");
            standgunsprite = content.Load<Texture2D>("Video\\rioman\\riomanstandgun");
            walkgunsprite = content.Load<Texture2D>("Video\\rioman\\riomanrungun");
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

            for (int i = 0; i <= 3; i++)
                warp[i] = content.Load<Texture2D>("Video\\rioman\\warp" + (1 + i).ToString());

            walkframes = wlkframes;
            sourcerect = Animation.GetSourceRect(sprite.Width / wlkframes, sprite.Height);
            location = loc;
            walksprite = sprite;

            Reset();
        }

        public void Reset()
        {
            frame = 1;
            animationtime = 0;
            reverseanimation = false;

            isrunning = false;
            isshooting = false;
            isstanding = true;
            isfalling = false;
            isclimbing = false;
            isinvincible = false;
            touchedground = false;
            stopx = false;

            direction = SpriteEffects.None;
            shoottime = 0;
            falltime = 0;
            jumptime = 0;
            invincibletime = 0;
            hittime = 0;
            pausetime = 0;
            touchtime = 0;
        }

        public void Update(double elapsedtime)
        {
            if (ispaused)
                Pause(elapsedtime);
            else
            {
                if (!isfalling)
                    falltime = 0;

                if (isclimbing)
                    Climb(elapsedtime);
                else if (isjumping)
                    Jump(elapsedtime);
                else if (isfalling)
                    Fall(elapsedtime);
                else if (isrunning && !isshooting)
                    Walk(elapsedtime);
                else if (isrunning && isshooting)
                    WalkGun(elapsedtime);
                else if (isshooting && isstanding)
                    StandGun(elapsedtime);
                else
                    Stand(elapsedtime);

                if (isinvincible)
                    Invincible(elapsedtime);

                if (ishit)
                    Hit(elapsedtime);
            }
        }

        public void Pause(double elapsedtime)
        {
            pausetime += elapsedtime;

            if (pausetime > 3)
            {
                pausetime = 0;
                ispaused = false;
            }
        }

        public void Moving(KeyboardState keyboardstate, KeyboardState previouskeyboardstate, Levels level, double elapsedtime)
        {
            isrunning = false;

            if (!stopx && !ispaused)
            {
                if (keyboardstate.IsKeyDown(Keys.Right) && !isclimbing && !level.stoprightxmovement)
                {
                    if (!level.stopleftscreenmovement && !level.stoprightscreenmovement)
                    {
                        isrunning = true;
                        level.MoveStuff(-3, 0);
                    }
                    else if (level.stoprightscreenmovement || level.stopleftscreenmovement)
                    {
                        isrunning = true;
                        location.X += 3;
                    }

                    direction = SpriteEffects.None;
                }
                else if (keyboardstate.IsKeyDown(Keys.Left) && !isclimbing && !level.stopleftxmovement)
                {
                    if (!level.stopleftscreenmovement && !level.stoprightscreenmovement)
                    {
                        isrunning = true;
                        level.MoveStuff(3, 0);
                    }
                    else if (level.stopleftscreenmovement || level.stoprightscreenmovement)
                    {
                        isrunning = true;
                        location.X -= 3;
                    }

                    direction = SpriteEffects.FlipHorizontally;
                }

                if (keyboardstate.IsKeyDown(Keys.Up) && !isclimbing)
                {
                    Rectangle result = level.CheckClimb(location, location.X, true, ref location);

                    if (result.Width != 0)
                    {
                        location.Y -= 10;
                        isclimbing = true;
                        isfalling = false;
                        isjumping = false;
                        climbdown = false;
                    }
                }
                else if (keyboardstate.IsKeyDown(Keys.Up) && isclimbing)
                {
                    location.Y -= 3;
                    climbtime += elapsedtime;
                    climbdown = false;
                }
                else if (keyboardstate.IsKeyDown(Keys.Down) && isclimbing)
                {
                    location.Y += 3;
                    climbtime += elapsedtime;
                    climbdown = true;
                }

                if (keyboardstate.IsKeyDown(Keys.Left) && isclimbing)
                    direction = SpriteEffects.FlipHorizontally;
                if (keyboardstate.IsKeyDown(Keys.Right) && isclimbing)
                    direction = SpriteEffects.None;

                if (keyboardstate.IsKeyDown(Keys.Down) && !isclimbing && !previouskeyboardstate.IsKeyDown(Keys.Down))
                {
                    Rectangle result = level.CheckClimb(location, location.X, false, ref location);
                    if (result.Width != 0)
                    {
                        sprite = climbtop;
                        location.Y += 20;
                        isclimbing = true;
                        isfalling = false;
                        isjumping = false;
                        climbdown = true;
                    }
                }

                if (keyboardstate.IsKeyDown(Keys.X) && !previouskeyboardstate.IsKeyDown(Keys.X) && !isfalling)
                {
                    isclimbing = false;
                    climbtime = 0;
                    location.Y -= 4;
                    isjumping = true;
                }

                if (keyboardstate.IsKeyDown(Keys.X) && !previouskeyboardstate.IsKeyDown(Keys.X) && isclimbing)
                {
                    isjumping = true;
                    jumptime = 0.2;
                    isclimbing = false;
                    climbtime = 0;
                }

                if (!keyboardstate.IsKeyDown(Keys.X) && jumptime > 0.1)
                {
                    isjumping = false;
                    jumptime = 0;
                    isfalling = true;
                }
            }
        }

        public void BackwardScroll(Levels level, Viewport vpr)
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
                    isshooting = true;
                    animationtime = 2;
                    shoottime = 0;
                    if (direction == SpriteEffects.FlipHorizontally)
                        bullet[num].BulletSpawn(location.X - 20, location.Center.Y - 8, direction);
                    else
                        bullet[num].BulletSpawn(location.X + 20, location.Center.Y - 8, direction);
                }
            }
        }

        private void LocationRect()
        {
            location.Width = sourcerect.Width;
            location.Height = sourcerect.Height;
        }

        public void Hit()
        {
            ishit = true;
            ispaused = false;
            pausetime = 0;
            stopx = true;
            isinvincible = true;
            isclimbing = false;
            isjumping = false;
            jumptime = 0;
            isfalling = true;
            touchedground = false;
            animationtime = 0;
            touchtime = 0;
            location.Y -= 8;
        }

        private void Hit(double elapsedtime)
        {
            hitloc = new Rectangle(location.X - hitloc.Width / 2, location.Center.Y - hitloc.Height / 2, hit.Width / 3, hit.Height);
            hittime += elapsedtime;

            if (hittime > 0.05 && hitframe == 3)
            {
                hittime = 0;
                hitframe = 0;
                ishit = false;
            }

            if (hittime > 0.05)
            {
                bool fake = false;
                hittime = 0;
                hitsource = Animation.GetSourceRect(3, ref hitframe, hit.Width, hit.Height, ref fake);
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
                location.Y -= 4;
            }

            if (isfalling && !touchedground)
            {
                stopx = true;
                sprite = airhit;
                sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
                LocationRect();
            }
            else if (touchedground && touchtime < 0.3)
            {
                stopx = true;
                touchtime += elapsetime;
                sprite = groundhit;
                sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
                LocationRect();
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

            if (isclimbing)
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

                sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
                LocationRect();

                location.Y += 15;

                if (location.Y >= warpy)
                {
                    location.Y = warpy;

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

                    sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
                    LocationRect();
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

                sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
                LocationRect();

                if (sprite == warp[0])
                {
                    sprite = warp[0];

                    sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
                    LocationRect();

                    location.Y -= 15;

                    if (location.Y <= warpy)
                    {
                        Reset();
                        iswarping = false;
                    }
                }
            }
        }

        private void Climb(double elapsedtime)
        {
            if (isshooting)
                shoottime += elapsedtime;

            if (isshooting && sprite != climbtop)
                sprite = climbgun;
            else if (climbtime > 0.175)
                sprite = climbflip;
            else if (sprite != climbtop)
                sprite = climb;

            if (climbtime > 0.35)
                climbtime = 0;

            if (shoottime > 0.25)
            {
                isshooting = false;
                shoottime = 0;
            }

            sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
            LocationRect();
        }

        private void Fall(double elapsedtime)
        {
            falltime += elapsedtime;

            if (falltime * 30 > 10)
                location.Y += 10;
            else
                location.Y += Convert.ToInt32(falltime * 30);

            JumpOrFall(elapsedtime);
        }

        private void Jump(double elapsedtime)
        {
            jumptime += elapsedtime;
            if (jumptime < 0.4)
            {
                location.Y -= Convert.ToInt32(10 - jumptime * 22);
            }
            else
            {
                isfalling = true;
                isjumping = false;
                jumptime = 0;
            }

            JumpOrFall(elapsedtime);
        }

        private void JumpOrFall(double elapsedtime)
        {
            if (!isinvincible)
                touchedground = false;

            sprite = jumpsprite;
            frame = 1;

            if (isshooting)
            {
                sprite = jumpgunsprite;
                shoottime += elapsedtime;
            }

            sourcerect = Animation.GetSourceRect(sprite.Width, sprite.Height);
            LocationRect();

            if (shoottime > 0.5)
            {
                isshooting = false;
                shoottime = 0;
            }
        }

        private void Stand(double elapsedtime)
        {
            isstanding = true;
            sprite = walksprite;
            frame = 0;

            reverseanimation = false;
            sourcerect = Animation.GetSourceRect(walkframes, ref frame, walksprite.Width, walksprite.Height, ref reverseanimation);

            LocationRect();
        }

        private void Walk(double elapsedtime)
        {
            isstanding = false;
            sprite = walksprite;

            animationtime += elapsedtime;
            if (animationtime > 0.1)
            {
                sourcerect = Animation.GetSourceRect(walkframes, ref frame, walksprite.Width, walksprite.Height, ref reverseanimation);
                animationtime = 0;
            }

            if (frame == 1)
            {
                sourcerect = Animation.GetSourceRect(walkframes, ref frame, walksprite.Width, walksprite.Height, ref reverseanimation);
                sourcerect = Animation.GetSourceRect(walkframes, ref frame, walksprite.Width, walksprite.Height, ref reverseanimation);
            }

            LocationRect();
        }

        private void StandGun(double elapsedtime)
        {
            isstanding = true;
            sprite = standgunsprite;
            sourcerect = Animation.GetSourceRect(standgunsprite.Width, standgunsprite.Height);

            LocationRect();
            shoottime += elapsedtime;

            if (shoottime > 0.5)
            {
                isshooting = false;
                shoottime = 0;
            }
        }

        private void WalkGun(double elapsedtime)
        {
            sprite = walkgunsprite;

            animationtime += elapsedtime;
            shoottime += elapsedtime;

            if (animationtime > 0.1)
            {
                sourcerect = Animation.GetSourceRect(4, ref frame, walkgunsprite.Width, walkgunsprite.Height, ref reverseanimation);
                animationtime = 0;
            }

            if (shoottime > 0.5)
            {
                isshooting = false;
                shoottime = 0;
                animationtime = 2;
            }

            LocationRect();
        }
    }
}