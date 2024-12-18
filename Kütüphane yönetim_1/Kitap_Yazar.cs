using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kütüphane_yönetim_1
{
    internal class Kitap_Yazar
    {
        private readonly KutuphaneDbContext context;

        // Constructor'da DbContext enjekte ediliyor
        public Kitap_Yazar(KutuphaneDbContext context)
        {
            this.context = context;
        }

        #region Kitap İşlemleri

        #region Veri Çekme İşlemi
        public async Task<List<Book>> GetAllAsync()
        {
            return await context.Books.ToListAsync();
        }

        #endregion

        #region Veri Ekleme İşlemi
        public async Task Add(Book book)
        {
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
        }
        #endregion

        #region Veri Silme İşlemi
        public async Task Remove(int bookId)
        {
            var bookToRemove = await context.Books
                .FirstOrDefaultAsync(m => m.Id == bookId);

            if (bookToRemove != null)
            {
                context.Books.Remove(bookToRemove);
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region Veri Güncelleme İşlemi
        public async Task Update(Book book)
        {
            var existingBook = await context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == book.Id);

            if (existingBook != null)
            {
                existingBook.Name = book.Name;
                existingBook.PageCount = book.PageCount;
                existingBook.IsAvailable = book.IsAvailable;

                // Kitap yazarlarını güncelle
                existingBook.BookAuthors.Clear();
                foreach (var author in book.BookAuthors)
                {
                    existingBook.BookAuthors.Add(author);
                }

                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Kitap bulunamadı.");
            }
        }

        public async Task<Book> GetBookById(int BookId)
        {
            var book = await context.Books
                .FirstOrDefaultAsync(u => u.Id == BookId);

            return book;
        }

        #endregion

        #endregion

        #region Yazar İşlemleri

        #region Veri Çekme İşlemi
        public async Task<List<Author>> GetAuthorsAsync(Author author)
        {
            using (var context = new KutuphaneDbContext())
            {
                IQueryable<Author> query = context.Authors;

                if (author != null && !string.IsNullOrEmpty(author.Name))
                {
                    query = query.Where(a => a.Name.Contains(author.Name));
                }

                return await query.ToListAsync();
            }
        }
        #endregion

        #region Veri Ekleme İşlemi
        public async Task AddAuthor(Author author)
        {
            await context.Authors.AddAsync(author);
            await context.SaveChangesAsync();
        }
        #endregion

        #region Veri Silme İşlemi
        public async Task RemoveAuthor(int authorId)
        {
            var authorToRemove = await context.Authors
                .FirstOrDefaultAsync(m => m.Id == authorId);

            if (authorToRemove != null)
            {
                context.Authors.Remove(authorToRemove);
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region Veri Güncelleme İşlemi
        public async Task UpdateAuthor(int authorId, string name, int numberOfWorks)
        {
            var author = await context.Authors
                .FirstOrDefaultAsync(a => a.Id == authorId);

            if (author != null)
            {
                author.Name = name;
                author.NumberOfWorks = numberOfWorks;

                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Yazar bulunamadı.");
            }
        }
        #endregion

        #endregion

        #region Kitap ve Yazar İlişkilendirme
        public async Task AddBookAuthor(BookAuthor bookAuthor)
        {
            await context.BookAuthors.AddAsync(bookAuthor);
            await context.SaveChangesAsync();
        }
        #endregion

    }
}
