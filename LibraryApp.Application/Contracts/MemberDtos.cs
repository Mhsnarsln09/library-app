namespace LibraryApp.Application.Contracts;

public record MemberListItemDto(int Id, string FullName, string Email);
public record MemberDetailDto(int Id, string FullName, string Email);
public record CreateMemberDto(string FullName, string Email);
public record UpdateMemberDto(string FullName, string Email);
