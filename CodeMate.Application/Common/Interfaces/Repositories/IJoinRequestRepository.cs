using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface IJoinRequestRepository
{
    Task<JoinRequest?> GetByIdAsync(Guid id);
}