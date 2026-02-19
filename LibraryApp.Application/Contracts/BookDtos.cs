namespace LibraryApp.Application.Contracts;

public record BookListItemDto(int Id, string Title, int TotalCopies, int AuthorId, int CategoryId);
public record BookDetailDto(int Id, string Title, string? Isbn, int TotalCopies, int AuthorId, int CategoryId);
public record CreateBookDto(string Title, string? Isbn, int TotalCopies, int AuthorId, int CategoryId);
public record UpdateBookDto(string Title, string? Isbn, int TotalCopies, int AuthorId, int CategoryId);
