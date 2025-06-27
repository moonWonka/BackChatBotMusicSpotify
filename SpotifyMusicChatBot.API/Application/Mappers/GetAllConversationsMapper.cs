using SpotifyMusicChatBot.API.Application.Query.GetAllConversations;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Mappers
{
    /// <summary>
    /// Mapper para conversiones de GetAllConversations entre capas
    /// </summary>
    public static class GetAllConversationsMapper
    {
        /// <summary>
        /// Crea una respuesta exitosa con todas las conversaciones
        /// </summary>
        public static GetAllConversationsResponse ToSuccessResponse(
            IList<ConversationSession> conversations)
        {
            return new GetAllConversationsResponse
            {
                Conversations = conversations,
                StatusCode = 200,
                Message = "Conversaciones obtenidas exitosamente"
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static GetAllConversationsResponse ToErrorResponse(
            int statusCode,
            string message)
        {
            return new GetAllConversationsResponse
            {
                Conversations = new List<ConversationSession>(),
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
