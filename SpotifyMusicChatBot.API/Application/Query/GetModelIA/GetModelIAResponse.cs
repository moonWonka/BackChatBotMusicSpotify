using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Query.GetModelIA
{
    public class GetModelIAResponse : BaseResponse
    {
        public string UserInfo { get; set; }
        public string MusicPreferences { get; set; }
    }
}