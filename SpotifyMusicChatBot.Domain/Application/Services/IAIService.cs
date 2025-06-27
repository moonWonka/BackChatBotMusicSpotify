namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Servicio unificado para interactuar con diferentes modelos de IA (Gemini, Anthropic)
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// Contextualiza una pregunta basándose en el historial de conversación
        /// </summary>
        /// <param name="question">Pregunta del usuario</param>
        /// <param name="conversationHistory">Historial de la conversación</param>
        /// <param name="modelName">Modelo a utilizar (Gemini, Anthropic)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la contextualización</returns>
        Task<ContextualizationResult> ContextualizeQuestionAsync(string question, string conversationHistory, string modelName = "Gemini", CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida si una pregunta es relevante para el asistente musical
        /// </summary>
        /// <param name="question">Pregunta a validar</param>
        /// <param name="modelName">Modelo a utilizar (Gemini, Anthropic)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la validación</returns>
        Task<ValidationResult> ValidateQuestionAsync(string question, string modelName = "Gemini", CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera una consulta SQL a partir de una pregunta en lenguaje natural
        /// </summary>
        /// <param name="question">Pregunta en lenguaje natural</param>
        /// <param name="resultLimit">Límite de resultados</param>
        /// <param name="modelName">Modelo a utilizar (Gemini, Anthropic)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Consulta SQL generada</returns>
        Task<SQLGenerationResult> GenerateSQLAsync(string question, int resultLimit = 50, string modelName = "Gemini", CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera una respuesta en lenguaje natural a partir de resultados de base de datos
        /// </summary>
        /// <param name="question">Pregunta original</param>
        /// <param name="databaseResults">Resultados de la base de datos</param>
        /// <param name="tone">Tono de la respuesta</param>
        /// <param name="modelName">Modelo a utilizar (Gemini, Anthropic)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Respuesta en lenguaje natural</returns>
        Task<NaturalResponseResult> GenerateNaturalResponseAsync(string question, string databaseResults, string tone = "casual", string modelName = "Gemini", CancellationToken cancellationToken = default);

        /// <summary>
        /// Analiza y mejora una respuesta existente
        /// </summary>
        /// <param name="question">Pregunta original</param>
        /// <param name="response">Respuesta a analizar</param>
        /// <param name="modelName">Modelo a utilizar (Gemini, Anthropic)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Respuesta mejorada</returns>
        Task<AnalysisResult> AnalyzeAndImproveResponseAsync(string question, string response, string modelName = "Gemini", CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene la lista de modelos disponibles
        /// </summary>
        /// <returns>Lista de nombres de modelos disponibles</returns>
        Task<List<string>> GetAvailableModelsAsync();

        /// <summary>
        /// Valida si un modelo está disponible
        /// </summary>
        /// <param name="modelName">Nombre del modelo</param>
        /// <returns>True si el modelo está disponible</returns>
        Task<bool> IsModelAvailableAsync(string modelName);

        /// <summary>
        /// Ejecuta un prompt personalizado directamente
        /// </summary>
        /// <param name="prompt">Prompt a ejecutar</param>
        /// <param name="modelName">Modelo a utilizar</param>
        /// <param name="temperature">Nivel de creatividad (0.0 - 1.0)</param>
        /// <param name="maxTokens">Máximo número de tokens</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Respuesta del modelo</returns>
        Task<AIModelResponse> ExecutePromptAsync(string prompt, string modelName = "Gemini", float temperature = 0.7f, int maxTokens = 1000, CancellationToken cancellationToken = default);
    }
}
