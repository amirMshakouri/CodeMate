
using CodeMate.Domain.Entities;
using CodeMate.Shared.Pagination;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);

    Task AddAsync(TaskItem task);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(TaskItem task);

    Task SaveChangesAsync();

    Task<PaginatedList<TaskItem>> SearchAsync(
        Guid? projectId,
        TaskStatusEnum? status,
        Guid? assignedUserId,
        int pageNumber,
        int pageSize);

    Task<IEnumerable<TaskItem>> GetByAssignedUserIdAsync(
        Guid userId);
}

