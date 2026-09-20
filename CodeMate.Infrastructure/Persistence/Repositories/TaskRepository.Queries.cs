using CodeMate.Domain.Entities;
using CodeMate.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class TaskRepository
{
    public async Task<PaginatedList<TaskItem>> SearchAsync(
        Guid? projectId,
        TaskStatusEnum? status,
        Guid? assignedUserId,
        int pageNumber,
        int pageSize)
    {
        var query = _context.TaskItems
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (assignedUserId.HasValue)
        {
            query = query.Where(x => x.AssignedUserId == assignedUserId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<TaskItem>(
            items,
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<IEnumerable<TaskItem>> GetByAssignedUserIdAsync(
        Guid userId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.AssignedUserId == userId)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }
}