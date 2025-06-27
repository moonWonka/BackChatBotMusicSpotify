using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ProcessQuestion
{
    /// <summary>
    /// Respuesta del procesamiento completo de una pregunta
    /// </summary>
    public class ProcessQuestionResponse : BaseResponse
    {
        /// <summary>
        /// Pregunta original del usuario
        /// </summary>
        public string OriginalQuestion { get; set; } = string.Empty;

        /// <summary>
        /// Pregunta contextualizada (si fue necesario)
        /// </summary>
        public string ContextualizedQuestion { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la pregunta requirió contextualización
        /// </summary>
        public bool WasContextualized { get; set; }

        /// <summary>
        /// Estado de validación de la pregunta
        /// </summary>
        public string ValidationStatus { get; set; } = string.Empty; // VALIDA, ACLARAR, FUERA_CONTEXTO

        /// <summary>
        /// Mensaje de aclaración si la pregunta necesita más información
        /// </summary>
        public string? ClarificationMessage { get; set; }

        /// <summary>
        /// Consulta SQL generada
        /// </summary>
        public string? GeneratedSQL { get; set; }

        /// <summary>
        /// Resultados obtenidos de la base de datos
        /// </summary>
        public string? DatabaseResults { get; set; }

        /// <summary>
        /// Respuesta final en lenguaje natural
        /// </summary>
        public string? NaturalResponse { get; set; }

        /// <summary>
        /// Modelo de IA utilizado para el procesamiento
        /// </summary>
        public string AIModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo total de procesamiento en millisegundos
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Detalles del proceso paso a paso
        /// </summary>
        public ProcessingSteps Steps { get; set; } = new ProcessingSteps();
    }

    /// <summary>
    /// Información detallada de cada paso del procesamiento
    /// </summary>
    public class ProcessingSteps
    {
        /// <summary>
        /// Tiempo de contextualización en ms
        /// </summary>
        public long ContextualizationTimeMs { get; set; }

        /// <summary>
        /// Tiempo de validación en ms
        /// </summary>
        public long ValidationTimeMs { get; set; }

        /// <summary>
        /// Tiempo de generación SQL en ms
        /// </summary>
        public long SQLGenerationTimeMs { get; set; }

        /// <summary>
        /// Tiempo de ejecución SQL en ms
        /// </summary>
        public long SQLExecutionTimeMs { get; set; }

        /// <summary>
        /// Tiempo de generación de respuesta natural en ms
        /// </summary>
        public long NaturalResponseTimeMs { get; set; }
    }
}
