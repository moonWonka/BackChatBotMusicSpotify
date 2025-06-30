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
        private readonly IChatBotRepository _chatRepository;

        private readonly ILogger<ProcessQuestionHandler> _logger;

        public ProcessQuestionHandler(
            IAIService aiService,
            IChatBotRepository repository,
            ILogger<ProcessQuestionHandler> logger)
        {
            _aiService = aiService;
            _chatRepository = repository;
            _logger = logger;
        }

        public async Task<ProcessQuestionResponse> Handle(ProcessQuestionRequest request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ProcessQuestionResponse response = new ProcessQuestionResponse
            {
                OriginalQuestion = request.Question,
                AIModelUsed = request.AIModel,
                Steps = new ProcessingSteps()
            };

            try
            {
                // Paso 1: Obtener historial para contextualización
                IList<ConversationTurn> conversationHistory = await GetConversationHistory(request.SessionId, request.ContextLimit);

                // Paso 2: Validación de la pregunta
                Stopwatch validationStopwatch = Stopwatch.StartNew();
                ValidationResult validationResult = await _aiService.ValidateQuestionAsync(
                    request.Question,
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

                // Paso 3: Contextualización de la pregunta
                Stopwatch contextStopwatch = Stopwatch.StartNew();
                ContextualizationResult contextualizationResult = await _aiService.ContextualizeQuestionAsync(
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

                // Paso 4: Generación de SQL
                Stopwatch sqlStopwatch = Stopwatch.StartNew();
                SQLGenerationResult sqlResult = await _aiService.GenerateSQLAsync(
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

                response.GeneratedSQL = sqlResult.GeneratedSQL.Trim();
                if (response.GeneratedSQL.ToLower().StartsWith("sql"))
                {
                    response.GeneratedSQL = response.GeneratedSQL.Substring(3).Trim();
                }

                // Paso 5: Ejecución de SQL
                Stopwatch executionStopwatch = Stopwatch.StartNew();
                string databaseResults = await ExecuteSQLQuery(response.GeneratedSQL);
                executionStopwatch.Stop();
                response.Steps.SQLExecutionTimeMs = executionStopwatch.ElapsedMilliseconds;

                response.DatabaseResults = databaseResults;

                // Paso 6: Generación de respuesta natural
                Stopwatch naturalResponseStopwatch = Stopwatch.StartNew();
                NaturalResponseResult naturalResult = await _aiService.GenerateNaturalResponseAsync(
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

        private async Task<IList<ConversationTurn>> GetConversationHistory(string firebaseUserId, int contextLimit)
        {
            try
            {
                // Si includeContext es false o contextLimit es 0, retorna lista vacía
                if (contextLimit <= 0) return [];

                IList<ConversationTurn> conversations = await _chatRepository.GetConversationBySessionIdAsync(firebaseUserId);
                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al obtener historial de conversación para sesión {firebase}", firebaseUserId);
                return [];
            }
        }

        private string FormatConversationHistory(IList<ConversationTurn> conversationHistory)
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
            try
            {
                return await _chatRepository.ExecuteRawSqlAsync(sqlQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando consulta SQL: {Message}", ex.Message);
                return $"Error ejecutando consulta SQL: {ex.Message}";
            }
        }
    }
}
