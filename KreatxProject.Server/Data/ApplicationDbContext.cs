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
        
    }
}