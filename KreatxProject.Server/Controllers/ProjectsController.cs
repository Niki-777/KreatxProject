using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KreatxProject.Server.Data;
using KreatxProject.Models;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //vetëm perdoruesit e loguar mund të kene akses
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // merr projektet (GET: api/Projects)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // Shto projekt te ri (POST: api/Projects)
        // Vetem Administratori ka te drejtë të krijoje projekte
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (project == null)
            {
                return BadRequest("Të dhënat e projektit janë të pavlefshme.");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // Fshij nje projekt (DELETE: api/Projects/5)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}