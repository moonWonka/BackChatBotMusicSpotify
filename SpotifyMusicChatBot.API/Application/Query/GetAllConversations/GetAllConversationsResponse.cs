using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Query.GetAllConversations
{
    public class GetAllConversationsResponse : BaseResponse
    {
        public IEnumerable<ConversationSession> Conversations { get; set; } = new List<ConversationSession>();
    }
}
