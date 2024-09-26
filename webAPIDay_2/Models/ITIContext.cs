using Microsoft.EntityFrameworkCore;

namespace webAPIDay_2.Models
{
    public class ITIContext:DbContext
    {
        public DbSet<Department> Department { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }

        public ITIContext(DbContextOptions<ITIContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Projects)
                .WithMany(d => d.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeProjects",
                    j => j
                        .HasOne<Project>()
                        .WithMany()
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<Employee>()
                        .WithMany()
                        .OnDelete(DeleteBehavior.Restrict) 
                );
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
