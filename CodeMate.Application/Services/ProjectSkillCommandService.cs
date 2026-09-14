using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Requests;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class ProjectSkillCommandService : IProjectSkillCommandService
{
    private readonly IProjectSkillRepository _projectSkillRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProjectSkillCommandService(
        IProjectSkillRepository projectSkillRepository,
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService)
    {
        _projectSkillRepository = projectSkillRepository;
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
    }

    public async Task AddAsync(AddProjectSkillRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to modify this project.");

        var projectSkill = new ProjectSkill
        {
            ProjectId = request.ProjectId,
            SkillId = request.SkillId,
            RequiredLevel = (Domain.Enums.SkillLevel)request.RequiredLevel,
            IsMandatory = request.IsMandatory
        };

        await _projectSkillRepository.AddAsync(projectSkill);
        await _projectSkillRepository.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid projectId, Guid skillId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to modify this project.");

        var projectSkills = await _projectSkillRepository
            .GetByProjectIdAsync(projectId);

        var projectSkill = projectSkills
            .FirstOrDefault(x => x.SkillId == skillId);

        if (projectSkill is null)
            throw new NotFoundException("Project skill not found.");

        projectSkill.IsDeleted = true;
        projectSkill.DeletedAt = DateTimeOffset.UtcNow;
        projectSkill.DeletedBy = _currentUserService.UserId;

        await _projectSkillRepository.DeleteAsync(projectSkill);
        await _projectSkillRepository.SaveChangesAsync();
    }
}