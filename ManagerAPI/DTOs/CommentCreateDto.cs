namespace ManagerAPI.DTOs
{
    public class CommentCreateDto
    {
        /// ID de la tarea a la que pertenece el comentario
        public int TaskId { get; set; }

        /// Contenido del comentario
        public string Content { get; set; } = "";
    }
}
