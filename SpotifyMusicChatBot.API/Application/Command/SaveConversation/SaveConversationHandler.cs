using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.API.Application.Command.SaveConversation
{
    public class SaveConversationHandler : IRequestHandler<SaveConversationRequest, SaveConversationResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<SaveConversationHandler> _logger;

        public SaveConversationHandler(IChatBotRepository chatRepository, ILogger<SaveConversationHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }        public async Task<SaveConversationResponse> Handle(SaveConversationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string sessionId = request.SessionId ?? _chatRepository.GenerateSessionId();
                bool success = await _chatRepository.SaveConversationAsync(request.UserPrompt, request.AiResponse, sessionId);
                  if (success)
                {
                    return new SaveConversationResponse
                    {
                        SessionId = sessionId,
                        StatusCode = 200,
                        Message = "Conversación guardada exitosamente"
                    };
                }
                else
                {
                    return new SaveConversationResponse
                    {
                        StatusCode = 400,
                        Message = "Error al guardar la conversación"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving conversation: {Message}", ex.Message);
                return new SaveConversationResponse
                {
                    StatusCode = 500,
                    Message = "Error interno del servidor"
                };
            }
        }
    }
}
