using LibraryApp.Application.Abstractions;
using LibraryApp.Domain.Entities;

namespace LibraryApp.Application.Services;

public class CategoriesService(ILibraryDb _db)
{
    public async Task<Category> CreateCategoryAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        var category = new Category { Name = name.Trim() };
        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);
        return category;
    }
}
