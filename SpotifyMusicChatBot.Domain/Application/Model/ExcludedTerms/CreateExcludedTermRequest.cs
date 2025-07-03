namespace SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms
{
    /// <summary>
    /// Representa los datos para crear un nuevo término excluido
    /// </summary>
    public class CreateExcludedTermRequest
    {
        public string FirebaseUserId { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
