using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Query.GetAllConversations
{
    public class GetAllConversationsResponse
    {
        public IEnumerable<ConversationSession> Conversations { get; set; } = new List<ConversationSession>();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
