namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Interfaz para el servicio de IA de Google Gemini
    /// </summary>
    public interface IGeminiIAService : IBaseAIService
    {
        /// <summary>
        /// Ejecuta un prompt específico utilizando el modelo Gemini
        /// </summary>
        /// <param name="prompt">Prompt a ejecutar</param>
        /// <param name="temperature">Nivel de creatividad (0.0 - 1.0)</param>
        /// <param name="maxTokens">Máximo número de tokens en la respuesta</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Respuesta del modelo Gemini</returns>
        Task<AIModelResponse> ExecutePromptAsync(string prompt, float temperature = 0.7f, int maxTokens = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida la conectividad y configuración del servicio Gemini
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si la validación es exitosa</returns>
        Task<bool> ValidateServiceAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene información sobre los límites y capacidades del modelo
        /// </summary>
        /// <returns>Información del modelo</returns>
        Task<ModelInfo> GetModelInfoAsync(CancellationToken cancellationToken = default);
    }
}