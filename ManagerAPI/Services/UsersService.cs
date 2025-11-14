using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagerAPI.Services
{
    public class UsersService
    {
        private readonly ApplicationDbContext _db;
        public UsersService(ApplicationDbContext db) { _db = db; }

        public async Task<List<UserResponseDto>> GetAll()
        {
            return await _db.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetById(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return null;

            return new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            };
        }

        public async Task<UserResponseDto> Create(UserCreateDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.Email == dto.Email))
                throw new ArgumentException("El email ya está registrado");

            if (dto.Role != "admin" && dto.Role != "user")
                throw new ArgumentException("Rol inválido");

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                Role = dto.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return await GetById(user.Id) ?? throw new Exception("No se pudo crear el usuario");
        }

        public async Task<UserResponseDto?> Update(int id, UserUpdateDto dto)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                u.Name = dto.Name!.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (dto.Role != "admin" && dto.Role != "user")
                    throw new ArgumentException("Rol inválido");
                u.Role = dto.Role!;
            }

            await _db.SaveChangesAsync();
            return await GetById(u.Id);
        }

        public async Task<bool> Delete(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return false;

            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

