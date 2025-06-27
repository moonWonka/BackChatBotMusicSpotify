using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SpotifyMusicChatBot.API.Application.Query.SearchConversations
{
    public class SearchConversationsRequest : IRequest<SearchConversationsResponse>
    {
        [FromQuery]
        public string SearchTerm { get; set; } = string.Empty;

        [FromQuery]
        public int? Limit { get; set; } = 50;

        [FromQuery]
        public bool SearchOnlyUserPrompts { get; set; } = false;

        [FromQuery]
        public bool SearchOnlyAiResponses { get; set; } = false;
    }
}
