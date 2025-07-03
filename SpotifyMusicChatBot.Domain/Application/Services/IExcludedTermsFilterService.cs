using SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms;

namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Interfaz para el servicio de filtrado de términos excluidos
    /// </summary>
    public interface IExcludedTermsFilterService
    {
        /// <summary>
        /// Filtra una respuesta eliminando términos excluidos del usuario
        /// </summary>
        Task<string> FilterResponseAsync(string originalResponse, string firebaseUserId);

        /// <summary>
        /// Obtiene todos los términos excluidos de un usuario formateados para el prompt
        /// </summary>
        Task<string> GetFormattedExcludedTermsAsync(string firebaseUserId);

        /// <summary>
        /// Verifica si una respuesta contiene términos excluidos
        /// </summary>
        Task<bool> ContainsExcludedTermsAsync(string response, string firebaseUserId);
    }
}
