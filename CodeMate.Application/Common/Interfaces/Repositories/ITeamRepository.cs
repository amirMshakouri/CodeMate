using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
}