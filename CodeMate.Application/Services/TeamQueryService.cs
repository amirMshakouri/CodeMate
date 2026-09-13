using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Responses;

namespace CodeMate.Application.Services;

public sealed class TeamQueryService : ITeamQueryService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;

    public TeamQueryService(
        ITeamRepository teamRepository,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
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

        return _mapper.Map<IEnumerable<ProjectMemberResponse>>(members);
    }
}