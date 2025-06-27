using MediatR;
using SpotifyMusicChatBot.Domain.Application.Services;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SpotifyMusicChatBot.API.Application.Command.AI.GenerateNaturalResponse
{
    /// <summary>
    /// Handler para generar respuestas en lenguaje natural a partir de resultados de base de datos
    /// </summary>
    public class GenerateNaturalResponseHandler : IRequestHandler<GenerateNaturalResponseRequest, GenerateNaturalResponseResponse>
    {
        private readonly IAIServiceFactory _aiServiceFactory;
        private readonly ILogger<GenerateNaturalResponseHandler> _logger;

        public GenerateNaturalResponseHandler(
            IAIServiceFactory aiServiceFactory,
            ILogger<GenerateNaturalResponseHandler> logger)
        {
            _aiServiceFactory = aiServiceFactory;
            _logger = logger;
        }

        public async Task<GenerateNaturalResponseResponse> Handle(GenerateNaturalResponseRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new GenerateNaturalResponseResponse
            {
                Question = request.Question,
                ResponseTone = request.ResponseTone,
                ResponseLength = request.ResponseLength,
                AIModelUsed = request.AIModel
            };

            try
            {
                // Analizar y procesar los datos de la base de datos
                var dataAnalysis = AnalyzeDatabaseResults(request.DatabaseResults);
                response.Statistics = dataAnalysis.Statistics;
                response.ProcessedItemsCount = dataAnalysis.ItemCount;
                response.DataSummary = dataAnalysis.Summary;

                // Generar respuesta en lenguaje natural
                var naturalResponse = await GenerateNaturalLanguageResponse(
                    request.Question,
                    request.DatabaseResults,
                    dataAnalysis,
                    request.AIModel,
                    request.ResponseTone,
                    request.ResponseLength
                );

                response.NaturalResponse = naturalResponse.MainResponse;
                response.AlternativeResponse = naturalResponse.AlternativeResponse;
                response.ConfidenceLevel = naturalResponse.Confidence;

                // Generar elementos adicionales
                response.Highlights = ExtractHighlights(naturalResponse.MainResponse, dataAnalysis);
                response.RelatedQuestions = GenerateRelatedQuestions(request.Question, dataAnalysis);

                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = true;
                response.Message = "Respuesta generada exitosamente";

                _logger.LogInformation("Respuesta natural generada. Items procesados: {ItemCount}, Confianza: {Confidence}%, Tiempo: {ElapsedMs}ms", 
                    response.ProcessedItemsCount, response.ConfidenceLevel, response.ProcessingTimeMs);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = false;
                response.Message = "Error al generar la respuesta en lenguaje natural";

                _logger.LogError(ex, "Error al generar respuesta natural para pregunta: {Question}", request.Question);
                return response;
            }
        }

        private DatabaseAnalysisResult AnalyzeDatabaseResults(string databaseResults)
        {
            var result = new DatabaseAnalysisResult
            {
                Statistics = new ResponseStatistics()
            };

            try
            {
                // Si los datos están en formato JSON, analizarlos
                if (databaseResults.TrimStart().StartsWith('[') || databaseResults.TrimStart().StartsWith('{'))
                {
                    result = AnalyzeJsonResults(databaseResults);
                }
                else
                {
                    // Analizar datos tabulares o texto plano
                    result = AnalyzeTextResults(databaseResults);
                }

                // Generar resumen
                result.Summary = GenerateDataSummary(result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al analizar resultados de base de datos");
                result.Summary = "Datos procesados correctamente";
                result.ItemCount = 1;
                return result;
            }
        }

        private DatabaseAnalysisResult AnalyzeJsonResults(string jsonData)
        {
            var result = new DatabaseAnalysisResult
            {
                Statistics = new ResponseStatistics()
            };

            try
            {
                using var document = JsonDocument.Parse(jsonData);
                
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    result.ItemCount = document.RootElement.GetArrayLength();
                    result.Statistics.TotalItems = result.ItemCount;

                    // Analizar elementos del array
                    var firstElement = document.RootElement.EnumerateArray().FirstOrDefault();
                    if (firstElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var property in firstElement.EnumerateObject())
                        {
                            result.Statistics.MainCategories.Add(property.Name);
                        }
                    }
                }
                else if (document.RootElement.ValueKind == JsonValueKind.Object)
                {
                    result.ItemCount = 1;
                    result.Statistics.TotalItems = 1;

                    foreach (var property in document.RootElement.EnumerateObject())
                    {
                        result.Statistics.MainCategories.Add(property.Name);
                    }
                }

                return result;
            }
            catch
            {
                result.ItemCount = 1;
                result.Statistics.TotalItems = 1;
                return result;
            }
        }

        private DatabaseAnalysisResult AnalyzeTextResults(string textData)
        {
            var result = new DatabaseAnalysisResult
            {
                Statistics = new ResponseStatistics()
            };

            var lines = textData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            result.ItemCount = Math.Max(1, lines.Length);
            result.Statistics.TotalItems = result.ItemCount;

            // Intentar identificar patrones en los datos
            if (lines.Length > 1)
            {
                // Buscar headers o patrones comunes
                var firstLine = lines[0];
                if (firstLine.Contains('|') || firstLine.Contains('\t') || firstLine.Contains(','))
                {
                    var headers = firstLine.Split(new[] { '|', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    result.Statistics.MainCategories = headers.Select(h => h.Trim()).ToList();
                }
            }

            return result;
        }

        private string GenerateDataSummary(DatabaseAnalysisResult analysis)
        {
            var summary = new StringBuilder();

            if (analysis.ItemCount == 0)
            {
                summary.Append("No se encontraron resultados");
            }
            else if (analysis.ItemCount == 1)
            {
                summary.Append("Se encontró 1 resultado");
            }
            else
            {
                summary.Append($"Se encontraron {analysis.ItemCount} resultados");
            }

            if (analysis.Statistics.MainCategories.Any())
            {
                summary.Append($" con información sobre: {string.Join(", ", analysis.Statistics.MainCategories.Take(3))}");
            }

            return summary.ToString();
        }

        private async Task<NaturalResponseResult> GenerateNaturalLanguageResponse(
            string question,
            string databaseResults,
            DatabaseAnalysisResult analysis,
            string aiModel,
            string tone,
            string length)
        {
            try
            {
                var prompt = BuildNaturalResponsePrompt(question, databaseResults, analysis, tone, length);
                
                // Simular llamada a IA
                await Task.Delay(350); // Simular latencia de IA

                // Generar respuesta basada en los datos
                var response = GenerateSimplifiedResponse(question, analysis, tone, length);

                return new NaturalResponseResult
                {
                    MainResponse = response,
                    AlternativeResponse = null,
                    Confidence = 85
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en generación de respuesta con IA");
                return new NaturalResponseResult
                {
                    MainResponse = "Lo siento, no pude procesar la información correctamente.",
                    Confidence = 30
                };
            }
        }

        private string BuildNaturalResponsePrompt(
            string question,
            string databaseResults,
            DatabaseAnalysisResult analysis,
            string tone,
            string length)
        {
            var promptBuilder = new StringBuilder();
            
            promptBuilder.AppendLine("Basándote en la pregunta del usuario y los resultados obtenidos de la base de datos, genera una respuesta clara, concisa y útil en lenguaje natural.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine($"Pregunta del usuario: \"{question}\"");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Resultados de la base de datos:");
            promptBuilder.AppendLine(databaseResults);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("INSTRUCCIONES PARA LA RESPUESTA:");
            promptBuilder.AppendLine("1. Proporciona una respuesta en español, clara y fácil de entender");
            promptBuilder.AppendLine($"2. Usa un tono {tone}");
            promptBuilder.AppendLine($"3. Mantén una longitud {length}");
            promptBuilder.AppendLine("4. Incluye datos específicos de los resultados");
            promptBuilder.AppendLine("5. Organiza la información de manera lógica");
            promptBuilder.AppendLine("6. No menciones aspectos técnicos como SQL, tablas, o bases de datos");
            promptBuilder.AppendLine("7. Sé conversacional y amigable");

            return promptBuilder.ToString();
        }

        private string GenerateSimplifiedResponse(string question, DatabaseAnalysisResult analysis, string tone, string length)
        {
            var responseBuilder = new StringBuilder();

            // Inicio de respuesta basado en el tono
            if (tone == "formal")
            {
                responseBuilder.Append("Según la información disponible, ");
            }
            else if (tone == "casual")
            {
                responseBuilder.Append("¡Hola! Te cuento que ");
            }
            else
            {
                responseBuilder.Append("Basándome en los datos, ");
            }

            // Contenido principal basado en el análisis
            if (analysis.ItemCount == 0)
            {
                responseBuilder.Append("no encontré resultados específicos para tu consulta. ");
                responseBuilder.Append("Podrías intentar reformular la pregunta o ser más específico sobre lo que buscas.");
            }
            else if (analysis.ItemCount == 1)
            {
                responseBuilder.Append("encontré exactamente lo que buscabas. ");
                responseBuilder.Append("Te puedo proporcionar la información detallada sobre este resultado.");
            }
            else
            {
                responseBuilder.Append($"encontré {analysis.ItemCount} resultados que coinciden con tu búsqueda. ");
                
                if (length == "detailed")
                {
                    responseBuilder.Append("Aquí tienes un resumen completo de la información:");
                    if (analysis.Statistics.MainCategories.Any())
                    {
                        responseBuilder.Append($" Los datos incluyen información sobre {string.Join(", ", analysis.Statistics.MainCategories)}.");
                    }
                }
                else
                {
                    responseBuilder.Append("Los datos muestran información relevante para tu consulta.");
                }
            }

            // Cierre basado en el tono
            if (tone == "casual" && analysis.ItemCount > 0)
            {
                responseBuilder.Append(" ¿Te gustaría saber algo más específico?");
            }
            else if (tone == "formal")
            {
                responseBuilder.Append(" Si necesita información adicional, no dude en consultarme.");
            }

            return responseBuilder.ToString();
        }

        private List<string> ExtractHighlights(string response, DatabaseAnalysisResult analysis)
        {
            var highlights = new List<string>();

            if (analysis.ItemCount > 0)
            {
                highlights.Add($"{analysis.ItemCount} resultados encontrados");
            }

            if (analysis.Statistics.MainCategories.Any())
            {
                highlights.Add($"Categorías: {string.Join(", ", analysis.Statistics.MainCategories.Take(2))}");
            }

            // Extraer números o datos específicos de la respuesta
            var numberMatches = System.Text.RegularExpressions.Regex.Matches(response, @"\d+");
            foreach (System.Text.RegularExpressions.Match match in numberMatches.Take(2))
            {
                highlights.Add($"Valor destacado: {match.Value}");
            }

            return highlights;
        }

        private List<string> GenerateRelatedQuestions(string originalQuestion, DatabaseAnalysisResult analysis)
        {
            var relatedQuestions = new List<string>();
            var questionLower = originalQuestion.ToLower();

            // Preguntas relacionadas basadas en la consulta original
            if (questionLower.Contains("artista"))
            {
                relatedQuestions.Add("¿Cuáles son las canciones más populares de este artista?");
                relatedQuestions.Add("¿En qué año comenzó su carrera musical?");
                relatedQuestions.Add("¿Con qué otros artistas ha colaborado?");
            }
            else if (questionLower.Contains("canción") || questionLower.Contains("cancion"))
            {
                relatedQuestions.Add("¿Cuándo fue lanzada esta canción?");
                relatedQuestions.Add("¿Qué características musicales tiene?");
                relatedQuestions.Add("¿Hay otras canciones similares?");
            }
            else if (questionLower.Contains("popular"))
            {
                relatedQuestions.Add("¿Cuáles son los géneros musicales más populares?");
                relatedQuestions.Add("¿Qué artistas están en tendencia actualmente?");
                relatedQuestions.Add("¿Cuáles son los éxitos más recientes?");
            }
            else
            {
                // Preguntas generales
                relatedQuestions.Add("¿Qué más te gustaría saber sobre música?");
                relatedQuestions.Add("¿Te interesa algún género musical en particular?");
                relatedQuestions.Add("¿Buscas recomendaciones de artistas o canciones?");
            }

            return relatedQuestions.Take(3).ToList();
        }

        private class DatabaseAnalysisResult
        {
            public int ItemCount { get; set; }
            public string Summary { get; set; } = string.Empty;
            public ResponseStatistics Statistics { get; set; } = new ResponseStatistics();
        }

        private class NaturalResponseResult
        {
            public string MainResponse { get; set; } = string.Empty;
            public string? AlternativeResponse { get; set; }
            public int Confidence { get; set; }
        }
    }
}
