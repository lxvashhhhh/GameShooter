using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Entities
{
    internal class SmallZombie : Enemy
    {
        internal SmallZombie(Vector location) : base(location, Resources.SmallZombie)
        {
            Speed = int.Parse(Resources.SmallZombieSpeed);

            Damage = int.Parse(Resources.SmallZombieDamage);

            MaxHealth = 50 * 1000;
            Health = MaxHealth;

            Coins = int.Parse(Resources.SmallZombieCoins);
        }
    }
}
