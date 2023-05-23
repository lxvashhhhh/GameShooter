using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Entities.Enemies
{
    internal class MediumZombie : Enemy
    {
        internal MediumZombie(Vector location) : base(location, Resources.MediumZombie)
        {
            Speed = int.Parse(Resources.MediumZombieSpeed);

            Damage = int.Parse(Resources.MediumZombieDamage);

            MaxHealth = 100 * 1000;
            Health = MaxHealth;

            Coins = int.Parse(Resources.MediumZombieCoins);
        }
    }
}
