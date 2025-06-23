using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.Domain.Application.Model.Search;

namespace SpotifyMusicChatBot.API.Application.Query.SearchConversations
{
    public class SearchConversationsResponse : BaseResponse
    {
        public IEnumerable<SearchResult> Results { get; set; } = new List<SearchResult>();
    }
}
