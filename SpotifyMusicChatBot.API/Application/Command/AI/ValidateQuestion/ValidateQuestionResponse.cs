using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ValidateQuestion
{
    /// <summary>
    /// Respuesta de la validación de una pregunta
    /// </summary>
    public class ValidateQuestionResponse : BaseResponse
    {
        /// <summary>
        /// Pregunta que fue validada
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Estado de validación: VALIDA, ACLARAR, FUERA_CONTEXTO
        /// </summary>
        public string ValidationStatus { get; set; } = string.Empty;

        /// <summary>
        /// Razón de la validación o mensaje de aclaración
        /// </summary>
        public string? ValidationReason { get; set; }

        /// <summary>
        /// Sugerencias para mejorar la pregunta (en caso de ACLARAR)
        /// </summary>
        public List<string> Suggestions { get; set; } = new List<string>();

        /// <summary>
        /// Categoría musical identificada en la pregunta
        /// </summary>
        public string? IdentifiedCategory { get; set; }

        /// <summary>
        /// Modelo de IA utilizado para la validación
        /// </summary>
        public string AIModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo de procesamiento en millisegundos
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Nivel de confianza de la validación (0-100)
        /// </summary>
        public int ConfidenceLevel { get; set; }
    }
}
