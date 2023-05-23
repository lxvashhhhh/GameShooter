using System.Windows.Forms;
using GameProject.Physics;
using GameProject.Properties;

namespace GameProject.Domain
{
    internal class Controller
    {
        internal static void ControlKeys(Keys key, bool isActive)
        {
            switch (key)
            {
                case Keys.W:
                    Game.Player.IsMovingUp = isActive;
                    break;
                case Keys.A:
                    Game.Player.IsMovingLeft = isActive;
                    break;
                case Keys.S:
                    Game.Player.IsMovingDown = isActive;
                    break;
                case Keys.D:
                    Game.Player.IsMovingRight = isActive;
                    break;
                case Keys.Space:
                    Game.Player.IsShooting = isActive;
                    break;
                //case Keys.Q:
                //    Game.Player.ActiveBoosters[BoosterTypes.SpeedBoost] = int.Parse(Resources.SpeedBoosterTime);
                //    break;
                //case Keys.C:
                //    Game.Player.ActiveBoosters[BoosterTypes.DamageBoost] = int.Parse(Resources.DamageBoosterTime);
                //    break;
                //case Keys.E:
                //    Game.Player.ActiveBoosters[BoosterTypes.HealthBoost] = int.Parse(Resources.HealthBoosterTime);
                //    break;

                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.R:
                    Application.Restart();
                    break;
            }
        }

        internal static void ControlMouse(MouseEventArgs e, bool isActive)
        {
            var cursorLocationOnScreen = new Vector(e.Location.X, e.Location.Y);
            var cursorLocationWithOffset = cursorLocationOnScreen + View.Offset;
            var angleToCursor = Game.Player.AngleToTarget(cursorLocationWithOffset);

            Game.Player.RotationAngle = angleToCursor;

            var pressedButton = e.Button;
            switch (pressedButton)
            {
                case MouseButtons.Left:
                    break;

                case MouseButtons.Right:
                    if (Game.Stage == GameStage.Battle)
                        Game.ChangeStage(GameStage.InShop);
                    break;
            }
        }
    }
}
