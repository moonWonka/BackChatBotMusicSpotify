using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.API.Application.Mappers;

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
                return GetConversationBySessionIdMapper.ToSuccessResponse(conversation, request.SessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation by session ID: {SessionId}, Message: {Message}", request.SessionId, ex.Message);
                return GetConversationBySessionIdMapper.ToErrorResponse(500, "Error interno del servidor");
            }
        }
    }
}
