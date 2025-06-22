namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Contrato para el servicio de configuración de variables de entorno
    /// Se ejecuta al arranque de la aplicación para validar configuración crítica
    /// </summary>
    public interface IEnvironmentConfigService
    {
        /// <summary>
        /// Valida que todas las variables de entorno requeridas estén configuradas
        /// Se ejecuta automáticamente al build/arranque de la aplicación
        /// </summary>
        void ValidateEnvironmentVariables();

        /// <summary>
        /// Obtiene la cadena de conexión construida desde variables de entorno
        /// </summary>
        string GetDatabaseConnectionString();

        /// <summary>
        /// Obtiene la API Key de OpenAI
        /// </summary>
        string GetOpenAIApiKey();

        /// <summary>
        /// Obtiene la API Key de Gemini
        /// </summary>
        string GetGeminiApiKey();

        /// <summary>
        /// Obtiene la API Key de Anthropic
        /// </summary>
        string GetAnthropicApiKey();
    }
}
