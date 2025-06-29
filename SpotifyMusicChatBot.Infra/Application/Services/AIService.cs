using SpotifyMusicChatBot.Domain.Application.Services;
using Microsoft.Extensions.Logging;
using SpotifyMusicChatBot.Domain.Application.Services.Prompts;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    /// <summary>
    /// Implementación del servicio de IA (pendiente de implementación real)
    /// </summary>
    public class AIService : IAIService
    {
        private readonly ILogger<AIService> _logger;
        private readonly IGeminiIAService _geminiIAService;
        private readonly IAnthropicIAService _anthropicIAService;

        public AIService(ILogger<AIService> logger, IGeminiIAService geminiIAService, IAnthropicIAService anthropicIAService)
        {
            _geminiIAService = geminiIAService;
            _anthropicIAService = anthropicIAService;
            _logger = logger;
        }

        public async Task<ContextualizationResult> ContextualizeQuestionAsync(string question, string conversationHistory, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            // Si no hay historial, no se requiere contextualización
            if (string.IsNullOrWhiteSpace(conversationHistory))
            {
                return new ContextualizationResult
                {
                    ContextualizedQuestion = question,
                    WasContextualized = false,
                    IsSuccess = true,
                    Message = "No se requirió contextualización"
                };
            }

            // Construir el prompt usando la plantilla definida en AIPrompts
            string prompt = AIPrompts.PROMPT_CONTEXTO_CONVERSACIONAL
                .Replace("{historial_conversacion}", conversationHistory)
                .Replace("{pregunta_actual}", question);

            // Ejecutar el prompt en el modelo seleccionado
            var response = await ExecutePromptAsync(prompt, modelName, cancellationToken: cancellationToken);

            // Procesar la respuesta del modelo
            string contextualizedQuestion;
            bool wasContextualized;
            if (response.Content.StartsWith("INDEPENDIENTE:", StringComparison.OrdinalIgnoreCase))
            {
                contextualizedQuestion = question;
                wasContextualized = false;
            }
            else if (response.Content.StartsWith("CONTEXTUALIZADA:", StringComparison.OrdinalIgnoreCase))
            {
                contextualizedQuestion = response.Content.Substring("CONTEXTUALIZADA:".Length).Trim();
                wasContextualized = true;
            }
            else
            {
                // Si el modelo responde en un formato inesperado, usar la respuesta tal cual
                contextualizedQuestion = response.Content.Trim();
                wasContextualized = true;
            }

            return new ContextualizationResult
            {
                ContextualizedQuestion = contextualizedQuestion,
                WasContextualized = wasContextualized,
                IsSuccess = true,
                Message = wasContextualized ? "Pregunta contextualizada correctamente" : "Pregunta independiente, no se requirió contextualización"
            };
        }

        public async Task<ValidationResult> ValidateQuestionAsync(string question, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            // Construir el prompt usando la plantilla y la pregunta del usuario
            string prompt = AIPrompts.PROMPT_VALIDAR_PREGUNTA.Replace("{pregunta}", question);
            AIModelResponse response = await ExecutePromptAsync(prompt, modelName, cancellationToken: cancellationToken);

            string respuesta = response.Content.Trim().ToUpperInvariant();
            bool esValida = respuesta == "VALIDA" || respuesta.StartsWith("VALIDA");
            bool necesitaAclarar = respuesta.StartsWith("ACLARAR");
            bool fueraContexto = respuesta.StartsWith("FUERA_CONTEXTO");

            return new ValidationResult
            {
                ValidationStatus = esValida ? "VALIDA" : (necesitaAclarar ? "ACLARAR" : (fueraContexto ? "FUERA_CONTEXTO" : "DESCONOCIDO")),
                ValidationReason = necesitaAclarar ? respuesta : (esValida ? "Pregunta válida" : (fueraContexto ? "Pregunta fuera de contexto musical" : "No se pudo determinar el estado de la pregunta")),
                IdentifiedCategory = esValida ? "Musical" : (fueraContexto ? "Fuera de contexto" : "Ambigua"),
                IsSuccess = esValida,
                ModelUsed = modelName
            };
        }

        public async Task<SQLGenerationResult> GenerateSQLAsync(string question, int resultLimit = 50, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            // Construir el prompt usando la plantilla y la pregunta del usuario
            string prompt = AIPrompts.PROMPT_GENERAR_SQL
                .Replace("{pregunta}", question)
                .Replace("{limite_resultados}", resultLimit.ToString());

            AIModelResponse response = await ExecutePromptAsync(prompt, modelName, cancellationToken: cancellationToken);

            // El modelo debe responder solo con la sentencia SQL o el mensaje de validación
            string sql = response.Content.Trim();
            bool isSuccess = !sql.StartsWith("No es posible responder", StringComparison.OrdinalIgnoreCase);

            return new SQLGenerationResult
            {
                GeneratedSQL = sql,
                IsSuccess = isSuccess,
                Message = isSuccess ? "Consulta SQL generada correctamente" : sql
            };
        }

        public async Task<NaturalResponseResult> GenerateNaturalResponseAsync(string question, string databaseResults, string tone = "casual", string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            bool hasResults = !string.IsNullOrWhiteSpace(databaseResults) && databaseResults.Trim() != "[]" && databaseResults.Trim() != "{}";
            string prompt;
            if (hasResults)
            {
                prompt = AIPrompts.PROMPT_RESPUESTA_NATURAL
                    .Replace("{pregunta}", question)
                    .Replace("{resultados_db}", databaseResults)
                    .Replace("{tono}", tone);
            }
            else
            {
                // Si no hay resultados, el prompt debe dejarlo claro
                prompt = $"No hay resultados en la base de datos para la pregunta: '{question}'. Responde de forma natural y amable, tono: {tone}.";
            }

            AIModelResponse response = await ExecutePromptAsync(prompt, modelName, cancellationToken: cancellationToken);
            string naturalResponse = response.Content.Trim();

            return new NaturalResponseResult
            {
                NaturalResponse = naturalResponse,
                IsSuccess = !string.IsNullOrWhiteSpace(naturalResponse),
                Message = string.IsNullOrWhiteSpace(naturalResponse) ? "No se pudo generar una respuesta natural" : "Respuesta generada correctamente"
            };
        }

        public async Task<AnalysisResult> AnalyzeAndImproveResponseAsync(string question, string response, string modelName = "Gemini", CancellationToken cancellationToken = default)
        {
            // TODO: Implementar lógica real
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            // TODO: Implementar lógica real
            return new List<string>();
        }

        public async Task<bool> IsModelAvailableAsync(string modelName)
        {
            // TODO: Implementar lógica real
            return false;
        }

        private async Task<AIModelResponse> ExecutePromptAsync(string prompt, string modelName = "Gemini", float temperature = 0.7f, int maxTokens = 1000, CancellationToken cancellationToken = default)
        {
            switch (modelName.ToLowerInvariant())
            {
                case "gemini":
                    return await _geminiIAService.ExecuteModelAsync(prompt, temperature, maxTokens, cancellationToken);
                case "anthropic":
                    return await _anthropicIAService.ExecuteModelAsync(prompt, temperature, maxTokens, cancellationToken);
                default:
                    throw new NotSupportedException($"Modelo de IA no soportado: {modelName}");
            }
        }
    }
}
