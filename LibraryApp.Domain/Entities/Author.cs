namespace LibraryApp.Domain.Entities;

public class Author
{
    public int Id { get; set; }
    public required string FullName { get; set; }

    // İleride EF Core ilişki kuracak
    public List<Book> Books { get; set; } = new();
}
