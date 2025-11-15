using System.ComponentModel.DataAnnotations;

namespace ManagerAPI.DTOs
{
    // DTO para crear una nueva tarea
    public class TaskCreateDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedToId { get; set; }
        public string? Status { get; set; }
    }

    // DTO para actualizar una tarea existente
    public class TaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedToId { get; set; }
        public string? Status { get; set; }
    }
}

