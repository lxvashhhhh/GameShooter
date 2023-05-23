using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Entities
{
    internal class SpeedBooster : Booster
    {
        internal SpeedBooster(Vector location) : base(location, Resources.SpeedBoost)
        {
            Type = BoosterTypes.SpeedBoost;
            Impact = int.Parse(Resources.SpeedBoosterImpact);
            Time = int.Parse(Resources.SpeedBoosterTime);
        }
    }
}
