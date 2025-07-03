using Microsoft.Data.SqlClient;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.Domain.Application.Model.Search;
using SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms;

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
        Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId, string firebaseUserId);

        /// <summary>
        /// Guarda una conversación en la base de datos con transacción externa
        /// </summary>
        Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId, string firebaseUserId, SqlTransaction transaction);

        /// <summary>
        /// Obtiene todas las sesiones de conversación únicas
        /// </summary>
        Task<IList<ConversationSession>> GetAllConversationsAsync();

        /// <summary>
        /// Obtiene todas las sesiones de conversación únicas de un usuario específico
        /// </summary>
        Task<IList<ConversationSession>> GetAllConversationsByUserAsync(string firebaseUserId);

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

        /// <summary>
        /// Ejecuta una sentencia SQL arbitraria y retorna el resultado como string
        /// </summary>
        Task<string> ExecuteRawSqlAsync(string sqlQuery);

        // Métodos para términos excluidos
        /// <summary>
        /// Crea un nuevo término excluido para un usuario
        /// </summary>
        Task<bool> CreateExcludedTermAsync(CreateExcludedTermRequest request);

        /// <summary>
        /// Obtiene todos los términos excluidos activos de un usuario
        /// </summary>
        Task<IList<ExcludedTerm>> GetExcludedTermsByUserAsync(string firebaseUserId);

        /// <summary>
        /// Actualiza un término excluido
        /// </summary>
        Task<bool> UpdateExcludedTermAsync(UpdateExcludedTermRequest request);

        /// <summary>
        /// Elimina (desactiva) un término excluido
        /// </summary>
        Task<bool> DeleteExcludedTermAsync(int termId, string firebaseUserId);

        /// <summary>
        /// Verifica si un término ya existe para un usuario
        /// </summary>
        Task<bool> ExcludedTermExistsAsync(string term, string category, string firebaseUserId);

        /// <summary>
        /// Obtiene términos excluidos por categoría
        /// </summary>
        Task<IList<ExcludedTerm>> GetExcludedTermsByCategoryAsync(string firebaseUserId, string category);
    }
}