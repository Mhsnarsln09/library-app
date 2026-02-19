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


}
