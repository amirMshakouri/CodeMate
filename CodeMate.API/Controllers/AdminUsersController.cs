using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Admin.Users.Requests;
using CodeMate.Contracts.Admin.Users.Responses;
using CodeMate.Contracts.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<AdminUserListItemResponse>>> Search(
        [FromQuery] SearchUsersRequest request)
    {
        var result = await _adminUserService.SearchUsersAsync(request);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminUserDetailsResponse>> GetById(Guid id)
    {
        var result = await _adminUserService.GetUserDetailsAsync(id);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _adminUserService.DeactivateUserAsync(id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _adminUserService.ActivateUserAsync(id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/promote")]
    public async Task<IActionResult> Promote(Guid id)
    {
        await _adminUserService.PromoteToAdminAsync(id);

        return NoContent();
    }
}