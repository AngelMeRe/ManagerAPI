namespace ManagerAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int TaskId { get; set; }
        public TaskItem? Task { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
