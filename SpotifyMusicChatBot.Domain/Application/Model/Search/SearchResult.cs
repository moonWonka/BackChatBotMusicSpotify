namespace SpotifyMusicChatBot.Domain.Application.Model.Search
{
    /// <summary>
    /// Resultado de búsqueda en conversaciones
    /// </summary>
    public class SearchResult
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string UserPrompt { get; set; } = string.Empty;
        public string AiResponse { get; set; } = string.Empty;
    }
}
