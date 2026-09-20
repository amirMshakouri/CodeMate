using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Responses;

namespace CodeMate.Application.Services;

public sealed class TeamQueryService : ITeamQueryService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public TeamQueryService(
        ITeamRepository teamRepository, IUserRepository userRepository,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeamCardResponse>> GetTeamsByProjectAsync(
        Guid projectId)
    {
        var teams = await _teamRepository.GetByProjectIdAsync(projectId);

        return _mapper.Map<IEnumerable<TeamCardResponse>>(teams);
    }

    public async Task<IEnumerable<ProjectMemberResponse>> GetTeamMembersAsync(
    Guid teamId)
    {
        var members = await _teamRepository.GetMembersAsync(teamId);

        var result = _mapper
            .Map<IEnumerable<ProjectMemberResponse>>(members)
            .ToList();

        foreach (var member in result)
        {
            var user = await _userRepository.GetByIdAsync(member.UserId);
            member.UserName = user?.UserName ?? string.Empty;
        }

        return result;
    }
}