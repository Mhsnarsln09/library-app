using LibraryApp.Domain.Common;

namespace LibraryApp.Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }

    public List<Book> Books { get; set; } = new();
}
