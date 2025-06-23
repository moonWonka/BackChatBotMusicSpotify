using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.Domain.Application.Model.Search;

namespace SpotifyMusicChatBot.Domain.Application.Repository
{
    /// <summary>
    /// Interfaz para operaciones de repositorio de Chat Bot
    /// </summary>
    public interface IChatBotRepository
    {
        /// <summary>
        /// Genera un ID único de sesión
        /// </summary>
        string GenerateSessionId();

        /// <summary>
        /// Guarda una conversación en la base de datos
        /// </summary>
        Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId);

        /// <summary>
        /// Obtiene todas las sesiones de conversación únicas
        /// </summary>
        Task<IEnumerable<ConversationSession>> GetAllConversationsAsync();

        /// <summary>
        /// Obtiene todos los turnos de una sesión específica
        /// </summary>
        Task<IEnumerable<ConversationTurn>> GetConversationBySessionIdAsync(string sessionId);

        /// <summary>
        /// Obtiene un resumen estadístico de una sesión
        /// </summary>
        Task<SessionSummary?> GetSessionSummaryAsync(string sessionId);

        /// <summary>
        /// Elimina una sesión completa del historial
        /// </summary>
        Task<bool> DeleteSessionAsync(string sessionId);

        /// <summary>
        /// Busca conversaciones que contengan un término específico
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchConversationsAsync(string searchTerm);
    }
}