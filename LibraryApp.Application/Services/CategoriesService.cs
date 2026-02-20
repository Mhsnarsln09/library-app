using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using Microsoft.EntityFrameworkCore;   
using LibraryApp.Application.Contracts;


namespace LibraryApp.Application.Services;

public class CategoriesService(ILibraryDb _db)
{
    public async Task<ApiResponse> CreateCategoryAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ApiResponse.Failure("Name is required.");

        var trimmedName = name.Trim();
        if (trimmedName.Length < 2)
            return ApiResponse.Failure("Name must be at least 2 characters long.");

        var exists = await _db.Categories.AnyAsync(c => c.Name == trimmedName, ct);
        if (exists)
            return ApiResponse.Failure($"Category '{trimmedName}' already exists.");

        var category = new Category { Name = trimmedName };
        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Category '{category.Name}' created with ID {category.Id}.");
    }

    public async Task<ApiResponse<List<CategoryListItemDto>>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).Select(c => new CategoryListItemDto(c.Id, c.Name)).ToListAsync(ct);
        return ApiResponse<List<CategoryListItemDto>>.Success(categories);
    }

    public async Task<ApiResponse<CategoryDetailDto>> GetCategoryByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (category == null)
            return ApiResponse<CategoryDetailDto>.Failure($"Category with ID {id} not found.");

        return ApiResponse<CategoryDetailDto>.Success(new CategoryDetailDto(category.Id, category.Name));
    }

    public async Task<ApiResponse> UpdateCategoryAsync(int id, string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ApiResponse.Failure("Name is required.");

        var trimmedName = name.Trim();
        if (trimmedName.Length < 2)
            return ApiResponse.Failure("Name must be at least 2 characters long.");

        var category = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (category == null)
            return ApiResponse.Failure($"Category with ID {id} not found.");

        var duplicate = await _db.Categories.AnyAsync(c => c.Name == trimmedName && c.Id != id, ct);
        if (duplicate)
            return ApiResponse.Failure($"Category '{trimmedName}' already exists.");

        category.Name = trimmedName;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Category with ID {id} updated successfully.");
    }

    public async Task<ApiResponse> DeleteCategoryAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (category == null)
            return ApiResponse.Failure($"Category with ID {id} not found.");

        var hasBooks = await _db.Books.AnyAsync(b => b.CategoryId == id, ct);
        if (hasBooks)
            return ApiResponse.Failure("Cannot delete category because there are books associated with this category.");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Category with ID {id} deleted successfully.");
    }

}
