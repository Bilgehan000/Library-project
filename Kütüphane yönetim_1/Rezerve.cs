using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kütüphane_yönetim_1
{
    internal class Rezerve
    {
        private readonly KutuphaneDbContext _context;

        // Constructor: Dependency Injection (DI) ile _context'i alıyoruz.
        public Rezerve(KutuphaneDbContext context)
        {
            _context = context;
        }

        #region Rezerve İşlemleri

        #region Veri Çekme İşlemi
        public async Task<List<Loan>> GetLoansAsync()
        {
            try
            {
                return await _context.Loans
                                     .Include(l => l.Book)
                                     .Include(l => l.User)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new List<Loan>();
            }
        }
        #endregion

        #region Veri Ekleme İşlemi
        public async Task AddLoansAsync(Loan loan)
        {
            var user = await _context.Users.FindAsync(loan.UserId);
            var book = await _context.Books.FindAsync(loan.BookId);

            if (user == null)
            {
                throw new ArgumentException("Geçersiz kullanıcı ID.");
            }

            if (book == null)
            {
                throw new ArgumentException("Geçersiz kitap ID.");
            }

            if (!book.IsAvailable || book.stok <= 0)
            {
                throw new InvalidOperationException("Kitap mevcut değil veya stoğu tükenmiş.");
            }

            try
            {
                book.stok -= 1;
                if (book.stok == 0)
                {
                    book.IsAvailable = false;
                }

                loan.LoanDate = DateTime.Now;
                loan.ReturnDate = null;

                await _context.Loans.AddAsync(loan);

                await _context.SaveChangesAsync();

                Console.WriteLine("Ödünç alma kaydı başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri ekleme hatası: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Veri Silme İşlemi
        public async Task RemoveLoanAsync(int userId, int bookId)
        {
            var loan = await _context.Loans
                                      .FirstOrDefaultAsync(l => l.UserId == userId && l.BookId == bookId);

            if (loan == null)
            {
                Console.WriteLine("Ödünç alma kaydı bulunamadı.");
                return;
            }

            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book != null)
                {
                    book.IsAvailable = true; 
                }

                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
                Console.WriteLine("Ödünç alma kaydı başarıyla silindi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri silme hatası: {ex.Message}");
            }
        }
        #endregion

        #region Veri Güncelleme İşlemi
        public async Task UpdateLoanAsync(int userId, int bookId, DateTime newLoanDate, DateTime? newReturnDate = null)
        {
            var loan = await _context.Loans
                                      .FirstOrDefaultAsync(l => l.UserId == userId && l.BookId == bookId);

            if (loan == null)
            {
                Console.WriteLine("Güncellenmek istenen ödünç alma kaydı bulunamadı.");
                return;
            }

            try
            {
                loan.LoanDate = newLoanDate;
                loan.ReturnDate = newReturnDate;

                if (newReturnDate.HasValue)
                {
                    var book = await _context.Books.FindAsync(bookId);
                    if (book != null)
                    {
                        book.IsAvailable = true;
                    }
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("Ödünç alma kaydı başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri güncelleme hatası: {ex.Message}");
            }
        }
        #endregion

        #endregion
    }
}