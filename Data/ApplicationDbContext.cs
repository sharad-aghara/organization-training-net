using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppliactionDbContext : DbContext
    {
        public AppliactionDbContext(DbContextOptions<AppliactionDbContext> options) : base(options)
        {

        }

        // Model intitiallization
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingEmployee> TrainingEmployees { get; set; }

        // relationship configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // one-to-many => Organization and Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Organization)
                .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict // Delete: Org -> Related Employees will be deleted

            // many-to-one => Training and Organization
            modelBuilder.Entity<Training>()
                .HasOne(t => t.Organization)
                .WithMany(o => o.Training)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict // Delete: Org -> Related Trainings will be deleted

            // many-to-one => Training and TEmployee
            modelBuilder.Entity<Training>()
                .HasMany(t => t.TrainingEmployees)
                .WithOne(te => te.Training)
                .HasForeignKey(e => e.TrainingId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict

            // TrainingEmployee
            modelBuilder.Entity<TrainingEmployee>()
                .HasKey(te => new { te.TrainingId, te.EmployeeId });

            modelBuilder.Entity<TrainingEmployee>()
                .HasOne(te => te.Training)
                .WithMany(t => t.TrainingEmployees)
                .HasForeignKey(te => te.TrainingId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict // Prevent cascade delete

            modelBuilder.Entity<TrainingEmployee>()
                .HasOne(te => te.Employee)
                .WithMany(e => e.TrainingEmployees)
                .HasForeignKey(te => te.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); // Prevent cascade delete


        }

        // Inject ApplicationDbContext to Program.cs - so it can be used by Controller
    }
}
