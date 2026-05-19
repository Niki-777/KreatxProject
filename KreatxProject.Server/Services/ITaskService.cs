using KreatxProject.Models;

namespace KreatxProject.Server.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(int projectId, string userId, string role);
        Task<ProjectTask> CreateTaskAsync(ProjectTask task, string userId, string role);
        Task<bool> UpdateTaskAsync(int id, ProjectTask updatedTask, string userId, string role);
        Task<bool> DeleteTaskAsync(int id, string role);
    }
}