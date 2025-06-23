using MediatR;

namespace SpotifyMusicChatBot.API.Application.Query.GetSessionSummary
{
    public class GetSessionSummaryRequest : IRequest<GetSessionSummaryResponse>
    {
        public string SessionId { get; set; } = string.Empty;
    }
}
