namespace LibraryApp.Application.Contracts;

public class MemberPenaltyDto
{
    public int MemberId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal TotalPenalty { get; set; }
    public List<PenaltyDetailDto> OverdueLoans { get; set; } = new();
}

public class PenaltyDetailDto
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public DateTime DueAtUtc { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }
    public decimal PenaltyAmount { get; set; }
    public int OverdueDays { get; set; }
}
