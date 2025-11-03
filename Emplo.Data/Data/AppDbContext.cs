using Emplo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emplot.Data.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Vacation> Vacations { get; set; }
    public DbSet<VacationPackage> VacationPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Employee>()
            .HasOne(e => e.Team)
            .WithMany(t => t.Employees)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Employee>()
            .HasOne(e => e.VacationPackage)
            .WithMany(vp => vp.Employees)
            .HasForeignKey(e => e.VacationPackageId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Vacation>()
            .HasOne(v => v.Employee)
            .WithMany(e => e.Vacations)
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
