using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;

namespace LibraryApp.Api.Controllers;

[ApiController]     

public class BooksController(
    BooksService booksService
) : BaseController
{
    // POST /books
    [HttpPost]
    public async Task<ApiResponse> CreateBook([FromBody] CreateBookDto request, CancellationToken ct)
    {
        return await booksService.CreateBookAsync(request.Title, request.AuthorId, request.CategoryId, ct);
    }

     // GET /books
    [HttpGet]
    public async Task<ApiResponse<List<BookListItemDto>>> GetBooks()
    {
        return await booksService.GetBooksAsync();
    }

    // GET /books/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<BookDetailDto>> GetBookById(int id, CancellationToken ct)
    {
        return await booksService.GetBookByIdAsync(id, ct);
    }

    // PUT /books/5
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateBook(int id, [FromBody] UpdateBookDto request, CancellationToken ct)
    {
        return await booksService.UpdateBookAsync(id, request.Title, request.AuthorId, request.CategoryId, ct);
    }

    // DELETE /books/5
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteBook(int id, CancellationToken ct)
    {
        return await booksService.DeleteBookAsync(id, ct);
    }
}