using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Responses;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class JoinRequestQueryService : IJoinRequestQueryService
{
    private readonly IJoinRequestRepository _joinRequestRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public JoinRequestQueryService(
        IJoinRequestRepository joinRequestRepository,
        ITeamRepository teamRepository,
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _joinRequestRepository = joinRequestRepository;
        _teamRepository = teamRepository;
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JoinRequestResponse>>
        GetPendingRequestsForTeamAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "Only the project owner can view pending join requests.");

        var requests =
            await _joinRequestRepository.GetPendingRequestsForTeamAsync(teamId);

        return _mapper.Map<IEnumerable<JoinRequestResponse>>(requests);
    }

    public async Task<IEnumerable<JoinRequestResponse>>
        GetMyJoinRequestsAsync()
    {
        var requests =
            await _joinRequestRepository.GetMyJoinRequestsAsync(
                _currentUserService.UserId);

        return _mapper.Map<IEnumerable<JoinRequestResponse>>(requests);
    }
}