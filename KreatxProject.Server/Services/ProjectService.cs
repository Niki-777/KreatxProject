using KreatxProject.Models;
using KreatxProject.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace KreatxProject.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        // Rregulluar emertimi i parametrit ne konstruktor
        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            project.StartDate = DateTime.Now; // Mbajme vleren tende te dates
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(p => p.Id == project.Id)) return false;
                throw;
            }
        }

        // Kthyer ne Task<bool> per te perputhur saktesisht me interface-in tend
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            // Kontrollojme nese ka taske qe nuk jane perfunduar (IsCompleted == false)
            var hasOpenTasks = await _context.ProjectTasks
                .AnyAsync(t => t.ProjectId == id && !t.IsCompleted);

            if (hasOpenTasks)
            {
                // Hedhim exception sipas kerkeses se profesorit qe te kapet nga middleware
                throw new InvalidOperationException("Nuk mund te fshini nje projekt qe ka ende taske te hapura.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        // Shtimi i punonjesit ne projekt (Many-to-Many)
        public async Task<bool> AddEmployeeToProjectAsync(int projectId, string employeeId)
        {
            var exists = await _context.ProjectEmployees
                .AnyAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

            if (exists) return true;

            var relation = new ProjectEmployee
            {
                ProjectId = projectId,
                EmployeeId = employeeId
            };

            _context.ProjectEmployees.Add(relation);
            await _context.SaveChangesAsync();
            return true;
        }

        // Heqja e punonjesit nga projekti
        public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, string employeeId)
        {
            var relation = await _context.ProjectEmployees
                .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

            if (relation == null) return false;

            _context.ProjectEmployees.Remove(relation);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}