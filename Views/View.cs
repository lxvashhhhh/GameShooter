using System;
using System.Drawing;
using System.Windows.Forms;
using GameProject.Domain;
using GameProject.Extensions;
using GameProject.Physics;
using GameProject.Properties;
using MainMenu = GameProject.Domain.MainMenu;

namespace GameProject
{
    internal class View
    {
        private static bool GameInitialized;
        private static bool MainMenuInitialized;
        internal static Vector Offset = Vector.Zero;
        internal static Rectangle ViewedZone { get; set; }
        internal static Form Form { get; set; }
        internal static bool IsFullscreen { get; set; }
        internal static Label testLabel;
        internal static Label ammoLabel { get; set; }
        internal static Label CoinsLabel { get; set; }
        internal static PictureBox CoinsIcon { get; set; }
        internal static ProgressBar healthTimeBar { get; set; }
        internal static PictureBox healthBarIcon { get; set; }
        internal static ProgressBar damageTimeBar { get; set; } //TODO: Move to new UIClass
        internal static PictureBox damageBarIcon { get; set; }
        internal static ProgressBar speedTimeBar { get; set; }
        internal static PictureBox speedBarIcon { get; set; }
        internal static Shop Shop { get; set; }
        internal static MainMenu MainMenu { get; set; }


        internal static void UpdateTextures(Graphics graphics, Form form)
        {
            if(Form == null)
                Form = form;

            var gameStage = Game.Stage;

            switch (gameStage)
            {
                case GameStage.NotStarted:
                    if (!MainMenuInitialized)
                    {
                        GoFullscreen(true);
                        MainMenuInitialized = true;
                        MainMenu = new MainMenu(Form);
                    }

                    if (!Form.Controls.Contains(MainMenu))
                    {
                        MainMenu.Open();
                        Form.Controls.Add(MainMenu);
                    }
                    break;

                case GameStage.Battle:
                    if (!GameInitialized)
                    {
                        GameInitialized = true;
                        Form.Cursor = Cursors.Cross;
                        ShowUserInterface();
                        InitializeUserInterface();
                        Game.Resume();
                        Shop = new Shop(Form);
                    }
                    if (Form.Controls.Contains(Shop))
                    {
                        Shop.Close();
                        Form.Controls.Remove(Shop);
                        Game.Resume();
                    }

                    if (Form.Controls.Contains(MainMenu))
                    {
                        MainMenu.Close();
                        Form.Controls.Remove(MainMenu);
                    }


                    UpdateCamera(graphics);

                    //graphics.DrawRectangle(new Pen(Color.Red), Game.GameZone); //GameZone hitbox
                    //graphics.DrawRectangle(new Pen(Color.Blue), Game.CameraZone); //CameraZone hitbox
                    //graphics.DrawRectangle(new Pen(Color.Yellow), ViewedZone); //Rectangle covering the observed area (and slightly larger)

                    UpdateBoosters(graphics);
                    UpdateMovement(graphics);
                    UpdateHealth(graphics);
                    UpdateRotation(graphics);
                    UpdateShooting(graphics);
                    UpdateBullets(graphics);
                    break;

                case GameStage.InShop:
                    if (!Form.Controls.Contains(Shop))
                    {
                        Game.Pause();
                        Game.UpdateAvailableWeapons();
                        Shop.Open();
                        Form.Controls.Add(Shop);
                    }
                    break;

                case GameStage.Finished:
                    ShowFinishWindow();
                    MainForm.MainTimer.Stop();
                    return;
            }
        }

        private static void UpdateBullets(Graphics graphics)
        {
            if (Game.SpawnedBullets.Count == 0) return;
            foreach (var bullet in Game.SpawnedBullets)
            {
                bullet.Move();
                bullet.PictureBox.Image?.Dispose();
                var bulletBitmap = new Bitmap(bullet.Image, bullet.PictureBox.Size);
                RotateBitmap(bulletBitmap, bullet.Angle, graphics, bullet.Hitbox.Location);
            }
        }

