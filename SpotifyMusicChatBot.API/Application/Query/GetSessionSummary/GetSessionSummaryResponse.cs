using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.API.Application.ViewModel.GetSessionSummary;

namespace SpotifyMusicChatBot.API.Application.Query.GetSessionSummary
{
    public class GetSessionSummaryResponse : BaseResponse
    {
        public SessionSummaryViewModel? Summary { get; set; }
    }
}
