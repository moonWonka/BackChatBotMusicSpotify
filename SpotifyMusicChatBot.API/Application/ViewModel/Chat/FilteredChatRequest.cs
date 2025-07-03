using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.ViewModel.Chat
{
    /// <summary>
    /// Request para procesar preguntas con filtros de términos excluidos
    /// </summary>
    public class FilteredChatRequest
    {
        /// <summary>
        /// Pregunta del usuario
        /// </summary>
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario de Firebase
        /// </summary>
        [Required]
        public string FirebaseUserId { get; set; } = string.Empty;

        /// <summary>
        /// ID de sesión (opcional, se genera uno nuevo si no se proporciona)
        /// </summary>
        public string? SessionId { get; set; }

        /// <summary>
        /// Tono de la respuesta (casual, formal, amigable, etc.)
        /// </summary>
        public string? Tone { get; set; } = "casual";

        /// <summary>
        /// Límite de resultados de la consulta SQL
        /// </summary>
        [Range(1, 100)]
        public int ResultLimit { get; set; } = 20;

        /// <summary>
        /// Modelo de IA a utilizar
        /// </summary>
        public string ModelName { get; set; } = "Gemini";
    }
}
