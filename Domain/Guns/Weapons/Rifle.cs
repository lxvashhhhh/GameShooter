using System;
using GameProject.Domain.Guns.Bullets;
using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Domain.Weapons
{
    internal class Rifle : Weapon
    {
        internal Rifle()
        {
            Type = WeaponTypes.Rifle;
            MaxAmmo = int.Parse(Resources.RifleAmmo);
            Ammo = MaxAmmo;
            Damage = int.Parse(Resources.RifleDamage);
        }
        internal override void Shoot(float angle)
        {
            var playerCenter = Game.Player.GetHitboxCenter();
            var bulletLocation = new Vector(
                playerCenter.X + Game.Player.Hitbox.Width / 2 * Math.Cos(angle),
                playerCenter.Y + Game.Player.Hitbox.Height / 2 * Math.Sin(angle));
            var bullet = new RifleBullet(bulletLocation, angle);
            Game.SpawnedBullets.Add(bullet);
            Ammo--;
            Recoil = MainForm.MainTimer.Interval * int.Parse(Resources.RifleRecoil);
        }

        internal override void Reload()
        {
            Recoil = MainForm.MainTimer.Interval * int.Parse(Resources.RifleReload);
            IsReloading = true;
        }
    }
}
