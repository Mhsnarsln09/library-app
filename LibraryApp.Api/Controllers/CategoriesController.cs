using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;

namespace LibraryApp.Api.Controllers;

[ApiController]        
public class CategoriesController(
    CategoriesService categoriesService
) : BaseController
{
    // POST /categories
    [HttpPost]
    public async Task<ApiResponse> CreateCategory([FromBody] CreateCategoryDto request, CancellationToken ct)
    {
        return await categoriesService.CreateCategoryAsync(request.Name, ct);
    }

    // GET /categories
    [HttpGet]
    public async Task<ApiResponse<List<CategoryListItemDto>>> GetCategories()
    {
        return await categoriesService.GetCategoriesAsync();
    }

    // GET /categories/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<CategoryDetailDto>> GetCategoryById(int id, CancellationToken ct)
    {
        return await categoriesService.GetCategoryByIdAsync(id, ct);
    }

    // PUT /categories/5
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateCategory(int id, [FromBody] UpdateCategoryDto request, CancellationToken ct)
    {
        return await categoriesService.UpdateCategoryAsync(id, request.Name, ct);
    }

    // DELETE /categories/5
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteCategory(int id, CancellationToken ct)
    {
        return await categoriesService.DeleteCategoryAsync(id, ct);
    }
}
