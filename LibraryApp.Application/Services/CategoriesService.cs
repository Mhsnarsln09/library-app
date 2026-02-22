using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Common;
using LibraryApp.Application.Contracts;
using LibraryApp.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Application.Common.Pagination;

using AutoMapper;

namespace LibraryApp.Application.Services;

public class CategoriesService(ILibraryDb _db, IMapper _mapper)
{
    public async Task<ApiResponse> CreateCategoryAsync(string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();

        var exists = await _db.Categories.AnyAsync(c => c.Name == trimmedName, ct);
        if (exists)
            return ApiResponse.Failure($"Category '{trimmedName}' already exists.");

        var category = new Category { Name = trimmedName };
        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Category '{category.Name}' created with ID {category.Id}.");
    }

    public async Task<ApiResponse<PagedResult<CategoryListItemDto>>> GetCategoriesAsync(PaginationFilter filter, CancellationToken ct = default)
    {
        var query = _db.Categories;
        var totalCount = await query.CountAsync(ct);
        
        var categories = await query
            .OrderBy(c => c.Name)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);
            
        var dtos = _mapper.Map<List<CategoryListItemDto>>(categories);
        var pagedResult = new PagedResult<CategoryListItemDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
        return ApiResponse<PagedResult<CategoryListItemDto>>.Success(pagedResult);
    }

    public async Task<ApiResponse<CategoryDetailDto>> GetCategoryByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (category == null)
            throw new NotFoundException($"Category with ID {id} not found.");

        return ApiResponse<CategoryDetailDto>.Success(_mapper.Map<CategoryDetailDto>(category));
    }

    public async Task<ApiResponse> UpdateCategoryAsync(int id, string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();

        var category = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (category == null)
            throw new NotFoundException($"Category with ID {id} not found.");

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
            throw new NotFoundException($"Category with ID {id} not found.");

        var hasBooks = await _db.Books.AnyAsync(b => b.CategoryId == id, ct);
        if (hasBooks)
            return ApiResponse.Failure("Cannot delete category because there are books associated with this category.");

        category.IsDeleted = true;
        category.DeletedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ApiResponse.Success($"Category with ID {id} deleted successfully.");
    }

}
