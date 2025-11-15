namespace ManagerAPI.DTOs
{
    // DTO para manejar las respuestas de inicio de sesion
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
        public string Name { get; set; } = "";
    }
}