        private static void UpdateShooting(Graphics graphics)
        {
            if (Game.Player.Weapon.Recoil != 0)
            {
                Game.Player.Weapon.Recoil -= MainForm.MainTimer.Interval;

                if (Game.Player.Weapon.IsReloading)
                {
                    if (Game.Player.Weapon.Recoil == 0)
                    {
                        Game.Player.Weapon.IsReloading = false;
                        Game.Player.Weapon.Ammo = Game.Player.Weapon.MaxAmmo;
                    }
                    else
                    {
                        UpdateReloading(graphics);
                    }
                }
            }

            Game.Player.Shoot();
            ammoLabel.Text = Game.Player.Weapon.Ammo.ToString();

        }
        private static void UpdateCamera(Graphics graphics)
        {
            graphics.TranslateTransform(-(int)Offset.X, -(int)Offset.Y);
        }

        private static void UpdateReloading(Graphics graphics)
        {
            var playerReloadingBarPosition = new Point(
                Game.Player.Hitbox.Location.X + (int)(0.25 * Game.Player.Hitbox.Width),
                Game.Player.Hitbox.Bottom);

            graphics.DrawRectangle(Pens.Black,
                playerReloadingBarPosition.X,
                playerReloadingBarPosition.Y,
                (int)(0.5 * Game.Player.Hitbox.Width),
                Game.Player.HealthBar.Height);

            graphics.FillRectangle(Brushes.CornflowerBlue,
                playerReloadingBarPosition.X,
                playerReloadingBarPosition.Y,
                (int)(0.5 * Game.Player.Hitbox.Width) * Game.Player.GetReloadingPercent(),
                Game.Player.HealthBar.Height);
        }
        private static void UpdateMovement(Graphics graphics)
        {
            UpdateEnemiesMovement(graphics);
            UpdatePlayerMovement(graphics);
            UpdateViewedZone();
        }
        private static void UpdateHealth(Graphics graphics)
        {
            UpdatePlayerHealth(graphics);
            UpdateEnemiesHealth(graphics);
        }

        private static void UpdatePlayerHealth(Graphics graphics)
        {
            var playerHealthBarPosition = new Point(
                Game.Player.Hitbox.Location.X + (int)(0.25 * Game.Player.Hitbox.Width),
                Game.Player.Hitbox.Location.Y - 10);

            graphics.DrawRectangle(Pens.Black,
                playerHealthBarPosition.X,
                playerHealthBarPosition.Y,
                Game.Player.HealthBar.Width,
                Game.Player.HealthBar.Height);

            graphics.FillRectangle(Brushes.Green,
                playerHealthBarPosition.X,
                playerHealthBarPosition.Y,
                Game.Player.HealthBar.Width * Game.Player.GetHpPercent(),
                Game.Player.HealthBar.Height);
        }
        private static void UpdateEnemiesHealth(Graphics graphics)
        {
            if(Game.SpawnedEnemies.Count == 0) return;
            foreach (var enemy in Game.SpawnedEnemies)
            {
                var enemyHealthBarPosition = new Point(
                    enemy.Hitbox.Location.X + (int)(0.25 * enemy.Hitbox.Width),
                    enemy.Hitbox.Location.Y - 10);

            graphics.DrawRectangle(Pens.Black,
                enemyHealthBarPosition.X,
                enemyHealthBarPosition.Y,
                enemy.HealthBar.Width,
                enemy.HealthBar.Height);

            graphics.FillRectangle(Brushes.Red,
                enemyHealthBarPosition.X,
                enemyHealthBarPosition.Y,
                enemy.HealthBar.Width * enemy.GetHpPercent(),
                enemy.HealthBar.Height);
            }
            
        }
        private static void UpdateRotation(Graphics graphics)
        {
            UpdateEnemiesRotation(graphics);
            UpdatePlayerRotation(graphics);
        }

        private static void UpdateBoosters(Graphics graphics)
        {
            UpdateSpawnedBoosters(graphics);
            UpdateBoostersUserInterface();
            UpdateEnemiesBoostersVisual(graphics);
        }

