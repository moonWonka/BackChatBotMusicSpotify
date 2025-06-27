using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Command.AI.GenerateSQL
{
    /// <summary>
    /// Solicitud para generar una consulta SQL a partir de una pregunta en lenguaje natural
    /// </summary>
    public class GenerateSQLRequest : IRequest<GenerateSQLResponse>
    {
        /// <summary>
        /// Pregunta validada en lenguaje natural
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
        /// Esquema de base de datos específico a utilizar (opcional)
        /// </summary>
        public string? DatabaseSchema { get; set; }

        /// <summary>
        /// Límite de resultados para la consulta
        /// </summary>
        [Range(1, 1000, ErrorMessage = "El límite debe estar entre 1 y 1000")]
        public int ResultLimit { get; set; } = 50;
    }
}
