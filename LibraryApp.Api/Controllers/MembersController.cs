using LibraryApp.Domain.Constants;
using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;
using LibraryApp.Application.Common.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApp.Api.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
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
    public async Task<ApiResponse<PagedResult<MemberListItemDto>>> GetMembers([FromQuery] PaginationFilter filter, CancellationToken ct)
    {
        return await membersService.GetMembersAsync(filter, ct);
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

    // GET /api/members/{id}/penalties
    [HttpGet("{id}/penalties")]
    // [Authorize(Roles = Roles.Admin)] // Bu endpoint admin yetkisi gerektirebilir veya member kendi id si ile istek atabilir
    public async Task<ApiResponse<MemberPenaltyDto>> GetMemberPenalties(int id, CancellationToken ct)
    {
        return await membersService.GetMemberPenaltiesAsync(id, ct);
    }
}
