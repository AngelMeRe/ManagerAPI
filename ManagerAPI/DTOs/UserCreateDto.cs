using System.ComponentModel.DataAnnotations;

namespace ManagerAPI.DTOs
{
    // DTO para crear un nuevo usuario
    public class UserCreateDto
    {
        [Required, MaxLength(120)] 
        public string Name { get; set; } = "";

        [Required, EmailAddress] 
        public string Email { get; set; } = "";

        [Required] 
        public string Role { get; set; } = "user"; // 'user' | 'admin'
        [Required, MinLength(6)] 
        public string Password { get; set; } = "";
    }

    // DTO para actualizar un usuario existente
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Role { get; set; } // 'user' | 'admin'
    }

    // DTO para manejar las respuestas de los usuarios
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "user";
    }
}
