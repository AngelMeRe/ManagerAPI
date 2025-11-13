using ManagerAPI.DTOs;

namespace ManagerAPI.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto> Add(int userId, CommentCreateDto dto);
        Task<List<CommentResponseDto>> GetByTask(int taskId);
        Task<CommentResponseDto?> GetById(int id);
        Task<bool> Delete(int id, int userId, string role);
    }
}
