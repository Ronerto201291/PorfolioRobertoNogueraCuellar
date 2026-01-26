using PruebaAngular.Domain.AggregateModels;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data.Core;
using PruebaAngular.Infrastructure.Data.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace PruebaAngular.Infrastructure.Data
{
    public class PruebaAngularContext : SqlBaseContext
    {
        public readonly IDbContextSchema DefaultSchema;

        public PruebaAngularContext(DbContextOptions<PruebaAngularContext> options) 
            : base(options, null!, null!)
        {
            System.Diagnostics.Debug.WriteLine("PruebaAngularDbContext::ctor (simple) ->" + GetHashCode());
        }

        public PruebaAngularContext(IDbContextSchema dbContextSchema, DbContextOptions<PruebaAngularContext> options, IMediator mediator, ICurrentSessionProvider currentSessionProvider) 
            : base(options, mediator, currentSessionProvider)
        {
            this.DefaultSchema = dbContextSchema;
            System.Diagnostics.Debug.WriteLine("PruebaAngularDbContext::ctor ->" + GetHashCode());
        }

        // Portfolio aggregate
        public DbSet<Project> Projects { get; set; }
        public DbSet<PortfolioTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set default schema to public for PostgreSQL
            modelBuilder.HasDefaultSchema("public");

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new PortfolioTaskConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
