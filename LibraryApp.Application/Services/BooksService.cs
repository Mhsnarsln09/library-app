using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;   
using LibraryApp.Application.Contracts;


namespace LibraryApp.Application.Services;

public class BooksService(ILibraryDb _db)
{
    public async Task<ApiResponse> CreateBookAsync(string title, int authorId, int categoryId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return ApiResponse.Failure("Title is required.");

        var trimmedTitle = title.Trim();
        if (trimmedTitle.Length < 2)
            return ApiResponse.Failure("Title must be at least 2 characters long.");

        var author = await _db.Authors.FindAsync(new object[] { authorId }, ct);
        if (author == null)
            return ApiResponse.Failure($"Author with ID {authorId} not found.");

        var category = await _db.Categories.FindAsync(new object[] { categoryId }, ct);
        if (category == null)
            return ApiResponse.Failure($"Category with ID {categoryId} not found.");

        var book = new Book { Title = trimmedTitle, AuthorId = authorId, CategoryId = categoryId };
        await _db.Books.AddAsync(book, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Book '{book.Title}' created with ID {book.Id}.");
    }

    public async Task<ApiResponse<List<BookListItemDto>>> GetBooksAsync(CancellationToken ct = default)
    {
        var books = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Select(b => new BookListItemDto(b.Id, b.Title, b.TotalCopies, new AuthorDetailDto(b.Author.Id, b.Author.FullName), new CategoryListItemDto(b.Category.Id, b.Category.Name)))
            .ToListAsync(ct);

        return ApiResponse<List<BookListItemDto>>.Success(books);
    }

    public async Task<ApiResponse<BookDetailDto>> GetBookByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (book == null)
            return ApiResponse<BookDetailDto>.Failure($"Book with ID {id} not found.");

        var bookDetail = new BookDetailDto(book.Id, book.Title, book.Isbn, book.TotalCopies, new AuthorDetailDto(book.Author.Id, book.Author.FullName), new CategoryListItemDto(book.Category.Id, book.Category.Name));
        return ApiResponse<BookDetailDto>.Success(bookDetail);
    }

    public async Task<ApiResponse> UpdateBookAsync(int id, string title, int authorId, int categoryId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return ApiResponse.Failure("Title is required.");

        var trimmedTitle = title.Trim();
        if (trimmedTitle.Length < 2)
            return ApiResponse.Failure("Title must be at least 2 characters long.");

        var book = await _db.Books.FindAsync(new object[] { id }, ct);
        if (book == null)
            return ApiResponse.Failure($"Book with ID {id} not found.");

        var author = await _db.Authors.FindAsync(new object[] { authorId }, ct);
        if (author == null)
            return ApiResponse.Failure($"Author with ID {authorId} not found.");

        var category = await _db.Categories.FindAsync(new object[] { categoryId }, ct);
        if (category == null)
            return ApiResponse.Failure($"Category with ID {categoryId} not found.");

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
            return ApiResponse.Failure($"Book with ID {id} not found.");

        var hasActiveLoans = await _db.Loans.AnyAsync(l => l.BookId == id && l.ReturnedAtUtc == null, ct);
        if (hasActiveLoans)
            return ApiResponse.Failure("Cannot delete book because it has active (unreturned) loans.");

        _db.Books.Remove(book);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Book with ID {id} deleted successfully.");
    }

}
