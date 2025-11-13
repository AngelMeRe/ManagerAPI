namespace ManagerAPI.DTOs
{
    public class CommentCreateDto
    {
        public int TaskId { get; set; }
        public string Content { get; set; } = "";
    }
}
