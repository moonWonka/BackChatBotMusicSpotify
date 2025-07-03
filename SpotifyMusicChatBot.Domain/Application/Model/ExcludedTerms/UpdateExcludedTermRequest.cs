namespace SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms
{
    /// <summary>
    /// Representa los datos para actualizar un término excluido
    /// </summary>
    public class UpdateExcludedTermRequest
    {
        public int Id { get; set; }
        public string FirebaseUserId { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
