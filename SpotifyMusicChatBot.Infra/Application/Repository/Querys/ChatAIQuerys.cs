namespace SpotifyMusicChatBot.Infra.Application.Repository.Querys
{
    public static class ChatAIQuerys
    {
        /// <summary>
        /// Query para insertar una nueva conversación en el historial
        /// </summary>
        internal const string SaveConversation = @"
            INSERT INTO conversation_history (session_id, user_prompt, ai_response, firebase_user_id) 
            VALUES (@SessionId, @UserPrompt, @AiResponse, @FirebaseUserId)";

        /// <summary>
        /// Query para obtener todas las sesiones de conversación únicas con el primer prompt
        /// </summary>
        internal const string GetAllConversations = @"
            WITH RankedConversations AS (
                SELECT
                    session_id,
                    user_prompt,
                    timestamp,
                    ROW_NUMBER() OVER(PARTITION BY session_id ORDER BY id ASC) as rn
                FROM conversation_history
            )
            SELECT
                session_id AS SessionId,
                user_prompt AS UserPrompt,
                timestamp AS Timestamp
            FROM RankedConversations
            WHERE rn = 1
            ORDER BY timestamp DESC";

        /// <summary>
        /// Query para obtener todas las sesiones de conversación únicas de un usuario específico con el primer prompt
        /// </summary>
        internal const string GetAllConversationsByUser = @"
            WITH RankedConversations AS (
                SELECT
                    session_id,
                    user_prompt,
                    timestamp,
                    ROW_NUMBER() OVER(PARTITION BY session_id ORDER BY id ASC) as rn
                FROM conversation_history
                WHERE firebase_user_id = @FirebaseUserId
            )
            SELECT
                session_id AS SessionId,
                user_prompt AS UserPrompt,
                timestamp AS Timestamp
            FROM RankedConversations
            WHERE rn = 1
            ORDER BY timestamp DESC";

        /// <summary>
        /// Query para obtener todos los turnos de una sesión específica
        /// </summary>
        internal const string GetConversationByFirebaseUserId = @"
            SELECT 
                id AS Id, 
                timestamp AS Timestamp, 
                session_id AS SessionId, 
                user_prompt AS UserPrompt, 
                ai_response AS AiResponse 
            FROM conversation_history 
            WHERE firebase_user_id = @FirebaseUserId";

        /// <summary>
        /// Query para obtener un resumen estadístico de una sesión
        /// </summary>
        internal const string GetSessionSummary = @"
            SELECT 
                COUNT(*) as TotalTurns,
                MIN(timestamp) as SessionStart,
                MAX(timestamp) as SessionEnd,
                MIN(user_prompt) as FirstPrompt
            FROM conversation_history 
            WHERE session_id = @SessionId";

        /// <summary>
        /// Query para eliminar una sesión completa del historial
        /// </summary>
        internal const string DeleteSession = @"
            DELETE FROM conversation_history 
            WHERE session_id = @SessionId";

        /// <summary>
        /// Query para buscar conversaciones que contengan un término específico
        /// </summary>
        internal const string SearchConversations = @"
            SELECT id, timestamp, session_id, user_prompt, ai_response
            FROM conversation_history 
            WHERE LOWER(user_prompt) LIKE LOWER(@SearchPattern) 
               OR LOWER(ai_response) LIKE LOWER(@SearchPattern)
            ORDER BY timestamp DESC";

        // Queries para términos excluidos
        /// <summary>
        /// Query para crear un nuevo término excluido
        /// </summary>
        internal const string CreateExcludedTerm = @"
            INSERT INTO excluded_terms (firebase_user_id, term, category, created_at, is_active) 
            VALUES (@FirebaseUserId, @Term, @Category, GETDATE(), 1)";

        /// <summary>
        /// Query para obtener todos los términos excluidos activos de un usuario
        /// </summary>
        internal const string GetExcludedTermsByUser = @"
            SELECT 
                id AS Id,
                firebase_user_id AS FirebaseUserId,
                term AS Term,
                category AS Category,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt,
                is_active AS IsActive
            FROM excluded_terms 
            WHERE firebase_user_id = @FirebaseUserId AND is_active = 1
            ORDER BY category, term";

        /// <summary>
        /// Query para actualizar un término excluido
        /// </summary>
        internal const string UpdateExcludedTerm = @"
            UPDATE excluded_terms 
            SET term = @Term, 
                category = @Category, 
                is_active = @IsActive,
                updated_at = GETDATE()
            WHERE id = @Id AND firebase_user_id = @FirebaseUserId";

        /// <summary>
        /// Query para eliminar (desactivar) un término excluido
        /// </summary>
        internal const string DeleteExcludedTerm = @"
            UPDATE excluded_terms 
            SET is_active = 0, updated_at = GETDATE()
            WHERE id = @Id AND firebase_user_id = @FirebaseUserId";

        /// <summary>
        /// Query para verificar si un término ya existe para un usuario
        /// </summary>
        internal const string ExcludedTermExists = @"
            SELECT COUNT(1) 
            FROM excluded_terms 
            WHERE firebase_user_id = @FirebaseUserId 
              AND LOWER(term) = LOWER(@Term) 
              AND LOWER(category) = LOWER(@Category) 
              AND is_active = 1";

        /// <summary>
        /// Query para obtener términos excluidos por categoría
        /// </summary>
        internal const string GetExcludedTermsByCategory = @"
            SELECT 
                id AS Id,
                firebase_user_id AS FirebaseUserId,
                term AS Term,
                category AS Category,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt,
                is_active AS IsActive
            FROM excluded_terms 
            WHERE firebase_user_id = @FirebaseUserId 
              AND LOWER(category) = LOWER(@Category) 
              AND is_active = 1
            ORDER BY term";
    }
}