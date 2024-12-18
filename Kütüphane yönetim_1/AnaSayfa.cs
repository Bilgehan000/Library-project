using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kütüphane_yönetim_1
{
    public partial class AnaSayfa : Form
    {
        KutuphaneDbContext context = new KutuphaneDbContext();
        private string Name;
        private string Email;
        private string Password;
        private int _userId;
        Kullanıcı _kullanıcı;
        Kitap_Yazar _kitapYaz;
        Rezerve _rezerve;

        public AnaSayfa(int userId)
        {
            _rezerve = new Rezerve(context);
            _kitapYaz = new Kitap_Yazar(context);
            _kullanıcı = new Kullanıcı();
            _userId = userId;
            InitializeComponent();
        }

        private async void AnaSayfa_Load(object sender, EventArgs e)
        {
            label8.Text = Name;
            label9.Text = Email;
            label10.Text = Password;

            await LoadAuthors();
            await LoadBooks();
            await LoadLoans();
            await LoadBooksToComboBox();

            await GetUserInfo(_userId);

        }


        public async Task LoadBooksToComboBox()
        {
            var loans = await _kitapYaz.GetAllAsync();
            comboBox1.Items.Clear();

            foreach (var loan in loans)
            {
                comboBox1.Items.Add($"Loan ID: {loan.Id} - User: {loan.BookAuthors}");
            }
        }

        public async Task LoadBooks()
        {
            var Books = await _kitapYaz.GetAllAsync();
            dataGridView1.DataSource = Books;
        }

        public async Task LoadAuthors()
        {
            var authors = await _kitapYaz.GetAuthorsAsync(new Author());

            dataGridView2.DataSource = authors;
        }

        public async Task LoadLoans()
        {
            var loans = await _rezerve.GetLoansAsync();
            dataGridView3.DataSource = loans;
        }

        public async Task GetUserInfo(int userId)
        {
            var userInfo = await _kullanıcı.GetUserById(userId);
            label8.Text = userInfo.Name;
            label9.Text = userInfo.Email;
            label10.Text = userInfo.Password;
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

            var updateduser = new User
            {
                Name = textBox1.Text,
                Email = textBox2.Text,
                Password = textBox3.Text
            };

            try
            {
                await _kullanıcı.Update(updateduser);
                MessageBox.Show("Bilgiler başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                var selectedBookName = comboBox1.SelectedItem.ToString();
                var userId = _userId;

                var user = await context.Users.FindAsync(userId);
                var book = await context.Books.FirstOrDefaultAsync(b => b.Name == selectedBookName);

                if (user == null)
                {
                    MessageBox.Show("Geçersiz kullanıcı.");
                    return;
                }

                if (book == null)
                {
                    MessageBox.Show("Kitap bulunamadı.");
                    return;
                }

                if (!book.IsAvailable)
                {
                    MessageBox.Show("Bu kitap şu anda ödünç alınamaz, başka bir kitap seçin.");
                    return;
                }

                var loan = new Loan
                {
                    UserId = userId,
                    BookId = book.Id,
                    LoanDate = DateTime.Now,
                    ReturnDate = null
                };

                try
                {
                    await _rezerve.AddLoansAsync(loan);

                    book.IsAvailable = false;
                    await context.SaveChangesAsync();

                    label2.Text = $"Loan ID: {loan.Id}, Kitap: {book.Name}, Kullanıcı: {user.Name}";
                    comboBox1.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir kitap seçin.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
