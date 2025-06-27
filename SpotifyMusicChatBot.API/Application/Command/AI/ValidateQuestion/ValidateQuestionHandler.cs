using MediatR;
using SpotifyMusicChatBot.Domain.Application.Services;
using System.Diagnostics;
using System.Text;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ValidateQuestion
{
    /// <summary>
    /// Handler para validar si una pregunta es relevante para el asistente musical
    /// </summary>
    public class ValidateQuestionHandler : IRequestHandler<ValidateQuestionRequest, ValidateQuestionResponse>
    {
        private readonly IAIService _aiService;
        private readonly ILogger<ValidateQuestionHandler> _logger;

        private readonly HashSet<string> _validMusicTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "artista", "cantante", "banda", "grupo", "músico",
            "canción", "tema", "track", "single",
            "álbum", "disco", "EP", "LP",
            "género", "estilo", "reggaetón", "pop", "rock", "jazz", "clásica",
            "playlist", "lista de reproducción",
            "letra", "lyrics",
            "duración", "tiempo",
            "popularidad", "éxitos", "hits",
            "año", "fecha", "lanzamiento",
            "colaboración", "featuring", "feat",
            "discografía", "repertorio"
        };

        private readonly HashSet<string> _invalidTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "política", "religión", "deportes", "cocina", "medicina",
            "programación", "tecnología", "ciencia", "matemáticas",
            "historia", "geografía", "economía", "finanzas"
        };

        public ValidateQuestionHandler(
            IAIService aiService,
            ILogger<ValidateQuestionHandler> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<ValidateQuestionResponse> Handle(ValidateQuestionRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ValidateQuestionResponse
            {
                Question = request.Question,
                AIModelUsed = request.AIModel
            };

            try
            {
                // Análisis inicial rápido basado en palabras clave
                var quickAnalysis = PerformQuickAnalysis(request.Question);
                
                // Validación con IA
                var validationResult = await _aiService.ValidateQuestionAsync(request.Question, request.AIModel, cancellationToken);

                if (!validationResult.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = validationResult.Message ?? "Error al validar la pregunta";
                    return response;
                }
                
                // Combinar resultados
                response.ValidationStatus = validationResult.ValidationStatus;
                response.ValidationReason = validationResult.ValidationReason;
                response.IdentifiedCategory = validationResult.IdentifiedCategory ?? quickAnalysis.Category;
                response.ConfidenceLevel = validationResult.ConfidenceLevel;

                if (validationResult.ValidationStatus == "ACLARAR")
                {
                    response.Suggestions = validationResult.Suggestions.Any() 
                        ? validationResult.Suggestions 
                        : GenerateSuggestions(request.Question, quickAnalysis);
                }

                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = true;
                response.Message = GetValidationMessage(response.ValidationStatus);

                _logger.LogInformation("Pregunta validada: {Status}, Confianza: {Confidence}%, Tiempo: {ElapsedMs}ms", 
                    response.ValidationStatus, response.ConfidenceLevel, response.ProcessingTimeMs);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = false;
                response.Message = "Error al validar la pregunta";
                response.ValidationStatus = "ERROR";

                _logger.LogError(ex, "Error al validar pregunta");
                return response;
            }
        }

        private QuickAnalysisResult PerformQuickAnalysis(string question)
        {
            var result = new QuickAnalysisResult();
            var questionLower = question.ToLower();

            // Buscar palabras clave válidas
            var validKeywords = _validMusicTopics.Where(topic => questionLower.Contains(topic)).ToList();
            var invalidKeywords = _invalidTopics.Where(topic => questionLower.Contains(topic)).ToList();

            if (validKeywords.Any())
            {
                result.IsValid = true;
                result.Category = DetermineCategory(validKeywords);
                result.Confidence = Math.Min(95, 60 + (validKeywords.Count * 10));
            }
            else if (invalidKeywords.Any())
            {
                result.IsValid = false;
                result.Category = "FUERA_CONTEXTO";
                result.Confidence = 80;
            }
            else
            {
                result.IsValid = null; // Necesita análisis más profundo con IA
                result.Category = "DESCONOCIDA";
                result.Confidence = 50;
            }

            return result;
        }

        private async Task<AIValidationResult> ValidateWithAI(string question, string aiModel)
        {
            try
            {
                // Construir prompt de validación usando las constantes
                var prompt = AIPrompts.PROMPT_VALIDAR_PREGUNTA
                    .Replace("{pregunta}", question);
                
                // Simular llamada a IA
                await Task.Delay(300); // Simular latencia de IA

                // Lógica simplificada de validación
                var questionLower = question.ToLower();
                
                if (_validMusicTopics.Any(topic => questionLower.Contains(topic)))
                {
                    return new AIValidationResult
                    {
                        Status = "VALIDA",
                        Reason = "La pregunta está relacionada con música",
                        Confidence = 85
                    };
                }
                else if (_invalidTopics.Any(topic => questionLower.Contains(topic)))
                {
                    return new AIValidationResult
                    {
                        Status = "FUERA_CONTEXTO",
                        Reason = "La pregunta no está relacionada con música",
                        Confidence = 90
                    };
                }
                else if (question.Length < 10)
                {
                    return new AIValidationResult
                    {
                        Status = "ACLARAR",
                        Reason = "La pregunta es demasiado corta o ambigua",
                        Confidence = 75
                    };
                }
                else
                {
                    return new AIValidationResult
                    {
                        Status = "ACLARAR",
                        Reason = "La pregunta necesita ser más específica sobre música",
                        Confidence = 70
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en validación con IA");
                return new AIValidationResult
                {
                    Status = "ERROR",
                    Reason = "Error en el procesamiento",
                    Confidence = 0
                };
            }
        }

        private string BuildValidationPrompt(string question)
        {
            return AIPrompts.PROMPT_VALIDAR_PREGUNTA.Replace("{pregunta}", question);
        }

        private string DetermineCategory(List<string> keywords)
        {
            if (keywords.Any(k => new[] { "artista", "cantante", "banda", "grupo", "músico" }.Contains(k.ToLower())))
                return "ARTISTAS";
            if (keywords.Any(k => new[] { "canción", "tema", "track", "single" }.Contains(k.ToLower())))
                return "CANCIONES";
            if (keywords.Any(k => new[] { "álbum", "disco", "ep", "lp" }.Contains(k.ToLower())))
                return "ALBUMES";
            if (keywords.Any(k => new[] { "género", "estilo" }.Contains(k.ToLower())))
                return "GENEROS";
            
            return "MUSICA_GENERAL";
        }

        private List<string> GenerateSuggestions(string question, QuickAnalysisResult analysis)
        {
            var suggestions = new List<string>();

            if (question.Length < 10)
            {
                suggestions.Add("Proporciona más detalles en tu pregunta");
                suggestions.Add("Especifica sobre qué artista, canción o álbum quieres saber");
            }

            if (!_validMusicTopics.Any(topic => question.ToLower().Contains(topic)))
            {
                suggestions.Add("Incluye términos musicales como 'artista', 'canción', 'álbum'");
                suggestions.Add("Pregunta sobre géneros musicales, colaboraciones o características de canciones");
            }

            if (analysis.Category == "DESCONOCIDA")
            {
                suggestions.Add("Reformula tu pregunta usando términos relacionados con música");
                suggestions.Add("Especifica si quieres información sobre un artista, canción o género musical");
            }

            return suggestions;
        }

        private string GetValidationMessage(string status)
        {
            return status switch
            {
                "VALIDA" => "Pregunta válida para el asistente musical",
                "ACLARAR" => "La pregunta requiere aclaración",
                "FUERA_CONTEXTO" => "La pregunta está fuera del contexto musical",
                "ERROR" => "Error al validar la pregunta",
                _ => "Estado de validación desconocido"
            };
        }

        private class QuickAnalysisResult
        {
            public bool? IsValid { get; set; }
            public string Category { get; set; } = string.Empty;
            public int Confidence { get; set; }
        }

        private class AIValidationResult
        {
            public string Status { get; set; } = string.Empty;
            public string Reason { get; set; } = string.Empty;
            public int Confidence { get; set; }
        }
    }
}
