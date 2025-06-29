using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Services;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using System.Diagnostics;
using System.Text;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ProcessQuestion
{
    /// <summary>
    /// Handler para procesar preguntas completas siguiendo el flujo de IA
    /// </summary>
    public class ProcessQuestionHandler : IRequestHandler<ProcessQuestionRequest, ProcessQuestionResponse>
    {
        private readonly IAIService _aiService;
        private readonly ILogger<ProcessQuestionHandler> _logger;

        public ProcessQuestionHandler(
            IAIService aiService,
            IChatBotRepository repository,
            ILogger<ProcessQuestionHandler> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<ProcessQuestionResponse> Handle(ProcessQuestionRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ProcessQuestionResponse
            {
                OriginalQuestion = request.Question,
                AIModelUsed = request.AIModel,
                Steps = new ProcessingSteps()
            };

            try
            {
                // Paso 1: Obtener historial para contextualización
                var conversationHistory = await GetConversationHistory(request.SessionId, request.ContextLimit);

                // Paso 2: Contextualización de la pregunta
                var contextStopwatch = Stopwatch.StartNew();
                var contextualizationResult = await _aiService.ContextualizeQuestionAsync(
                    request.Question, 
                    FormatConversationHistory(conversationHistory), 
                    request.AIModel,
                    cancellationToken);
                contextStopwatch.Stop();
                response.Steps.ContextualizationTimeMs = contextStopwatch.ElapsedMilliseconds;

                if (!contextualizationResult.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = contextualizationResult.Message ?? "Error al contextualizar la pregunta";
                    return response;
                }

                response.ContextualizedQuestion = contextualizationResult.ContextualizedQuestion;
                response.WasContextualized = contextualizationResult.WasContextualized;

                // Paso 3: Validación de la pregunta
                var validationStopwatch = Stopwatch.StartNew();
                var validationResult = await _aiService.ValidateQuestionAsync(
                    response.ContextualizedQuestion, 
                    request.AIModel, 
                    cancellationToken);
                validationStopwatch.Stop();
                response.Steps.ValidationTimeMs = validationStopwatch.ElapsedMilliseconds;

                if (!validationResult.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = validationResult.Message ?? "Error al validar la pregunta";
                    return response;
                }

                response.ValidationStatus = validationResult.ValidationStatus;

                if (validationResult.ValidationStatus != "VALIDA")
                {
                    response.IsSuccess = false;
                    response.ClarificationMessage = validationResult.ValidationReason;
                    
                    if (validationResult.ValidationStatus == "FUERA_CONTEXTO")
                    {
                        response.Message = "La pregunta está fuera del contexto del asistente musical.";
                    }
                    else if (validationResult.ValidationStatus.StartsWith("ACLARAR"))
                    {
                        response.Message = "La pregunta requiere aclaración.";
                    }
                    
                    return response;
                }

                // Paso 4: Generación de SQL
                var sqlStopwatch = Stopwatch.StartNew();
                var sqlResult = await _aiService.GenerateSQLAsync(
                    response.ContextualizedQuestion, 
                    50, 
                    request.AIModel, 
                    cancellationToken);
                sqlStopwatch.Stop();
                response.Steps.SQLGenerationTimeMs = sqlStopwatch.ElapsedMilliseconds;

                if (!sqlResult.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = sqlResult.Message ?? "Error al generar consulta SQL";
                    return response;
                }

                response.GeneratedSQL = sqlResult.GeneratedSQL;

                // Paso 5: Ejecución de SQL
                var executionStopwatch = Stopwatch.StartNew();
                var databaseResults = await ExecuteSQLQuery(response.GeneratedSQL);
                executionStopwatch.Stop();
                response.Steps.SQLExecutionTimeMs = executionStopwatch.ElapsedMilliseconds;

                response.DatabaseResults = databaseResults;

                // Paso 6: Generación de respuesta natural
                var naturalResponseStopwatch = Stopwatch.StartNew();
                var naturalResult = await _aiService.GenerateNaturalResponseAsync(
                    response.ContextualizedQuestion, 
                    databaseResults, 
                    "casual", 
                    request.AIModel, 
                    cancellationToken);
                naturalResponseStopwatch.Stop();
                response.Steps.NaturalResponseTimeMs = naturalResponseStopwatch.ElapsedMilliseconds;

                if (!naturalResult.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = naturalResult.Message ?? "Error al generar respuesta natural";
                    return response;
                }

                response.NaturalResponse = naturalResult.NaturalResponse;

                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = true;
                response.Message = "Pregunta procesada exitosamente";

                _logger.LogInformation("Pregunta procesada exitosamente para sesión {SessionId} con modelo {AIModel} en {ElapsedMs}ms", 
                    request.SessionId, request.AIModel, response.ProcessingTimeMs);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = false;
                response.Message = "Error interno al procesar la pregunta";

                _logger.LogError(ex, "Error al procesar pregunta para sesión {SessionId} con modelo {AIModel}", 
                    request.SessionId, request.AIModel);
                return response;
            }
        }

        private async Task<List<ConversationTurn>> GetConversationHistory(string sessionId, int contextLimit)
        {
            try
            {
                // Si includeContext es false o contextLimit es 0, retorna lista vacía
                if (contextLimit <= 0)
                    return new List<ConversationTurn>();

                // Aquí se implementaría la obtención real del historial
                // Por ahora retornamos lista vacía
                await Task.Delay(10); // Simular consulta
                return new List<ConversationTurn>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al obtener historial de conversación para sesión {SessionId}", sessionId);
                return new List<ConversationTurn>();
            }
        }

        private string FormatConversationHistory(List<ConversationTurn> conversationHistory)
        {
            if (!conversationHistory.Any())
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var conversation in conversationHistory.TakeLast(5)) // Últimas 5 conversaciones
            {
                sb.AppendLine($"Usuario: {conversation.UserPrompt}");
                sb.AppendLine($"Asistente: {conversation.AiResponse}");
                sb.AppendLine("---");
            }

            return sb.ToString();
        }

        private async Task<string> ExecuteSQLQuery(string sqlQuery)
        {
            // Aquí se implementaría la ejecución real de la consulta SQL
            // Por ahora retornamos datos simulados
            await Task.Delay(100); // Simular latencia de BD
            return "Resultados simulados de la base de datos";
        }
    }
}
