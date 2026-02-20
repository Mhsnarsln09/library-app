using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;

namespace LibraryApp.Api.Controllers;

[ApiController]        
public class AuthorsController(
    AuthorsService authorsService
) : BaseController
{
    // POST /authors
    [HttpPost]
    public async Task<ApiResponse> CreateAuthor([FromBody] CreateAuthorDto request, CancellationToken ct)
    {

        return await authorsService.CreateAuthorAsync(request.FullName, ct);
    }

     // GET /authors
    [HttpGet]
    public async Task<ApiResponse<List<AuthorListItemDto>>> GetAuthors()
    {
        return await authorsService.GetAuthorsAsync();
    }

    // GET /authors/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<AuthorDetailDto>> GetAuthorById(int id, CancellationToken ct)
    {
        return await authorsService.GetAuthorByIdAsync(id, ct);
    }

    // PUT /authors/5
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateAuthor(int id, [FromBody] UpdateAuthorDto request, CancellationToken ct)
    {
        return await authorsService.UpdateAuthorAsync(id, request.FullName, ct);
    }

    // DELETE /authors/5
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteAuthor(int id, CancellationToken ct)
    {
        return await authorsService.DeleteAuthorAsync(id, ct);
    }
}
