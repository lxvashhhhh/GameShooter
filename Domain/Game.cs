using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameProject.Domain.Weapons;
using GameProject.Entities;
using GameProject.Physics;

namespace GameProject.Domain
{
    internal class Game
    {
        internal Form Form { get; set; }
        internal static Player Player { get; set; }
        internal static Rectangle GameZone { get; private set; }
        internal static Rectangle CameraZone { get; private set; }
        internal static GameStage Stage { get; private set; } = GameStage.NotStarted;
        internal static event Action<GameStage> StageChanged;
        internal static SpawnManager SpawnManager;
        internal static KillManager KillManager;
        internal static BoosterManager BoosterManager;
        internal static List<Enemy> SpawnedEnemies { get; set; }
        internal static List<Booster> SpawnedBoosters { get; set; }
        internal static List<Bullet> SpawnedBullets { get; set; }
        internal static List<WeaponTypes> AvailableWeapons { get; set; }
        internal static int Coins { get; set; }

        internal Game(Player player, Rectangle gameZone, Form form)
        {
            Form = form;
            Player = player;
            GameZone = gameZone;
            
            var cameraZoneLocation = new Point(
                GameZone.Location.X + (Screen.PrimaryScreen.WorkingArea.Width - Player.Hitbox.Size.Width) / 2,
                GameZone.Location.Y + (Screen.PrimaryScreen.WorkingArea.Height - Player.Hitbox.Size.Height) / 2);

            var cameraZoneSize = new Size(
                GameZone.Width - Screen.PrimaryScreen.WorkingArea.Width,
                GameZone.Height - Screen.PrimaryScreen.WorkingArea.Height);

            CameraZone = new Rectangle(cameraZoneLocation, cameraZoneSize);

            SpawnedEnemies = new List<Enemy>();
            SpawnedBoosters = new List<Booster>();
            SpawnedBullets = new List<Bullet>();
            AvailableWeapons = new List<WeaponTypes>();

            SpawnManager = new SpawnManager();
            KillManager = new KillManager();
            BoosterManager = new BoosterManager();

            Pause();
        }

        internal static void ChangeStage(GameStage stage)
        {
            Stage = stage;
            StageChanged?.Invoke(stage);
        }

        internal static void Pause()
        {
            SpawnManager.StopTimers();
            KillManager.StopTimer();
            BoosterManager.StopTimer();
        }
        internal static void Resume()
        {
            SpawnManager.StartTimers();
            KillManager.StartTimer();
            BoosterManager.StartTimer();
        }
        internal static bool InBounds(Vector location)
        {
            return GameZone.Contains(location.ToPoint());
        }
        internal static bool InBounds(Rectangle hitbox)
        {
            return GameZone.Contains(hitbox);
        }

        internal static bool InCameraBoundsX(Rectangle hitbox)
        {
            return hitbox.Location.X > CameraZone.Left && hitbox.Location.X < CameraZone.Right;
        }

        internal static bool InCameraBoundsY(Rectangle hitbox)
        {
            return hitbox.Location.Y > CameraZone.Top && hitbox.Location.Y < CameraZone.Bottom;
        }

        internal static void UpdateAvailableWeapons()
        {
            if (!AvailableWeapons.Contains(Player.Weapon.Type))
            {
                AvailableWeapons.Add(Player.Weapon.Type);
            }
        }
    }
}
