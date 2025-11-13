using ManagerAPI.DTOs;

namespace ManagerAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<UserResponseDto> Register(UserCreateDto dto);
    }
}
