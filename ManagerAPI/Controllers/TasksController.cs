using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TasksController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _db.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _db.Tasks
                .Include(t => t.Comments)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [Authorize] // any authenticated user can create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            // get user id from token
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description ?? "",
                DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(7),
                Status = "Pendiente",
                CreatedById = userId,
                AssignedToId = dto.AssignedToId ?? userId
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return Ok(task);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            // optional: check permission - e.g. only admin or creator
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            if (role != "admin" && task.CreatedById != userId && task.AssignedToId != userId)
                return Forbid();

            task.Title = dto.Title;
            task.Description = dto.Description ?? task.Description;
            task.DueDate = dto.DueDate ?? task.DueDate;
            task.AssignedToId = dto.AssignedToId ?? task.AssignedToId;

            await _db.SaveChangesAsync();
            return Ok(task);
        }

        [Authorize(Roles = "admin")] // only admin can delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
