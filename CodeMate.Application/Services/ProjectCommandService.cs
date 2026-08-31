using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class ProjectCommandService : IProjectCommandService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ProjectCommandService(
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request)
    {
        var project = _mapper.Map<Project>(request);

        project.OwnerId = _currentUserService.UserId;
        project.Status = ProjectStatus.Draft;

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<ProjectResponse> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        EnsureOwnership(project);

        _mapper.Map(request, project);

        await _projectRepository.UpdateAsync(project);
        await _projectRepository.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task DeleteAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        EnsureOwnership(project);

        project.IsDeleted = true;
        project.DeletedAt = DateTimeOffset.UtcNow;
        project.DeletedBy = _currentUserService.UserId;

        await _projectRepository.DeleteAsync(project);
        await _projectRepository.SaveChangesAsync();
    }

    public async Task<ProjectResponse> ChangeStatusAsync(
        Guid projectId,
        ProjectStatus status)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        EnsureOwnership(project);

        project.Status = status;

        await _projectRepository.UpdateAsync(project);
        await _projectRepository.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }

    private void EnsureOwnership(Project project)
    {
        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to modify this project.");
    }
}