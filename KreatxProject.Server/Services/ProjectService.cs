using KreatxProject.Models;
using KreatxProject.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KreatxProject.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            // Marrim projektet bashkë me tasket e tyre përkatëse
            return await _context.Projects.Include(p => p.Tasks).ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            project.StartDate = DateTime.Now;
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

        public async Task<(bool Success, string Message)> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return (false, "Projekti nuk u gjet.");
            }

            // KËRKESA E DETYRËS: Administratori nuk mund të fshijë projekte që kanë taske të hapura (IsCompleted == false)
            bool hasOpenTasks = project.Tasks.Any(t => !t.IsCompleted);
            if (hasOpenTasks)
            {
                return (false, "Nuk mund ta fshini këtë projekt sepse ka ende detyra (tasks) të papërfunduara!");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return (true, "Projekti u fshi me sukses.");
        }
    }
}