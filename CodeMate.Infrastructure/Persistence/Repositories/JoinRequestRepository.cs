using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Domain.Entities;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class JoinRequestRepository : IJoinRequestRepository
{
    private readonly ApplicationDbContext _context;

    public JoinRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JoinRequest?> GetByIdAsync(Guid id)
    {
        return await _context.JoinRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
}