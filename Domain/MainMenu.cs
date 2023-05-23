using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameProject.Properties;

namespace GameProject.Domain
{
    internal class MainMenu : Control
    {
        private Form Form;

        private PictureBox Logo;
        //private bool Initialized;
        private List<Control> controls;
        private Font buttonFont;
        //private PictureBox background;
        private Button startGame;
        private Button tutorial;
        private Button tutorialExitButton;
        private Button exitGame;
        private Control tutorialWindow;
        private Label tutorialControls;
        internal MainMenu(Form form)
        {
            Form = form;
            controls = new List<Control>();
            buttonFont = new Font(FontFamily.GenericSansSerif, 16);
            Location = Point.Empty;
            Size = Form.ClientSize;
        }

        private void UpdateButtons()
        {
            var centerX = Form.ClientSize.Width / 2;
            var centerY = Form.ClientSize.Height / 2;

            tutorial = new Button
            {
                Size = new Size(300, 70),
                Location = new Point(centerX - 300 / 2, centerY - 70 / 2),
                Text = Resources.Tutorial,
                Font = buttonFont,
                TabStop = false
            };
            tutorial.Click += (s,a) =>
            {
                tutorial.Enabled = false;
                ShowTutorialWindow();
            };
            Form.Controls.Add(tutorial);
            controls.Add(tutorial);

            

            startGame = new Button
            {
                Size = tutorial.Size,
                Location = new Point(tutorial.Left, tutorial.Top - tutorial.Size.Height - 50),
                Text = Resources.StartGame,
                Font = buttonFont,
                TabStop = false
            };

            startGame.Click += (s, a) =>
            {
                Game.ChangeStage(GameStage.Battle);
            };
            Form.Controls.Add(startGame);
            controls.Add(startGame);

            Logo = new PictureBox
            {
                Location = new Point(startGame.Left - 15, Form.ClientSize.Height / 16),
                Size = Resources.MainMenuLogo.Size,
                Image = Resources.MainMenuLogo,

            };
            Form.Controls.Add(Logo);
            controls.Add(Logo);

            exitGame = new Button
            {
                Size = tutorial.Size,
                Location = new Point(tutorial.Left, tutorial.Bottom + 50),
                Text = Resources.ExitGame,
                Font = buttonFont,
                TabStop = false
            };
            exitGame.Click += (s, a) => Application.Exit();
            Form.Controls.Add(exitGame);
            controls.Add(exitGame);

            tutorialWindow = new Control
            {
                Location = new Point(Form.Width / 5, 30),
                Size = new Size(Form.Width * 3 / 5, Form.Height - 2 * 30),
                BackColor = Color.Wheat
            };
            Form.Controls.Add(tutorialWindow);
            tutorialWindow.Hide();
            controls.Add(tutorialWindow);

            tutorialExitButton = new Button
            {
                BackColor = Color.WhiteSmoke,
                Location = new Point(centerX - 20, centerY - tutorialWindow.Height / 2),
                Size = new Size(150, 50),
                Text = Resources.Exit,
                TabStop = false,
                Font = buttonFont
            };
            tutorialExitButton.Click += (s, a) => CloseTutorialWindow();
            tutorialWindow.Controls.Add(tutorialExitButton);
            controls.Add(tutorialExitButton);

            tutorialControls = new Label
            {
                Location = new Point(tutorialWindow.Width / 2 - 200, 200),
                Size = tutorialWindow.Size,
                BackColor = tutorialWindow.BackColor,
                Font = new Font(FontFamily.GenericMonospace, 26, FontStyle.Italic),
                Text = "W - движение вверх" + "\n\n" +
                       "A - движение влево" + "\n\n" +
                       "S - движение вниз" + "\n\n" +
                       "D - движение вправо" + "\n\n" +
                       "Space - выстрел" + "\n\n" +
                       "RMB - магазин",

            };
            tutorialWindow.Controls.Add(tutorialControls);
            controls.Add(tutorialControls);
        }

        private void ShowTutorialWindow()
        {
            tutorialWindow.BringToFront();
            tutorialWindow.Show();
            tutorialExitButton.BringToFront();
            tutorialExitButton.Show();
            tutorialControls.BringToFront();
            tutorialControls.Show();
        }

        private void CloseTutorialWindow()
        {
            tutorial.Enabled = true;
            tutorialExitButton.Hide();
            tutorialWindow.Hide();
            tutorialControls.Hide();
        }
        internal void Open()
        {
            //if (!Initialized)
            //{
            //    Initialized = true;
            //    background = new PictureBox
            //    {
            //        Location = Point.Empty,
            //        Size = Form.ClientSize,
            //        Image = Resources.MainMenuBackground,
            //        SizeMode = PictureBoxSizeMode.StretchImage
            //    };
            //    Form.Controls.Add(background);
            //    controls.Add(background);
            //}
            UpdateButtons();
        }

        internal void Close()
        {
            foreach (var control in controls)
            {
                if (Form.Controls.Contains(control))
                {
                    Form.Controls.Remove(control);
                }
            }
        }
    }
}
