using LibraryApp.Domain.Common;
using LibraryApp.Domain.Constants;

namespace LibraryApp.Domain.Entities;

public class Member : BaseEntity
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Member;

    public List<Loan> Loans { get; set; } = new();
    
    public decimal TotalPenalty { get; set; }
}
