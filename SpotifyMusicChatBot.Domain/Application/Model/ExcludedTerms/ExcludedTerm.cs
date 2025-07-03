namespace SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms
{
    /// <summary>
    /// Representa un término excluido asociado a un usuario
    /// </summary>
    public class ExcludedTerm
    {
        public int Id { get; set; }
        public string FirebaseUserId { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Artista, Género, Palabra clave, etc.
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
