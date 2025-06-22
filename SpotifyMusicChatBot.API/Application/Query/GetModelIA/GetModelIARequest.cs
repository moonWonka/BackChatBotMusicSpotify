using MediatR;

namespace SpotifyMusicChatBot.API.Application.Query.GetModelIA
{
    public class GetModelIARequest : IRequest<GetModelIAResponse>
    {
        public string UserId { get; set; }
    }
}