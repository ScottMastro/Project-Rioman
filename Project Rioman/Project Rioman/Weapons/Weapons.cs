using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    static class Weapons
    {
        private static Texture2D[] weapon = new Texture2D[12];
        private static Texture2D[] weaponPoint = new Texture2D[12];
        private static bool[] weaponHave = new bool[12];
        private static int[] weaponAmmo = new int[12];

        private static Texture2D pauseSelector;
        private static int activeWeapon;

        private static Texture2D pauseBackground;
        private static Texture2D ammoMeter;

        private static Texture2D tank;
        private static int eTanks;
        private static int wTanks;

        private static KeyboardState previousKeyboardState;


        private static Texture2D[] lives = new Texture2D[10];

        public static void LoadContent(ContentManager content)
        {
            pauseSelector = content.Load<Texture2D>("Video\\pause\\pauseselector");

            activeWeapon = 0;
            weaponHave[0] = true;

            for (int i = 0; i <= 11; i++)
            {
                weaponHave[i] = true;
                weaponAmmo[i] = 27;
            }

            eTanks = 4;
            wTanks = 4;

            for (int i = 0; i <= 11; i++)
            {
                weapon[i] = content.Load<Texture2D>("Video\\pause\\weapon" + i.ToString());
                weaponPoint[i] = content.Load<Texture2D>("Video\\pause\\weapon" + i.ToString() + "ammo");
            }

            pauseBackground = content.Load<Texture2D>("Video\\pause\\PauseMenuBackground");
            ammoMeter = content.Load<Texture2D>("Video\\pause\\ammometer");
            tank = content.Load<Texture2D>("Video\\pause\\tank");

            for (int i = 0; i <= 5; i++)
                lives[i] = content.Load<Texture2D>("Video\\pause\\" + i.ToString());

            previousKeyboardState = new KeyboardState();
        }

        public static void DrawPause(SpriteBatch spriteBatch, int numLives)
        {
            spriteBatch.Draw(pauseBackground, new Vector2(50, 50), Color.White);

            int counter = 0;
            for (int i = 0; i <= 11; i++)
            {
                if (weaponHave[i])
                {
                    if (counter % 2 == 0)
                    {
                        spriteBatch.Draw(weapon[i], new Vector2(70, counter * 17 + 67), Color.White);

                        spriteBatch.Draw(ammoMeter, new Vector2(98 + weaponPoint[i].Width, (counter - 1) * 17 + 92), Color.White);

                        for (int j = 1; j <= weaponAmmo[i]; j++)
                            spriteBatch.Draw(weaponPoint[i], new Vector2(100 + (weaponPoint[i].Width - 2) * j,
                                (counter - 1) * 17 + 92), Color.White);

                        if (activeWeapon == i)
                            spriteBatch.Draw(pauseSelector, new Vector2(70, counter * 17 + 67), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(weapon[i], new Vector2(255, (counter - 1) * 17 + 67), Color.White);

                        spriteBatch.Draw(ammoMeter, new Vector2(283 + weaponPoint[i].Width, (counter - 1) * 17 + 75), Color.White);

                        for (int j = 1; j <= weaponAmmo[i]; j++)
                            spriteBatch.Draw(weaponPoint[i], new Vector2(285 + (weaponPoint[i].Width - 2) * j,
                                (counter - 1) * 17 + 75), Color.White);

                        if (activeWeapon == i)
                            spriteBatch.Draw(pauseSelector, new Vector2(255, (counter - 1) * 17 + 67), Color.White);
                    }

                    counter++;
                }
            }

            if (numLives <= 5)
                spriteBatch.Draw(lives[numLives], new Vector2(370, 287), Color.White);

            for (int i = 1; i <= eTanks; i++)
                spriteBatch.Draw(tank, new Vector2(90 + 30 * i, 274), Color.White);

            for (int i = 1; i <= wTanks; i++)
                spriteBatch.Draw(tank, new Vector2(90 + 30 * i, 300), Color.White);

        }

        public static AbstractBullet CreateBullet(int totalWeight, int x, int y, bool facingRight)
        {

            if (totalWeight + BulletAttributes.GetWeight(activeWeapon) > Constant.MAX_BULLET)
                return null;

            if (weaponAmmo[activeWeapon] < BulletAttributes.GetAmmoUse(activeWeapon))
                return null;

            if (activeWeapon == Constant.RIOBULLET)
                return new RioBullet(x, y, facingRight);

            weaponAmmo[activeWeapon] = weaponAmmo[activeWeapon] - BulletAttributes.GetAmmoUse(activeWeapon);

            if (activeWeapon == Constant.INFERNOBULLET)
                return new InfernoBullet(x, y, facingRight);
            else if (activeWeapon == Constant.AURORABULLET)
                return new AuroraBullet(x, y, facingRight);
            else if (activeWeapon == Constant.CLOVERBULLET)
                return new CloverBullet(x, y, facingRight);
            else if (activeWeapon == Constant.GEOBULLET)
                return new GeoBullet(x, y, facingRight);
            else if (activeWeapon == Constant.LURKERBULLET)
                return null;
            else if (activeWeapon == Constant.POSTERBULLET)
                return new PosterBullet(x, y, facingRight);
            else if (activeWeapon == Constant.TOXICBULLET)
                return new ToxicBullet(x, y, facingRight);
            else if (activeWeapon == Constant.BUNNYBULLET)
                return new BunnyBullet(x, y, facingRight);
            return null;
        }



        public static void ChangeActiveWeapon(Rioman player)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            int tempactive = activeWeapon;

            int jump = 0;
            int pos = activeWeapon;

            if (keyboardState.IsKeyDown(Constant.UP) && !previousKeyboardState.IsKeyDown(Constant.UP))
                jump = -2;
            if (keyboardState.IsKeyDown(Constant.DOWN) && !previousKeyboardState.IsKeyDown(Constant.DOWN))
                jump = 2;
            if (keyboardState.IsKeyDown(Constant.LEFT) && !previousKeyboardState.IsKeyDown(Constant.LEFT))
                jump = -1;
            if (keyboardState.IsKeyDown(Constant.RIGHT) && !previousKeyboardState.IsKeyDown(Constant.RIGHT))
                jump = 1;

            while (jump != 0)
            {
                pos += jump / Math.Abs(jump);

                if (pos < 0 || pos > 11)
                    break;

                if (weaponHave[pos])
                {
                    if (jump == 2)
                        jump = 1;
                    else if (jump == -2)
                        jump = -1;
                    else {
                        activeWeapon = pos;
                        break;
                    }
                }
            }


            if (activeWeapon != tempactive)
            {
                Audio.PlayPauseSelection();
                player.StopLurking();
            }

            previousKeyboardState = keyboardState;
        }

        public static Texture2D GetAmmoPoint()
        {
            return weaponPoint[activeWeapon];
        }
        public static int GetAmmo()
        {
            return weaponAmmo[activeWeapon];
        }
        public static int GetAmmo(int value)
        {
            return weaponAmmo[value];
        }
        public static int GetActiveWeapon()
        {
            return activeWeapon;
        }

        public static void Lurk()
        {
            weaponAmmo[Constant.LURKERBULLET] -= BulletAttributes.GetAmmoUse(Constant.LURKERBULLET);
        }


        public static Color GetRiomanColour()
        {
            return BulletAttributes.GetRiomanColour(activeWeapon);
        }

    }
}