using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.Domain.Application.Model.Search;

namespace SpotifyMusicChatBot.API.Application.Query.SearchConversations
{
    public class SearchConversationsResponse : BaseResponse
    {
        public IList<SearchResult> Results { get; set; } = [];
    }
}
