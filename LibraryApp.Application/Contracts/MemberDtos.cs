namespace LibraryApp.Application.Contracts;

public record MemberListItemDto(int Id, string FullName, string Email, string Role);
public record MemberDetailDto(int Id, string FullName, string Email, string Role);
public record UpdateMemberDto(string FullName, string Email, string Role);
