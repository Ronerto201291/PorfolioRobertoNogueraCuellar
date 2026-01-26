using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// EF Core configuration for Project entity - PostgreSQL conventions
    /// </summary>
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            // Table name
            builder.ToTable("projects", "public");

            // Primary Key
            builder.HasKey(p => p.ProjectId);
            builder.Property(p => p.ProjectId)
                .HasColumnName("id");

            // Name - required, max 200 chars
            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            // Description - optional, max 1000 chars
            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired(false);

            // CreatedAt - required
            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Relationship with Tasks
            builder.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
