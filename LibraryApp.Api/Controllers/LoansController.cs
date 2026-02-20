using LibraryApp.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Application.Services;
using LibraryApp.Application.Common;

namespace LibraryApp.Api.Controllers;

[ApiController]
public class LoansController(
    LoansService loansService
) : BaseController
{
    // POST /api/loans
    [HttpPost]
    public async Task<ApiResponse> CreateLoan([FromBody] CreateLoanDto request, CancellationToken ct)
    {
        return await loansService.CreateLoanAsync(request.BookId, request.MemberId, ct);
    }

    // GET /api/loans
    [HttpGet]
    public async Task<ApiResponse<List<LoanListItemDto>>> GetLoans(CancellationToken ct)
    {
        return await loansService.GetLoansAsync(ct);
    }

    // GET /api/loans/{id}
    [HttpGet("{id}")]
    public async Task<ApiResponse<LoanDetailDto>> GetLoanById(int id, CancellationToken ct)
    {
        return await loansService.GetLoanByIdAsync(id, ct);
    }

    // PATCH /api/loans/{id}/return
    [HttpPatch("{id}/return")]
    public async Task<ApiResponse> ReturnLoan(int id, CancellationToken ct)
    {
        return await loansService.ReturnLoanAsync(id, ct);
    }

    // DELETE /api/loans/{id}
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteLoan(int id, CancellationToken ct)
    {
        return await loansService.DeleteLoanAsync(id, ct);
    }
}
