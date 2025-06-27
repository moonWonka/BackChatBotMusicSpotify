using SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Mappers
{
    /// <summary>
    /// Mapper para conversiones de GetConversationBySessionId entre capas
    /// </summary>
    public static class GetConversationBySessionIdMapper
    {
        /// <summary>
        /// Crea una respuesta exitosa con la conversación
        /// </summary>
        public static GetConversationBySessionIdResponse ToSuccessResponse(
            IList<ConversationTurn> conversation,
            string sessionId)
        {
            return new GetConversationBySessionIdResponse
            {
                Conversation = conversation,
                StatusCode = 200,
                Message = "Conversación obtenida exitosamente"
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static GetConversationBySessionIdResponse ToErrorResponse(
            int statusCode,
            string message)
        {
            return new GetConversationBySessionIdResponse
            {
                Conversation = new List<ConversationTurn>(),
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
