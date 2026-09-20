using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Services;

public sealed class ProjectMembershipChecker : IProjectMembershipChecker
{
    private readonly ApplicationDbContext _context;

    public ProjectMembershipChecker(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsProjectMemberAsync(
        Guid projectId,
        Guid userId)
    {
        return await _context.TeamMembers
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.IsActive &&
                !x.IsDeleted &&
                _context.Teams.Any(t =>
                    t.Id == x.TeamId &&
                    t.ProjectId == projectId &&
                    !t.IsDeleted));
    }
}