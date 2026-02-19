namespace LibraryApp.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<Book> Books { get; set; } = new();
}
