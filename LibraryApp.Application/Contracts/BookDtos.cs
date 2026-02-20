namespace LibraryApp.Application.Contracts;

public record BookListItemDto(int Id, string Title, int TotalCopies,  AuthorDetailDto Author, CategoryListItemDto Category);
public record BookDetailDto(int Id, string Title, string? Isbn, int TotalCopies, AuthorDetailDto Author, CategoryListItemDto Category);
public record CreateBookDto(string Title, string? Isbn, int TotalCopies, int AuthorId, int CategoryId);
public record UpdateBookDto(string Title, string? Isbn, int TotalCopies, int AuthorId, int CategoryId);
