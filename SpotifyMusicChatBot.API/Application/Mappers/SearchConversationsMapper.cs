using SpotifyMusicChatBot.API.Application.Query.SearchConversations;
using SpotifyMusicChatBot.API.Application.ViewModel.SearchConversations;
using SpotifyMusicChatBot.Domain.Application.Model.Search;

namespace SpotifyMusicChatBot.API.Application.Mappers
{
    /// <summary>
    /// Mapper para conversiones de SearchConversations entre capas
    /// </summary>
    public static class SearchConversationsMapper
    {
        /// <summary>
        /// Convierte SearchResult de Domain a ViewModel
        /// </summary>
        public static SearchResultViewModel ToViewModel(SearchResult domainModel)
        {
            return new SearchResultViewModel
            {
                Id = domainModel.Id,
                Timestamp = domainModel.Timestamp,
                SessionId = domainModel.SessionId,
                UserPrompt = domainModel.UserPrompt,
                AiResponse = domainModel.AiResponse,
                MatchType = "Both", // Por defecto, se puede mejorar según la lógica de negocio
                HighlightedText = null // Se puede implementar highlighting más adelante
            };
        }

        /// <summary>
        /// Crea una respuesta exitosa con resultados
        /// </summary>
        public static SearchConversationsResponse ToSuccessResponse(
            IList<SearchResult> domainResults,
            string searchTerm,
            long searchTimeMs)
        {
            return new SearchConversationsResponse
            {
                Results = domainResults.Select(ToViewModel).ToList(),
                TotalResults = domainResults.Count,
                SearchTerm = searchTerm,
                SearchTimeMs = searchTimeMs,
                StatusCode = 200,
                Message = "Búsqueda realizada exitosamente"
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static SearchConversationsResponse ToErrorResponse(
            string searchTerm,
            long searchTimeMs,
            int statusCode,
            string message)
        {
            return new SearchConversationsResponse
            {
                Results = new List<SearchResultViewModel>(),
                TotalResults = 0,
                SearchTerm = searchTerm,
                SearchTimeMs = searchTimeMs,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
