using KreatxProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KreatxProject.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> CreateProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(Project project);
        Task<(bool Success, string Message)> DeleteProjectAsync(int id);
    }
}