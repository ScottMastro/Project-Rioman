using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Rioman
{
    class Weapons
    {
        public Texture2D[] weapon = new Texture2D[12];
        public Texture2D[] power = new Texture2D[12];
        public bool[] weaponhave = new bool[12];
        public int[] weaponammo = new int[12];

        public Texture2D pauseselector;
        public int activeweapon;

        public Texture2D pausebackground;
        public Texture2D ammometer;

        public Texture2D tank;
        public int etanks;
        public int wtanks;

        public Texture2D[] lives = new Texture2D[10];

        public Weapons(ContentManager content)
        {
            pauseselector = content.Load<Texture2D>("Video\\pause\\pauseselector");

            activeweapon = 0;
            weaponhave[0] = true;
            weaponammo[0] = 27;

            etanks = 4;
            wtanks = 4;

            for (int i = 0; i <= 11; i++)
            {
                weapon[i] = content.Load<Texture2D>("Video\\pause\\weapon" + i.ToString());
                power[i] = content.Load<Texture2D>("Video\\pause\\weapon" + i.ToString() + "ammo");
            }

            pausebackground = content.Load<Texture2D>("Video\\pause\\PauseMenuBackground");
            ammometer = content.Load<Texture2D>("Video\\pause\\ammometer");
            tank = content.Load<Texture2D>("Video\\pause\\tank");

            for (int i = 0; i <= 5; i++)
                lives[i] = content.Load<Texture2D>("Video\\pause\\" + i.ToString());
        }

        public void DrawPause(SpriteBatch spriteBatch, int lvs)
        {
            spriteBatch.Draw(pausebackground, new Vector2(50, 50), Color.White);

            int loc = 0;
            for (int i = 0; i <= 11; i++)
            {
                if (weaponhave[i])
                {
                    if (loc % 2 == 0)
                    {
                        spriteBatch.Draw(weapon[i], new Vector2(70, loc * 17 + 67), Color.White);

                        spriteBatch.Draw(ammometer, new Vector2(98 + power[i].Width, (loc - 1) * 17 + 92), Color.White);

                        for (int z = 1; z <= weaponammo[i]; z++)
                            spriteBatch.Draw(power[i], new Vector2(100 + (power[i].Width - 2) * z,
                                (loc - 1) * 17 + 92), Color.White);

                        if (activeweapon == i)
                            spriteBatch.Draw(pauseselector, new Vector2(70, loc * 17 + 67), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(weapon[i], new Vector2(255, (loc - 1) * 17 + 67), Color.White);

                        spriteBatch.Draw(ammometer, new Vector2(283 + power[i].Width, (loc - 1) * 17 + 75), Color.White);

                        for (int z = 1; z <= weaponammo[i]; z++)
                            spriteBatch.Draw(power[i], new Vector2(285 + (power[i].Width - 2) * z,
                                (loc - 1) * 17 + 75), Color.White);

                        if (activeweapon == i)
                            spriteBatch.Draw(pauseselector, new Vector2(255, (loc - 1) * 17 + 67), Color.White);
                    }

                    loc++;
                }
            }

            if (lvs <= 9)
                spriteBatch.Draw(lives[lvs], new Vector2(370, 287), Color.White);

            for (int i = 1; i <= etanks; i++)
                spriteBatch.Draw(tank, new Vector2(90 + 30 * i, 274), Color.White);

            for (int i = 1; i <= wtanks; i++)
                spriteBatch.Draw(tank, new Vector2(90 + 30 * i, 300), Color.White);

        }

        public void ChangeActiveWeapon(KeyboardState keyboardstate, KeyboardState previouskeyboardstate)
        {
            int tempactive = activeweapon;

            if (keyboardstate.IsKeyDown(Keys.Up) && !previouskeyboardstate.IsKeyDown(Keys.Up))
            {
                int count = 1;

                while (true)
                {
                    if (activeweapon - count < 0)
                        break;

                    if (weaponhave[activeweapon - count])
                    {
                        activeweapon -= count;
                        break;
                    }

                    count++;
                }
            }

            if (keyboardstate.IsKeyDown(Keys.Down) && !previouskeyboardstate.IsKeyDown(Keys.Down))
            {
                int count = 1;

                while (true)
                {
                    if (activeweapon + count > 11)
                        break;

                    if (weaponhave[activeweapon + count])
                    {
                        activeweapon += count;
                        break;
                    }

                    count++;
                }
            }

            if (activeweapon != tempactive)
                Audio.selection.Play(0.5f, 1f, 0f);
        }
    }
}