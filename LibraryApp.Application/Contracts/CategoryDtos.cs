namespace LibraryApp.Application.Contracts;

public record CategoryListItemDto(int Id, string Name);
public record CategoryDetailDto(int Id, string Name);
public record CreateCategoryDto(string Name);
public record UpdateCategoryDto(string Name);
