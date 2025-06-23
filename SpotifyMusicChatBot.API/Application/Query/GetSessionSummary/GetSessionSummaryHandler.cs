using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

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
        }

        public async Task<GetSessionSummaryResponse> Handle(GetSessionSummaryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                SessionSummary? summary = await _chatRepository.GetSessionSummaryAsync(request.SessionId);
                
                if (summary != null)
                {
                    return new GetSessionSummaryResponse
                    {
                        Summary = summary,
                        Success = true,
                        Message = "Resumen de sesión obtenido exitosamente"
                    };
                }
                else
                {
                    return new GetSessionSummaryResponse
                    {
                        Success = false,
                        Message = "Sesión no encontrada"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session summary for session ID: {SessionId}, Message: {Message}", request.SessionId, ex.Message);
                return new GetSessionSummaryResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                };
            }
        }
    }
}