        private static void UpdateSpawnedBoosters(Graphics graphics)
        {
            if (Game.SpawnedBoosters.Count == 0) return;

            foreach (var booster in Game.SpawnedBoosters)
            {
                var image = new Bitmap(booster.Hitbox.Width, booster.Hitbox.Height);
                using (var g = Graphics.FromImage(image))
                {
                    g.DrawImage(booster.Image, 0, 0, booster.Hitbox.Width, booster.Hitbox.Height);
                }
                graphics.DrawImage(image, booster.Hitbox.Location);

                //graphics.DrawRectangle(new Pen(Color.MediumPurple), booster.Hitbox);
            }
        }

        private static void UpdateBoostersUserInterface()
        {
            if (Game.Player.ActiveBoosters[BoosterTypes.HealthBoost] != 0)
                healthTimeBar.Value = (int)(Game.Player.ActiveBoosters[BoosterTypes.HealthBoost] /
                    double.Parse(Resources.HealthBoosterTime) * healthTimeBar.Maximum);

            if (Game.Player.ActiveBoosters[BoosterTypes.DamageBoost] != 0)
                damageTimeBar.Value = (int)(Game.Player.ActiveBoosters[BoosterTypes.DamageBoost] /
                    double.Parse(Resources.DamageBoosterTime) * damageTimeBar.Maximum);

            if (Game.Player.ActiveBoosters[BoosterTypes.SpeedBoost] != 0)
                speedTimeBar.Value =(int)(Game.Player.ActiveBoosters[BoosterTypes.SpeedBoost] /
                    double.Parse(Resources.SpeedBoosterTime) * speedTimeBar.Maximum);
        }

        private static void UpdateEnemiesBoostersVisual(Graphics graphics)
        {
            if (Game.SpawnedEnemies.Count == 0) return;
            foreach (var enemy in Game.SpawnedEnemies)
            {
                if (enemy.ActiveBoosters[BoosterTypes.HealthBoost] != 0)
                {
                    var enemyHealthTimeBarLocation = new Point(enemy.Hitbox.Left + (int)(0.25 * enemy.Hitbox.Width), enemy.Hitbox.Bottom + 5);
                    graphics.DrawRectangle(Pens.Black, enemyHealthTimeBarLocation.X, enemyHealthTimeBarLocation.Y, (int)(0.5 * enemy.Hitbox.Width), 10);
                    graphics.FillRectangle(Brushes.OrangeRed, enemyHealthTimeBarLocation.X, enemyHealthTimeBarLocation.Y,
                        (int)(enemy.ActiveBoosters[BoosterTypes.HealthBoost] / (double)(5 * 1000) * (int)(0.5 * enemy.Hitbox.Width)), 10);
                }

                if (enemy.ActiveBoosters[BoosterTypes.DamageBoost] != 0)
                {
                    var enemyDamageTimeBarLocation = new Point(enemy.Hitbox.Left + (int)(0.25 * enemy.Hitbox.Width), enemy.Hitbox.Bottom + 20);
                    graphics.DrawRectangle(Pens.Black, enemyDamageTimeBarLocation.X, enemyDamageTimeBarLocation.Y, (int)(0.5 * enemy.Hitbox.Width), 10);
                    graphics.FillRectangle(Brushes.DarkOrange, enemyDamageTimeBarLocation.X, enemyDamageTimeBarLocation.Y,
                        (int)(enemy.ActiveBoosters[BoosterTypes.DamageBoost] / (double)(10 * 1000) * (int)(0.5 * enemy.Hitbox.Width)), 10);
                }

                if (enemy.ActiveBoosters[BoosterTypes.SpeedBoost] != 0)
                {
                    var enemySpeedTimeBarLocation = new Point(enemy.Hitbox.Left + (int)(0.25 * enemy.Hitbox.Width), enemy.Hitbox.Bottom + 35);
                    graphics.DrawRectangle(Pens.Black, enemySpeedTimeBarLocation.X, enemySpeedTimeBarLocation.Y, (int)(0.5 * enemy.Hitbox.Width), 10);
                    graphics.FillRectangle(Brushes.DodgerBlue, enemySpeedTimeBarLocation.X, enemySpeedTimeBarLocation.Y,
                        (int)(enemy.ActiveBoosters[BoosterTypes.SpeedBoost] / (double)(10 * 1000) * (int)(0.5 * enemy.Hitbox.Width)), 10);
                }
            }
        }

