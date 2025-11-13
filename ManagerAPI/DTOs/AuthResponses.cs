namespace ManagerAPI.DTOs
{
    public class AuthResponses
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
        public string Name { get; set; } = "";
    }
}
