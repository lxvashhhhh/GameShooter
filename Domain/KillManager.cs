using Timer = System.Windows.Forms.Timer;
using GameProject.Domain.Weapons;
using GameProject.Entities;

namespace GameProject.Domain
{
    internal class KillManager
    {
        private static Timer? _killTimer;

        internal KillManager()
        {
            _killTimer = new Timer();
            if (MainForm.MainTimer != null) _killTimer.Interval = MainForm.MainTimer.Interval * 2;
            _killTimer.Tick += (sender, args) => CheckConfrontations();
            _killTimer.Start();
        }

        private static void CheckConfrontations()
        {
            if(Game.SpawnedEnemies.Count == 0) return;
            var spawnedEnemies = new List<Enemy>(Game.SpawnedEnemies);
            CheckEnemiesHits(spawnedEnemies);

            if(Game.SpawnedBullets.Count == 0) return;
            var spawnedBullets = new List<Bullet>(Game.SpawnedBullets);
            CheckBulletsHits(spawnedBullets, spawnedEnemies);
        }

        private static void CheckEnemiesHits(List<Enemy> spawnedEnemies)
        {
            foreach (var enemy in spawnedEnemies.Where(enemy => enemy.Hitbox.IntersectsWith(Game.Player.Hitbox)))
            {
                enemy.DealDamage(Game.Player);

                if (Math.Abs(Game.Player.Health - Game.Player.MinHealth) < 10)
                    Game.ChangeStage(GameStage.Finished);
            }
            
        }
        private static void CheckBulletsHits(List<Bullet> spawnedBullets, List<Enemy> spawnedEnemies)
        {
            foreach (var enemy in spawnedEnemies)
            {
                foreach (var bullet in spawnedBullets.Where(bullet => bullet.Hitbox.IntersectsWith(enemy.Hitbox)))
                {
                    enemy.TakeDamage(Game.Player.Weapon.Damage * 1000);
                    Game.SpawnedBullets.Remove(bullet);

                    if (Math.Abs(enemy.Health - enemy.MinHealth) < 10)
                    {
                        Game.Coins += enemy.Coins;
                        enemy.Coins = 0;
                        View.CoinsLabel.Text = Game.Coins.ToString();
                        Game.SpawnedEnemies.Remove(enemy);
                    }

                    if (bullet.Hitbox.IntersectsWith(View.ViewedZone)) continue;
                    Game.SpawnedBullets.Remove(bullet);
                }
            }
        }
        internal static void StopTimer() => _killTimer?.Stop();
        internal static void StartTimer() => _killTimer?.Start();
    }
}