        private static void UpdatePlayerMovement(Graphics graphics)
        {
            Game.Player.Move();
            //graphics.DrawRectangle(new Pen(Color.Green), Game.Player.Hitbox);
            Game.Player.PictureBox.BringToFront();
        }
        private static void UpdateViewedZone()
        {
            var viewedZoneLocation = new Point(
                Game.Player.Hitbox.Location.X - Form.ClientSize.Width,
                Game.Player.Hitbox.Location.Y - Form.ClientSize.Height);

            var viewedZoneSize = new Size(2 * Form.ClientSize.Width, 2 * Form.ClientSize.Height);

            ViewedZone = new Rectangle(viewedZoneLocation,
                viewedZoneSize);
        }

        private static void UpdateEnemiesMovement(Graphics graphics)
        {
            if (Game.SpawnedEnemies.Count == 0) return;

            foreach (var enemy in Game.SpawnedEnemies)
            {
                enemy.Move();
                //graphics.DrawRectangle(new Pen(Color.Red), enemy.Hitbox);
            }
        }
        private static void UpdatePlayerRotation(Graphics graphics)
        {
            Game.Player.PictureBox.Image?.Dispose();
            var playerBitmap = new Bitmap(Game.Player.Image, Game.Player.PictureBox.Size);
            RotateBitmap(playerBitmap, Game.Player.RotationAngle, graphics, Game.Player.Hitbox.Location);
            Game.Player.PictureBox.BringToFront();
        }

        private static void UpdateEnemiesRotation(Graphics graphics)
        {
            if (Game.SpawnedEnemies.Count == 0) return;

            foreach (var enemy in Game.SpawnedEnemies)
            {
                enemy.RotationAngle = enemy.AngleToPlayer();
                enemy.PictureBox.Image?.Dispose();
                var enemyBitmap = new Bitmap(enemy.Image, enemy.PictureBox.Size);
                RotateBitmap(enemyBitmap, enemy.RotationAngle, graphics, enemy.Hitbox.Location);
            }
        }
        
        private static void RotateBitmap(Bitmap bitmap, float angle, Graphics graphics, Point location)
        {
            const float convertToDegree = 180 / (float)Math.PI;
            var rotated = new Bitmap(bitmap.Width, bitmap.Height);

            using (var g = Graphics.FromImage(rotated))
            {
                g.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
                g.RotateTransform(angle * convertToDegree);
                g.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2);
                g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            }
            graphics.DrawImage(rotated, location);
        }

        private static void ShowFinishWindow()
        {
            var restart = new Button
            {
                Text = "Заново",
                Location = new Point(500, 400),
                Size = new Size(500, 100),
                Font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold)
            };

