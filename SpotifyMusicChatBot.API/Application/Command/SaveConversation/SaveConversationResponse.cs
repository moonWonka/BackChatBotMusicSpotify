using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Command.SaveConversation
{
    public class SaveConversationResponse : BaseResponse
    {
        public string? SessionId { get; set; }
    }
}
