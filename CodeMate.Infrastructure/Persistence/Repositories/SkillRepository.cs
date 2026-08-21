using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Domain.Entities;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public sealed class SkillRepository : ISkillRepository
{
    private readonly ApplicationDbContext _context;

    public SkillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Skill?> GetByNameAsync(string name)
    {
        return await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted);
    }

    public async Task<IEnumerable<Skill>> SearchAsync(string? term)
    {
        var query = _context.Skills
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(x =>
                x.Name.Contains(term));
        }

        return await query.ToListAsync();
    }

    public async Task AddSkillAsync(Skill skill)
    {
        await _context.Skills.AddAsync(skill);
    }

    public async Task<IEnumerable<UserSkill>> GetUserSkillsAsync(Guid userId)
    {
        return await _context.UserSkills
            .AsNoTracking()
            .Include(x => x.Skill)
            .Where(x =>
                x.UserId == userId &&
                !x.IsDeleted &&
                !x.Skill.IsDeleted)
            .ToListAsync();
    }

    public async Task AddUserSkillAsync(UserSkill userSkill)
    {
        await _context.UserSkills.AddAsync(userSkill);
    }

    public async Task RemoveUserSkillAsync(Guid userId, Guid skillId)
    {
        var userSkill = await _context.UserSkills
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.SkillId == skillId &&
                !x.IsDeleted);

        if (userSkill is not null)
        {
            userSkill.IsDeleted = true;
            userSkill.DeletedAt = DateTimeOffset.UtcNow;
        }
    }

    public async Task<bool> HasUserSkillAsync(Guid userId, Guid skillId)
    {
        return await _context.UserSkills
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.SkillId == skillId &&
                !x.IsDeleted);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}