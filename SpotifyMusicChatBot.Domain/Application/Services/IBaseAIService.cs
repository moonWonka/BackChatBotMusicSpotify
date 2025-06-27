namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Interfaz base para todos los servicios de IA
    /// </summary>
    public interface IBaseAIService
    {
        /// <summary>
        /// Nombre del modelo de IA
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// Proveedor del servicio (Google, Anthropic, etc.)
        /// </summary>
        string Provider { get; }

        /// <summary>
        /// Indica si el servicio está disponible
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Contextualiza una pregunta basándose en el historial de conversación
        /// </summary>
        /// <param name="question">Pregunta del usuario</param>
        /// <param name="conversationHistory">Historial de la conversación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la contextualización</returns>
        Task<ContextualizationResult> ContextualizeQuestionAsync(string question, string conversationHistory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida si una pregunta es relevante para el asistente musical
        /// </summary>
        /// <param name="question">Pregunta a validar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateQuestionAsync(string question, CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera una consulta SQL a partir de una pregunta en lenguaje natural
        /// </summary>
        /// <param name="question">Pregunta en lenguaje natural</param>
        /// <param name="resultLimit">Límite de resultados</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Consulta SQL generada</returns>
        Task<SQLGenerationResult> GenerateSQLAsync(string question, int resultLimit = 50, CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera una respuesta en lenguaje natural a partir de resultados de base de datos
        /// </summary>
        /// <param name="question">Pregunta original</param>
        /// <param name="databaseResults">Resultados de la base de datos</param>
        /// <param name="tone">Tono de la respuesta</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Respuesta en lenguaje natural</returns>
        Task<NaturalResponseResult> GenerateNaturalResponseAsync(string question, string databaseResults, string tone = "casual", CancellationToken cancellationToken = default);

        /// <summary>
        /// Analiza y mejora una respuesta existente
        /// </summary>
        /// <param name="question">Pregunta original</param>
        /// <param name="response">Respuesta a analizar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Respuesta mejorada</returns>
        Task<AnalysisResult> AnalyzeAndImproveResponseAsync(string question, string response, CancellationToken cancellationToken = default);
    }
}
