using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class JoinRequestRepository
{
    public async Task<IEnumerable<JoinRequest>> GetPendingRequestsForTeamAsync(
        Guid teamId)
    {
        return await _context.JoinRequests
            .AsNoTracking()
            .Where(x =>
                x.TeamId == teamId &&
                x.Status == JoinRequestStatus.Pending &&
                !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<JoinRequest>> GetMyJoinRequestsAsync(
        Guid userId)
    {
        return await _context.JoinRequests
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}