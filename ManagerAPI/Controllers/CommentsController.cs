using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CommentsController(ApplicationDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Comment dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            var comment = new Comment
            {
                Content = dto.Content,
                TaskId = dto.TaskId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // Mapear a DTO para la respuesta
            var user = await _db.Users.FindAsync(userId);

            var response = new CommentResponseDto
            {
                Id = comment.Id,
                Text = comment.Content,
                CreatedAt = comment.CreatedAt,
                User = new CommentResponseDto.UserDto
                {
                    Id = user.Id,
                    Name = user.Name
                }
            };

            return Ok(response);
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var comments = await _db.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.User)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name
                })
                .ToListAsync();

            return Ok(comments);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Solo admin o autor pueden borrar
            if (role != "admin" && comment.UserId != userId)
                return Forbid();

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Comentario eliminado" });
        }
    }
}
