using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using ManagerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManagerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtService _jwt;

        public AuthService(ApplicationDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new Exception("Credenciales inválidas");

            var token = _jwt.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                Role = user.Role,
                UserId = user.Id,
                Name = user.Name
            };
        }

        public async Task<UserResponseDto> Register(UserCreateDto dto)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) throw new Exception("El email ya está registrado");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}

