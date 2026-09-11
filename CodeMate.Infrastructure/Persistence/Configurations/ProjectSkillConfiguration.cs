using CodeMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeMate.Infrastructure.Persistence.Configurations;

public sealed class ProjectSkillConfiguration : IEntityTypeConfiguration<ProjectSkill>
{
    public void Configure(EntityTypeBuilder<ProjectSkill> builder)
    {
        builder.ToTable("ProjectSkills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequiredLevel)
            .IsRequired();

        builder.Property(x => x.IsMandatory)
            .IsRequired();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Skill>()
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProjectId);

        builder.HasIndex(x => x.SkillId);

        builder.HasIndex(x => new { x.ProjectId, x.SkillId })
            .IsUnique();
    }
}