using LibraryApp.Domain.Constants;
using LibraryApp.Application.Abstractions;
using LibraryApp.Application.Common;
using LibraryApp.Application.Contracts;
using LibraryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace LibraryApp.Application.Services;

public class AuthService(
    ILibraryDb _db,
    IJwtProvider _jwtProvider,
    IPasswordHasher _passwordHasher)
{
    private static bool IsValidEmail(string email)
        => MailAddress.TryCreate(email, out _);

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto request, CancellationToken ct = default)
    {
        var trimmedName = request.FullName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
            return ApiResponse<AuthResponseDto>.Failure("FullName cannot be empty.");

        var trimmedEmail = request.Email.Trim().ToLowerInvariant();
        if (!IsValidEmail(trimmedEmail))
            return ApiResponse<AuthResponseDto>.Failure("Email format is invalid.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return ApiResponse<AuthResponseDto>.Failure("Password must be at least 6 characters long.");

        var emailExists = await _db.Members.AnyAsync(m => m.Email == trimmedEmail, ct);
        if (emailExists)
            return ApiResponse<AuthResponseDto>.Failure($"A member with email '{trimmedEmail}' already exists.");

        var member = new Member 
        { 
            FullName = trimmedName, 
            Email = trimmedEmail,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = Roles.Member
        };

        await _db.Members.AddAsync(member, ct);
        await _db.SaveChangesAsync(ct);

        var token = _jwtProvider.Generate(member);

        return ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            Email = member.Email,
            FullName = member.FullName,
            Role = member.Role
        });
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto request, CancellationToken ct = default)
    {
        var trimmedEmail = request.Email.Trim().ToLowerInvariant();

        var member = await _db.Members.FirstOrDefaultAsync(m => m.Email == trimmedEmail, ct);

        if (member == null)
            return ApiResponse<AuthResponseDto>.Failure("Invalid email or password.");

        bool isPasswordValid = _passwordHasher.Verify(request.Password, member.PasswordHash);
        if (!isPasswordValid)
            return ApiResponse<AuthResponseDto>.Failure("Invalid email or password.");

        var token = _jwtProvider.Generate(member);

        return ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            Email = member.Email,
            FullName = member.FullName,
            Role = member.Role
        });
    }
}
