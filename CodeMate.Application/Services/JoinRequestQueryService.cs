using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class JoinRequestQueryService : IJoinRequestQueryService
{
    private readonly IJoinRequestRepository _joinRequestRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public JoinRequestQueryService(
        IJoinRequestRepository joinRequestRepository,
        ITeamRepository teamRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _joinRequestRepository = joinRequestRepository;
        _teamRepository = teamRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
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

        return await ToResponsesAsync(requests);
    }
   

    public async Task<IEnumerable<JoinRequestResponse>>
        GetMyJoinRequestsAsync()
    {
        var requests =
            await _joinRequestRepository.GetMyJoinRequestsAsync(
                _currentUserService.UserId);

        return await ToResponsesAsync(requests);

    }

    private async Task<List<JoinRequestResponse>> ToResponsesAsync(
   IEnumerable<JoinRequest> requests)
    {
        var responses = new List<JoinRequestResponse>();

        foreach (var request in requests)
        {
            var response = _mapper.Map<JoinRequestResponse>(request);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            var team = await _teamRepository.GetByIdAsync(request.TeamId);
            var project = team is null
                ? null
                : await _projectRepository.GetByIdAsync(team.ProjectId);

            response.UserName = user?.UserName ?? string.Empty;
            response.TeamName = team?.Name ?? string.Empty;
            response.ProjectId = project?.Id ?? Guid.Empty;
            response.ProjectTitle = project?.Title ?? string.Empty;

            responses.Add(response);
        }

        return responses;
    }
}