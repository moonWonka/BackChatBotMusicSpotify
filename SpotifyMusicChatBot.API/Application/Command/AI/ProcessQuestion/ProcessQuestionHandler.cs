using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using System.Diagnostics;

namespace SpotifyMusicChatBot.API.Application.Command.AI.ProcessQuestion
{
    /// <summary>
    /// Handler para procesar preguntas completas siguiendo el flujo de IA
    /// </summary>
    public class ProcessQuestionHandler : IRequestHandler<ProcessQuestionRequest, ProcessQuestionResponse>
    {
        private readonly IMediator _mediator;
        private readonly IChatBotRepository _repository;
        private readonly ILogger<ProcessQuestionHandler> _logger;

        public ProcessQuestionHandler(
            IMediator mediator,
            IChatBotRepository repository,
            ILogger<ProcessQuestionHandler> logger)
        {
            _mediator = mediator;
            _repository = repository;
            _logger = logger;
        }

        public async Task<ProcessQuestionResponse> Handle(ProcessQuestionRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ProcessQuestionResponse
            {
                OriginalQuestion = request.Question,
                AIModelUsed = request.AIModel
            };

            try
            {
                // Paso 1: Contextualización de la pregunta
                var contextStopwatch = Stopwatch.StartNew();
                var contextualizeRequest = new ContextualizeQuestion.ContextualizeQuestionRequest
                {
                    SessionId = request.SessionId,
                    Question = request.Question,
                    AIModel = request.AIModel,
                    IncludeContext = request.IncludeContext,
                    ContextLimit = request.ContextLimit
                };

                var contextualizeResponse = await _mediator.Send(contextualizeRequest, cancellationToken);
                contextStopwatch.Stop();
                response.Steps.ContextualizationTimeMs = contextStopwatch.ElapsedMilliseconds;

                if (!contextualizeResponse.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = contextualizeResponse.Message;
                    return response;
                }

                response.ContextualizedQuestion = contextualizeResponse.ContextualizedQuestion;
                response.WasContextualized = contextualizeResponse.WasContextualized;

                // Paso 2: Validación de la pregunta
                var validationStopwatch = Stopwatch.StartNew();
                var validateRequest = new ValidateQuestion.ValidateQuestionRequest
                {
                    Question = response.ContextualizedQuestion,
                    AIModel = request.AIModel
                };

                var validateResponse = await _mediator.Send(validateRequest, cancellationToken);
                validationStopwatch.Stop();
                response.Steps.ValidationTimeMs = validationStopwatch.ElapsedMilliseconds;

                response.ValidationStatus = validateResponse.ValidationStatus;

                if (validateResponse.ValidationStatus != "VALIDA")
                {
                    response.IsSuccess = false;
                    response.ClarificationMessage = validateResponse.Message;
                    
                    if (validateResponse.ValidationStatus == "FUERA_CONTEXTO")
                    {
                        response.Message = "La pregunta está fuera del contexto del asistente musical.";
                    }
                    else if (validateResponse.ValidationStatus.StartsWith("ACLARAR"))
                    {
                        response.Message = "La pregunta requiere aclaración.";
                    }
                    
                    return response;
                }

                // Paso 3: Generación de SQL
                var sqlStopwatch = Stopwatch.StartNew();
                var generateSQLRequest = new GenerateSQL.GenerateSQLRequest
                {
                    Question = response.ContextualizedQuestion,
                    AIModel = request.AIModel
                };

                var generateSQLResponse = await _mediator.Send(generateSQLRequest, cancellationToken);
                sqlStopwatch.Stop();
                response.Steps.SQLGenerationTimeMs = sqlStopwatch.ElapsedMilliseconds;

                if (!generateSQLResponse.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = generateSQLResponse.Message;
                    return response;
                }

                response.GeneratedSQL = generateSQLResponse.GeneratedSQL;

                // Paso 4: Ejecución de SQL
                var executionStopwatch = Stopwatch.StartNew();
                // Aquí se ejecutaría la consulta SQL contra la base de datos
                // Por ahora simulamos la ejecución
                var databaseResults = await ExecuteSQLQuery(response.GeneratedSQL);
                executionStopwatch.Stop();
                response.Steps.SQLExecutionTimeMs = executionStopwatch.ElapsedMilliseconds;

                response.DatabaseResults = databaseResults;

                // Paso 5: Generación de respuesta natural
                var naturalResponseStopwatch = Stopwatch.StartNew();
                var generateNaturalRequest = new GenerateNaturalResponse.GenerateNaturalResponseRequest
                {
                    Question = response.ContextualizedQuestion,
                    DatabaseResults = databaseResults,
                    AIModel = request.AIModel
                };

                var generateNaturalResponse = await _mediator.Send(generateNaturalRequest, cancellationToken);
                naturalResponseStopwatch.Stop();
                response.Steps.NaturalResponseTimeMs = naturalResponseStopwatch.ElapsedMilliseconds;

                if (!generateNaturalResponse.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = generateNaturalResponse.Message;
                    return response;
                }

                response.NaturalResponse = generateNaturalResponse.NaturalResponse;

                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = true;
                response.Message = "Pregunta procesada exitosamente";

                _logger.LogInformation("Pregunta procesada exitosamente para sesión {SessionId} en {ElapsedMs}ms", 
                    request.SessionId, response.ProcessingTimeMs);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = false;
                response.Message = "Error interno al procesar la pregunta";

                _logger.LogError(ex, "Error al procesar pregunta para sesión {SessionId}", request.SessionId);
                return response;
            }
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
