namespace ManagerAPI.DTOs
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime DueDate { get; set; }
        public SimpleUserDto? AssignedTo { get; set; }
        public SimpleUserDto? CreatedBy { get; set; }

        public List<CommentDto>? Comments { get; set; }

        public class SimpleUserDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        public class CommentDto
        {
            public int Id { get; set; }
            public string Text { get; set; } = "";
            public int UserId { get; set; }
        }

    }
}
