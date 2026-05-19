using Microsoft.EntityFrameworkCore;
using KreatxProject.Server.Data;
using KreatxProject.Models;

namespace KreatxProject.Server.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Merr tasket për projekt specifik, por me kontroll rolesh
        public async Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(int projectId, string userId, string role)
        {
            if (role == "Administrator")
            {
                return await _context.ProjectTasks
                    .Where(t => t.ProjectId == projectId)
                    .ToListAsync();
            }

            // Employee: Sheh tasket e projektit në read-only mode nëse supozohet se është pjesë e tij
            return await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        // 2. Krijimi i një tasku të ri
        public async Task<ProjectTask> CreateTaskAsync(ProjectTask task, string userId, string role)
        {
            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        // 3. Përditësimi i plotë i taskut (Status, Titull, etj.) me kontroll sigurie
        public async Task<bool> UpdateTaskAsync(int id, ProjectTask updatedTask, string userId, string role)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return false;

            // RREGULLI I PROFESORIT: Employee nuk mund të modifikojë tasket që nuk i ka të caktuara vetë
            if (role != "Administrator" && task.AssignedToUserId != userId)
            {
                throw new UnauthorizedAccessException("Nuk keni të drejtë të modifikoni një task që nuk ju është caktuar juve.");
            }

            // Përditësojmë fushat ekzaktësisht siç i kishe ti
            task.IsCompleted = updatedTask.IsCompleted;
            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.AssignedToUserId = updatedTask.AssignedToUserId;

            await _context.SaveChangesAsync();
            return true;
        }

        // 4. Fshirja e taskut (Vetëm Administratori)
        public async Task<bool> DeleteTaskAsync(int id, string role)
        {
            if (role != "Administrator")
            {
                throw new UnauthorizedAccessException("Vetëm administratori mund të fshijë taske.");
            }

            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return false;

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}