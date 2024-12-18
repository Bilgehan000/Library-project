using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Kütüphane_yönetim_1
{
    public partial class Sign_Up : Form
    {

        Kullanıcı _kullanıcı;

        public Sign_Up()
        {
            _kullanıcı = new Kullanıcı();
            InitializeComponent();
        }

        private void Sign_Up_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text;
            string userEmail = textBox2.Text;
            string userPassword = textBox3.Text;

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Name boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(userEmail))
            {
                MessageBox.Show("E-mail boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(userPassword))
            {
                MessageBox.Show("Password boş olamaz");
                return;
            }

            KullanıcıBilgisi kullanıcıBilgisi = new KullanıcıBilgisi(userName, userEmail, userPassword);

            User user = new User
            {
                Name = kullanıcıBilgisi.Name,
                Email = kullanıcıBilgisi.Email,
                Password = kullanıcıBilgisi.Password
            };

            await _kullanıcı.AddUsers(user);

            Login login = new Login(kullanıcıBilgisi);
            login.Show();
            this.Hide();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }
    }
}

