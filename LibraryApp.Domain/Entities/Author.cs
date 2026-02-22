using LibraryApp.Domain.Common;

namespace LibraryApp.Domain.Entities;

public class Author : BaseEntity
{
    public required string FullName { get; set; }

    // İleride EF Core ilişki kuracak
    public List<Book> Books { get; set; } = new();
}
