using LibraryApp.Domain.Constants;
using LibraryApp.Application.Common;
using LibraryApp.Application.Contracts;
using LibraryApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ApiResponse<AuthResponseDto>> Register([FromBody] RegisterDto request, CancellationToken ct)
    {
        return await authService.RegisterAsync(request, ct);
    }

    [HttpPost("login")]
    public async Task<ApiResponse<AuthResponseDto>> Login([FromBody] LoginDto request, CancellationToken ct)
    {
        return await authService.LoginAsync(request, ct);
    }
}
