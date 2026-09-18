using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByUserNameAsync(string userName);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);

        Task<User?> GetByPasswordResetTokenAsync(string token);

        Task<bool> ExistsByUserNameAsync(string userName);

        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task SaveChangesAsync();


        Task<PaginatedList<User>> SearchAsync(
            string? searchTerm,
            UserRole? role,
            bool? isActive,
            int pageNumber,
            int pageSize);
    }
}
