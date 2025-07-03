using SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms;

namespace SpotifyMusicChatBot.API.Application.Command.ExcludedTerms
{
    /// <summary>
    /// Comando para actualizar un término excluido
    /// </summary>
    public class UpdateExcludedTermCommand
    {
        public int Id { get; set; }
        public string FirebaseUserId { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Convierte el comando a un request del dominio
        /// </summary>
        public UpdateExcludedTermRequest ToRequest()
        {
            return new UpdateExcludedTermRequest
            {
                Id = Id,
                FirebaseUserId = FirebaseUserId,
                Term = Term.Trim(),
                Category = Category.Trim(),
                IsActive = IsActive
            };
        }
    }
}
