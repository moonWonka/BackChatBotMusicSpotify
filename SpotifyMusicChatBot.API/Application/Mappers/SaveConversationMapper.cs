using SpotifyMusicChatBot.API.Application.Command.SaveConversation;

namespace SpotifyMusicChatBot.API.Application.Mappers
{
    /// <summary>
    /// Mapper para conversiones de SaveConversation entre capas
    /// </summary>
    public static class SaveConversationMapper
    {
        /// <summary>
        /// Crea una respuesta exitosa
        /// </summary>
        public static SaveConversationResponse ToSuccessResponse(string sessionId)
        {
            return new SaveConversationResponse
            {
                StatusCode = 200,
                Message = "Conversación guardada exitosamente",
                SessionId = sessionId
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static SaveConversationResponse ToErrorResponse(
            int statusCode,
            string message)
        {
            return new SaveConversationResponse
            {
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
