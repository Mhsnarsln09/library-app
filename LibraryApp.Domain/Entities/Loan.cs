using LibraryApp.Domain.Common;

namespace LibraryApp.Domain.Entities;

public class Loan : BaseEntity
{
    public int BookId { get; set; }
    public Book? Book { get; set; }

    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public DateTime LoanedAtUtc { get; set; } = DateTime.UtcNow;

    // Kural: default 14 gün sonra iade bekleriz (şimdilik sabit)
    public DateTime DueAtUtc { get; set; } = DateTime.UtcNow.AddDays(14);

    public DateTime? ReturnedAtUtc { get; set; }

    public bool IsReturned => ReturnedAtUtc is not null;
    public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueAtUtc;
    
    public decimal PenaltyAmount { get; set; }

    public void MarkReturned() => ReturnedAtUtc = DateTime.UtcNow;
}
