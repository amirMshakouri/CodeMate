using CodeMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class TeamRepository
{
    public async Task<IEnumerable<Team>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Teams
            .AsNoTracking()
            .Where(x =>
                x.ProjectId == projectId &&
                !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<TeamMember>> GetMembersAsync(Guid teamId)
    {
        return await _context.TeamMembers
            .AsNoTracking()
            .Where(x =>
                x.TeamId == teamId &&
                x.IsActive &&
                !x.IsDeleted)
            .OrderBy(x => x.JoinedAt)
            .ToListAsync();
    }
}