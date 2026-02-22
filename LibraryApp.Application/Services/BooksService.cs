using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;   
using LibraryApp.Application.Contracts;
using LibraryApp.Application.Common.Exceptions;
using LibraryApp.Application.Common.Pagination;


using AutoMapper;

namespace LibraryApp.Application.Services;

public class BooksService(ILibraryDb _db, IMapper _mapper)
{
    public async Task<ApiResponse> CreateBookAsync(string title, int authorId, int categoryId, CancellationToken ct = default)
    {
        var trimmedTitle = title.Trim();

        var author = await _db.Authors.FindAsync(new object[] { authorId }, ct);
        if (author == null)
            throw new NotFoundException($"Author with ID {authorId} not found.");

        var category = await _db.Categories.FindAsync(new object[] { categoryId }, ct);
        if (category == null)
            throw new NotFoundException($"Category with ID {categoryId} not found.");

        var book = new Book { Title = trimmedTitle, AuthorId = authorId, CategoryId = categoryId };
        await _db.Books.AddAsync(book, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Book '{book.Title}' created with ID {book.Id}.");
    }

    public async Task<ApiResponse<PagedResult<BookListItemDto>>> GetBooksAsync(PaginationFilter filter, CancellationToken ct = default)
    {
        var query = _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category);

        var totalCount = await query.CountAsync(ct);

        var books = await query
            .OrderByDescending(b => b.Id)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);
            
        var dtos = _mapper.Map<List<BookListItemDto>>(books);
        var pagedResult = new PagedResult<BookListItemDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
        return ApiResponse<PagedResult<BookListItemDto>>.Success(pagedResult);
    }

    public async Task<ApiResponse<BookDetailDto>> GetBookByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (book == null)
            throw new NotFoundException($"Book with ID {id} not found.");

        var bookDetail = _mapper.Map<BookDetailDto>(book);
        return ApiResponse<BookDetailDto>.Success(bookDetail);
    }

    public async Task<ApiResponse> UpdateBookAsync(int id, string title, int authorId, int categoryId, CancellationToken ct = default)
    {
        var trimmedTitle = title.Trim();

        var book = await _db.Books.FindAsync(new object[] { id }, ct);
        if (book == null)
            throw new NotFoundException($"Book with ID {id} not found.");

        var author = await _db.Authors.FindAsync(new object[] { authorId }, ct);
        if (author == null)
            throw new NotFoundException($"Author with ID {authorId} not found.");

        var category = await _db.Categories.FindAsync(new object[] { categoryId }, ct);
        if (category == null)
            throw new NotFoundException($"Category with ID {categoryId} not found.");

        book.Title = trimmedTitle;
        book.AuthorId = authorId;
        book.CategoryId = categoryId;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Book with ID {id} updated successfully.");
    }

    public async Task<ApiResponse> DeleteBookAsync(int id, CancellationToken ct = default)
    {
        var book = await _db.Books.FindAsync(new object[] { id }, ct);
        if (book == null)
            throw new NotFoundException($"Book with ID {id} not found.");

        var hasActiveLoans = await _db.Loans.AnyAsync(l => l.BookId == id && l.ReturnedAtUtc == null, ct);
        if (hasActiveLoans)
            return ApiResponse.Failure("Cannot delete book because it has active (unreturned) loans.");

        book.IsDeleted = true;
        book.DeletedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Book with ID {id} deleted successfully.");
    }

}
