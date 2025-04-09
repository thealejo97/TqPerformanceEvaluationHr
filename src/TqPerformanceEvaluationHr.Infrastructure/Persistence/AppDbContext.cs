using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;

namespace TqPerformanceEvaluationHr.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvaluationModel> EvaluationModels { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<GroupEmployee> GroupEmployees { get; set; }
    public DbSet<EvaluationGroup> EvaluationGroups { get; set; }
    public DbSet<EvaluationCycle> EvaluationCycles { get; set; }
    public DbSet<Evaluation> Evaluations { get; set; }
    public DbSet<Questionnaire> Questionnaires { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<EvaluationResponse> EvaluationResponses { get; set; }
    public DbSet<EvaluationResult> EvaluationResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // EvaluationModel
        modelBuilder.Entity<EvaluationModel>(entity =>
        {
            entity.ToTable("EvaluationModels");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Position
        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("Positions");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(500);
        });

        // Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });

        // GroupEmployee
        modelBuilder.Entity<GroupEmployee>(entity =>
        {
            entity.ToTable("GroupEmployees");
            entity.HasKey(g => g.Id);
        });

        // EvaluationGroup
        modelBuilder.Entity<EvaluationGroup>(entity =>
        {
            entity.ToTable("EvaluationGroups");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
        });

        // EvaluationCycle
        modelBuilder.Entity<EvaluationCycle>(entity =>
        {
            entity.ToTable("EvaluationCycles");
            entity.HasKey(c => c.Id);
        });

        // Questionnaire
        modelBuilder.Entity<Questionnaire>(entity =>
        {
            entity.ToTable("Questionnaires");
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Title).IsRequired().HasMaxLength(100);
        });

        // Question
        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Text).IsRequired().HasMaxLength(500);
        });

        // Evaluation
        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.ToTable("Evaluations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EvaluationDate).IsRequired();
        });

        // EvaluationResponse
        modelBuilder.Entity<EvaluationResponse>(entity =>
        {
            entity.ToTable("EvaluationResponses");
            entity.HasKey(r => r.Id);
        });

        // EvaluationResult
        modelBuilder.Entity<EvaluationResult>(entity =>
        {
            entity.ToTable("EvaluationResults");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.QualitativeConcept).IsRequired().HasMaxLength(200);
        });
    }
}

