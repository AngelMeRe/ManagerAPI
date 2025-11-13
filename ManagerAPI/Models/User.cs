namespace ManagerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "user";

        public ICollection<TaskItem>? CreatedTasks { get; set; }
        public ICollection<TaskItem>? AssignedTasks { get; set; }
    }
}
