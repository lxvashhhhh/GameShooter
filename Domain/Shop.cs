using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameProject.Domain.Weapons;
using GameProject.Properties;

namespace GameProject.Domain
{
    internal sealed class Shop : Control
    {
        private Form Form;
        private bool Initialized;
        private List<Control> controls;
        private Font buttonFont;
        private Button exitButton;
        private Button handgunButton;
        private Button rifleButton;
        private Button shotgunButton;
        private PictureBox handgunPicture;
        private PictureBox riflePicture;
        private PictureBox shotgunPicture;
        private Font characteristicsFont;
        private Label handgunCharacteristics;
        private Label rifleCharacteristics;
        private Label shotgunCharacteristics;

        internal Shop(Form form)
        {
            Form = form;
            controls = new List<Control>();
            Location = new Point(Form.Left + Form.Width / 5, Form.Top + 30);
            Size = new Size(Form.Width * 3 / 5, Form.Height - 2 * 30);
            BackColor = Color.Wheat;
            buttonFont = new Font(FontFamily.GenericSansSerif, 16);
            characteristicsFont = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold);
        }

        internal void UpdateButtons()
        {
            Initialized = false;
            exitButton = new Button
            {
                Location = new Point(Location.X + Size.Width - 150 - 20, Location.Y + 20),
                Size = new Size(150, 50),
                Text = Resources.Exit,
                TabStop = false,
                Font = buttonFont
            };
            exitButton.Click += (s,a) => Game.ChangeStage(GameStage.Battle);
            Form.Controls.Add(exitButton);
            controls.Add(exitButton);



            handgunButton = new Button
            {
                Location = new Point(Location.X + 35, Bottom - 100 - 20),
                Size = new Size(250, 100),
                Font = buttonFont,
                TabStop = false
            };

            if (!Game.AvailableWeapons.Contains(WeaponTypes.Handgun))
                handgunButton.Text = Resources.BuyHandgunFor + Resources.HandgunCost + Resources._Coins;
            else
                MakeButtonEquipable(handgunButton, WeaponTypes.Handgun);
            DefineButtonClickEvent(handgunButton, int.Parse(Resources.HandgunCost), WeaponTypes.Handgun);
            Form.Controls.Add(handgunButton);
            controls.Add(handgunButton);



            rifleButton = new Button
            {
                Location = new Point(handgunButton.Right + 50, handgunButton.Top),
                Size = handgunButton.Size,
                Font = buttonFont,
                TabStop = false
            };

            if (!Game.AvailableWeapons.Contains(WeaponTypes.Rifle))
                rifleButton.Text = Resources.BuyRifleFor + Resources.RifleCost + Resources._Coins;
            else
                MakeButtonEquipable(rifleButton, WeaponTypes.Rifle);
            DefineButtonClickEvent(rifleButton, int.Parse(Resources.RifleCost), WeaponTypes.Rifle);
            Form.Controls.Add(rifleButton);
            controls.Add(rifleButton);



            shotgunButton = new Button
            {
                Location = new Point(rifleButton.Right + 50, rifleButton.Top),
                Size = rifleButton.Size,
                Font = buttonFont,
                TabStop = false
            };

            if (!Game.AvailableWeapons.Contains(WeaponTypes.Shotgun))
                shotgunButton.Text = Resources.BuyShotgunFor + Resources.ShotgunCost + Resources._Coins;
            else
                MakeButtonEquipable(shotgunButton, WeaponTypes.Shotgun);
            DefineButtonClickEvent(shotgunButton, int.Parse(Resources.ShotgunCost), WeaponTypes.Shotgun);
            Form.Controls.Add(shotgunButton);
            controls.Add(shotgunButton);
        }

        private void BuyWeapon(Button button, WeaponTypes weaponType)
        {
            View.CoinsLabel.Text = Game.Coins.ToString();
            ChangeWeapon(button);
            EquipWeapon(weaponType);
        }
        private void ChangeWeapon(Button button)
        {
            foreach (var control in controls.Where(control => control.Text == Resources.Equipped))
            {
                control.Text = Resources.Equip;
                control.Enabled = true;
            }

            MakeButtonEquipped(button);
        }
        private void DefineButtonClickEvent(Button button, int cost, WeaponTypes weaponType)
        {
            button.Click += (s, a) =>
            {
                if (Game.Coins >= cost && !Game.AvailableWeapons.Contains(weaponType))
                {
                    Game.Coins -= cost;
                    BuyWeapon(button, weaponType);
                }
                else if (Game.AvailableWeapons.Contains(weaponType))
                {
                    EquipWeapon(weaponType);
                    ChangeWeapon(button);
                }
            };
        }
        private void EquipWeapon(WeaponTypes weaponType)
        {
            switch (weaponType)
            {
                case WeaponTypes.Handgun:
                    Game.Player.Weapon = new Handgun();
                    Game.Player.Image = Resources.HeroHandgun;
                    break;
                case WeaponTypes.Rifle:
                    Game.Player.Weapon = new Rifle();
                    Game.Player.Image = Resources.HeroRifle;
                    break;
                case WeaponTypes.Shotgun:
                    Game.Player.Weapon = new Shotgun();
                    Game.Player.Image = Resources.HeroShotgun;
                    break;
            }
            Game.UpdateAvailableWeapons();
        }
        private void MakeButtonEquipped(Button button)
        {
            button.Text = "Экипировано";
            button.Enabled = false;
        }
        private void MakeButtonEquipable(Button button, WeaponTypes weaponType)
        {
            if (Game.Player.Weapon.Type != weaponType)
            {
                button.Text = Resources.Equip;
                button.Click += (s, a) => ChangeWeapon(button);
            }
            else MakeButtonEquipped(button);
        }

