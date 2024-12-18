using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kütüphane_yönetim_1
{
    public partial class Login : Form
    {
        Yonetim _yonetim;
        Kullanıcı _kullanıcı;
        private KullanıcıBilgisi _kullanıcıBilgisi;

        public Login(KullanıcıBilgisi kullanıcıBilgisi)
        {
            _kullanıcıBilgisi = kullanıcıBilgisi;
            _kullanıcı = new Kullanıcı();
            _yonetim = new Yonetim();
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string email = textBox2.Text;
            string password = textBox3.Text;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Name boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("E-mail boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password boş olamaz");
                return;
            }

            int userId = await _kullanıcı.GetUserId(email);
            try
            {
                bool basariliGiris = await _kullanıcı.Login(name, email, password);
                if (basariliGiris)
                {
                    AnaSayfa Ana = new AnaSayfa(userId);
                    Ana.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("E-posta veya şifre hatalı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            Sign_Up sign = new Sign_Up();
            sign.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int userId = 0;
            AnaSayfa ana = new AnaSayfa(userId);
            ana.Show();
            this.Close();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            //Admin ad = new Admin(_kullanıcıBilgisi);
            //ad.Show();
            //this.Close();


            string name = textBox1.Text;
            string email = textBox2.Text;
            string password = textBox3.Text;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Name boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("E-mail boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password boş olamaz");
                return;
            }

            try
            {
                bool basariliGiris = await _yonetim.Login(name, email, password);
                if (basariliGiris)
                {
                    Admin admin = new Admin(_kullanıcıBilgisi);
                    admin.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("E-posta veya şifre hatalı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
