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

namespace Kütüphane_yönetim_1
{
    public partial class Admin : Form
    {
        KutuphaneDbContext context = new KutuphaneDbContext();
        Kullanıcı _kullanıcı;
        Yonetim _yonetim;
        Rezerve _rezerve;
        Kitap_Yazar _kitapyaz;
        public Admin(KullanıcıBilgisi kullanıcıBilgisi)
        {
            _kullanıcı = new Kullanıcı();
            _yonetim = new Yonetim();
            _rezerve = new Rezerve(context);
            _kitapyaz = new Kitap_Yazar(context);
            InitializeComponent();
        }

        private async void Admin_Load(object sender, EventArgs e)
        {

            await Yukle();

        }

        #region comboboxlar ve datagridviewler


        public async Task Yukle()
        {
            // comboboxlar
            await Yazarlar();
            await Kullanıcılar();
            await Kitaplar();
            // datagridview
            await LoanListe();
            await LoadBooks();
            await LoadAuthors();
            await LoadUser();
        }


        public async Task LoadBooks()
        {
            var Books = await _kitapyaz.GetAllAsync();
            dataGridView1.DataSource = Books;
        }

        public async Task LoadAuthors()
        {
            var authors = await _kitapyaz.GetAuthorsAsync(new Author());

            dataGridView2.DataSource = authors;
        }

        public async Task Yazarlar()
        {
            List<Author> authors = await _kitapyaz.GetAuthorsAsync(new Author());

            comboBox1.Items.Clear();

            foreach (var author in authors)
            {
                comboBox1.Items.Add(author.Name);
            }
        }

        public async Task Kullanıcılar()
        {
            var users = await _kullanıcı.GetUsersAsync(new User());

            comboBox2.DataSource = users;
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "Id";
        }

        public async Task Kitaplar()
        {
            var books = await _kitapyaz.GetAllAsync();

            comboBox3.DataSource = books;
            comboBox3.DisplayMember = "Name";
            comboBox3.ValueMember = "Id";
        }

        public async Task LoadUser()
        {
            var loans = await _kullanıcı.GetUsersAsync(new User());

            dataGridView4.DataSource = loans;
        }

        public async Task LoanListe()
        {
            var loans = await _rezerve.GetLoansAsync();

            dataGridView3.DataSource = loans;
        }

        #endregion

