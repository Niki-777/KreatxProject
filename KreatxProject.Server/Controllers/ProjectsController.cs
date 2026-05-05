using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KreatxProject.Server.Data;
using KreatxProject.Models;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. MERR TË GJITHA PROJEKTET (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        // 2. SHTO NJË PROJEKT TË RI (POST)
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return Ok(project);
        }
    }
}
