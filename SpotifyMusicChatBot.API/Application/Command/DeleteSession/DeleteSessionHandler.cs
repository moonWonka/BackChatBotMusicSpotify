using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.API.Application.Command.DeleteSession
{
    public class DeleteSessionHandler : IRequestHandler<DeleteSessionRequest, DeleteSessionResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<DeleteSessionHandler> _logger;

        public DeleteSessionHandler(IChatBotRepository chatRepository, ILogger<DeleteSessionHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }        public async Task<DeleteSessionResponse> Handle(DeleteSessionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                bool deleted = await _chatRepository.DeleteSessionAsync(request.SessionId);
                  if (deleted)
                {
                    return new DeleteSessionResponse
                    {
                        StatusCode = 200,
                        Message = "Sesión eliminada exitosamente"
                    };
                }
                else
                {
                    return new DeleteSessionResponse
                    {
                        StatusCode = 404,
                        Message = "Sesión no encontrada"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session ID: {SessionId}, Message: {Message}", request.SessionId, ex.Message);
                return new DeleteSessionResponse
                {
                    StatusCode = 500,
                    Message = "Error interno del servidor"
                };
            }
        }
    }
}
