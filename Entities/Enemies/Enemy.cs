using System;
using System.Collections.Generic;
using System.Drawing;
using GameProject.Domain;
using GameProject.Interfaces;
using GameProject.Physics;

namespace GameProject.Entities
{
    abstract class Enemy : Entity, IFightable
    {
        public int MinHealth { get; set; }
        public double Health { get; set; }
        public double MaxHealth { get; set; }
        public float RotationAngle { get; set; }
        public int Speed { get; set; }
        public int BonusSpeed { get; set; }
        public int Damage { get; set; }
        public int BonusDamage { get; set; }
        internal int Coins { get; set; }
        public Dictionary<BoosterTypes, int> ActiveBoosters { get; set; }

        protected Enemy(Vector location, Image image) : base(location, image)
        {
            HealthBar = new Rectangle(Hitbox.Location.X + (int)(0.25 * Hitbox.Width), Hitbox.Location.Y - 10,
                (int)(0.5 * Hitbox.Width), 10);

            ActiveBoosters = new Dictionary<BoosterTypes, int>
            {
                [BoosterTypes.HealthBoost] = 0,
                [BoosterTypes.DamageBoost] = 0,
                [BoosterTypes.SpeedBoost] = 0
            };
        }

        internal void Move()
        {
            var delta = new Vector(Math.Cos(RotationAngle), Math.Sin(RotationAngle)) * Speed;
            var nextLocation = new Point((int)(Hitbox.Location.X + delta.X), (int)(Hitbox.Location.Y + delta.Y));

            Hitbox = new Rectangle(nextLocation, Hitbox.Size);
        }

        internal float AngleToPlayer()
        {
            var x = (Game.Player.Hitbox.Location.X + Game.Player.Hitbox.Size.Width / 2f) - (Hitbox.Location.X + Hitbox.Size.Width / 2f);
            var y = (Game.Player.Hitbox.Location.Y + Game.Player.Hitbox.Size.Height / 2f) - (Hitbox.Location.Y + Hitbox.Size.Height / 2f);
            //return new Vector(x, y).AngleInDegrees;
            return new Vector(x, y).AngleInRadians;
        }

        public void GetBoost(Booster booster)
        {
            ActiveBoosters[booster.Type] = booster.Time;
        }
        public void GetHealthBoost(double impact)
        {
            if (Health == MaxHealth)
            {
                ActiveBoosters[BoosterTypes.HealthBoost] = 0;
                return;
            }

            if (Health + impact > MaxHealth)
            {
                Health = MaxHealth;
                return;
            }

            Health += impact;
        }

        public void GetDamageBoost(int impact)
        {
            BonusDamage = impact;
            Damage += BonusDamage;
        }
        public void GetSpeedBoost(int impact)
        {
            BonusSpeed = impact;
            Speed += BonusSpeed;
        }

        public void DealDamage(Entity entity)
        {
            var player = (Player)entity;
            player.TakeDamage(Damage * 1000);
        }

        public void TakeDamage(int damage)
        {
            if (Health - damage < MinHealth)
            {
                Health = MinHealth;
                return;
            }
            Health -= damage;
        }
        internal float GetHpPercent()
        {
            return (float)(Health / MaxHealth);
        }
    }
}
