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
            return ApiResponse.Failure("Name is required.");

        var trimmedName = name.Trim();
        if (trimmedName.Length < 2)
            return ApiResponse.Failure("Name must be at least 2 characters long.");

        var exists = await _db.Authors.AnyAsync(a => a.FullName == trimmedName, ct);
        if (exists)
            return ApiResponse.Failure($"Author '{trimmedName}' already exists.");

        var author = new Author { FullName = trimmedName };
        await _db.Authors.AddAsync(author, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Author '{author.FullName}' created with ID {author.Id}.");
    }

    public async Task<ApiResponse<List<AuthorListItemDto>>> GetAuthorsAsync(CancellationToken ct = default)
    {
        var authors = await _db.Authors.Select(a => new AuthorListItemDto(a.Id, a.FullName)).ToListAsync(ct);
        return ApiResponse<List<AuthorListItemDto>>.Success(authors);
    }

    public async Task<ApiResponse<AuthorDetailDto>> GetAuthorByIdAsync(int id, CancellationToken ct = default)
    {
        var author = await _db.Authors.FindAsync(new object[] { id }, ct);
        if (author == null)
            return ApiResponse<AuthorDetailDto>.Failure($"Author with ID {id} not found.");

        return ApiResponse<AuthorDetailDto>.Success(new AuthorDetailDto(author.Id, author.FullName));
    }

    public async Task<ApiResponse> UpdateAuthorAsync(int id, string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ApiResponse.Failure("Name is required.");

        var trimmedName = name.Trim();
        if (trimmedName.Length < 2)
            return ApiResponse.Failure("Name must be at least 2 characters long.");

        var author = await _db.Authors.FindAsync(new object[] { id }, ct);
        if (author == null)
            return ApiResponse.Failure($"Author with ID {id} not found.");

        var duplicate = await _db.Authors.AnyAsync(a => a.FullName == trimmedName && a.Id != id, ct);
        if (duplicate)
            return ApiResponse.Failure($"Author '{trimmedName}' already exists.");

        author.FullName = trimmedName;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Author with ID {id} updated successfully.");
    }

    public async Task<ApiResponse> DeleteAuthorAsync(int id, CancellationToken ct = default)
    {
        var author = await _db.Authors.FindAsync(new object[] { id }, ct);
        if (author == null)
            return ApiResponse.Failure($"Author with ID {id} not found.");

        var hasBooks = await _db.Books.AnyAsync(b => b.AuthorId == id, ct);
        if (hasBooks)
            return ApiResponse.Failure("Cannot delete author because there are books associated with this author.");

        _db.Authors.Remove(author);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Author with ID {id} deleted successfully.");
    }

}
