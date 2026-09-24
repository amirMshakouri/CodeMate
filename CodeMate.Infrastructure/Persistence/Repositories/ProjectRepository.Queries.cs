using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class ProjectRepository
{
    public async Task<PaginatedList<Project>> SearchAsync(
        string? title,
        ProjectStatus? status,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Projects
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(x => x.Title.Contains(title));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
           .OrderByDescending(x => x.CreatedAt)
.ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<Project>(
            items,
            totalCount,
            pageNumber,
            pageSize);
    }
}