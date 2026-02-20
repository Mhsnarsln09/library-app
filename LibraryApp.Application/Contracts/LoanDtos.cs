namespace LibraryApp.Application.Contracts;

public record LoanListItemDto(
    int Id,
    string BookTitle,
    string MemberFullName,
    DateTime LoanedAtUtc,
    DateTime DueAtUtc,
    bool IsReturned,
    bool IsOverdue
);

public record LoanDetailDto(
    int Id,
    BookListItemDto Book,
    MemberListItemDto Member,
    DateTime LoanedAtUtc,
    DateTime DueAtUtc,
    DateTime? ReturnedAtUtc,
    bool IsReturned,
    bool IsOverdue
);

public record CreateLoanDto(int BookId, int MemberId);
