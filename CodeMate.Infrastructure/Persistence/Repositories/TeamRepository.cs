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
    
    public async Task AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
    }

    public Task UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Team team)
    {
        _context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}