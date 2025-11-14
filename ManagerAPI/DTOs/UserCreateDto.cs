using System.ComponentModel.DataAnnotations;

namespace ManagerAPI.DTOs
{
    public class UserCreateDto
    {
        [Required, MaxLength(120)] public string Name { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Role { get; set; } = "user"; // 'user' | 'admin'
        [Required, MinLength(6)] public string Password { get; set; } = "";
    }
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Role { get; set; } // 'user' | 'admin'
    }
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "user";
    }
}
