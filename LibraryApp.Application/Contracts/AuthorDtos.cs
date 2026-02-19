namespace LibraryApp.Application.Contracts;

public record AuthorListItemDto(int Id, string FullName);
public record AuthorDetailDto(int Id, string FullName);
public record CreateAuthorDto(string FullName);
public record UpdateAuthorDto(string FullName);
