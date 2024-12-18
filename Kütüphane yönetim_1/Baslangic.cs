using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Kütüphane_yönetim_1
{
    public partial class Baslangic : Form
    {
        KutuphaneDbContext context = new KutuphaneDbContext();
        Rezerve _rezerve;
        Kitap_Yazar _kitapYaz;
        public Baslangic()
        {
            context = new KutuphaneDbContext();
            _rezerve = new Rezerve(context);
            _kitapYaz = new Kitap_Yazar(context);
            InitializeComponent();
        }

        private async void Baslangic_Load(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            LoadBooks();
            await LoadLoans();
            await LoadAuthors();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            KullanıcıBilgisi kullanıcıBilgisi = new KullanıcıBilgisi("Kullanıcı Adı", "email@example.com", "parola");

            Login lgn = new Login(kullanıcıBilgisi);
            lgn.Show();
            this.Hide();
        }

        private async void LoadBooks()
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

        #region Tek görevli Buttonlar

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = true;
            groupBox2.Visible = false;
            groupBox4.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox4.Visible = true;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
        }
        #endregion

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}
