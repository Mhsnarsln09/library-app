using LibraryApp.Domain.Common;

namespace LibraryApp.Domain.Entities;

public class Book : BaseEntity
{
    public required string Title { get; set; }
    public string? Isbn { get; set; }

    // Stok: aynı kitaptan kaç adet var
    public int TotalCopies { get; set; } = 1;

    // Navigation + FK'ler
    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<Loan> Loans { get; set; } = new();
    
    public int PublishedYear { get; set; }
}
