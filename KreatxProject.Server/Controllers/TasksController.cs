using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KreatxProject.Server.Data;
using KreatxProject.Models;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Të dy rolet kanë qasje të përgjithshme
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Merr të gjitha tasket për një projekt specifik (GET: api/tasks/project/5)
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasksByProject(int projectId)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            return Ok(tasks);
        }

        // 2. Shto një Task të ri (POST: api/tasks)
        [HttpPost]
        public async Task<ActionResult<ProjectTask>> PostTask([FromBody] ProjectTask task)
        {
            if (task == null) return BadRequest("Të dhënat janë të pavlefshme.");

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasksByProject), new { projectId = task.ProjectId }, task);
        }

        // 3. Përditëso statusin e Taskut - Psh. shëno si i përfunduar (PUT: api/tasks/5)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] ProjectTask updatedTask)
        {
            if (id != updatedTask.Id) return BadRequest("ID nuk përputhet.");

            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return NotFound("Tasku nuk u gjet.");

            // Përditësojmë fushat kryesore
            task.IsCompleted = updatedTask.IsCompleted;
            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.AssignedToUserId = updatedTask.AssignedToUserId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 4. Fshij një task (DELETE: api/tasks/5) - Vetëm Administratori
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}