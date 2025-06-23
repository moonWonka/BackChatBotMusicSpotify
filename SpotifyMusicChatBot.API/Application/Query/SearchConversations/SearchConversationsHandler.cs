using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Model.Search;

namespace SpotifyMusicChatBot.API.Application.Query.SearchConversations
{
    public class SearchConversationsHandler : IRequestHandler<SearchConversationsRequest, SearchConversationsResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<SearchConversationsHandler> _logger;

        public SearchConversationsHandler(IChatBotRepository chatRepository, ILogger<SearchConversationsHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        public async Task<SearchConversationsResponse> Handle(SearchConversationsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    return new SearchConversationsResponse
                    {
                        Success = false,
                        Message = "El término de búsqueda es requerido"
                    };
                }

                IEnumerable<SearchResult> results = await _chatRepository.SearchConversationsAsync(request.SearchTerm);
                
                return new SearchConversationsResponse
                {
                    Results = results,
                    Success = true,
                    Message = "Búsqueda realizada exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching conversations with term: {SearchTerm}, Message: {Message}", request.SearchTerm, ex.Message);
                return new SearchConversationsResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                };
            }
        }
    }
}
