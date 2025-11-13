namespace ManagerAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "Pendiente";
        public DateTime DueDate { get; set; } = DateTime.Now;

        public int AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