        #region Kitap Ekleme Silme Güncelleme
        #region ekleme
        private async void button1_Click(object sender, EventArgs e)
        {
            string Name = textBox1.Text;
            string PageCount = textBox2.Text;
            string Stok = textBox3.Text;
            string Author = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("İsim boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(PageCount))
            {
                MessageBox.Show("Sayfa sayısı boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(Stok))
            {
                MessageBox.Show("Stok sayısı boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(Author))
            {
                MessageBox.Show("Stok sayısı boş olamaz");
                return;
            }

            var existingAuthor = (await _kitapyaz.GetAuthorsAsync(new Author()))
                                                 .FirstOrDefault(a => a.Name == Author);

            Author authorEntity;
            if (existingAuthor == null)
            {
                authorEntity = new Author
                {
                    Name = Author,
                    NumberOfWorks = 1
                };

                await _kitapyaz.AddAuthor(authorEntity);
            }
            else
            {
                authorEntity = existingAuthor;
            }


            var newBook = new Book
            {
                Name = Name,
                PageCount = PageCount,
                stok = int.Parse(Stok),
                IsAvailable = true
            };

            Book bookentity = (await _kitapyaz.GetAllAsync())
                                              .FirstOrDefault(b => b.Name == newBook.Name);

            if (bookentity != null)
            {
                MessageBox.Show("Kitap Zaten Mevcut");
            }
            else
            {
                await _kitapyaz.Add(newBook);
                MessageBox.Show("Kitap başarıyla eklendi.");
            }

            var bookAuthorRelation = new BookAuthor
            {
                BookId = newBook.Id,
                AuthorId = authorEntity.Id
            };

            await _kitapyaz.AddBookAuthor(bookAuthorRelation);
            await Yukle();

            MessageBox.Show("Kitap başarıyla eklendi.");
        }
        #endregion

        #region Silme
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kitabı seçin.");
                return;
            }

            var selectedBook = (Book)dataGridView1.SelectedRows[0].DataBoundItem;

            try
            {
                if (selectedBook == null)
                {
                    MessageBox.Show("Silinecek kitap seçilmedi.");
                    return;
                }

                context.Books.Remove(selectedBook);

                await context.SaveChangesAsync();
                await Yukle();
                MessageBox.Show("Kitap başarıyla silindi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region Günceleme
        private async void button3_Click(object sender, EventArgs e)
        {
            string Name = textBox1.Text;
            string PageCount = textBox2.Text;
            string Stok = textBox3.Text;
            string Author = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("İsim boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(PageCount))
            {
                MessageBox.Show("Sayfa sayısı boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(Stok))
            {
                MessageBox.Show("Stok sayısı boş olamaz");
                return;
            }
            if (string.IsNullOrEmpty(Author))
            {
                MessageBox.Show("Geçerli bir yazar seçilmedi");
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellemek için bir kitap seçin.");
                return;
            }

            var selectedBook = (Book)dataGridView1.SelectedRows[0].DataBoundItem;

            selectedBook.Name = Name;
            selectedBook.PageCount = PageCount;
            selectedBook.stok = Convert.ToInt32(Stok);

            if (selectedBook.BookAuthors == null)
            {
                selectedBook.BookAuthors = new List<BookAuthor>();
            }

            var selectedAuthorName = comboBox1.SelectedItem.ToString();

            var author = await context.Authors.FirstOrDefaultAsync(a => a.Name == selectedAuthorName);

            if (author != null)
            {
                var bookAuthor = new BookAuthor
                {
                    BookId = selectedBook.Id,
                    AuthorId = author.Id,
                    Book = selectedBook,
                    Author = author
                };

                selectedBook.BookAuthors.Clear();
                selectedBook.BookAuthors.Add(bookAuthor);
            }
            else
            {
                MessageBox.Show("Geçerli bir yazar seçilmedi.");
                return;
            }

            try
            {
                await _kitapyaz.Update(selectedBook);
                await Yukle();
                MessageBox.Show("Güncelleme Başarıyla Tamamlanmıştır.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #region Yazar Ekleme Silme Güncelleme

        #region Ekleme
        private async void button9_Click(object sender, EventArgs e)
        {
            string Name = textBox9.Text;
            string NumberOfWorks = textBox8.Text;

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("İsim boş olamaz");
                return;
            }

            if (string.IsNullOrEmpty(NumberOfWorks) || !int.TryParse(NumberOfWorks, out int numberOfWorks))
            {
                MessageBox.Show("Geçerli bir eser sayısı girin");
                return;
            }

            var existingAuthor = (await _kitapyaz.GetAuthorsAsync(new Author()))
                                                  .FirstOrDefault(a => a.Name == Name);

            if (existingAuthor != null)
            {
                existingAuthor.NumberOfWorks += numberOfWorks;

                try
                {
                    await _kitapyaz.UpdateAuthor(existingAuthor.Id, existingAuthor.Name, existingAuthor.NumberOfWorks);
                    MessageBox.Show("Yazar zaten mevcut, eser sayısı güncellendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Yazar güncellenirken bir hata oluştu: {ex.Message}");
                }
            }
            else
            {
                var newAuthor = new Author
                {
                    Name = Name,
                    NumberOfWorks = numberOfWorks
                };

                try
                {
                    await _kitapyaz.AddAuthor(newAuthor);
                    await Yukle();
                    MessageBox.Show("Yeni yazar başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Yazar eklenirken bir hata oluştu: {ex.Message}");
                }
            }

        }
        #endregion

        #region silme
        private async void button8_Click(object sender, EventArgs e)
        {

            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kitabı seçin.");
                return;
            }

            var selectedAuthor = (Author)dataGridView2.SelectedRows[0].DataBoundItem;

            try
            {
                context.Authors.Remove(selectedAuthor);

                await context.SaveChangesAsync();
                await Yukle();
                MessageBox.Show("Kitap başarıyla silindi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region Güncelleme

        private async void button7_Click(object sender, EventArgs e)
        {
            string Name = textBox9.Text;
            string NumberOfWorks = textBox8.Text;

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("İsim boş olamaz");
                return;
            }

            if (string.IsNullOrEmpty(NumberOfWorks) || !int.TryParse(NumberOfWorks, out int numberOfWorks))
            {
                MessageBox.Show("Geçerli bir eser sayısı girin");
                return;
            }

            var existingAuthor = (await _kitapyaz.GetAuthorsAsync(new Author()))
                                                      .FirstOrDefault(a => a.Name == Name);

            if (existingAuthor != null)
            {
                existingAuthor.NumberOfWorks += numberOfWorks;

                try
                {
                    await _kitapyaz.UpdateAuthor(existingAuthor.Id, existingAuthor.Name, existingAuthor.NumberOfWorks);
                    MessageBox.Show("Yazar zaten mevcut, eser sayısı güncellendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Yazar güncellenirken bir hata oluştu: {ex.Message}");
                }
            }
            else
            {
                var newAuthor = new Author
                {
                    Name = Name,
                    NumberOfWorks = numberOfWorks
                };

                try
                {
                    await _kitapyaz.AddAuthor(newAuthor);
                    await Yukle();
                    MessageBox.Show("Yeni yazar başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Yazar eklenirken bir hata oluştu: {ex.Message}");
                }
            }
        }
        #endregion

        #endregion

        private async void button13_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Rezerve Ekleme Silme Güncelleme

        #region Ekleme
        private async void button6_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.");
                return;
            }

            if (comboBox3.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir kitap seçin.");
                return;
            }

            int userId = Convert.ToInt32(comboBox2.SelectedValue);
            int bookId = Convert.ToInt32(comboBox3.SelectedValue);

            DateTime loanDate = dateTimePicker1.Value;
            DateTime returnDate = dateTimePicker2.Value;

            var loan = new Loan
            {
                UserId = userId,
                BookId = bookId,
                LoanDate = loanDate,
                ReturnDate = null
            };

            try
            {
                await _rezerve.AddLoansAsync(loan);
                await Yukle();
                MessageBox.Show("Kitap başarıyla ödünç alındı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rezerve eklenirken bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region Güncelleme
        private async void button4_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(comboBox2.SelectedValue);
            int bookId = Convert.ToInt32(comboBox3.SelectedValue);
            DateTime newLoanDate = dateTimePicker1.Value;
            DateTime? returnDate = dateTimePicker2.Value;

            try
            {
                await _rezerve.UpdateLoanAsync(userId, bookId, newLoanDate, returnDate);
                await Yukle();
                MessageBox.Show("Ödünç alma kaydı güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ödünç güncellenirken bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region Silme
        private async void button5_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(comboBox2.SelectedValue);
            int bookId = Convert.ToInt32(comboBox3.SelectedValue);

            await _rezerve.RemoveLoanAsync(userId, bookId);
            await Yukle();
            MessageBox.Show("Ödünç alma kaydı başarıyla silindi.");
        }
        #endregion
        #endregion

        #region Kullanıcı Ekleme Silme Güncelleme

        #region Ekle
        private async void button12_Click(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellenecek bir kullanıcı seçin.");
                return;
            }

            string name = textBox12.Text;
            string email = textBox11.Text;
            string password = textBox10.Text;

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
                int userId = Convert.ToInt32(dataGridView4.SelectedRows[0].Cells["Id"].Value);

                var updateduser = new User
                {
                    Id = userId,
                    Name = name,
                    Email = email,
                    Password = password
                };

                await _kullanıcı.Update(updateduser);

                await Yukle();

                MessageBox.Show("Bilgiler başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region Güncelle
        private async void button10_Click(object sender, EventArgs e)
        {
            string name = textBox12.Text;
            string email = textBox11.Text;
            string password = textBox10.Text;

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
                Name = textBox12.Text,
                Email = textBox11.Text,
                Password = textBox10.Text
            };

            try
            {
                await _kullanıcı.Update(updateduser);
                await Yukle();
                MessageBox.Show("Bilgiler başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region Silme
        private async void button11_Click(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz rezerveyi seçin.");
                return;
            }

            var selectedUser = (User)dataGridView4.SelectedRows[0].DataBoundItem;

            try
            {
                context.Users.Remove(selectedUser);

                await context.SaveChangesAsync();
                await Yukle();

                MessageBox.Show("Rezerve başarıyla silindi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #region DataGridViewler

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedBook = (Book)dataGridView1.SelectedRows[0].DataBoundItem;

                textBox1.Text = selectedBook.Name;
                textBox2.Text = selectedBook.PageCount;
                textBox3.Text = selectedBook.stok.ToString();

                comboBox1.SelectedItem = selectedBook.BookAuthors?.FirstOrDefault()?.Author?.Name;
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                var selectedAuthor = (Author)dataGridView2.SelectedRows[0].DataBoundItem;

                textBox9.Text = selectedAuthor.Name;
                textBox8.Text = selectedAuthor.NumberOfWorks.ToString();
                textBox7.Text = selectedAuthor.BookAuthors?.Count.ToString();
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedBook = (Book)dataGridView3.Rows[e.RowIndex].DataBoundItem;

                if (selectedBook != null && selectedBook.IsAvailable && selectedBook.stok > 0)
                {
                    try
                    {
                        int userId = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["UserId"].Value);

                        DateTime loanDate = DateTime.Now;

                        using (var context = new KutuphaneDbContext())
                        {
                            var loan = new Loan
                            {
                                UserId = userId,
                                BookId = selectedBook.Id,
                                LoanDate = loanDate,
                                ReturnDate = null
                            };

                            context.Loans.Add(loan);

                            var bookToUpdate = context.Books.Find(selectedBook.Id);
                            if (bookToUpdate != null)
                            {
                                bookToUpdate.stok -= 1;

                                if (bookToUpdate.stok == 0)
                                {
                                    bookToUpdate.IsAvailable = false;
                                }

                                context.SaveChanges();
                            }

                            MessageBox.Show("Rezervasyon başarıyla tamamlandı.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Rezervasyon sırasında bir hata oluştu: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Seçilen kitap rezerve edilemez. Stok tükenmiş olabilir.");
                }
            }
        }

        private async void button15_Click(object sender, EventArgs e)
        {
            await Yukle();
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                try
                {
                    var selecteduser = (User)dataGridView4.SelectedRows[0].DataBoundItem;

                    textBox12.Text = selecteduser.Name;
                    textBox11.Text = selecteduser.Email;
                    textBox10.Text = selecteduser.Password;

                    Console.WriteLine($"Seçilen Kullanıcı: {selecteduser.Name}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.");
            }
        }
        #endregion
    }
}
    

