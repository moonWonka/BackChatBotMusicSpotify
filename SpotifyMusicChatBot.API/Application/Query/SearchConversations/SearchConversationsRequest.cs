using MediatR;

namespace SpotifyMusicChatBot.API.Application.Query.SearchConversations
{
    public class SearchConversationsRequest : IRequest<SearchConversationsResponse>
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}
