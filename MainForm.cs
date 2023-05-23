using GameProject.Domain;
using GameProject.Entities;
using GameProject.Physics;
using GameProject.Properties;
using Timer = System.Windows.Forms.Timer;

namespace GameProject
{
    public partial class MainForm : Form
    {
        internal static Timer? MainTimer;

        public MainForm()
        {
            InitializeComponent();
            Text = Resources.GameTitle;
            MainTimer = new Timer
            {
                Interval = 15
            };
            MainTimer.Tick += (_, _) => Invalidate();
            MainTimer.Start();

            InitGame(new Size(3000, 3000));

            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;
            MouseMove += MainForm_MouseMove;
            MouseDown += MainForm_MouseDown;
            MouseUp += MainForm_MouseUp;
        }

        internal void InitGame(Size gameSize)
        {
            // ReSharper disable once PossibleLossOfFraction
            var center = new Vector(Screen.PrimaryScreen.WorkingArea.Width / 2,
                // ReSharper disable once PossibleLossOfFraction
                Screen.PrimaryScreen.WorkingArea.Height / 2);
            var gameZone = new Rectangle(Point.Empty, gameSize);
            var playerSpawnPoint = new Vector(center.X, center.Y);
            var _ = new Game(new Player(playerSpawnPoint), gameZone, this);
        }

        private static void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Controller.ControlKeys(e.KeyCode, true);
        }

        private static void MainForm_KeyUp(object? sender, KeyEventArgs e)
        {
            Controller.ControlKeys(e.KeyCode, false);
        }

        private static void MainForm_MouseMove(object? sender, MouseEventArgs e)
        {
            Controller.ControlMouse(e, false);
        }
        private static void MainForm_MouseDown(object? sender, MouseEventArgs e)
        {
            Controller.ControlMouse(e, true);
        }
        private static void MainForm_MouseUp(object? sender, MouseEventArgs e)
        {
            Controller.ControlMouse(e, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            View.UpdateTextures(e.Graphics, this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
        }
    }
}
