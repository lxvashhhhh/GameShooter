using System;
using System.Drawing;
using GameProject.Entities;
using GameProject.Physics;

namespace GameProject.Domain.Weapons
{
    internal abstract class Bullet : Entity
    {
        internal int Speed { get; set; }
        internal float Angle { get; set; }

        internal Bullet(Vector location, Image image, float angle) : base(location, image)
        {
            Angle = angle;
        }

        internal void Move()
        {
            var delta = new Vector(Math.Cos(Angle), Math.Sin(Angle)) * Speed;
            var nextLocation = new Point((int)(Hitbox.Location.X + delta.X), (int)(Hitbox.Location.Y + delta.Y));
            Hitbox = new Rectangle(nextLocation, Hitbox.Size);
        }
    }
}
