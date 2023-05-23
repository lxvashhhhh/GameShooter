using GameProject.Domain.Weapons;
using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Domain.Guns.Bullets
{
    internal class HandgunBullet : Bullet
    {
        internal HandgunBullet(Vector location, float angle) : base(location, Resources.HandgunBullet, angle)
        {
            Speed = int.Parse(Resources.HandgunBulletSpeed);
        }
    }
}
