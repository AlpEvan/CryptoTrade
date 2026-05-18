using System;
using System.Drawing;
using System.Windows.Forms;

namespace CryptoTrade
{
    public partial class LoginForm : Form
    {
        public int UserId { get; private set; }
        private DatabaseService _db = new DatabaseService(password: "Super");

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;

        public LoginForm()
        {
            InitializeCustomComponent();
        }

        private void InitializeCustomComponent()
        {
            this.Text = "CryptoTrade — Connexion";
            this.Size = new Size(350, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 18, 26);

            var lblUser = new Label { Text = "Pseudo", Top = 50, Left = 50, ForeColor = Color.White };
            txtUsername = new TextBox { Top = 75, Left = 50, Width = 230 };

            var lblPass = new Label { Text = "Mot de passe", Top = 120, Left = 50, ForeColor = Color.White };
            txtPassword = new TextBox { Top = 145, Left = 50, Width = 230, PasswordChar = '*' };

            btnLogin = new Button { Text = "Se connecter", Top = 210, Left = 50, Width = 230, BackColor = Color.FromArgb(14, 203, 129), FlatStyle = FlatStyle.Flat };
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button { Text = "Créer un compte", Top = 250, Left = 50, Width = 230, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRegister.Click += BtnRegister_Click;

            this.Controls.Add(lblUser); this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass); this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin); this.Controls.Add(btnRegister);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            int? id = _db.VerifyUser(txtUsername.Text, txtPassword.Text);
            if (id.HasValue) { this.UserId = id.Value; this.DialogResult = DialogResult.OK; this.Close(); }
            else MessageBox.Show("Erreur de connexion.");
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (_db.CreateUser(txtUsername.Text, txtPassword.Text)) MessageBox.Show("Compte créé !");
            else MessageBox.Show("Pseudo déjà pris.");
        }
    }
}