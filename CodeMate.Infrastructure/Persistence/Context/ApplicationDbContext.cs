using System;
using System.Collections.Generic;
using System.Text;
using CodeMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}
