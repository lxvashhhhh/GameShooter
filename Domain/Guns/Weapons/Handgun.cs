using System;
using GameProject.Domain.Guns.Bullets;
using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Domain.Weapons
{
    internal class Handgun : Weapon
    {
        internal Handgun()
        {
            Type = WeaponTypes.Handgun;
            MaxAmmo = int.Parse(Resources.HandgunAmmo);
            Ammo = MaxAmmo;
            Damage = int.Parse(Resources.HandgunDamage);
        }

        internal override void Shoot(float angle)
        {
            var playerCenter = Game.Player.GetHitboxCenter();
            var bulletLocation = new Vector(
                playerCenter.X + Game.Player.Hitbox.Width / 2 * Math.Cos(angle),
                playerCenter.Y + Game.Player.Hitbox.Height / 2 * Math.Sin(angle));
            var bullet = new HandgunBullet(bulletLocation, angle);
            Game.SpawnedBullets.Add(bullet);
            Ammo--;
            Recoil = MainForm.MainTimer.Interval * int.Parse(Resources.HandgunRecoil);
        }

        internal override void Reload()
        {
            Recoil = MainForm.MainTimer.Interval * int.Parse(Resources.HandgunReload);
            IsReloading = true;
        }
    }
}
