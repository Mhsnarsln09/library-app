using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;   
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Services;

public class AuthorsService(ILibraryDb _db)
{
    public async Task<ApiResponse> CreateAuthorAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        var author = new Author { FullName = name.Trim() };
        await _db.Authors.AddAsync(author, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Author '{author.FullName}' created with ID {author.Id}.");
    }

    public async Task<ApiResponse<List<AuthorListItemDto>>> GetAuthorsAsync(CancellationToken ct = default)
    {
        var authors = await _db.Authors.Select(a => new AuthorListItemDto(a.Id, a.FullName)).ToListAsync(ct);
        return ApiResponse<List<AuthorListItemDto>>.Success(authors);
    }
}
