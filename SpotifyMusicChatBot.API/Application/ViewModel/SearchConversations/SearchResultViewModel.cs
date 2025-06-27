using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.ViewModel.SearchConversations
{
    /// <summary>
    /// Modelo de vista para un resultado de búsqueda de conversaciones
    /// </summary>
    public class SearchResultViewModel
    {
        /// <summary>
        /// ID único del registro en la base de datos
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Fecha y hora de la conversación
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ID de la sesión de conversación
        /// </summary>
        [Required]
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Pregunta del usuario
        /// </summary>
        [Required]
        public string UserPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Respuesta de la IA
        /// </summary>
        [Required]
        public string AiResponse { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de coincidencia encontrada (UserPrompt, AiResponse, Both)
        /// </summary>
        public string? MatchType { get; set; }

        /// <summary>
        /// Texto resaltado con el término de búsqueda (opcional para highlighting)
        /// </summary>
        public string? HighlightedText { get; set; }
    }
}