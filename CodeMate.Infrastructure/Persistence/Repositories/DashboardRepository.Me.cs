using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class DashboardRepository
{
    public async Task<List<TaskItem>> GetMyTasksAsync(Guid userId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Where(t => t.AssignedUserId == userId && !t.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Guid>> GetMyProjectIdsAsync(Guid userId)
    {
        var memberProjectIds = _context.TeamMembers
            .AsNoTracking()
            .Where(tm => tm.UserId == userId && tm.IsActive && !tm.IsDeleted)
            .Join(_context.Teams.Where(t => !t.IsDeleted),
                tm => tm.TeamId,
                t => t.Id,
                (tm, t) => t.ProjectId);

        var ownedProjectIds = _context.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == userId && !p.IsDeleted)
            .Select(p => p.Id);

        return await memberProjectIds.Union(ownedProjectIds).ToListAsync();
    }
}