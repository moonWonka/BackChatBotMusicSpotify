using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.Domain.Application.Model.Search;
using SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms;
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
        public async Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId, string firebaseUserId)
        {
            try
            {
                int result = await ExecuteAsync(ChatAIQuerys.SaveConversation, new { SessionId = sessionId, UserPrompt = userPrompt, AiResponse = aiResponse, FirebaseUserId = firebaseUserId });
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
        public async Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId, string firebaseUserId, SqlTransaction transaction)
        {
            try
            {
                int result = await ExecuteWithTransactionAsync(ChatAIQuerys.SaveConversation, new { SessionId = sessionId, UserPrompt = userPrompt, AiResponse = aiResponse, FirebaseUserId = firebaseUserId }, transaction);
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
        public async Task<IList<ConversationTurn>> GetConversationBySessionIdAsync(string firebaseUserId)
        {
            try
            {
                var result = await GetAllAsync<ConversationTurn>(ChatAIQuerys.GetConversationByFirebaseUserId, new { FirebaseUserId = firebaseUserId });
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
        /// Ejecuta una sentencia SQL arbitraria y retorna el resultado como string
        /// </summary>
        public async Task<string> ExecuteRawSqlAsync(string sqlQuery)
        {
            try
            {
                // Utiliza GetAllAsync<dynamic> para ejecutar la consulta y obtener los resultados
                var result = await GetAllAsync<dynamic>(sqlQuery, new { });
                // Serializa el resultado a JSON para retornarlo como string
                return System.Text.Json.JsonSerializer.Serialize(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error ejecutando SQL arbitrario: {Message}", ex.Message);
                return $"Error ejecutando SQL: {ex.Message}";
            }
        }

        /// <summary>
        /// Retrieves all unique conversation sessions from the database for a specific user, showing the first prompt of each.
        /// </summary>
        public async Task<IList<ConversationSession>> GetAllConversationsByUserAsync(string firebaseUserId)
        {
            try
            {
                var result = await GetAllAsync<ConversationSession>(ChatAIQuerys.GetAllConversationsByUser, new { FirebaseUserId = firebaseUserId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error retrieving conversation sessions for user {firebaseUserId}: {ex.Message}");
                return new List<ConversationSession>();
            }
        }

        #region Métodos para Términos Excluidos

        /// <summary>
        /// Crea un nuevo término excluido para un usuario
        /// </summary>
        public async Task<bool> CreateExcludedTermAsync(CreateExcludedTermRequest request)
        {
            try
            {
                // Verificar si el término ya existe
                bool exists = await ExcludedTermExistsAsync(request.Term, request.Category, request.FirebaseUserId);
                if (exists)
                {
                    _logger?.LogWarning("El término '{Term}' en la categoría '{Category}' ya existe para el usuario {UserId}", 
                        request.Term, request.Category, request.FirebaseUserId);
                    return false;
                }

                int result = await ExecuteAsync(ChatAIQuerys.CreateExcludedTerm, new 
                { 
                    FirebaseUserId = request.FirebaseUserId, 
                    Term = request.Term, 
                    Category = request.Category 
                });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error creando término excluido: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los términos excluidos activos de un usuario
        /// </summary>
        public async Task<IList<ExcludedTerm>> GetExcludedTermsByUserAsync(string firebaseUserId)
        {
            try
            {
                var result = await GetAllAsync<ExcludedTerm>(ChatAIQuerys.GetExcludedTermsByUser, new { FirebaseUserId = firebaseUserId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error obteniendo términos excluidos para usuario {UserId}: {Message}", firebaseUserId, ex.Message);
                return new List<ExcludedTerm>();
            }
        }

        /// <summary>
        /// Actualiza un término excluido
        /// </summary>
        public async Task<bool> UpdateExcludedTermAsync(UpdateExcludedTermRequest request)
        {
            try
            {
                int result = await ExecuteAsync(ChatAIQuerys.UpdateExcludedTerm, new 
                { 
                    Id = request.Id,
                    FirebaseUserId = request.FirebaseUserId, 
                    Term = request.Term, 
                    Category = request.Category,
                    IsActive = request.IsActive
                });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error actualizando término excluido: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina (desactiva) un término excluido
        /// </summary>
        public async Task<bool> DeleteExcludedTermAsync(int termId, string firebaseUserId)
        {
            try
            {
                int result = await ExecuteAsync(ChatAIQuerys.DeleteExcludedTerm, new { Id = termId, FirebaseUserId = firebaseUserId });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error eliminando término excluido: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Verifica si un término ya existe para un usuario
        /// </summary>
        public async Task<bool> ExcludedTermExistsAsync(string term, string category, string firebaseUserId)
        {
            try
            {
                var result = await GetByIdAsync<int>(ChatAIQuerys.ExcludedTermExists, new 
                { 
                    FirebaseUserId = firebaseUserId, 
                    Term = term, 
                    Category = category 
                });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error verificando existencia de término excluido: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene términos excluidos por categoría
        /// </summary>
        public async Task<IList<ExcludedTerm>> GetExcludedTermsByCategoryAsync(string firebaseUserId, string category)
        {
            try
            {
                var result = await GetAllAsync<ExcludedTerm>(ChatAIQuerys.GetExcludedTermsByCategory, new 
                { 
                    FirebaseUserId = firebaseUserId, 
                    Category = category 
                });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error obteniendo términos excluidos por categoría {Category} para usuario {UserId}: {Message}", 
                    category, firebaseUserId, ex.Message);
                return new List<ExcludedTerm>();
            }
        }

        #endregion
    }
}