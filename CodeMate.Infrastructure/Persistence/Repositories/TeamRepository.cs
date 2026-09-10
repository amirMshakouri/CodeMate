using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Domain.Entities;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
}