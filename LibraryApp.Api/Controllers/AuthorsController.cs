using LibraryApp.Domain.Constants;
using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;
using LibraryApp.Application.Common.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApp.Api.Controllers;

[ApiController]
[Authorize]
public class AuthorsController(
    AuthorsService authorsService
) : BaseController
{
    // POST /authors
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> CreateAuthor([FromBody] CreateAuthorDto request, CancellationToken ct)
    {

        return await authorsService.CreateAuthorAsync(request.FullName, ct);
    }

     // GET /authors
    [HttpGet]
    public async Task<ApiResponse<PagedResult<AuthorListItemDto>>> GetAuthors([FromQuery] PaginationFilter filter, CancellationToken ct)
    {
        return await authorsService.GetAuthorsAsync(filter, ct);
    }

    // GET /authors/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<AuthorDetailDto>> GetAuthorById(int id, CancellationToken ct)
    {
        return await authorsService.GetAuthorByIdAsync(id, ct);
    }

    // PUT /authors/5
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> UpdateAuthor(int id, [FromBody] UpdateAuthorDto request, CancellationToken ct)
    {
        return await authorsService.UpdateAuthorAsync(id, request.FullName, ct);
    }

    // DELETE /authors/5
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> DeleteAuthor(int id, CancellationToken ct)
    {
        return await authorsService.DeleteAuthorAsync(id, ct);
    }
}
