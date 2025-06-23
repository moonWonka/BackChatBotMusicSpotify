namespace SpotifyMusicChatBot.Domain.Application.Model.Conversation
{
    /// <summary>
    /// Representa una sesión de conversación con información básica
    /// </summary>
    public class ConversationSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string UserPrompt { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
