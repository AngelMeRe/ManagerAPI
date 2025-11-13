using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using ManagerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManagerAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _db;

        public CommentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CommentResponseDto> Add(int userId, CommentCreateDto dto)
        {
            var comment = new Comment
            {
                UserId = userId,
                TaskId = dto.TaskId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            var user = await _db.Users.FindAsync(userId);

            return new CommentResponseDto
            {
                Id = comment.Id,
                Text = comment.Content,
                CreatedAt = comment.CreatedAt,
                User = new CommentResponseDto.UserDto
                {
                    Id = user!.Id,
                    Name = user.Name
                }
            };
        }

        public async Task<List<CommentResponseDto>> GetByTask(int taskId)
        {
            var comments = await _db.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.User)
                .ToListAsync();

            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Text = c.Content,
                CreatedAt = c.CreatedAt,
                User = new CommentResponseDto.UserDto
                {
                    Id = c.User!.Id,
                    Name = c.User.Name
                }
            }).ToList();
        }

        public async Task<CommentResponseDto?> GetById(int id)
        {
            var c = await _db.Comments.Include(x => x.User).FirstOrDefaultAsync(c => c.Id == id);
            if (c == null) return null;

            return new CommentResponseDto
            {
                Id = c.Id,
                Text = c.Content,
                CreatedAt = c.CreatedAt,
                User = new CommentResponseDto.UserDto
                {
                    Id = c.User!.Id,
                    Name = c.User.Name
                }
            };
        }

        public async Task<bool> Delete(int id, int userId, string role)
        {
            var c = await _db.Comments.FindAsync(id);
            if (c == null) return false;

            if (role != "admin" && c.UserId != userId)
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este comentario");

            _db.Comments.Remove(c);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
