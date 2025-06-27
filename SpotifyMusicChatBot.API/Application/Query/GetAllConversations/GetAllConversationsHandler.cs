using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.API.Application.Mappers;

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
                IList<Domain.Application.Model.Conversation.ConversationSession> conversations = await _chatRepository.GetAllConversationsAsync();
                return GetAllConversationsMapper.ToSuccessResponse(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations");
                return GetAllConversationsMapper.ToErrorResponse(500, "Error al obtener las conversaciones");
            }
        }
    }
}
