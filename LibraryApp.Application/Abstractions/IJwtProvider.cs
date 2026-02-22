using LibraryApp.Domain.Entities;

namespace LibraryApp.Application.Abstractions;

public interface IJwtProvider
{
    string Generate(Member member);
}
