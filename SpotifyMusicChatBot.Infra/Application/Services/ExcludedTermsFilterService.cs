using Microsoft.Extensions.Logging;
using SpotifyMusicChatBot.Domain.Application.Model.ExcludedTerms;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Services;
using SpotifyMusicChatBot.Domain.Application.Services.Prompts;
using System.Text;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    /// <summary>
    /// Servicio para filtrar respuestas basado en términos excluidos del usuario
    /// </summary>
    public class ExcludedTermsFilterService : IExcludedTermsFilterService
    {
        private readonly IChatBotRepository _repository;
        private readonly IBaseAIService _aiService;
        private readonly ILogger<ExcludedTermsFilterService> _logger;

        public ExcludedTermsFilterService(
            IChatBotRepository repository, 
            IBaseAIService aiService,
            ILogger<ExcludedTermsFilterService> logger)
        {
            _repository = repository;
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Filtra una respuesta eliminando términos excluidos del usuario
        /// </summary>
        public async Task<string> FilterResponseAsync(string originalResponse, string firebaseUserId)
        {
            try
            {
                // Obtener términos excluidos del usuario
                var excludedTerms = await _repository.GetExcludedTermsByUserAsync(firebaseUserId);
                
                if (!excludedTerms.Any())
                {
                    _logger.LogDebug("No hay términos excluidos para el usuario {UserId}", firebaseUserId);
                    return originalResponse;
                }

                // Formatear términos excluidos para el prompt
                var formattedTerms = FormatExcludedTermsForPrompt(excludedTerms);

                // Crear el prompt de filtrado
                var filterPrompt = AIPrompts.PROMPT_FILTRAR_TERMINOS_EXCLUIDOS
                    .Replace("{terminos_excluidos}", formattedTerms)
                    .Replace("{respuesta_original}", originalResponse);

                _logger.LogInformation("🔍 Filtrando respuesta para usuario {UserId} con {Count} términos excluidos", 
                    firebaseUserId, excludedTerms.Count);

                // Enviar al modelo de IA para filtrar
                var aiResponse = await _aiService.ExecuteModelAsync(filterPrompt, temperature: 0.3f, maxTokens: 2000);
                
                if (!aiResponse.IsSuccess)
                {
                    _logger.LogWarning("⚠️ Error en la IA al filtrar respuesta: {Message}", aiResponse.Message);
                    return originalResponse;
                }

                // Procesar la respuesta del filtro
                var finalResponse = ProcessFilterResponse(aiResponse.Content, originalResponse);

                _logger.LogInformation("✅ Respuesta filtrada exitosamente para usuario {UserId}", firebaseUserId);
                
                return finalResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error filtrando respuesta para usuario {UserId}: {Message}", firebaseUserId, ex.Message);
                // En caso de error, devolver la respuesta original
                return originalResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los términos excluidos de un usuario formateados para el prompt
        /// </summary>
        public async Task<string> GetFormattedExcludedTermsAsync(string firebaseUserId)
        {
            try
            {
                var excludedTerms = await _repository.GetExcludedTermsByUserAsync(firebaseUserId);
                return FormatExcludedTermsForPrompt(excludedTerms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo términos excluidos formateados para usuario {UserId}", firebaseUserId);
                return string.Empty;
            }
        }

        /// <summary>
        /// Verifica si una respuesta contiene términos excluidos
        /// </summary>
        public async Task<bool> ContainsExcludedTermsAsync(string response, string firebaseUserId)
        {
            try
            {
                var excludedTerms = await _repository.GetExcludedTermsByUserAsync(firebaseUserId);
                
                if (!excludedTerms.Any())
                    return false;

                var responseLower = response.ToLowerInvariant();
                
                return excludedTerms.Any(term => 
                    responseLower.Contains(term.Term.ToLowerInvariant()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error verificando términos excluidos en respuesta para usuario {UserId}", firebaseUserId);
                return false;
            }
        }

        /// <summary>
        /// Formatea los términos excluidos para incluir en el prompt
        /// </summary>
        private string FormatExcludedTermsForPrompt(IList<ExcludedTerm> excludedTerms)
        {
            if (!excludedTerms.Any())
                return "No hay términos excluidos para este usuario.";

            var sb = new StringBuilder();
            
            // Agrupar por categoría
            var groupedTerms = excludedTerms.GroupBy(t => t.Category);
            
            foreach (var group in groupedTerms)
            {
                sb.AppendLine($"\n{group.Key.ToUpper()}:");
                foreach (var term in group)
                {
                    sb.AppendLine($"- {term.Term}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Procesa la respuesta del filtro de IA
        /// </summary>
        private string ProcessFilterResponse(string filterResponse, string originalResponse)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filterResponse))
                    return originalResponse;

                // Buscar los diferentes tipos de respuesta
                if (filterResponse.Contains("RESPUESTA_LIMPIA:"))
                {
                    return filterResponse.Replace("RESPUESTA_LIMPIA:", "").Trim();
                }
                else if (filterResponse.Contains("RESPUESTA_FILTRADA:"))
                {
                    return filterResponse.Replace("RESPUESTA_FILTRADA:", "").Trim();
                }
                else if (filterResponse.Contains("RESPUESTA_ALTERNATIVA:"))
                {
                    return filterResponse.Replace("RESPUESTA_ALTERNATIVA:", "").Trim();
                }
                else
                {
                    // Si no sigue el formato esperado, devolver la respuesta del filtro tal como está
                    _logger.LogWarning("⚠️ Respuesta de filtro no sigue el formato esperado");
                    return filterResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error procesando respuesta del filtro");
                return originalResponse;
            }
        }
    }
}
