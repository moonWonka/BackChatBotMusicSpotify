using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Services;
using System.Diagnostics;
using System.Text;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ContextualizeQuestion
{
    /// <summary>
    /// Handler para contextualizar preguntas basándose en el historial de conversación
    /// </summary>
    public class ContextualizeQuestionHandler : IRequestHandler<ContextualizeQuestionRequest, ContextualizeQuestionResponse>
    {
        private readonly IChatBotRepository _repository;
        private readonly IAIService _aiService;
        private readonly ILogger<ContextualizeQuestionHandler> _logger;

        public ContextualizeQuestionHandler(
            IChatBotRepository repository,
            IAIService aiService,
            ILogger<ContextualizeQuestionHandler> logger)
        {
            _repository = repository;
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<ContextualizeQuestionResponse> Handle(ContextualizeQuestionRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ContextualizeQuestionResponse
            {
                OriginalQuestion = request.Question,
                AIModelUsed = request.AIModel
            };

            try
            {
                var conversationHistory = new List<ConversationContext>();

                // Obtener historial de conversación si se solicita
                if (request.IncludeContext)
                {
                    conversationHistory = await GetConversationHistory(request.SessionId, request.ContextLimit);
                    response.ConversationHistory = conversationHistory;
                }

                // Analizar si la pregunta necesita contextualización
                var contextualizationResult = await _aiService.ContextualizeQuestionAsync(
                    request.Question, 
                    FormatConversationHistory(conversationHistory), 
                    request.AIModel,
                    cancellationToken);

                if (!contextualizationResult.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = contextualizationResult.Message ?? "Error al contextualizar la pregunta";
                    return response;
                }

                response.WasContextualized = contextualizationResult.WasContextualized;
                response.AnalysisType = contextualizationResult.AnalysisType;
                response.ContextualizedQuestion = contextualizationResult.ContextualizedQuestion;

                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = true;
                response.Message = "Pregunta contextualizada exitosamente";

                _logger.LogInformation("Pregunta contextualizada para sesión {SessionId}. Tipo: {AnalysisType}, Tiempo: {ElapsedMs}ms", 
                    request.SessionId, response.AnalysisType, response.ProcessingTimeMs);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = false;
                response.Message = "Error al contextualizar la pregunta";

                _logger.LogError(ex, "Error al contextualizar pregunta para sesión {SessionId}", request.SessionId);
                return response;
            }
        }

        private async Task<List<ConversationContext>> GetConversationHistory(string sessionId, int limit)
        {
            try
            {
                // Aquí se obtendría el historial real de la base de datos
                // Por ahora simulamos algunos datos
                await Task.Delay(50); // Simular consulta a BD

                return new List<ConversationContext>
                {
                    new ConversationContext
                    {
                        Turn = 1,
                        MessageType = "User",
                        Content = "¿Quién es Bad Bunny?",
                        Timestamp = DateTime.Now.AddMinutes(-10)
                    },
                    new ConversationContext
                    {
                        Turn = 2,
                        MessageType = "Assistant",
                        Content = "Bad Bunny es un cantante y rapero puertorriqueño muy popular en el género del reggaetón...",
                        Timestamp = DateTime.Now.AddMinutes(-9)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial para sesión {SessionId}", sessionId);
                return new List<ConversationContext>();
            }
        }

        private string FormatConversationHistory(List<ConversationContext> history)
        {
            if (!history.Any())
                return "No hay historial de conversación previo.";

            var formattedHistory = new StringBuilder();
            
            foreach (var turn in history.OrderBy(h => h.Turn))
            {
                formattedHistory.AppendLine($"{turn.MessageType}: {turn.Content}");
            }
            
            return formattedHistory.ToString();
        }

        private async Task<string> AnalyzeAndContextualizeQuestion(string question, List<ConversationContext> history, string aiModel)
        {
            try
            {
                // Simular el prompt de contextualización usando las constantes
                var prompt = AIPrompts.PROMPT_CONTEXTO_CONVERSACIONAL
                    .Replace("{historial_conversacion}", FormatConversationHistory(history))
                    .Replace("{pregunta_actual}", question);
                
                // Aquí se llamaría al modelo de IA real (Gemini o Anthropic)
                // Por ahora simulamos la respuesta
                await Task.Delay(200); // Simular latencia de IA

                // Lógica simple para determinar si necesita contexto
                var needsContext = question.Contains("sus ") || question.Contains("él ") || 
                                 question.Contains("ella ") || question.Contains("esto ") ||
                                 question.Contains("eso ") || question.Contains("aquello ");

                if (needsContext && history.Any())
                {
                    return $"CONTEXTUALIZADA: {question}";
                }
                else
                {
                    return $"INDEPENDIENTE: {question}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al analizar pregunta con IA");
                return $"INDEPENDIENTE: {question}"; // Fallback
            }
        }
    }
}
