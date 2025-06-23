using MediatR;

namespace SpotifyMusicChatBot.API.Application.Command.DeleteSession
{
    public class DeleteSessionRequest : IRequest<DeleteSessionResponse>
    {
        public string SessionId { get; set; } = string.Empty;
    }
}
