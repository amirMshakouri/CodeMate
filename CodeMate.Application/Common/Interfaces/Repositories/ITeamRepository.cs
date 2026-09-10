using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);

    Task AddAsync(Team team);

    Task UpdateAsync(Team team);

    Task DeleteAsync(Team team);

    Task SaveChangesAsync();
}