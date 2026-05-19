using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KreatxProject.Models;  
using Microsoft.EntityFrameworkCore;

namespace KreatxProject.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Përcaktimi i Composite Key për tabelën lidhëse
            modelBuilder.Entity<ProjectEmployee>()
                .HasKey(pe => new { pe.ProjectId, pe.EmployeeId });

            // Lidhja me Projektin
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Project)
                .WithMany() // Mund të shtosh .WithMany(p => p.ProjectEmployees) nëse dëshiron më vonë te modeli Project
                .HasForeignKey(pe => pe.ProjectId);

            // Lidhja me Përdoruesin (Employee)
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Employee)
                .WithMany()
                .HasForeignKey(pe => pe.EmployeeId);
        }
    }
}