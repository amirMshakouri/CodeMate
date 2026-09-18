using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CodeMate.Infrastructure.Persistence.Seed
{
    public static class SeedData
    {
        public static async Task SeedAdminAsync(
            ApplicationDbContext db,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            var hasAdmin = await db.Users
                .AnyAsync(u => u.Role == UserRole.Admin);

            if (hasAdmin)
                return;

            var email = configuration["AdminSeed:Email"] ?? "admin@codemate.com";
            var password = configuration["AdminSeed:Password"] ?? "Admin@12345";

            var admin = new User
            {
                UserName = "admin",
                Email = email,
                PasswordHash = passwordHasher.Hash(password),
                Role = UserRole.Admin,
                IsActive = true
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}