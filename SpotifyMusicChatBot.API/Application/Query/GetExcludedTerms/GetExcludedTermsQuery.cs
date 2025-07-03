namespace SpotifyMusicChatBot.API.Application.Query.GetExcludedTerms
{
    /// <summary>
    /// Query para obtener términos excluidos de un usuario
    /// </summary>
    public class GetExcludedTermsQuery
    {
        public string FirebaseUserId { get; set; } = string.Empty;
        public string? Category { get; set; }
    }
}
