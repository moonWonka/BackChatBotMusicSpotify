using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.API.Application.Mappers;
using System.Diagnostics;
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
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                if (string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    return SearchConversationsMapper.ToErrorResponse(
                        request.SearchTerm,
                        stopwatch.ElapsedMilliseconds,
                        400,
                        "El término de búsqueda es requerido");
                }

                // Obtener resultados del dominio
                IList<SearchResult> domainResults = await _chatRepository.SearchConversationsAsync(request.SearchTerm);

                stopwatch.Stop();

                return SearchConversationsMapper.ToSuccessResponse(
                    domainResults,
                    request.SearchTerm,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error searching conversations with term: {SearchTerm}, Message: {Message}", request.SearchTerm, ex.Message);
                
                return SearchConversationsMapper.ToErrorResponse(
                    request.SearchTerm,
                    stopwatch.ElapsedMilliseconds,
                    500,
                    "Error interno del servidor");
            }
        }
    }
}
