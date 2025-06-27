using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.API.Application.ViewModel.SearchConversations;

namespace SpotifyMusicChatBot.API.Application.Query.SearchConversations
{
    public class SearchConversationsResponse : BaseResponse
    {
        public IList<SearchResultViewModel> Results { get; set; } = [];
        public int TotalResults { get; set; }
        public string SearchTerm { get; set; } 
        public long SearchTimeMs { get; set; }
    }
}
