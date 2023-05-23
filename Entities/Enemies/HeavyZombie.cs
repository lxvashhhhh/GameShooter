using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Entities.Enemies
{
    internal class HeavyZombie : Enemy
    {
        internal HeavyZombie(Vector location) : base(location, Resources.HeavyZombie)
        {
            Speed = int.Parse(Resources.HeavyZombieSpeed);

            Damage = int.Parse(Resources.HeavyZombieDamage);

            MaxHealth = 200 * 1000;
            Health = MaxHealth;

            Coins = int.Parse(Resources.HeavyZombieCoins);
        }
    }
}
