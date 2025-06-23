using MediatR;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Query.GetAllConversations
{
    public class GetAllConversationsRequest : IRequest<GetAllConversationsResponse>
    {
    }
}
