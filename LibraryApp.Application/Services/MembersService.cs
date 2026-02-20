using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Contracts;
using System.Net.Mail;

namespace LibraryApp.Application.Services;

public class MembersService(ILibraryDb _db)
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

    public async Task<ApiResponse<List<MemberListItemDto>>> GetMembersAsync(CancellationToken ct = default)
    {
        var members = await _db.Members
            .OrderBy(m => m.FullName)
            .Select(m => new MemberListItemDto(m.Id, m.FullName, m.Email))
            .ToListAsync(ct);

        return ApiResponse<List<MemberListItemDto>>.Success(members);
    }

    public async Task<ApiResponse<MemberDetailDto>> GetMemberByIdAsync(int id, CancellationToken ct = default)
    {
        var member = await _db.Members.FindAsync(new object[] { id }, ct);
        if (member == null)
            return ApiResponse<MemberDetailDto>.Failure($"Member with ID {id} not found.");

        return ApiResponse<MemberDetailDto>.Success(new MemberDetailDto(member.Id, member.FullName, member.Email));
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
            return ApiResponse.Failure($"Member with ID {id} not found.");

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
            return ApiResponse.Failure($"Member with ID {id} not found.");

        var hasActiveLoans = await _db.Loans.AnyAsync(l => l.MemberId == id && l.ReturnedAtUtc == null, ct);
        if (hasActiveLoans)
            return ApiResponse.Failure("Cannot delete member because they have active (unreturned) loans.");

        _db.Members.Remove(member);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Member with ID {id} deleted successfully.");
    }
}
