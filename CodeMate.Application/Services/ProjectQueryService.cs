using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class ProjectQueryService : IProjectQueryService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public ProjectQueryService(
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<ProjectDetailsResponse> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        return _mapper.Map<ProjectDetailsResponse>(project);
    }

    public async Task<PaginationResponse<ProjectCardResponse>> SearchAsync(
        SearchProjectRequest request)
    {
        var status = request.Status.HasValue
            ? (ProjectStatus?)request.Status.Value
            : null;

        var projects = await _projectRepository.SearchAsync(
            request.Title,
            status,
            request.PageNumber,
            request.PageSize);

        return new PaginationResponse<ProjectCardResponse>
        {
            Items = _mapper.Map<IEnumerable<ProjectCardResponse>>(projects),
            PageNumber = projects.PageNumber,
            PageSize = projects.PageSize,
            TotalCount = projects.TotalCount,
            TotalPages = projects.TotalPages
        };
    }
}