using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Common.Exceptions;
using LibraryApp.Application.Contracts;
using LibraryApp.Application.Common.Pagination;
using System.Net.Mail;

using AutoMapper;

namespace LibraryApp.Application.Services;

public class MembersService(ILibraryDb _db, IMapper _mapper)
{
    private static bool IsValidEmail(string email)
        => MailAddress.TryCreate(email, out _);

    public async Task<ApiResponse> CreateMemberAsync(string fullName, string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return ApiResponse.Failure("FullName is required.");

        var trimmedName = fullName.Trim();
        if (trimmedName.Length < 2)
            return ApiResponse.Failure("FullName must be at least 2 characters long.");

        if (string.IsNullOrWhiteSpace(email))
            return ApiResponse.Failure("Email is required.");

        var trimmedEmail = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(trimmedEmail))
            return ApiResponse.Failure("Email format is invalid.");

        var emailExists = await _db.Members.AnyAsync(m => m.Email == trimmedEmail, ct);
        if (emailExists)
            return ApiResponse.Failure($"A member with email '{trimmedEmail}' already exists.");

        var member = new Member { FullName = trimmedName, Email = trimmedEmail };
        await _db.Members.AddAsync(member, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Member '{member.FullName}' created with ID {member.Id}.");
    }

    public async Task<ApiResponse<PagedResult<MemberListItemDto>>> GetMembersAsync(PaginationFilter filter, CancellationToken ct = default)
    {
        var query = _db.Members;
        var totalCount = await query.CountAsync(ct);
        
        var members = await query
            .OrderBy(m => m.FullName)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        var dtos = _mapper.Map<List<MemberListItemDto>>(members);
        var pagedResult = new PagedResult<MemberListItemDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
        return ApiResponse<PagedResult<MemberListItemDto>>.Success(pagedResult);
    }

    public async Task<ApiResponse<MemberDetailDto>> GetMemberByIdAsync(int id, CancellationToken ct = default)
    {
        var member = await _db.Members.FindAsync(new object[] { id }, ct);
        if (member == null)
            throw new NotFoundException($"Member with ID {id} not found.");

        return ApiResponse<MemberDetailDto>.Success(_mapper.Map<MemberDetailDto>(member));
    }

    public async Task<ApiResponse> UpdateMemberAsync(int id, string fullName, string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return ApiResponse.Failure("FullName is required.");

        var trimmedName = fullName.Trim();
        if (trimmedName.Length < 2)
            return ApiResponse.Failure("FullName must be at least 2 characters long.");

        if (string.IsNullOrWhiteSpace(email))
            return ApiResponse.Failure("Email is required.");

        var trimmedEmail = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(trimmedEmail))
            return ApiResponse.Failure("Email format is invalid.");

        var member = await _db.Members.FindAsync(new object[] { id }, ct);
        if (member == null)
            throw new NotFoundException($"Member with ID {id} not found.");

        var emailExists = await _db.Members.AnyAsync(m => m.Email == trimmedEmail && m.Id != id, ct);
        if (emailExists)
            return ApiResponse.Failure($"A member with email '{trimmedEmail}' already exists.");

        member.FullName = trimmedName;
        member.Email = trimmedEmail;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Member with ID {id} updated successfully.");
    }

    public async Task<ApiResponse> DeleteMemberAsync(int id, CancellationToken ct = default)
    {
        var member = await _db.Members.FindAsync(new object[] { id }, ct);
        if (member == null)
            throw new NotFoundException($"Member with ID {id} not found.");

        var hasActiveLoans = await _db.Loans.AnyAsync(l => l.MemberId == id && l.ReturnedAtUtc == null, ct);
        if (hasActiveLoans)
            return ApiResponse.Failure("Cannot delete member because they have active (unreturned) loans.");

        member.IsDeleted = true;
        member.DeletedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Member with ID {id} deleted successfully.");
    }

    public async Task<ApiResponse<MemberPenaltyDto>> GetMemberPenaltiesAsync(int id, CancellationToken ct = default)
    {
        var member = await _db.Members
            .Include(m => m.Loans)
                .ThenInclude(l => l.Book)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (member == null)
            throw new NotFoundException($"Member with ID {id} not found.");

        var overdueLoans = member.Loans
            .Where(l => l.PenaltyAmount > 0 || l.IsOverdue)
            .Select(l => new PenaltyDetailDto
            {
                LoanId = l.Id,
                BookTitle = l.Book?.Title ?? "Unknown Book",
                DueAtUtc = l.DueAtUtc,
                ReturnedAtUtc = l.ReturnedAtUtc,
                PenaltyAmount = l.PenaltyAmount > 0 
                    ? l.PenaltyAmount 
                    : (decimal)Math.Ceiling((DateTime.UtcNow - l.DueAtUtc).TotalDays) * 1.50m,
                OverdueDays = l.ReturnedAtUtc.HasValue 
                    ? (int)Math.Ceiling((l.ReturnedAtUtc.Value - l.DueAtUtc).TotalDays)
                    : (int)Math.Ceiling((DateTime.UtcNow - l.DueAtUtc).TotalDays)
            }).ToList();

        var dto = new MemberPenaltyDto
        {
            MemberId = member.Id,
            FullName = member.FullName,
            TotalPenalty = member.TotalPenalty,
            OverdueLoans = overdueLoans
        };

        return ApiResponse<MemberPenaltyDto>.Success(dto);
    }
}
