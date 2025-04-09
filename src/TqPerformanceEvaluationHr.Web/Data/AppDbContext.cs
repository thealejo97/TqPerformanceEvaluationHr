using Microsoft.EntityFrameworkCore;

namespace TqPerformanceEvaluationHr.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Aquí se agregarán los DbSet para cada entidad
    // Ejemplo:
    // public DbSet<Employee> Employees { get; set; }
} 