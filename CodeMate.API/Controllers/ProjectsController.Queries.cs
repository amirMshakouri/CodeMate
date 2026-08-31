using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

public partial class ProjectsController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDetailsResponse>> GetById(Guid id)
    {
        var result = await _projectQueryService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<ProjectCardResponse>>> Search(
        [FromQuery] SearchProjectRequest request)
    {
        var result = await _projectQueryService.SearchAsync(request);
        return Ok(result);
    }
}