using System;
using GameProject.Domain.Guns.Bullets;
using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Domain.Weapons
{
    internal class Shotgun : Weapon
    {
        internal Shotgun()
        {
            Type = WeaponTypes.Shotgun;
            MaxAmmo = int.Parse(Resources.ShotgunAmmo);
            Ammo = MaxAmmo;
            Damage = int.Parse(Resources.ShotgunDamage);
        }
        internal override void Shoot(float angle)
        {
            var r = new Random();
            var playerCenter = Game.Player.GetHitboxCenter();

            for (var i = 0; i < 5; i++)
            {
                var spreadCoefficient = r.NextDouble() * (Math.PI / 5) - Math.PI / 10;

                var bulletLocation = new Vector(
                    playerCenter.X + Game.Player.Hitbox.Width / 2 * Math.Cos(angle + spreadCoefficient),
                    playerCenter.Y + Game.Player.Hitbox.Height / 2 * Math.Sin(angle + spreadCoefficient));

                var bullet = new ShotgunBullet(bulletLocation, angle + (float)spreadCoefficient);
                Game.SpawnedBullets.Add(bullet);
            }

            Recoil = MainForm.MainTimer.Interval * int.Parse(Resources.ShotgunRecoil);
            Ammo--;
        }

        internal override void Reload()
        {
            Recoil = MainForm.MainTimer.Interval * int.Parse(Resources.ShotgunReload);
            IsReloading = true;
        }
    }
}
