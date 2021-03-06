﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Project_Rioman
{
    static class Audio
    {
        public static SoundEffectInstance activesong;
        public static SoundEffectInstance activesoundeffect;

        public static SoundEffect titlescreen;
        public static SoundEffect stageselect;
        public static SoundEffect auroraman;
        public static SoundEffect bunnyman;
        public static SoundEffect cloverman;
        public static SoundEffect geogirl;
        public static SoundEffect infernoman;
        public static SoundEffect lurkerman;
        public static SoundEffect posterman;
        public static SoundEffect toxman;
        public static SoundEffect bossbattle;
        public static SoundEffect fortressstage;
        public static SoundEffect fortressbossbattle;
        public static SoundEffect mushbattle1;
        public static SoundEffect mushbattle2;
        public static SoundEffect mushbattle3;

        public static SoundEffect shoot1;
        public static SoundEffect shoot2;
        public static SoundEffect shoot3;
        public static SoundEffect infernoBullet;
        public static SoundEffect killEnemy;
        public static SoundEffect killEnemy2;
        public static SoundEffect killBoss;
        public static SoundEffect heal;
        public static SoundEffect jump1;
        public static SoundEffect jump2;
        public static SoundEffect jump3;
        public static SoundEffect land;
        public static SoundEffect selection;
        public static SoundEffect shieldHit;
        public static SoundEffect die;
        private static SoundEffect warp;
        public static SoundEffect pickup;

        public static void LoadAudio(ContentManager content)
        {
            titlescreen = content.Load<SoundEffect>("Audio\\titlescreen");
            stageselect = content.Load<SoundEffect>("Audio\\stageselect");
            auroraman = content.Load<SoundEffect>("Audio\\auroraman");
            bunnyman = content.Load<SoundEffect>("Audio\\bunnyman");
            cloverman = content.Load<SoundEffect>("Audio\\cloverman");
            geogirl = content.Load<SoundEffect>("Audio\\geogirl");
            infernoman = content.Load<SoundEffect>("Audio\\infernoman");
            lurkerman = content.Load<SoundEffect>("Audio\\lurkerman");
            posterman = content.Load<SoundEffect>("Audio\\posterman");
            toxman = content.Load<SoundEffect>("Audio\\toxman");
            bossbattle = content.Load<SoundEffect>("Audio\\bossbattle");
            fortressstage = content.Load<SoundEffect>("Audio\\fortressstage");
            fortressbossbattle = content.Load<SoundEffect>("Audio\\fortressbossbattle");
            mushbattle1 = content.Load<SoundEffect>("Audio\\mushbattle1");
            mushbattle2 = content.Load<SoundEffect>("Audio\\mushbattle2");
            mushbattle3 = content.Load<SoundEffect>("Audio\\mushbattle3");

            shoot1 = content.Load<SoundEffect>("Audio\\soundeffects\\shoot");
            shoot2 = content.Load<SoundEffect>("Audio\\soundeffects\\shoot2");
            shoot3 = content.Load<SoundEffect>("Audio\\soundeffects\\shoot3");
            infernoBullet = content.Load<SoundEffect>("Audio\\soundeffects\\infernobullet");
            killEnemy = content.Load<SoundEffect>("Audio\\soundeffects\\killenemy");
            killEnemy2 = content.Load<SoundEffect>("Audio\\soundeffects\\killenemy2");
            killBoss = content.Load<SoundEffect>("Audio\\soundeffects\\killboss");
            heal = content.Load<SoundEffect>("Audio\\soundeffects\\heal");
            jump1 = content.Load<SoundEffect>("Audio\\soundeffects\\jump");
            jump2 = content.Load<SoundEffect>("Audio\\soundeffects\\jump2");
            jump3 = content.Load<SoundEffect>("Audio\\soundeffects\\jump3");
            land = content.Load<SoundEffect>("Audio\\soundeffects\\land2");
            selection = content.Load<SoundEffect>("Audio\\soundeffects\\selection");
            shieldHit = content.Load<SoundEffect>("Audio\\soundeffects\\shieldhit");
            die = content.Load<SoundEffect>("Audio\\soundeffects\\die");
            warp = content.Load<SoundEffect>("Audio\\soundeffects\\warp");
            pickup = content.Load<SoundEffect>("Audio\\soundeffects\\powerup");
        }

        public static void ChangeActiveSong(SoundEffect song)
        {
            activesong = song.CreateInstance();
            activesong.IsLooped = true;
            activesong.Play();
        }

        public static void ChangeActiveSong(int part)
        {
            if (activesong != null)
                activesong.Dispose();

            if (part == Constant.TITLE_SCREEN)
                ChangeActiveSong(titlescreen);
            else if (part == Constant.SELECTION_SCREEN)
                ChangeActiveSong(stageselect);
            else if (part == Constant.GEOGIRL)
                ChangeActiveSong(geogirl);
            else if (part == Constant.INFERNOMAN)
                ChangeActiveSong(infernoman);
            else if (part == Constant.TOXICMAN)
                ChangeActiveSong(toxman);
            else if (part == Constant.AURORAMAN)
                ChangeActiveSong(auroraman);
            else if (part == 14)
                ChangeActiveSong(fortressstage);
            else if (part == Constant.CLOVERMAN)
                ChangeActiveSong(cloverman);
            else if (part == Constant.LURKERMAN)
                ChangeActiveSong(lurkerman);
            else if (part == Constant.POSTERMAN)
                ChangeActiveSong(posterman);
            else if (part == Constant.BUNNYMAN)
                ChangeActiveSong(bunnyman);
        }

        public static void PlayWarp()
        {

            if (activesoundeffect == null || activesoundeffect.State != SoundState.Playing)
            {
                activesoundeffect = pickup.CreateInstance();
                activesoundeffect.Volume = Constant.VOLUME;
                activesoundeffect.Play();
            }
        }

        public static void PlayLand()
        {
            land.Play(Constant.VOLUME, 0f, 0f);
        }

        public static void PlayDoor()
        {
            jump2.Play(Constant.VOLUME, -1f, 0f);
        }

        public static void PlayShoot()
        {

            if(Weapons.GetActiveWeapon() == Constant.INFERNOBULLET)
                infernoBullet.Play(Constant.VOLUME, 0f, 0f);
            
            else
                shoot1.Play(Constant.VOLUME, 0f, 0f);
        }

        public static void PlayEnemyShoot1()
        {
            shoot3.Play(Constant.VOLUME, -0.5f, 0f);
        }

        public static void PlayKillEnemy()
        {
            killEnemy2.Play(Constant.VOLUME, 0f, 0f);
        }
        public static void PlayKillBoss()
        {
            killBoss.Play(Constant.VOLUME, 0.1f, 0f);
        }

        public static void PlayShieldHit()
        {
            shieldHit.Play(Constant.VOLUME, 0.5f, 0f);
        }

        public static void PlayRestorePoint()
        {
            heal.Play(Constant.VOLUME, 0.4f, 0f);
        }

        public static void PlayPauseSelection()
        {
            selection.Play(Constant.VOLUME, 1f, 0f);
        }
    }
}