namespace SpotifyMusicChatBot.Domain.Application.Model.Conversation
{
    /// <summary>
    /// Representa un turno individual en una conversación
    /// </summary>
    public class ConversationTurn
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserPrompt { get; set; } = string.Empty;
        public string AiResponse { get; set; } = string.Empty;
    }
}