        private void InitPictures()
        {
            handgunPicture = new PictureBox
            {
                Image = Resources.HandgunPicture,
                Size = new Size(handgunButton.Width, Resources.HandgunPicture.Height * handgunButton.Width / Resources.HandgunPicture.Width),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(handgunButton.Left, exitButton.Bottom + 60),
                BackColor = Color.Wheat
            };
            Form.Controls.Add(handgunPicture);
            controls.Add(handgunPicture);

            riflePicture = new PictureBox
            {
                Image = Resources.RiflePicture,
                Size = new Size(rifleButton.Width, Resources.RiflePicture.Height * rifleButton.Width / Resources.RiflePicture.Width),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(rifleButton.Left, handgunPicture.Top),
                BackColor = Color.Wheat
            };
            Form.Controls.Add(riflePicture);
            controls.Add(riflePicture);

            shotgunPicture = new PictureBox
            {
                Image = Resources.ShotgunPicture,
                Size = new Size(shotgunButton.Width, Resources.ShotgunPicture.Height * shotgunButton.Width / Resources.ShotgunPicture.Width),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(shotgunButton.Left, riflePicture.Top),
                BackColor = Color.Wheat
            };
            Form.Controls.Add(shotgunPicture);
            controls.Add(shotgunPicture);
        }

        private void InitWeaponCharacteristics()
        {
            handgunCharacteristics = new Label
            {
                Location = new Point(handgunPicture.Left, handgunPicture.Bottom + 60),
                Size = new Size(handgunPicture.Width, handgunButton.Top - handgunPicture.Bottom - 60),
                Text = Resources.Damage__ + Resources.HandgunDamage + "\n\n" +
                       Resources.Ammo__ + Resources.HandgunAmmo + "\n\n" +
                       Resources.Recoil__ + Resources.HandgunRecoil + "\n\n" +
                       Resources.Reload__ + Resources.HandgunReload + "\n\n" +
                       Resources.BulletSpeed__ + Resources.HandgunBulletSpeed,
                BackColor = Color.Wheat,
                Font = characteristicsFont
            };
            Form.Controls.Add(handgunCharacteristics);
            controls.Add(handgunCharacteristics);

            rifleCharacteristics = new Label
            {
                Location = new Point(riflePicture.Left, handgunCharacteristics.Top),
                Size = new Size(riflePicture.Width, rifleButton.Top - riflePicture.Bottom - 60),
                Text = Resources.Damage__ + Resources.RifleDamage + "\n\n" +
                       Resources.Ammo__ + Resources.RifleAmmo + "\n\n" +
                       Resources.Recoil__ + Resources.RifleRecoil + "\n\n" +
                       Resources.Reload__ + Resources.RifleReload + "\n\n" +
                       Resources.BulletSpeed__ + Resources.RifleBulletSpeed,
                BackColor = Color.Wheat,
                Font = characteristicsFont
            };
            Form.Controls.Add(rifleCharacteristics);
            controls.Add(rifleCharacteristics);

            shotgunCharacteristics = new Label
            {
                Location = new Point(shotgunPicture.Left, rifleCharacteristics.Top),
                Size = new Size(shotgunPicture.Width, shotgunButton.Top - shotgunPicture.Bottom - 60),
                Text = Resources.Damage__ + Resources.ShotgunDamage + "\n\n" +
                       Resources.Ammo__ + Resources.ShotgunAmmo + "\n\n" +
                       Resources.Recoil__ + Resources.ShotgunRecoil + "\n\n" +
                       Resources.Reload__ + Resources.ShotgunReload + "\n\n" +
                       Resources.BulletSpeed__ + Resources.ShotgunBulletSpeed,
                BackColor = Color.Wheat,
                Font = characteristicsFont
            };
            Form.Controls.Add(shotgunCharacteristics);
            controls.Add(shotgunCharacteristics);
        }
        internal void Open()
        {
            UpdateButtons();
            if (Initialized) return;

            InitPictures();
            InitWeaponCharacteristics();
            Initialized = true;
        }

        internal void Close()
        {
            foreach (var control in controls)
            {
                Form.Controls.Remove(control);
            }
        }
    }
}
