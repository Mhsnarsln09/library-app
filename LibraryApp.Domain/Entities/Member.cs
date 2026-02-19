namespace LibraryApp.Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }

    public List<Loan> Loans { get; set; } = new();
}
