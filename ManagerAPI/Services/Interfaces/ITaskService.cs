using ManagerAPI.DTOs;

namespace ManagerAPI.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskResponseDto>> GetAll();
        Task<TaskResponseDto?> GetById(int id);
        Task<TaskResponseDto> Create(TaskCreateDto dto, int userId);
        Task<TaskResponseDto?> Update(int id, TaskUpdateDto dto, int userId, string role);
        Task<bool> Delete(int id);
    }
}
