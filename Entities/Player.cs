using System.Collections.Generic;
using System.Drawing;
using GameProject.Domain;
using GameProject.Domain.Weapons;
using GameProject.Interfaces;
using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Entities
{
    
    internal class Player : Entity, IMovable, IFightable
    {
        public int MinHealth { get; set; }
        public double Health { get; set; }
        public double MaxHealth { get; set; }
        public int Speed{ get; set; }
        public int BonusSpeed { get; set; }
        public float RotationAngle { get; set; }
        public bool IsMovingUp { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsMovingDown { get; set; }
        public bool IsMovingRight { get; set; }
        internal bool IsShooting { get; set; }
        public int BonusDamage { get; set; }
        public Dictionary<BoosterTypes, int> ActiveBoosters { get; set; }
        public Weapon Weapon { get; set; }

        internal Player(Vector location) : base(location, Resources.HeroHandgun)
        {
            Hitbox = new Rectangle(new Vector(location.X - Hitbox.Size.Width / 2, location.Y - Hitbox.Size.Height / 2).ToPoint(), Hitbox.Size);
            
            Speed = 7;

            Weapon = new Handgun();

            MaxHealth = 100 * 1000;
            Health = MaxHealth;

            HealthBar = new Rectangle(Hitbox.Location.X + (int) (0.25 * Hitbox.Width), Hitbox.Location.Y - 10,
                (int) (0.5 * Hitbox.Width), 10);

            ActiveBoosters = new Dictionary<BoosterTypes, int>
            {
                [BoosterTypes.HealthBoost] = 0,
                [BoosterTypes.DamageBoost] = 0,
                [BoosterTypes.SpeedBoost] = 0
            };

            
        }

        public void Move()
        {
            if (IsMovingLeft)
            {
                var delta = new Vector(-1, 0) * Speed;
                var nextLocation = new Point((int)(Hitbox.Location.X + delta.X), (int)(Hitbox.Location.Y + delta.Y));

                if (!Game.InBounds(new Rectangle(nextLocation, Hitbox.Size))) return;

                Hitbox = new Rectangle(nextLocation, Hitbox.Size);

                if (Game.InCameraBoundsX(Hitbox))
                    View.Offset += delta;
            }

            if (IsMovingRight)
            {
                var delta = new Vector(1, 0) * Speed;
                var nextLocation = new Point((int)(Hitbox.Location.X + delta.X), (int)(Hitbox.Location.Y + delta.Y));

                if (!Game.InBounds(new Rectangle(nextLocation, Hitbox.Size))) return;

                Hitbox = new Rectangle(nextLocation, Hitbox.Size);

                if (Game.InCameraBoundsX(Hitbox))
                    View.Offset += delta;
            }

            if (IsMovingUp)
            {
                var delta = new Vector(0, -1) * Speed;
                var nextLocation = new Point((int)(Hitbox.Location.X + delta.X), (int)(Hitbox.Location.Y + delta.Y));

                if (!Game.InBounds(new Rectangle(nextLocation, Hitbox.Size))) return;

                Hitbox = new Rectangle(nextLocation, Hitbox.Size);

                if (Game.InCameraBoundsY(Hitbox))
                    View.Offset += delta;
            }

            if (IsMovingDown)
            {
                var delta = new Vector(0, 1) * Speed;
                var nextLocation = new Point((int)(Hitbox.Location.X + delta.X), (int)(Hitbox.Location.Y + delta.Y));

                if (!Game.InBounds(new Rectangle(nextLocation, Hitbox.Size))) return;

                Hitbox = new Rectangle(nextLocation, Hitbox.Size);

                if (Game.InCameraBoundsY(Hitbox))
                    View.Offset += delta;
            }
        }

        internal float AngleToTarget(Vector targetLocation)
        {
            var x = targetLocation.X - (Hitbox.Location.X + Hitbox.Size.Width / 2f);
            var y = targetLocation.Y - (Hitbox.Location.Y + Hitbox.Size.Height / 2f);
            //return new Vector(x, y).AngleInDegrees;
            return new Vector(x, y).AngleInRadians;
        }

        internal float AngleToTarget(Rectangle hitbox)
        {
            var x = (hitbox.Location.X + hitbox.Size.Width / 2f) - (Hitbox.Location.X + Hitbox.Size.Width / 2f);
            var y = (hitbox.Location.Y + hitbox.Size.Height / 2f) - (Hitbox.Location.Y + Hitbox.Size.Height / 2f);
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
            Weapon.Damage += BonusDamage;
        }
        public void GetSpeedBoost(int impact)
        {
            BonusSpeed = impact;
            Speed += BonusSpeed;
        }

        internal void Shoot()
        {
            if (IsShooting)
            {
                if (Weapon.Ammo != 0 && Weapon.Recoil == 0)
                {
                    Weapon.Shoot(RotationAngle);
                }

                else if(Weapon.Ammo == 0 && !Weapon.IsReloading)
                {
                    Weapon.Reload();
                }
            }
            
        }
        public void DealDamage(Entity entity)
        {
            ((Enemy)entity).TakeDamage(Weapon.Damage * 1000);
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
        internal float GetReloadingPercent()
        {
            var reloadingTime = GetReloadingTime();
            return Weapon.Recoil / (float)reloadingTime;
        }

        internal int GetReloadingTime()
        {
            var reloadingTime = 0;
            switch (Weapon.Type)
            {
                case WeaponTypes.Handgun:
                    reloadingTime = int.Parse(Resources.HandgunReload) * MainForm.MainTimer.Interval;
                    break;
                case WeaponTypes.Rifle:
                    reloadingTime = int.Parse(Resources.RifleReload) * MainForm.MainTimer.Interval;
                    break;
                case WeaponTypes.Shotgun:
                    reloadingTime = int.Parse(Resources.ShotgunReload) * MainForm.MainTimer.Interval;
                    break;
            }

            return reloadingTime;
        }
        internal Vector GetHitboxCenter()
        {
            var x = Hitbox.X + Hitbox.Width / 2;
            var y = Hitbox.Y + Hitbox.Height / 2;
            return new Vector(x, y);
        }
    }
}
