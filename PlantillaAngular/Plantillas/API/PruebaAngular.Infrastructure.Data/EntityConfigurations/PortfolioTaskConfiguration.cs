using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// EF Core configuration for PortfolioTask entity - PostgreSQL conventions
    /// </summary>
    public class PortfolioTaskConfiguration : IEntityTypeConfiguration<PortfolioTask>
    {
        public void Configure(EntityTypeBuilder<PortfolioTask> builder)
        {
            // Table name
            builder.ToTable("tasks", "public");

            // Primary Key
            builder.HasKey(t => t.TaskId);
            builder.Property(t => t.TaskId)
                .HasColumnName("id");

            // Foreign Key - project_id
            builder.Property(t => t.ProjectId)
                .HasColumnName("project_id")
                .IsRequired();

            // Title - required, max 200 chars
            builder.Property(t => t.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            // Description - optional, max 1000 chars
            builder.Property(t => t.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired(false);

            // Status - required, max 20 chars (stored as string enum)
            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired();

            // Priority - required, max 20 chars (stored as string enum)
            builder.Property(t => t.Priority)
                .HasColumnName("priority")
                .HasMaxLength(20)
                .IsRequired();

            // DueDate - optional
            builder.Property(t => t.DueDate)
                .HasColumnName("due_date")
                .IsRequired(false);

            // CreatedAt - required
            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Indexes
            builder.HasIndex(t => t.ProjectId)
                .HasDatabaseName("ix_tasks_project_id");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("ix_tasks_status");
        }
    }
}
