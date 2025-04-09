using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;

namespace TqPerformanceEvaluationHr.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvaluationModel> EvaluationModels => Set<EvaluationModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EvaluationModel>(entity =>
        {
            entity.ToTable("EvaluationModels");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(e => e.Description)
                  .HasMaxLength(500);
        });
    }
}
