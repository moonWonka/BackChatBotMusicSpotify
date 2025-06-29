using Microsoft.Data.SqlClient;
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
        /// Guarda una conversación en la base de datos con transacción externa
        /// </summary>
        Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId, SqlTransaction transaction);

        /// <summary>
        /// Obtiene todas las sesiones de conversación únicas
        /// </summary>
        Task<IList<ConversationSession>> GetAllConversationsAsync();

        /// <summary>
        /// Obtiene todos los turnos de una sesión específica
        /// </summary>
        Task<IList<ConversationTurn>> GetConversationBySessionIdAsync(string firebaseUserId);

        /// <summary>
        /// Obtiene un resumen estadístico de una sesión
        /// </summary>
        Task<SessionSummary?> GetSessionSummaryAsync(string sessionId);

        /// <summary>
        /// Elimina una sesión completa del historial
        /// </summary>
        Task<bool> DeleteSessionAsync(string sessionId);

        /// <summary>
        /// Elimina una sesión completa del historial con transacción externa
        /// </summary>
        Task<bool> DeleteSessionAsync(string sessionId, SqlTransaction transaction);

        /// <summary>
        /// Busca conversaciones que contengan un término específico
        /// </summary>
        Task<IList<SearchResult>> SearchConversationsAsync(string searchTerm);
    }
}