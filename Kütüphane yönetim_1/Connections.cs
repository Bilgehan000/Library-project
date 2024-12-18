using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kütüphane_yönetim_1
{
    class Manager
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    class User
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; } 
        public string Password { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }

    class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int stok {  get; set; }
        public string PageCount { get; set; } 
        public bool IsAvailable { get; set; }

        public ICollection<Loan> Loans { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }

    class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfWorks { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }
    }

    class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; } 

        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    class BookAuthor
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    internal class KutuphaneDbContext : DbContext
    {
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Manager> Managers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = BILGEHAN\\MSSQLSERVER01; Database = KutuphaneDb; Integrated Security = True; TrustServerCertificate = True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loan>()
         .HasKey(l => new { l.UserId, l.BookId });

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);


            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId);
        }
    }
}
