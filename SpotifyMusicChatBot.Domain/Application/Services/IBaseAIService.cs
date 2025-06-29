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

        Task<AIModelResponse> ExecuteModelAsync(string prompt, float temperature = 0.7f, int maxTokens = 1000, CancellationToken cancellationToken = default);
    }
}
