using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using ManagerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _db;

        public TaskService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TaskResponseDto>> GetAll()
        {
            var tasks = await _db.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .ToListAsync();

            return tasks.Select(t => new TaskResponseDto
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

                Comments = t.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name
                }).ToList()

            }).ToList();
        }

        public async Task<TaskResponseDto?> GetById(int id)
        {
            var t = await _db.Tasks
                .Include(x => x.AssignedTo)
                .Include(x => x.CreatedBy)
                .Include(x => x.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (t == null) return null;

            return new TaskResponseDto
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

                Comments = t.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name
                }).ToList()
            };
        }


        public async Task<List<TaskResponseDto>> GetMine(int userId)
        {
            var tasks = await _db.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments).ThenInclude(c => c.User)
                .Where(t => t.AssignedToId == userId || t.CreatedById == userId)
                .ToListAsync();

            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                AssignedTo = t.AssignedTo == null ? null : new TaskResponseDto.SimpleUserDto { Id = t.AssignedTo.Id, Name = t.AssignedTo.Name },
                CreatedBy = t.CreatedBy == null ? null : new TaskResponseDto.SimpleUserDto { Id = t.CreatedBy.Id, Name = t.CreatedBy.Name },
                Comments = t.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name
                }).ToList()
            }).ToList();
        }

        public async Task<TaskResponseDto> Create(TaskCreateDto dto, int userId)
        {
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

            return await GetById(task.Id) ?? throw new Exception("Error al crear tarea");
        }

        public async Task<TaskResponseDto?> Update(int id, TaskUpdateDto dto, int userId, string role)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return null;

            if (role != "admin" && task.CreatedById != userId && task.AssignedToId != userId)
                throw new UnauthorizedAccessException("No tienes permisos para editar esta tarea");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                task.Title = dto.Title;

            if (dto.Description != null)
                task.Description = dto.Description;

            if (dto.DueDate.HasValue)
                task.DueDate = dto.DueDate.Value;

            if (dto.AssignedToId.HasValue)
                task.AssignedToId = dto.AssignedToId.Value;

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                var normalized = dto.Status.Trim();
                var allowed = new[] { "Pendiente", "En Progreso", "Completada" };
                if (!allowed.Contains(normalized, StringComparer.Ordinal))
                    throw new ArgumentException("Estado inválido");
                task.Status = normalized;
            }

            await _db.SaveChangesAsync();
            return await GetById(task.Id);
        }

        public async Task<bool> Delete(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return false;

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
