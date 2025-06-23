using MediatR;

namespace SpotifyMusicChatBot.API.Application.Command.SaveConversation
{
    public class SaveConversationRequest : IRequest<SaveConversationResponse>
    {
        public string UserPrompt { get; set; } = string.Empty;
        public string AiResponse { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }
}
