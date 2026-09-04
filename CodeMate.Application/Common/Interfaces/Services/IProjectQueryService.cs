using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IProjectQueryService
{
    Task<ProjectDetailsResponse> GetByIdAsync(Guid id);

    Task<PaginationResponse<ProjectCardResponse>> SearchAsync(
        SearchProjectRequest request);
}