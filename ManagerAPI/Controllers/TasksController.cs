using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static ManagerAPI.DTOs.TaskResponseDto;

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
                .Include(t => t.Comments)
                .ToListAsync();

            var dtoList = tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,

                AssignedTo = t.AssignedTo == null ? null : new TaskResponseDto.SimpleUserDto
                {
                    Id = t.AssignedTo.Id,
                    Name = t.AssignedTo.Name
                },

                CreatedBy = t.CreatedBy == null ? null : new TaskResponseDto.SimpleUserDto
                {
                    Id = t.CreatedBy.Id,
                    Name = t.CreatedBy.Name
                },

                Comments = t.Comments?.Select(c => new TaskResponseDto.CommentDto
                {
                    Id = c.Id,
                    Text = c.Content,
                    UserId = c.UserId
                }).ToList() ?? new List<TaskResponseDto.CommentDto>()
            })
            .ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _db.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            var dto = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                AssignedTo = task.AssignedTo == null ? null : new SimpleUserDto
                {
                    Id = task.AssignedTo.Id,
                    Name = task.AssignedTo.Name
                },
                CreatedBy = task.CreatedBy == null ? null : new SimpleUserDto
                {
                    Id = task.CreatedBy.Id,
                    Name = task.CreatedBy.Name
                },
                Comments = task.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Content,
                    UserId = c.UserId
                }).ToList()
            };

            return Ok(dto);
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
                DueDate = (dto.DueDate?.ToUniversalTime()) ?? DateTime.UtcNow.AddDays(7),
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
            task.DueDate = dto.DueDate?.ToUniversalTime() ?? task.DueDate;
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
