using KreatxProject.Models;

namespace KreatxProject.Services
{
    public interface IProjectService
    {
        // Merr te gjithe projektet ne menyre asinkrone
        Task<IEnumerable<Project>> GetAllProjectsAsync();

        // Merr nje projekt specifik bazuar ne ID
        Task<Project?> GetProjectByIdAsync(int id);

        // Krijon nje projekt te ri
        Task<Project> CreateProjectAsync(Project project);

        // Perditeson te dhenat e nje projekti ekzistues
        Task<bool> UpdateProjectAsync(Project project);

        // Fshin nje projekt bazuar ne ID (Kthen true/false ose hedh exception)
        Task<bool> DeleteProjectAsync(int id);

        // Shton nje punonjes ne projekt (Lidhja many-to-many)
        Task<bool> AddEmployeeToProjectAsync(int projectId, string employeeId);

        // Heq nje punonjes nga projekti
        Task<bool> RemoveEmployeeFromProjectAsync(int projectId, string employeeId);
    }
}