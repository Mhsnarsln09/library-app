using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;

namespace LibraryApp.Api.Controllers;

[ApiController]
public class MembersController(
    MembersService membersService
) : BaseController
{
    // POST /api/members
    [HttpPost]
    public async Task<ApiResponse> CreateMember([FromBody] CreateMemberDto request, CancellationToken ct)
    {
        return await membersService.CreateMemberAsync(request.FullName, request.Email, ct);
    }

    // GET /api/members
    [HttpGet]
    public async Task<ApiResponse<List<MemberListItemDto>>> GetMembers(CancellationToken ct)
    {
        return await membersService.GetMembersAsync(ct);
    }

    // GET /api/members/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<MemberDetailDto>> GetMemberById(int id, CancellationToken ct)
    {
        return await membersService.GetMemberByIdAsync(id, ct);
    }

    // PUT /api/members/{id}
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateMember(int id, [FromBody] UpdateMemberDto request, CancellationToken ct)
    {
        return await membersService.UpdateMemberAsync(id, request.FullName, request.Email, ct);
    }

    // DELETE /api/members/{id}
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteMember(int id, CancellationToken ct)
    {
        return await membersService.DeleteMemberAsync(id, ct);
    }
}
