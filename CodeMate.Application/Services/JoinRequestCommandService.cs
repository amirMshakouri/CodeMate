using CodeMate.Shared.Exceptions;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Requests;
using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;

namespace CodeMate.Application.Services;

public sealed class JoinRequestCommandService : IJoinRequestCommandService
{
    private readonly IJoinRequestRepository _joinRequestRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IProjectSkillRepository _projectSkillRepository;
    private readonly ICurrentUserService _currentUserService;
    

    public JoinRequestCommandService(
        IJoinRequestRepository joinRequestRepository,
        ITeamRepository teamRepository,
        IProjectRepository projectRepository,
        ISkillRepository skillRepository,
        IProjectSkillRepository projectSkillRepository,
        ICurrentUserService currentUserService)
    {
        _joinRequestRepository = joinRequestRepository;
        _teamRepository = teamRepository;
        _projectRepository = projectRepository;
        _skillRepository = skillRepository;
        _projectSkillRepository = projectSkillRepository;
        _currentUserService = currentUserService;
    }

    public async Task SendAsync(SendJoinRequest request)
    {
        var team = await _teamRepository.GetByIdAsync(request.TeamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        if (await _teamRepository.IsMemberAsync(
                request.TeamId,
                _currentUserService.UserId))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["TeamId"] = new[] { "You are already a member of this team." }
            });
        }
        
        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        var projectSkills = await _projectSkillRepository
            .GetByProjectIdAsync(team.ProjectId);

        var mandatorySkills = projectSkills
            .Where(x => x.IsMandatory)
            .ToList();
        
        var userSkills = await _skillRepository
            .GetUserSkillsAsync(_currentUserService.UserId);
        
        foreach (var requiredSkill in mandatorySkills)
        {
            var userSkill = userSkills.FirstOrDefault(
                x => x.SkillId == requiredSkill.SkillId);

            if (userSkill is null || userSkill.Level < requiredSkill.RequiredLevel)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["Skills"] = new[]
                    {
                        "You do not meet the required skills for this project."
                    }
                });
            }
        }
        
        var joinRequest = new JoinRequest
        {
            TeamId = request.TeamId,
            UserId = _currentUserService.UserId,
            Message = request.Message,
            Status = JoinRequestStatus.Pending
        };

        await _joinRequestRepository.AddAsync(joinRequest);
        await _joinRequestRepository.SaveChangesAsync();
        
    }

    public async Task AcceptAsync(AcceptJoinRequest request)
    {
        var joinRequest = await _joinRequestRepository.GetByIdAsync(request.RequestId);

        if (joinRequest is null)
            throw new NotFoundException("Join request not found.");

        var team = await _teamRepository.GetByIdAsync(joinRequest.TeamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to manage join requests for this project.");

        if (joinRequest.Status != JoinRequestStatus.Pending)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["RequestId"] = new[]
                {
                    "Only pending join requests can be accepted."
                }
            });
        }

        var teamMember = new TeamMember
        {
            TeamId = joinRequest.TeamId,
            UserId = joinRequest.UserId,
            Role = TeamMemberRole.Member,
            JoinedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        joinRequest.Status = JoinRequestStatus.Accepted;

        await _teamRepository.AddMemberAsync(teamMember);
        await _joinRequestRepository.UpdateAsync(joinRequest);

        await _joinRequestRepository.SaveChangesAsync();
    }

    public async Task RejectAsync(RejectJoinRequest request)
    {
        var joinRequest = await _joinRequestRepository.GetByIdAsync(request.RequestId);

        if (joinRequest is null)
            throw new NotFoundException("Join request not found.");

        var team = await _teamRepository.GetByIdAsync(joinRequest.TeamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to manage join requests for this project.");

        if (joinRequest.Status != JoinRequestStatus.Pending)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["RequestId"] = new[]
                {
                    "Only pending join requests can be rejected."
                }
            });
        }

        joinRequest.Status = JoinRequestStatus.Rejected;

        await _joinRequestRepository.UpdateAsync(joinRequest);
        await _joinRequestRepository.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid teamId, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
            throw new NotFoundException("Team not found.");

        var project = await _projectRepository.GetByIdAsync(team.ProjectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to remove team members.");

        var teamMember = await _teamRepository.GetMemberAsync(teamId, userId);

        if (teamMember is null)
            throw new NotFoundException("Team member not found.");

        if (!teamMember.IsActive)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["UserId"] = new[]
                {
                    "This user is already inactive in this team."
                }
            });
        }

        teamMember.IsActive = false;

        await _teamRepository.UpdateMemberAsync(teamMember);
        await _joinRequestRepository.SaveChangesAsync();
    }
}
