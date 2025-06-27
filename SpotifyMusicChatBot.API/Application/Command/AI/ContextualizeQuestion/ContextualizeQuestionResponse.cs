using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ContextualizeQuestion
{
    /// <summary>
    /// Respuesta de la contextualización de una pregunta
    /// </summary>
    public class ContextualizeQuestionResponse : BaseResponse
    {
        /// <summary>
        /// Pregunta original del usuario
        /// </summary>
        public string OriginalQuestion { get; set; } = string.Empty;

        /// <summary>
        /// Pregunta contextualizada
        /// </summary>
        public string ContextualizedQuestion { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la pregunta fue contextualizada o era independiente
        /// </summary>
        public bool WasContextualized { get; set; }

        /// <summary>
        /// Tipo de análisis realizado (INDEPENDIENTE o CONTEXTUALIZADA)
        /// </summary>
        public string AnalysisType { get; set; } = string.Empty;

        /// <summary>
        /// Historial de conversación utilizado para el contexto
        /// </summary>
        public List<ConversationContext> ConversationHistory { get; set; } = new List<ConversationContext>();

        /// <summary>
        /// Modelo de IA utilizado
        /// </summary>
        public string AIModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo de procesamiento en millisegundos
        /// </summary>
        public long ProcessingTimeMs { get; set; }
    }

    /// <summary>
    /// Contexto de una conversación para la contextualización
    /// </summary>
    public class ConversationContext
    {
        /// <summary>
        /// Turno de la conversación
        /// </summary>
        public int Turn { get; set; }

        /// <summary>
        /// Tipo de mensaje (User, Assistant)
        /// </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// Contenido del mensaje
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora del mensaje
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
