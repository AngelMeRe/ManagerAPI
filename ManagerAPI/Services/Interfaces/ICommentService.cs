using ManagerAPI.DTOs;

namespace ManagerAPI.Services.Interfaces
{
    // Interfaz para el servicio de comentarios
    public interface ICommentService
    {
        Task<CommentResponseDto> Add(int userId, CommentCreateDto dto);
        Task<List<CommentResponseDto>> GetByTask(int taskId);
        Task<CommentResponseDto?> GetById(int id);
        Task<bool> Delete(int id, int userId, string role);
    }
}
