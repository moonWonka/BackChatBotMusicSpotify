using SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms;

namespace SpotifyMusicChatBot.API.Application.Command.ExcludedTerms
{
    /// <summary>
    /// Comando para crear un nuevo término excluido
    /// </summary>
    public class CreateExcludedTermCommand
    {
        public string FirebaseUserId { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Convierte el comando a un request del dominio
        /// </summary>
        public CreateExcludedTermRequest ToRequest()
        {
            return new CreateExcludedTermRequest
            {
                FirebaseUserId = FirebaseUserId,
                Term = Term.Trim(),
                Category = Category.Trim()
            };
        }
    }
}
