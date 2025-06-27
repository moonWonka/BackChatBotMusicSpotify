using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ProcessQuestion
{
    /// <summary>
    /// Solicitud para procesar una pregunta completa siguiendo todo el flujo de IA
    /// </summary>
    public class ProcessQuestionRequest : IRequest<ProcessQuestionResponse>
    {
        /// <summary>
        /// ID de la sesión de conversación
        /// </summary>
        [Required(ErrorMessage = "El ID de sesión es requerido")]
        [StringLength(50, ErrorMessage = "El ID de sesión no puede exceder 50 caracteres")]
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Pregunta del usuario en lenguaje natural
        /// </summary>
        [Required(ErrorMessage = "La pregunta es requerida")]
        [StringLength(1000, ErrorMessage = "La pregunta no puede exceder 1000 caracteres")]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Modelo de IA a utilizar (Gemini o Anthropic)
        /// </summary>
        [Required(ErrorMessage = "El modelo de IA es requerido")]
        public string AIModel { get; set; } = "Gemini";

        /// <summary>
        /// Indica si se debe incluir el historial de conversación para contextualización
        /// </summary>
        public bool IncludeContext { get; set; } = true;

        /// <summary>
        /// Límite de turnos del historial a considerar para contexto
        /// </summary>
        [Range(1, 20, ErrorMessage = "El límite de contexto debe estar entre 1 y 20")]
        public int ContextLimit { get; set; } = 10;
    }
}
