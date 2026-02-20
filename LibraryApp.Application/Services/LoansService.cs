using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Services;

public class LoansService(ILibraryDb _db)
{
    public async Task<ApiResponse> CreateLoanAsync(int bookId, int memberId, CancellationToken ct = default)
    {
        var book = await _db.Books.FindAsync(new object[] { bookId }, ct);
        if (book == null)
            return ApiResponse.Failure($"Book with ID {bookId} not found.");

        var member = await _db.Members.FindAsync(new object[] { memberId }, ct);
        if (member == null)
            return ApiResponse.Failure($"Member with ID {memberId} not found.");

        var alreadyBorrowed = await _db.Loans.AnyAsync(
            l => l.BookId == bookId && l.MemberId == memberId && l.ReturnedAtUtc == null, ct);
        if (alreadyBorrowed)
            return ApiResponse.Failure("This member already has an active loan for this book.");

        var activeLoanCount = await _db.Loans.CountAsync(l => l.BookId == bookId && l.ReturnedAtUtc == null, ct);
        if (activeLoanCount >= book.TotalCopies)
            return ApiResponse.Failure("No available copies of this book. All copies are currently on loan.");

        var loan = new Loan { BookId = bookId, MemberId = memberId };
        await _db.Loans.AddAsync(loan, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Loan created with ID {loan.Id}.");
    }

    public async Task<ApiResponse<List<LoanListItemDto>>> GetLoansAsync(CancellationToken ct = default)
    {
        var loans = await _db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanedAtUtc)
            .Select(l => new LoanListItemDto(
                l.Id,
                l.Book!.Title,
                l.Member!.FullName,
                l.LoanedAtUtc,
                l.DueAtUtc,
                l.IsReturned,
                l.IsOverdue
            ))
            .ToListAsync(ct);

        return ApiResponse<List<LoanListItemDto>>.Success(loans);
    }

    public async Task<ApiResponse<LoanDetailDto>> GetLoanByIdAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans
            .Include(l => l.Book)
                .ThenInclude(b => b!.Author)
            .Include(l => l.Book)
                .ThenInclude(b => b!.Category)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (loan == null)
            return ApiResponse<LoanDetailDto>.Failure($"Loan with ID {id} not found.");

        var bookDto = new BookListItemDto(
            loan.Book!.Id,
            loan.Book.Title,
            loan.Book.TotalCopies,
            new AuthorDetailDto(loan.Book.Author!.Id, loan.Book.Author.FullName),
            new CategoryListItemDto(loan.Book.Category!.Id, loan.Book.Category.Name)
        );

        var memberDto = new MemberListItemDto(loan.Member!.Id, loan.Member.FullName, loan.Member.Email);

        var loanDetail = new LoanDetailDto(
            loan.Id,
            bookDto,
            memberDto,
            loan.LoanedAtUtc,
            loan.DueAtUtc,
            loan.ReturnedAtUtc,
            loan.IsReturned,
            loan.IsOverdue
        );

        return ApiResponse<LoanDetailDto>.Success(loanDetail);
    }

    public async Task<ApiResponse> ReturnLoanAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans.FindAsync(new object[] { id }, ct);
        if (loan == null)
            return ApiResponse.Failure($"Loan with ID {id} not found.");

        if (loan.IsReturned)
            return ApiResponse.Failure($"Loan with ID {id} has already been returned.");

        loan.MarkReturned();
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Loan with ID {id} returned successfully.");
    }

    public async Task<ApiResponse> DeleteLoanAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans.FindAsync(new object[] { id }, ct);
        if (loan == null)
            return ApiResponse.Failure($"Loan with ID {id} not found.");

        if (!loan.IsReturned)
            return ApiResponse.Failure("Cannot delete an active loan. Please return the book first.");

        _db.Loans.Remove(loan);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Loan with ID {id} deleted successfully.");
    }
}
