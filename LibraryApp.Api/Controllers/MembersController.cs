using LibraryApp.Domain.Constants;
using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;
using LibraryApp.Application.Common.Pagination;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LibraryApp.Application.Common.Exceptions;

namespace LibraryApp.Api.Controllers;

[ApiController]
[Authorize]
public class MembersController(
    MembersService membersService
) : BaseController
{
    // GET /api/members
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse<PagedResult<MemberListItemDto>>> GetMembers([FromQuery] PaginationFilter filter, CancellationToken ct)
    {
        return await membersService.GetMembersAsync(filter, ct);
    }

    // GET /api/members/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse<MemberDetailDto>> GetMemberById(int id, CancellationToken ct)
    {
        return await membersService.GetMemberByIdAsync(id, ct);
    }

    // PUT /api/members/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> UpdateMember(int id, [FromBody] UpdateMemberDto request, CancellationToken ct)
    {
        return await membersService.UpdateMemberAsync(id, request.FullName, request.Email, request.Role, ct);
    }

    // DELETE /api/members/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ApiResponse> DeleteMember(int id, CancellationToken ct)
    {
        return await membersService.DeleteMemberAsync(id, ct);
    }

    // GET /api/members/{id}/penalties
    [HttpGet("{id}/penalties")]
    public async Task<ApiResponse<MemberPenaltyDto>> GetMemberPenalties(int id, CancellationToken ct)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole(Roles.Admin);

        if (!isAdmin && currentUserId != id.ToString())
        {
            throw new ForbiddenException("You are only allowed to view your own penalties.");
        }

        return await membersService.GetMemberPenaltiesAsync(id, ct);
    }
}
