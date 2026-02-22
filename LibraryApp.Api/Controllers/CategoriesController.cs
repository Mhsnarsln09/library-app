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
public class CategoriesController(
    CategoriesService categoriesService
) : BaseController
{
    // POST /categories
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> CreateCategory([FromBody] CreateCategoryDto request, CancellationToken ct)
    {
        return await categoriesService.CreateCategoryAsync(request.Name, ct);
    }

    // GET /categories
    [HttpGet]
    public async Task<ApiResponse<PagedResult<CategoryListItemDto>>> GetCategories([FromQuery] PaginationFilter filter, CancellationToken ct)
    {
        return await categoriesService.GetCategoriesAsync(filter, ct);
    }

    // GET /categories/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<CategoryDetailDto>> GetCategoryById(int id, CancellationToken ct)
    {
        return await categoriesService.GetCategoryByIdAsync(id, ct);
    }

    // PUT /categories/5
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> UpdateCategory(int id, [FromBody] UpdateCategoryDto request, CancellationToken ct)
    {
        return await categoriesService.UpdateCategoryAsync(id, request.Name, ct);
    }

    // DELETE /categories/5
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> DeleteCategory(int id, CancellationToken ct)
    {
        return await categoriesService.DeleteCategoryAsync(id, ct);
    }
}
