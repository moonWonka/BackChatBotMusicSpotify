using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Command.SaveConversation
{
    public class SaveConversationRequest : IRequest<SaveConversationResponse>
    {
        /// <summary>
        /// Pregunta o mensaje del usuario
        /// </summary>
        public string UserPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Respuesta generada por la IA
        /// </summary>
        public string AiResponse { get; set; } = string.Empty;

        /// <summary>
        /// ID de la sesión de conversación. 
        /// - Si se proporciona: La conversación se asocia a una sesión existente
        /// - Si no se proporciona: Se genera automáticamente un nuevo ID de sesión
        /// </summary>
        [StringLength(50, ErrorMessage = "El ID de sesión no puede exceder 50 caracteres")]
        public string? SessionId { get; set; }

        /// <summary>
        /// ID del usuario de Firebase para asociar la conversación al usuario correcto
        /// </summary>
        [Required(ErrorMessage = "El ID de usuario de Firebase es requerido")]
        [StringLength(100, ErrorMessage = "El ID de usuario de Firebase no puede exceder 100 caracteres")]
        public string FirebaseUserId { get; set; } = string.Empty;
    }
}
