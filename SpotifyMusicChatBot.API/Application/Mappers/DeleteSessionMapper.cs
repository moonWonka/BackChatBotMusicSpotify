using SpotifyMusicChatBot.API.Application.Command.DeleteSession;

namespace SpotifyMusicChatBot.API.Application.Mappers
{
    /// <summary>
    /// Mapper para conversiones de DeleteSession entre capas
    /// </summary>
    public static class DeleteSessionMapper
    {
        /// <summary>
        /// Crea una respuesta exitosa
        /// </summary>
        public static DeleteSessionResponse ToSuccessResponse(string sessionId)
        {
            return new DeleteSessionResponse
            {
                StatusCode = 200,
                Message = $"Sesión '{sessionId}' eliminada exitosamente"
            };
        }

        /// <summary>
        /// Crea una respuesta cuando no se encuentra la sesión
        /// </summary>
        public static DeleteSessionResponse ToNotFoundResponse(string sessionId)
        {
            return new DeleteSessionResponse
            {
                StatusCode = 404,
                Message = $"Sesión '{sessionId}' no encontrada"
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static DeleteSessionResponse ToErrorResponse(
            int statusCode,
            string message)
        {
            return new DeleteSessionResponse
            {
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
