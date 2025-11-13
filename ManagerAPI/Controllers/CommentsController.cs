using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Add([FromBody] CommentCreateDto dto)
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

            return Ok(new
            {
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.TaskId,
                comment.UserId
            });
        }
    }
}
