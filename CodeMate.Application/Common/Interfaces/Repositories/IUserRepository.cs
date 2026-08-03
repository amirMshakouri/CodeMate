using CodeMate.Domain.Entities;
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

        Task<bool> ExistsByUserNameAsync(string userName);

        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(User user);

        Task UpdateAsync(User user);
    }
}
