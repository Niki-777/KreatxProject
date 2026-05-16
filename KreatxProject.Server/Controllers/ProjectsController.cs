using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KreatxProject.Models;
using KreatxProject.Services;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Vetëm përdoruesit e loguar mund të kenë akses
    public class ProjectsController : ControllerBase
    {
        // RREGULLIMI: Injektojmë Service-in në vend të DbContext-it direkt
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

        // Merr një projekt specifik (GET: api/projects/5)
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

        // Shto projekt të ri (POST: api/projects)
        // Vetëm Administratori ka të drejtë të krijojë projekte
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Project>> PostProject([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest("Të dhënat e projektit janë të pavlefshme.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProject = await _projectService.CreateProjectAsync(project);
            return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
        }

        // Fshij një projekt (DELETE: api/projects/5)
        // Vetëm Administratori dhe mbrohet nëse ka taske të hapura brenda Service-it
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectService.DeleteProjectAsync(id);

            if (!result.Success)
            {
                // Nëse ka taske të hapura, Service kthen false dhe mesazhin e bllokimit
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}