namespace SpotifyMusicChatBot.Domain.Application.Model.Conversation
{
    /// <summary>
    /// Resumen estadístico de una sesión de conversación
    /// </summary>
    public class SessionSummary
    {
        public int TotalTurns { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public string FirstPrompt { get; set; } = string.Empty;
        
        /// <summary>
        /// Duración de la sesión en minutos
        /// </summary>
        public double DurationMinutes => SessionEnd != SessionStart 
            ? (SessionEnd - SessionStart).TotalMinutes 
            : 0;
    }
}
