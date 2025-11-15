using ManagerAPI.DTOs;

namespace ManagerAPI.Services.Interfaces
{
    // Interfaz para el servicio de autenticación
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<UserResponseDto> Register(UserCreateDto dto);
    }
}
