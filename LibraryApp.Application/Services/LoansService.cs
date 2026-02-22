using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using LibraryApp.Application.Contracts;
using LibraryApp.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Common.Pagination;

using AutoMapper;

namespace LibraryApp.Application.Services;

public class LoansService(ILibraryDb _db, IMapper _mapper)
{
    private const decimal DailyPenaltyRate = 1.50m;
    public async Task<ApiResponse> CreateLoanAsync(int bookId, int memberId, CancellationToken ct = default)
    {
        var book = await _db.Books.FindAsync(new object[] { bookId }, ct);
        if (book == null)
            throw new NotFoundException($"Book with ID {bookId} not found.");

        var member = await _db.Members.FindAsync(new object[] { memberId }, ct);
        if (member == null)
            throw new NotFoundException($"Member with ID {memberId} not found.");

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

    public async Task<ApiResponse<PagedResult<LoanListItemDto>>> GetLoansAsync(PaginationFilter filter, CancellationToken ct = default)
    {
        var query = _db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member);

        var totalCount = await query.CountAsync(ct);

        var loans = await query
            .OrderByDescending(l => l.LoanedAtUtc)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        var dtos = _mapper.Map<List<LoanListItemDto>>(loans);
        var pagedResult = new PagedResult<LoanListItemDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
        return ApiResponse<PagedResult<LoanListItemDto>>.Success(pagedResult);
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
            throw new NotFoundException($"Loan with ID {id} not found.");

        var loanDetail = _mapper.Map<LoanDetailDto>(loan);
        return ApiResponse<LoanDetailDto>.Success(loanDetail);
    }

    public async Task<ApiResponse> ReturnLoanAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
        if (loan == null)
            throw new NotFoundException($"Loan with ID {id} not found.");

        if (loan.IsReturned)
            return ApiResponse.Failure($"Loan with ID {id} has already been returned.");

        loan.MarkReturned();

        // Calculate overdue penalty
        if (loan.ReturnedAtUtc > loan.DueAtUtc)
        {
            var overdueDays = (int)Math.Ceiling((loan.ReturnedAtUtc!.Value - loan.DueAtUtc).TotalDays);
            loan.PenaltyAmount = overdueDays * DailyPenaltyRate;
            
            if (loan.Member != null)
                loan.Member.TotalPenalty += loan.PenaltyAmount;
        }

        await _db.SaveChangesAsync(ct);
        
        var message = loan.PenaltyAmount > 0
            ? $"Loan with ID {id} returned with a penalty of {loan.PenaltyAmount:F2} TL ({(int)Math.Ceiling((loan.ReturnedAtUtc!.Value - loan.DueAtUtc).TotalDays)} days overdue)."
            : $"Loan with ID {id} returned successfully.";

        return ApiResponse.Success(message);
    }

    public async Task<ApiResponse> DeleteLoanAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans.FindAsync(new object[] { id }, ct);
        if (loan == null)
            throw new NotFoundException($"Loan with ID {id} not found.");

        if (!loan.IsReturned)
            return ApiResponse.Failure("Cannot delete an active loan. Please return the book first.");

        loan.IsDeleted = true;
        loan.DeletedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Loan with ID {id} deleted successfully.");
    }
}
