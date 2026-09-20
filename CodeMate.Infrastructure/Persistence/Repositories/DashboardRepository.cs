using System;
using System.Collections.Generic;
using System.Text;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Domain.Entities;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetProjectTasksAsync(Guid projectId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId && !t.IsDeleted)
            .ToListAsync();
    }
}