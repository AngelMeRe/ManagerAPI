namespace ManagerAPI.DTOs
{
    public class CommentDto
    {
        /// ID del comentario
        public int Id { get; set; }

        /// Contenido del comentario
        public string Content { get; set; } = "";

        /// Fecha de creación del comentario
        public DateTime CreatedAt { get; set; }

        /// Información del usuario que creo el comentario
        public int UserId { get; set; }

        /// Nombre del usuario que creo el comentario
        public string UserName { get; set; } = "";
    }
}
