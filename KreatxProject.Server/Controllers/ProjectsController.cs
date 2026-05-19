using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KreatxProject.Models;
using KreatxProject.Services;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Vetem perdoruesit e loguar mund te kene akses
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // Merr projektet (GET: api/projects)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        // Merr nje projekt specifik (GET: api/projects/5)
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound("Projekti nuk u gjet.");
            }

            return Ok(project);
        }

        // Shto projekt te ri (POST: api/projects)
        // Vetem Administratori ka te drejte te krijoje projekte
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Project>> PostProject([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest("Te dhenat e projektit jane te pavlefshme.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProject = await _projectService.CreateProjectAsync(project);
            return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
        }

        // Fshij nje projekt (DELETE: api/projects/5)
        // Vetem Administratori dhe mbrohet nese ka taske te hapura brenda Service-it
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            // Shërbimi kthen true ose false, ose hedh Exception nëse ka taske të hapura
            var deleted = await _projectService.DeleteProjectAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Project not found." });
            }

            return Ok(new { message = "Project deleted successfully." });
        }

        // Shto punonjes ne projekt (POST: api/projects/{projectId}/employees)
        [HttpPost("{projectId}/employees")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddEmployeeToProject(int projectId, [FromBody] string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                return BadRequest(new { message = "Id e punonjesit eshte e detyrueshme." });
            }

            var result = await _projectService.AddEmployeeToProjectAsync(projectId, employeeId);
            if (!result)
            {
                return BadRequest(new { message = "Nuk u mundesua shtimi i punonjesit ne projekt." });
            }

            return Ok(new { message = "Punonjesi u shtua me sukses ne projekt." });
        }

        // Hiq punonjes nga projekti (DELETE: api/projects/{projectId}/employees/{employeeId})
        [HttpDelete("{projectId}/employees/{employeeId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveEmployeeFromProject(int projectId, string employeeId)
        {
            var result = await _projectService.RemoveEmployeeFromProjectAsync(projectId, employeeId);
            if (!result)
            {
                return NotFound(new { message = "Lidhja midis projektit dhe punonjesit nuk u gjet." });
            }

            return Ok(new { message = "Punonjesi u hoq me sukses nga projekti." });
        }
    }
}