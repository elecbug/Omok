using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omok
{
    public partial class LoginForm : Form
    {
        private Button GameStartButton { get; set; }

        public LoginForm()
        {
            InitializeComponent();

            ClientSize = new Size(400, 600);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            GameStartButton = new Button()
            {
                Parent = this,
                Visible = true,
                Location = new Point(10, 560),
                Size = new Size(380, 30),
                Text = "Matching",
            };
            GameStartButton.Click += GameStartButtonClick;

            if (File.Exists("ip-port") == false )
            {
                File.Create("ip-port");
            }
        }

        private void GameStartButtonClick(object? sender, EventArgs e)
        {
            MainForm form = new MainForm(this);

            Hide();

            form.Show();
        }
    }
}
