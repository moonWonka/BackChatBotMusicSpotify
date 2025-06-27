using SpotifyMusicChatBot.Domain.Application.Services;
using Microsoft.Extensions.Logging;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    /// <summary>
    /// Implementación mock del servicio de IA para desarrollo y testing
    /// </summary>
    public class MockAIService : IAIService
    {
        private readonly ILogger<MockAIService> _logger;

        public MockAIService(ILogger<MockAIService> logger)
        {
            _logger = logger;
        }

        public async Task<ContextualizationResult> ContextualizeQuestionAsync(string question, string conversationHistory, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🤖 MockAI: Contextualizing question with model {ModelName}", modelName);
            await Task.Delay(200, cancellationToken); // Simular latencia

            // Análisis simple: si la pregunta es muy corta, solicitar más contexto
            bool needsContext = question.Length < 15 || 
                               question.Contains("esa", StringComparison.OrdinalIgnoreCase) ||
                               question.Contains("la anterior", StringComparison.OrdinalIgnoreCase);

            return new ContextualizationResult
            {
                ContextualizedQuestion = needsContext ? 
                    $"Basándome en el contexto previo: {question}" : question,
                WasContextualized = needsContext,
                OriginalQuestion = question,
                AnalysisType = needsContext ? "CONTEXTUALIZADA" : "INDEPENDIENTE",
                IsSuccess = true,
                ModelUsed = modelName,
                ProcessingTimeMs = 200,
                ConfidenceLevel = 85
            };
        }

        public async Task<ValidationResult> ValidateQuestionAsync(string question, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🤖 MockAI: Validating question with model {ModelName}", modelName);
            await Task.Delay(150, cancellationToken);

            var musicKeywords = new[] { "música", "song", "album", "artista", "artist", "canción", "banda", "spotify", "playlist" };
            var invalidKeywords = new[] { "política", "religión", "deportes", "cocina", "medicina" };

            bool hasMusicKeywords = musicKeywords.Any(k => question.Contains(k, StringComparison.OrdinalIgnoreCase));
            bool hasInvalidKeywords = invalidKeywords.Any(k => question.Contains(k, StringComparison.OrdinalIgnoreCase));

            string status, reason, category;
            int confidence;

            if (hasInvalidKeywords)
            {
                status = "RECHAZAR";
                reason = "La pregunta no está relacionada con música";
                category = "No Musical";
                confidence = 90;
            }
            else if (hasMusicKeywords)
            {
                status = "ACEPTAR";
                reason = "Pregunta válida sobre música";
                category = "Musical";
                confidence = 95;
            }
            else if (question.Length < 10)
            {
                status = "ACLARAR";
                reason = "La pregunta es demasiado corta o ambigua";
                category = "Ambigua";
                confidence = 75;
            }
            else
            {
                status = "ACLARAR";
                reason = "Necesita especificar el contexto musical";
                category = "Contexto requerido";
                confidence = 70;
            }

            return new ValidationResult
            {
                ValidationStatus = status,
                ValidationReason = reason,
                IdentifiedCategory = category,
                IsSuccess = true,
                ModelUsed = modelName,
                ProcessingTimeMs = 150,
                ConfidenceLevel = confidence
            };
        }

        public async Task<SQLGenerationResult> GenerateSQLAsync(string question, int resultLimit = 50, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🤖 MockAI: Generating SQL with model {ModelName}", modelName);
            await Task.Delay(300, cancellationToken);

            // Generar SQL simulado basado en palabras clave
            string sql = "";
            string explanation = "";
            var tablesUsed = new List<string>();

            if (question.Contains("artista", StringComparison.OrdinalIgnoreCase) || 
                question.Contains("artist", StringComparison.OrdinalIgnoreCase))
            {
                sql = $@"SELECT TOP {resultLimit} 
                    a.artist_id AS ArtistId,
                    a.artist_name AS ArtistName,
                    a.genres AS Genres,
                    a.popularity AS Popularity
                FROM artists a 
                WHERE a.artist_name IS NOT NULL
                ORDER BY a.popularity DESC";
                explanation = "Consulta para obtener información de artistas ordenados por popularidad";
                tablesUsed.AddRange(new[] { "artists" });
            }
            else if (question.Contains("canción", StringComparison.OrdinalIgnoreCase) || 
                     question.Contains("song", StringComparison.OrdinalIgnoreCase))
            {
                sql = $@"SELECT TOP {resultLimit}
                    t.track_id AS TrackId,
                    t.track_name AS TrackName,
                    t.artist_name AS ArtistName,
                    t.album_name AS AlbumName,
                    t.popularity AS Popularity
                FROM tracks t 
                WHERE t.track_name IS NOT NULL
                ORDER BY t.popularity DESC";
                explanation = "Consulta para obtener canciones ordenadas por popularidad";
                tablesUsed.AddRange(new[] { "tracks" });
            }
            else
            {
                sql = $@"SELECT TOP {resultLimit}
                    t.track_name AS TrackName,
                    t.artist_name AS ArtistName,
                    t.popularity AS Popularity
                FROM tracks t
                ORDER BY t.popularity DESC";
                explanation = "Consulta general para obtener pistas populares";
                tablesUsed.AddRange(new[] { "tracks" });
            }

            return new SQLGenerationResult
            {
                GeneratedSQL = sql,
                SQLExplanation = explanation,
                TablesUsed = tablesUsed,
                IsSuccess = true,
                ModelUsed = modelName,
                ProcessingTimeMs = 300,
                ConfidenceLevel = 80
            };
        }

        public async Task<NaturalResponseResult> GenerateNaturalResponseAsync(string question, string databaseResults, string tone = "casual", string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🤖 MockAI: Generating natural response with model {ModelName}", modelName);
            await Task.Delay(250, cancellationToken);

            // Analizar los resultados y generar una respuesta apropiada
            var resultLines = databaseResults.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var resultCount = Math.Max(0, resultLines.Length - 1); // Restar header si existe

            string response = tone.ToLower() switch
            {
                "formal" => $"Basándome en su consulta sobre {question.ToLower()}, he encontrado {resultCount} resultados relevantes en nuestra base de datos musical.",
                "casual" => $"¡Genial! Encontré {resultCount} resultados para tu pregunta sobre {question.ToLower()}. Aquí tienes lo que encontré:",
                "técnico" => $"Consulta procesada exitosamente. Se obtuvieron {resultCount} registros que coinciden con los criterios especificados para: {question}",
                _ => $"He encontrado {resultCount} resultados para tu consulta sobre {question.ToLower()}."
            };

            // Agregar información adicional si hay resultados
            if (resultCount > 0)
            {
                response += tone.ToLower() switch
                {
                    "casual" => " ¿Te gustaría que profundice en algún resultado específico?",
                    "formal" => " Si desea información adicional sobre algún resultado en particular, no dude en consultarme.",
                    _ => " ¿Necesitas más detalles sobre algún resultado?"
                };
            }

            return new NaturalResponseResult
            {
                NaturalResponse = response,
                DataSummary = $"Procesados {resultCount} resultados",
                RelatedQuestions = new List<string>(),
                Highlights = new List<string>(),
                ResponseTone = tone,
                IsSuccess = true,
                ModelUsed = modelName,
                ProcessingTimeMs = 250,
                ConfidenceLevel = 88
            };
        }

        public async Task<AnalysisResult> AnalyzeAndImproveResponseAsync(string question, string response, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🤖 MockAI: Analyzing response with model {ModelName}", modelName);
            await Task.Delay(200, cancellationToken);

            return new AnalysisResult
            {
                OriginalResponse = response,
                ImprovedResponse = response + " [Análisis: Respuesta optimizada por IA]",
                ImprovementsApplied = new List<string> { "Análisis aplicado", "Optimización de tono" },
                QualityScore = 85,
                IsSuccess = true,
                ModelUsed = modelName,
                ProcessingTimeMs = 200,
                ConfidenceLevel = 82
            };
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            await Task.Delay(50);
            return new List<string> { "Gemini", "Anthropic" };
        }

        public async Task<bool> IsModelAvailableAsync(string modelName)
        {
            await Task.Delay(50);
            var availableModels = new[] { "Gemini", "Anthropic", "Claude" };
            return availableModels.Contains(modelName, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<AIModelResponse> ExecutePromptAsync(string prompt, string modelName = "Gemini", float temperature = 0.7f, int maxTokens = 1000, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🤖 MockAI: Executing custom prompt with model {ModelName}", modelName);
            await Task.Delay(300, cancellationToken);

            return new AIModelResponse
            {
                Content = $"[MockAI Response to: {prompt.Substring(0, Math.Min(50, prompt.Length))}...]",
                IsSuccess = true,
                ModelUsed = modelName,
                ProcessingTimeMs = 300,
                ConfidenceLevel = 75,
                TokensUsed = prompt.Length / 4 // Aproximación simple
            };
        }
    }
}
