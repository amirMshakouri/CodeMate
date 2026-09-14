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
        public DbSet<Project> Projects { get; set; } = null!;
        //public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        //public DbSet<JoinRequest> JoinRequests { get; set; } = null!;
        public DbSet<UserSkill> UserSkills { get; set; } = null!;
        
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public DbSet<JoinRequest> JoinRequests { get; set; } = null!;
        public DbSet<ProjectSkill> ProjectSkills { get; set; } = null!;
        
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
