using LibraryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Application.Abstractions;

public interface ILibraryDb
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Book> Books { get; set; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
