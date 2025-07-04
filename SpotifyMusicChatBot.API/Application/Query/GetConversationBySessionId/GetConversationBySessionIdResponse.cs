using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId
{
    public class GetConversationBySessionIdResponse : BaseResponse
    {
        public IList<ConversationTurn> Conversation { get; set; } = new List<ConversationTurn>();
    }
}
