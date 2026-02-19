namespace LibraryApp.Domain.Entities;

public class Book
{
    public int Id { get; set; }

    public required string Title { get; set; }
    public string? Isbn { get; set; }

    // Stok: aynı kitaptan kaç adet var
    public int TotalCopies { get; set; } = 1;

    // Navigation + FK'ler (Sprint 2'de DB’ye gidecek)
    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<Loan> Loans { get; set; } = new();
    
    public int PublishedYear { get; set; }
}
