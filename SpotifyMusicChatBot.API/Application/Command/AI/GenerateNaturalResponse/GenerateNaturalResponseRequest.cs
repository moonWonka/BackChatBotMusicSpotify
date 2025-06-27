using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Command.AI.GenerateNaturalResponse
{
    /// <summary>
    /// Solicitud para generar una respuesta en lenguaje natural a partir de resultados de base de datos
    /// </summary>
    public class GenerateNaturalResponseRequest : IRequest<GenerateNaturalResponseResponse>
    {
        /// <summary>
        /// Pregunta original del usuario
        /// </summary>
        [Required(ErrorMessage = "La pregunta es requerida")]
        [StringLength(1000, ErrorMessage = "La pregunta no puede exceder 1000 caracteres")]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Resultados obtenidos de la base de datos
        /// </summary>
        [Required(ErrorMessage = "Los resultados de la base de datos son requeridos")]
        public string DatabaseResults { get; set; } = string.Empty;

        /// <summary>
        /// Modelo de IA a utilizar (Gemini o Anthropic)
        /// </summary>
        [Required(ErrorMessage = "El modelo de IA es requerido")]
        public string AIModel { get; set; } = "Gemini";

        /// <summary>
        /// Tono de la respuesta (formal, casual, técnico)
        /// </summary>
        public string ResponseTone { get; set; } = "casual";

        /// <summary>
        /// Longitud preferida de la respuesta (short, medium, detailed)
        /// </summary>
        public string ResponseLength { get; set; } = "medium";

        /// <summary>
        /// Incluir información adicional contextual
        /// </summary>
        public bool IncludeContext { get; set; } = true;
    }
}
