using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SpotifyMusicChatBot.API.Application.Query.GetSessionSummary
{
    public class GetSessionSummaryRequest : IRequest<GetSessionSummaryResponse>
    {
        [FromRoute]
        public string SessionId { get; set; } = string.Empty;
    }
}
