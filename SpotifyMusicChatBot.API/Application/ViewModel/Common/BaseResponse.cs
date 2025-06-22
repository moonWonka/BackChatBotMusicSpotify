namespace SpotifyMusicChatBot.API.Application.ViewModel.Common
{
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}