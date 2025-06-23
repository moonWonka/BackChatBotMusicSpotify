using MediatR;

namespace SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId
{
    public class GetConversationBySessionIdRequest : IRequest<GetConversationBySessionIdResponse>
    {
        public string SessionId { get; set; } = string.Empty;
    }
}
