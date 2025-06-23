using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.API.Application.Query.GetAllConversations
{
    public class GetAllConversationsHandler : IRequestHandler<GetAllConversationsRequest, GetAllConversationsResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<GetAllConversationsHandler> _logger;

        public GetAllConversationsHandler(IChatBotRepository chatRepository, ILogger<GetAllConversationsHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }        public async Task<GetAllConversationsResponse> Handle(GetAllConversationsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Domain.Application.Model.Conversation.ConversationSession> conversations = await _chatRepository.GetAllConversationsAsync();
                  return new GetAllConversationsResponse
                {
                    Conversations = conversations,
                    StatusCode = 200,
                    Message = "Conversaciones obtenidas exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations");
                return new GetAllConversationsResponse
                {
                    StatusCode = 500,
                    Message = "Error al obtener las conversaciones"
                };
            }
        }
    }
}
