namespace GameProject.Domain.Weapons
{
    internal abstract class Weapon
    {
        internal bool IsShooting { get; set; }
        internal bool IsReloading { get; set; }
        internal int Recoil { get; set; }
        internal int Ammo { get; set; }
        internal int MaxAmmo { get; set; }
        internal int Damage { get; set; }
        internal WeaponTypes Type { get; set; }

        internal Weapon()
        {

        }

        internal virtual void Shoot(float angle)
        {
        }

        internal virtual void Reload()
        {
        }
    }
}
