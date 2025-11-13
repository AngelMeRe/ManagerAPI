namespace ManagerAPI.DTOs
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public UserDto User { get; set; }

        public class UserDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }
    }
}
