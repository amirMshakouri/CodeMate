using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);

    Task<bool> IsMemberAsync(Guid teamId, Guid userId);

    Task<TeamMember?> GetMemberAsync(Guid teamId, Guid userId);

    Task AddMemberAsync(TeamMember teamMember);

    Task UpdateMemberAsync(TeamMember teamMember);

    Task AddAsync(Team team);

    Task UpdateAsync(Team team);

    Task DeleteAsync(Team team);

    Task SaveChangesAsync();
}