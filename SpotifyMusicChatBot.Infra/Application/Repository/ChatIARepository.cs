using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using SpotifyMusicChatBot.Infra.Application;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.Domain.Application.Model.Search;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Infra.Application.Repository.Querys;

namespace SpotifyMusicChatBot.Infra.Application.Repository
{
    /// <summary>
    /// Repositorio principal para chat usando variable de entorno CHATDB
    /// </summary>
    public class ChatIARepository : AbstractRepository, IChatBotRepository
    {
        private static string DB => "CHATDB";

        // Usa la variable de entorno "CHATDB" que contiene la cadena de conexión completa
        public ChatIARepository(ILogger<ChatIARepository> logger) : base(environmentVariableName: DB, fromEnvironment: true, logger: logger)
        {
        }

        /// <summary>
        /// Generates a unique session ID.
        /// </summary>
        public string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Saves a conversation to the database.
        /// </summary>
        public async Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId)
        {
            try
            {
                int result = await ExecuteAsync(ChatAIQuerys.SaveConversation, new { SessionId = sessionId, UserPrompt = userPrompt, AiResponse = aiResponse });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error saving conversation: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Saves a conversation to the database with external transaction.
        /// </summary>
        public async Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId, SqlTransaction transaction)
        {
            try
            {
                int result = await ExecuteWithTransactionAsync(ChatAIQuerys.SaveConversation, new { SessionId = sessionId, UserPrompt = userPrompt, AiResponse = aiResponse }, transaction);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error saving conversation with transaction: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Retrieves all unique conversation sessions from the database, showing the first prompt of each.
        /// </summary>
        public async Task<IList<ConversationSession>> GetAllConversationsAsync()
        {
            try
            {
                var result = await GetAllAsync<ConversationSession>(ChatAIQuerys.GetAllConversations, new { });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error retrieving conversation sessions: {ex.Message}");
                return new List<ConversationSession>();
            }
        }

        /// <summary>
        /// Retrieves all turns for a specific conversation session ID, ordered by timestamp.
        /// </summary>
        public async Task<IList<ConversationTurn>> GetConversationBySessionIdAsync(string sessionId)
        {
            try
            {
                var result = await GetAllAsync<ConversationTurn>(ChatAIQuerys.GetConversationBySessionId, new { SessionId = sessionId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error retrieving conversation details by session ID: {ex.Message}");
                return new List<ConversationTurn>();
            }
        }

        /// <summary>
        /// Obtiene un resumen de la sesión con información útil.
        /// </summary>
        public async Task<SessionSummary?> GetSessionSummaryAsync(string sessionId)
        {
            try
            {
                return await GetByIdAsync<SessionSummary>(ChatAIQuerys.GetSessionSummary, new { SessionId = sessionId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo resumen de sesión: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Elimina una sesión completa del historial.
        /// </summary>
        public async Task<bool> DeleteSessionAsync(string sessionId)
        {
            try
            {
                int deletedRows = await ExecuteAsync(ChatAIQuerys.DeleteSession, new { SessionId = sessionId });
                return deletedRows > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error eliminando sesión: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina una sesión completa del historial con transacción externa.
        /// </summary>
        public async Task<bool> DeleteSessionAsync(string sessionId, SqlTransaction transaction)
        {
            try
            {
                int deletedRows = await ExecuteWithTransactionAsync(ChatAIQuerys.DeleteSession, new { SessionId = sessionId }, transaction);
                return deletedRows > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error eliminando sesión con transacción: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Busca conversaciones que contengan el término de búsqueda.
        /// </summary>
        public async Task<IList<SearchResult>> SearchConversationsAsync(string searchTerm)
        {
            try
            {
                string searchPattern = $"%{searchTerm}%";
                var result = await GetAllAsync<SearchResult>(ChatAIQuerys.SearchConversations, new { SearchPattern = searchPattern });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error buscando conversaciones: {ex.Message}");
                return new List<SearchResult>();
            }
        }

        /// <summary>
        /// Ejemplo de método que usa transacciones múltiples para guardar conversación y actualizar estadísticas
        /// </summary>
        public async Task<bool> SaveConversationWithStatsAsync(string userPrompt, string aiResponse, string sessionId)
        {
            try
            {
                // Ejemplo de múltiples operaciones en una sola transacción
                int totalAffected = await ExecuteMultipleAsync(
                    (ChatAIQuerys.SaveConversation, new { SessionId = sessionId, UserPrompt = userPrompt, AiResponse = aiResponse }),
                    // Ejemplo: actualizar contador de conversaciones (esta consulta es ficticia como ejemplo)
                    ("UPDATE ConversationStats SET TotalConversations = TotalConversations + 1 WHERE SessionId = @SessionId", new { SessionId = sessionId })
                );
                
                return totalAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving conversation with stats: {ex.Message}");
                return false;
            }
        }
    }
}