            var result = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "У тебя получилось собрать " + Game.Coins + Resources._Coins,
                Location = new Point(restart.Left - 100, restart.Top - 40),
                Size = new Size(restart.Width + 200, 30),
                Font = new Font(FontFamily.GenericMonospace, 18, FontStyle.Bold)
            };
            restart.Click += (s, e) => Application.Restart();

            Form.Controls.Add(restart);
            Form.Controls.Add(result);

        }
        internal static void GoFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                //TopMost = true;
                Form.FormBorderStyle = FormBorderStyle.None;
                Form.WindowState = FormWindowState.Maximized;
                IsFullscreen = true;
            }
            else
            {
                Form.FormBorderStyle = FormBorderStyle.Sizable;
                Form.WindowState = FormWindowState.Normal;
                Form.Bounds = Screen.PrimaryScreen.Bounds;
                IsFullscreen = false;
            }
        }

        private static void ShowUserInterface()
        {
            testLabel = new Label
            {
                Location = new Point(50, 50),
                Size = new Size(90, 70),
            };
            Form.Controls.Add(testLabel);
            ShowCoins();
            ShowBoosters();
            ShowAmmo();
        }

        private static void ShowBoosters()
        {
            speedTimeBar = new ProgressBar
            {
                Location = new Point(Form.Left + 30, Form.Bottom - 70),
                Size = new Size(150, 30),
                Minimum = 0,
                Maximum = int.Parse(Resources.SpeedBoosterTime)
            };
            Form.Controls.Add(speedTimeBar);

            speedBarIcon = new PictureBox
            {
                Location = new Point(speedTimeBar.Right + 5, speedTimeBar.Top),
                Image = Resources.SpeedBoost,
                Size = new Size(speedTimeBar.Height, speedTimeBar.Height),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Form.Controls.Add(speedBarIcon);

            damageTimeBar = new ProgressBar
            {
                Location = new Point(speedTimeBar.Left, speedTimeBar.Top - 40),
                Size = new Size(150, 30),
                Minimum = 0,
                Maximum = int.Parse(Resources.DamageBoosterTime),
            };
            damageTimeBar.SetState(3);
            Form.Controls.Add(damageTimeBar);

            damageBarIcon = new PictureBox
            {
                Location = new Point(damageTimeBar.Right + 5, damageTimeBar.Top),
                Image = Resources.DamageBoost,
                Size = new Size(damageTimeBar.Height, damageTimeBar.Height),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Form.Controls.Add(damageBarIcon);

            healthTimeBar = new ProgressBar
            {
                Location = new Point(damageTimeBar.Left, damageTimeBar.Top - 40),
                Size = new Size(150, 30),
                Minimum = 0,
                Maximum = int.Parse(Resources.HealthBoosterTime)
            };
            healthTimeBar.SetState(2);
            Form.Controls.Add(healthTimeBar);

            healthBarIcon = new PictureBox
            {
                Location = new Point(healthTimeBar.Right + 5, healthTimeBar.Top),
                Image = Resources.HealthBoost,
                Size = new Size(healthTimeBar.Height, healthTimeBar.Height),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Form.Controls.Add(healthBarIcon);
        }

        private static void ShowCoins()
        {
            CoinsIcon = new PictureBox
            {
                Location = new Point(Form.Right - Resources.Coin.Width - 50, 40),
                Size = new Size(Resources.Coin.Width, Resources.Coin.Height),
                Image = Resources.Coin
            };
            Form.Controls.Add(CoinsIcon);

            CoinsLabel = new Label
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Text = Game.Coins.ToString(),
                Location = new Point(CoinsIcon.Left - 100, CoinsIcon.Top),
                Size = new Size(80, CoinsIcon.Height),
                Font = new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold)
            };
            Form.Controls.Add(CoinsLabel);
        }

        private static void ShowAmmo()
        {
            var ammoIcon = new PictureBox
            {
                Location = new Point(Form.Right - Resources.AmmoPicture.Width - 50, Form.Bottom - Resources.AmmoPicture.Height - 40),
                Image = Resources.AmmoPicture,
            };
            Form.Controls.Add(ammoIcon);

            ammoLabel = new Label
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold),
                Size = new Size(CoinsLabel.Width, ammoIcon.Height),
                Location = new Point(CoinsLabel.Left, ammoIcon.Top),
                Text = Game.Player.Weapon.Ammo.ToString()
            };
            Form.Controls.Add(ammoLabel);
        }
        private static void InitializeUserInterface()
        {
            Form.MouseMove += (s, e) =>
            {
                testLabel.Text = "Camera offset: " + Offset + "\nPlayer location: " + Game.Player.Hitbox.Location;
            };
            Form.KeyDown += (s, e) =>
            {
                testLabel.Text = "Camera offset: " + Offset + "\nPlayer location: " + Game.Player.Hitbox.Location;
            };
        }
    }
}
