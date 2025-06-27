using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.API.Application.Mappers;

namespace SpotifyMusicChatBot.API.Application.Query.GetSessionSummary
{
    public class GetSessionSummaryHandler : IRequestHandler<GetSessionSummaryRequest, GetSessionSummaryResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<GetSessionSummaryHandler> _logger;

        public GetSessionSummaryHandler(IChatBotRepository chatRepository, ILogger<GetSessionSummaryHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }        public async Task<GetSessionSummaryResponse> Handle(GetSessionSummaryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                SessionSummary? summary = await _chatRepository.GetSessionSummaryAsync(request.SessionId);
                
                if (summary != null)
                {
                    return GetSessionSummaryMapper.ToSuccessResponse(summary);
                }
                else
                {
                    return GetSessionSummaryMapper.ToNotFoundResponse(request.SessionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session summary for session ID: {SessionId}, Message: {Message}", request.SessionId, ex.Message);
                return GetSessionSummaryMapper.ToErrorResponse(500, "Error interno del servidor");
            }
        }
    }
}
