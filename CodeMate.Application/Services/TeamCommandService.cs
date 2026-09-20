using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Requests;
using CodeMate.Contracts.Teams.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class TeamCommandService : ITeamCommandService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public TeamCommandService(
        ITeamRepository teamRepository,
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TeamResponse> CreateAsync(
        CreateTeamRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        EnsureOwnership(project);

        var team = _mapper.Map<Team>(request);

        await _teamRepository.AddAsync(team);
        await _teamRepository.SaveChangesAsync();

        return _mapper.Map<TeamResponse>(team);
    }


    public async Task<TeamResponse> UpdateAsync(
        Guid teamId,
        UpdateTeamRequest request)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        EnsureOwnership(project);

        _mapper.Map(request, team);

        await _teamRepository.UpdateAsync(team);
        await _teamRepository.SaveChangesAsync();

        return _mapper.Map<TeamResponse>(team);
    }

    public async Task DeleteAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        EnsureOwnership(project);

        team.IsDeleted = true;
        team.DeletedAt = DateTimeOffset.UtcNow;
        team.DeletedBy = _currentUserService.UserId;

        await _teamRepository.DeleteAsync(team);
        await _teamRepository.SaveChangesAsync();
    }

    private void EnsureOwnership(Project project)
    {
        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to modify this project.");
    }
}