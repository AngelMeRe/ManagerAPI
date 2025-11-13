using System.ComponentModel.DataAnnotations;

namespace ManagerAPI.DTOs
{
    public class UserCreateDto
    {
        [Required] 
        public string Name { get; set; } = "";

        [Required, EmailAddress] 
        public string Email { get; set; } = "";

        [Required, MinLength(6)] 
        public string Password { get; set; } = "";
        [Required]
        public string Role { get; set; } = "user";
    }
}
