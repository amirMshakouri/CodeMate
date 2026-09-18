using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Infrastructure.Persistence.Context;
using CodeMate.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                        .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                        .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.UserName == userNameOrEmail ||
                    x.Email == userNameOrEmail);


        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PasswordResetToken == token);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName)
        {
            return await _context.Users
                .AnyAsync(x => x.UserName == userName);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);

            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<User>> SearchAsync(
    string? searchTerm,
    UserRole? role,
    bool? isActive,
    int pageNumber,
    int pageSize)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    x.UserName.Contains(searchTerm) ||
                    x.Email.Contains(searchTerm) ||
                    (x.FullName != null && x.FullName.Contains(searchTerm)));
            }

            if (role.HasValue)
            {
                query = query.Where(x => x.Role == role.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<User>(
                items,
                totalCount,
                pageNumber,
                pageSize);
        }
    }
}
