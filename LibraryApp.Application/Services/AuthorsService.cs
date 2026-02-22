using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using LibraryApp.Application.Contracts;
using LibraryApp.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Common.Pagination;

using AutoMapper;

namespace LibraryApp.Application.Services;

public class AuthorsService(ILibraryDb _db, IMapper _mapper)
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

    public async Task<ApiResponse<PagedResult<AuthorListItemDto>>> GetAuthorsAsync(PaginationFilter filter, CancellationToken ct = default)
    {
        var query = _db.Authors;
        var totalCount = await query.CountAsync(ct);
        
        var authors = await query
            .OrderBy(a => a.FullName)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);
            
        var dtos = _mapper.Map<List<AuthorListItemDto>>(authors);
        var pagedResult = new PagedResult<AuthorListItemDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
        return ApiResponse<PagedResult<AuthorListItemDto>>.Success(pagedResult);
    }

    public async Task<ApiResponse<AuthorDetailDto>> GetAuthorByIdAsync(int id, CancellationToken ct = default)
    {
        var author = await _db.Authors.FindAsync(new object[] { id }, ct);
        if (author == null)
            throw new NotFoundException($"Author with ID {id} not found.");

        return ApiResponse<AuthorDetailDto>.Success(_mapper.Map<AuthorDetailDto>(author));
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
            throw new NotFoundException($"Author with ID {id} not found.");

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
            throw new NotFoundException($"Author with ID {id} not found.");

        var hasBooks = await _db.Books.AnyAsync(b => b.AuthorId == id, ct);
        if (hasBooks)
            return ApiResponse.Failure("Cannot delete author because there are books associated with this author.");

        author.IsDeleted = true;
        author.DeletedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Author with ID {id} deleted successfully.");
    }

}
