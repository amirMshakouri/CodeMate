using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Enums;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IProjectCommandService
{
    Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request);

    Task<ProjectResponse> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request);

    Task DeleteAsync(
        Guid projectId);

    Task<ProjectResponse> ChangeStatusAsync(
        Guid projectId,
        ProjectStatus status);
}