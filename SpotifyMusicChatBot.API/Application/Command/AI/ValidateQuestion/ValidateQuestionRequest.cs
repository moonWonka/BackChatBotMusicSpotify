using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ValidateQuestion
{
    /// <summary>
    /// Solicitud para validar si una pregunta es relevante para el asistente musical
    /// </summary>
    public class ValidateQuestionRequest : IRequest<ValidateQuestionResponse>
    {
        /// <summary>
        /// Pregunta a validar
        /// </summary>
        [Required(ErrorMessage = "La pregunta es requerida")]
        [StringLength(1000, ErrorMessage = "La pregunta no puede exceder 1000 caracteres")]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Modelo de IA a utilizar (Gemini o Anthropic)
        /// </summary>
        [Required(ErrorMessage = "El modelo de IA es requerido")]
        public string AIModel { get; set; } = "Gemini";
    }
}
