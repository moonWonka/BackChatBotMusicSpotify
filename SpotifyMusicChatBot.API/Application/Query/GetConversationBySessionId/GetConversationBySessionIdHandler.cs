using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId
{
    public class GetConversationBySessionIdHandler : IRequestHandler<GetConversationBySessionIdRequest, GetConversationBySessionIdResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<GetConversationBySessionIdHandler> _logger;

        public GetConversationBySessionIdHandler(IChatBotRepository chatRepository, ILogger<GetConversationBySessionIdHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }        public async Task<GetConversationBySessionIdResponse> Handle(GetConversationBySessionIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                IList<ConversationTurn> conversation = await _chatRepository.GetConversationBySessionIdAsync(request.SessionId);
                  return new GetConversationBySessionIdResponse
                {
                    Conversation = conversation,
                    StatusCode = 200,
                    Message = "Conversación obtenida exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation by session ID: {SessionId}, Message: {Message}", request.SessionId, ex.Message);
                return new GetConversationBySessionIdResponse
                {
                    StatusCode = 500,
                    Message = "Error interno del servidor"
                };
            }
        }
    }
}
