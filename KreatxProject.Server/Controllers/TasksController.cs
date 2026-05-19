using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KreatxProject.Models;
using KreatxProject.Server.Services;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // 1. Merr të gjitha tasket për një projekt specifik (GET: api/tasks/project/5)
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var tasks = await _taskService.GetTasksByProjectAsync(projectId, userId, role);
            return Ok(tasks);
        }

        // 2. Shto një Task të ri (POST: api/tasks)
        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody] ProjectTask task)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var createdTask = await _taskService.CreateTaskAsync(task, userId, role);

            return CreatedAtAction(nameof(GetTasksByProject), new { projectId = createdTask.ProjectId }, createdTask);
        }

        // 3. Përditëso statusin dhe fushat e Taskut (PUT: api/tasks/5)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] ProjectTask updatedTask)
        {
            if (id != updatedTask.Id) return BadRequest("ID nuk përputhet.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            try
            {
                var result = await _taskService.UpdateTaskAsync(id, updatedTask, userId, role);
                if (!result) return NotFound("Tasku nu u gjet.");

                return NoContent(); // Kthen 204 ekzaktësisht si kodi yt i vjetër
            }
            catch (UnauthorizedAccessException ex)
            {
                // Nëse punonjësi tenton të ndryshojë një task që s'është i tij, kthehet 403 Forbidden
                return StatusCode(403, ex.Message);
            }
        }

        // 4. Fshij një task (DELETE: api/tasks/5) - Vetëm Administratori
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            try
            {
                var result = await _taskService.DeleteTaskAsync(id, role);
                if (!result) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}