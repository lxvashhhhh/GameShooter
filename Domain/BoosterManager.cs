using Timer = System.Windows.Forms.Timer;
using GameProject.Entities;
using GameProject.Properties;

namespace GameProject.Domain
{
    internal class BoosterManager
    {
        private static Timer? _boosterTimer;

        internal BoosterManager()
        {

            _boosterTimer = new Timer();
            _boosterTimer.Interval = 50;
            _boosterTimer.Tick += (sender, args) => CheckBoostersRemainingTime();
            _boosterTimer.Tick += (sender, args) => CheckBoostersIntersections();
            _boosterTimer.Start();
        }

        internal static void StopTimer() => _boosterTimer?.Stop();
        internal static void StartTimer() => _boosterTimer?.Start();
        private static void CheckBoostersRemainingTime()
        {
            var activeBoosters = new Dictionary<BoosterTypes, int>(Game.Player.ActiveBoosters);
            CheckPlayerBoostersRemainingTime(activeBoosters);

            if (Game.SpawnedEnemies.Count == 0) return;
            var spawnedEnemies = new List<Enemy>(Game.SpawnedEnemies);
            CheckEnemiesBoostersRemainingTime(spawnedEnemies);
        }

        private static void CheckPlayerBoostersRemainingTime(Dictionary<BoosterTypes, int> activeBoosters)
        {
            foreach (var boosterTypeRemainingTime in activeBoosters
                         .Where(boosterTypeRemainingTime => boosterTypeRemainingTime.Value > 0))
            {
                switch (boosterTypeRemainingTime.Key)
                {
                    case BoosterTypes.HealthBoost:
                        Game.Player.GetHealthBoost(int.Parse(Resources.HealthBoosterImpact));
                        break;

                    case BoosterTypes.DamageBoost:
                        if (Game.Player.BonusDamage == 0)
                            Game.Player.GetDamageBoost((int)(int.Parse(Resources.DamageBoosterImpact) * 2.5));
                        break;

                    case BoosterTypes.SpeedBoost:
                        if (Game.Player.BonusSpeed == 0)
                            Game.Player.GetSpeedBoost(int.Parse(Resources.SpeedBoosterImpact));
                        break;
                }

                if (_boosterTimer != null)
                    Game.Player.ActiveBoosters[boosterTypeRemainingTime.Key] =
                        boosterTypeRemainingTime.Value - _boosterTimer.Interval;

                if (Game.Player.ActiveBoosters[boosterTypeRemainingTime.Key] == 0)
                {
                    switch (boosterTypeRemainingTime.Key)
                    {
                        case BoosterTypes.DamageBoost:
                            Game.Player.Weapon.Damage -= Game.Player.BonusDamage;
                            Game.Player.BonusDamage = 0;
                            break;

                        case BoosterTypes.SpeedBoost:
                            Game.Player.Speed -= Game.Player.BonusSpeed;
                            Game.Player.BonusSpeed = 0;
                            break;
                    }
                }
            }
        }

        private static void CheckEnemiesBoostersRemainingTime(List<Enemy> spawnedEnemies)
        {
            foreach (var enemy in spawnedEnemies)
            {
                var activeBoosters = new Dictionary<BoosterTypes, int>(enemy.ActiveBoosters);
                foreach (var boosterTypeRemainingTime in activeBoosters)
                {
                    if (boosterTypeRemainingTime.Value <= 0) continue;

                    switch (boosterTypeRemainingTime.Key)
                    {
                        case BoosterTypes.HealthBoost:
                            enemy.GetHealthBoost(int.Parse(Resources.HealthBoosterImpact) * 3);
                            break;

                        case BoosterTypes.DamageBoost:
                            if (enemy.BonusDamage == 0)
                                enemy.GetDamageBoost((int)(int.Parse(Resources.DamageBoosterImpact) * 0.5));
                            break;

                        case BoosterTypes.SpeedBoost:
                            if (enemy.BonusSpeed == 0)
                                enemy.GetSpeedBoost(int.Parse(Resources.SpeedBoosterImpact));
                            break;
                    }

                    if (_boosterTimer != null)
                        enemy.ActiveBoosters[boosterTypeRemainingTime.Key] =
                            boosterTypeRemainingTime.Value - _boosterTimer.Interval;

                    if (enemy.ActiveBoosters[boosterTypeRemainingTime.Key] == 0)
                    {
                        switch (boosterTypeRemainingTime.Key)
                        {
                            case BoosterTypes.DamageBoost:
                                enemy.Damage -= enemy.BonusDamage;
                                enemy.BonusDamage = 0;
                                break;
                            case BoosterTypes.SpeedBoost:
                                enemy.Speed -= enemy.BonusSpeed;
                                enemy.BonusSpeed = 0;
                                break;
                            case BoosterTypes.HealthBoost:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            
        }
        private static void CheckBoostersIntersections()
        {
            if (Game.SpawnedBoosters.Count == 0) return;
            var spawnedBoosters = new List<Booster>(Game.SpawnedBoosters);

            CheckPlayerBoosterIntersections(spawnedBoosters);

            if (Game.SpawnedEnemies.Count == 0) return;
            var spawnedEnemies = new List<Enemy>(Game.SpawnedEnemies);
            CheckEnemyBoosterIntersections(spawnedEnemies, spawnedBoosters);
        }

        private static void CheckPlayerBoosterIntersections(List<Booster> spawnedBoosters)
        {
            foreach (var booster in spawnedBoosters)
            {
                if (!Game.Player.Hitbox.IntersectsWith(booster.Hitbox)) continue;

                Game.Player.GetBoost(booster);
                Game.SpawnedBoosters.Remove(booster);
            }
        }

        private static void CheckEnemyBoosterIntersections(List<Enemy> spawnedEnemies, List<Booster> spawnedBoosters)
        {
            foreach (var booster in spawnedBoosters)
            {
                foreach (var enemy in spawnedEnemies)
                {
                    if (!enemy.Hitbox.IntersectsWith(booster.Hitbox)) continue;

                    enemy.GetBoost(booster);
                    Game.SpawnedBoosters.Remove(booster);
                }
            }
        }
    }
}