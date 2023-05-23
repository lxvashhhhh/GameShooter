using System;
using Timer = System.Windows.Forms.Timer;
using GameProject.Entities;
using GameProject.Entities.Enemies;
using GameProject.Physics;

namespace GameProject.Domain
{
    internal class SpawnManager
    {
        private readonly Random _r;

        private static Timer? _enemySpawner;
        private static Timer? _boosterSpawner;

        private static int _enemiesLimit;
        private static int _boostersLimit;

        internal SpawnManager()
        {
            _enemiesLimit = 6;
            _boostersLimit = 10;

            _r = new Random();

            _enemySpawner = new Timer();
            _enemySpawner.Interval = 3 * 1000;
            _enemySpawner.Tick += (s,a) =>
                SpawnEnemy((EnemyTypes)_r.Next(3), GetValidSpawnLocation());
            _enemySpawner.Start();

            _boosterSpawner = new Timer();
            _boosterSpawner.Interval = 4 * 1000;
            _boosterSpawner.Tick += (s, a) =>
                SpawnBooster((BoosterTypes)_r.Next(3), GetValidSpawnLocation());
            _boosterSpawner.Start();
        }

        internal static void StopTimers()
        {
            _enemySpawner?.Stop();
            _boosterSpawner?.Stop();
        }
        internal static void StartTimers()
        {
            _enemySpawner?.Start();
            _boosterSpawner?.Start();
        }
        private static void SpawnEnemy(EnemyTypes enemyType, Vector location)
        {
            if (!CanSpawnEnemy()) return;
            switch (enemyType)
            {
                case EnemyTypes.SmallEnemy:
                    var smallZombie = new SmallZombie(location);
                    Game.SpawnedEnemies.Add(smallZombie);
                    break;

                case EnemyTypes.MediumZombie:
                    var mediumZombie = new MediumZombie(location);
                    Game.SpawnedEnemies.Add(mediumZombie);
                    break;

                case EnemyTypes.HeavyZombie:
                    var heavyZombie = new HeavyZombie(location);
                    Game.SpawnedEnemies.Add(heavyZombie);
                    break;
            }
        }
        private static void SpawnBooster(BoosterTypes booster, Vector location)
        {
            if (!CanSpawnBooster()) return;
            switch (booster)
            {
                case BoosterTypes.HealthBoost:
                    var healthBoost = new HealthBooster(location);
                    Game.SpawnedBoosters.Add(healthBoost);
                    break;

                case BoosterTypes.DamageBoost:
                    var damageBoost = new DamageBooster(location);
                    Game.SpawnedBoosters.Add(damageBoost);
                    break;

                case BoosterTypes.SpeedBoost:
                    var speedBoost = new SpeedBooster(location);
                    Game.SpawnedBoosters.Add(speedBoost);
                    break;
            }
        }

        private Vector GetValidSpawnLocation()
        {
            var result = new Vector(Game.Player.Hitbox.Location);

            while (!InSpawnZone(result))
            {
                var randomLocationX = _r.Next(Game.GameZone.X + Game.Player.Hitbox.Size.Width,
                    Game.GameZone.Right - Game.Player.Hitbox.Size.Width);

                var randomLocationY = _r.Next(Game.GameZone.Y + Game.Player.Hitbox.Size.Height,
                    Game.GameZone.Bottom - Game.Player.Hitbox.Size.Height);

                result = new Vector(randomLocationX, randomLocationY);
            }

            return result;
        }
        private static bool InSpawnZone(Vector location)
        {
            return Game.InBounds(location) && !View.ViewedZone.Contains(location.ToPoint());
        }

        private static bool CanSpawnEnemy()
        {
            return Game.SpawnedEnemies.Count < _enemiesLimit;
        }
        private static bool CanSpawnBooster()
        {
            return Game.SpawnedBoosters.Count < _boostersLimit;
        }
    }
}
