using System.ComponentModel.DataAnnotations;

namespace ManagerAPI.DTOs
{
    //DTO para manejar las solicitudes de inicio de sesión
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
