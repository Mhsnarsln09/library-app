using LibraryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Abstractions;

namespace LibraryApp.Infrastructure.Data;

public class LibraryDbContext : DbContext, ILibraryDb
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FullName = "George Orwell" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Dystopian" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "1984",
                Isbn = "978-0451524935",
                TotalCopies = 3,
                AuthorId = 1,
                CategoryId = 1
            }
            
        );

        modelBuilder.Entity<Member>().HasData(
            new Member { Id = 1, FullName = "Ada Lovelace", Email = "ada@example.com" }
        );
    }
}
