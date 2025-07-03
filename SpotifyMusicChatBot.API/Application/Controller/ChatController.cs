using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Command.SaveConversation;
using SpotifyMusicChatBot.API.Application.Command.DeleteSession;
using SpotifyMusicChatBot.API.Application.Query.GetAllConversations;
using SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId;
using SpotifyMusicChatBot.API.Application.Query.GetSessionSummary;
using SpotifyMusicChatBot.API.Application.Query.SearchConversations;
using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using SpotifyMusicChatBot.API.Application.ViewModel.SearchConversations;
using SpotifyMusicChatBot.API.Application.ViewModel.Chat;
using SpotifyMusicChatBot.Domain.Application.Services;
using SpotifyMusicChatBot.Domain.Application.Repository;
using System.ComponentModel.DataAnnotations;
using SpotifyMusicChatBot.API.Application.ViewModel.Chat;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    /// <summary>
    /// Controlador para la gestión de conversaciones del ChatBot de Spotify
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChatController> _logger;
        private readonly IAIService _aiService;
        private readonly IChatBotRepository _repository;

        public ChatController(IMediator mediator, ILogger<ChatController> logger, IAIService aiService, IChatBotRepository repository)
        {
            _mediator = mediator;
            _logger = logger;
            _aiService = aiService;
            _repository = repository;
        }

        /// <summary>
        /// Procesa una pregunta del usuario aplicando filtros de términos excluidos
        /// </summary>
        /// <param name="request">Datos de la pregunta</param>
        /// <returns>Respuesta filtrada del ChatBot</returns>
        [HttpPost("ask-filtered")]
        public async Task<IActionResult> AskWithFilter([FromBody] FilteredChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Question))
                {
                    return BadRequest(new { Success = false, Message = "La pregunta es requerida" });
                }

                if (string.IsNullOrWhiteSpace(request.FirebaseUserId))
                {
                    return BadRequest(new { Success = false, Message = "FirebaseUserId es requerido" });
                }

                _logger.LogInformation("🎵 Procesando pregunta con filtros para usuario {UserId}: {Question}", 
                    request.FirebaseUserId, request.Question);

                // 1. Validar la pregunta
                var validation = await _aiService.ValidateQuestionAsync(request.Question);
                if (validation.ValidationStatus != "VALIDA")
                {
                    return Ok(new { 
                        Success = false, 
                        Message = validation.ValidationReason ?? "Pregunta no válida",
                        Type = "VALIDATION_ERROR",
                        ValidationStatus = validation.ValidationStatus
                    });
                }

                // 2. Generar SQL
                var sqlResult = await _aiService.GenerateSQLAsync(request.Question, request.ResultLimit);
                if (!sqlResult.IsSuccess)
                {
                    return Ok(new { 
                        Success = false, 
                        Message = "No se pudo generar una consulta para tu pregunta",
                        Type = "SQL_ERROR"
                    });
                }

                // 3. Ejecutar consulta
                var dbResults = await _repository.ExecuteRawSqlAsync(sqlResult.GeneratedSQL);

                // 4. Generar respuesta natural CON FILTRADO
                var naturalResponse = await _aiService.GenerateFilteredNaturalResponseAsync(
                    request.Question, 
                    dbResults, 
                    request.FirebaseUserId, 
                    request.Tone ?? "casual");

                if (!naturalResponse.IsSuccess)
                {
                    return Ok(new { 
                        Success = false, 
                        Message = "No se pudo generar una respuesta",
                        Type = "RESPONSE_ERROR"
                    });
                }

                // 5. Guardar la conversación (opcional)
                if (!string.IsNullOrWhiteSpace(request.SessionId))
                {
                    await _repository.SaveConversationAsync(
                        request.Question, 
                        naturalResponse.NaturalResponse, 
                        request.SessionId, 
                        request.FirebaseUserId);
                }

                _logger.LogInformation("✅ Respuesta filtrada generada exitosamente para usuario {UserId}", 
                    request.FirebaseUserId);

                return Ok(new {
                    Success = true,
                    Response = naturalResponse.NaturalResponse,
                    SessionId = request.SessionId ?? _repository.GenerateSessionId(),
                    Type = "FILTERED_RESPONSE"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error procesando pregunta filtrada para usuario {UserId}", request.FirebaseUserId);
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Guarda una nueva conversación entre el usuario y la IA
        /// </summary>
        /// <param name="request">Datos de la conversación</param>
        /// <returns>Confirmación de guardado con el ID de sesión</returns>
        /// <response code="200">Conversación guardada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("conversation")]
        [ProducesResponseType(typeof(SaveConversationResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> SaveConversation([FromBody] SaveConversationRequest request)
        {
            SaveConversationResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Obtiene todas las sesiones de conversación de un usuario específico
        /// </summary>
        /// <param name="firebaseUserId">ID del usuario de Firebase</param>
        /// <returns>Lista de todas las sesiones del usuario con información básica</returns>
        /// <response code="200">Conversaciones obtenidas exitosamente</response>
        /// <response code="400">ID de usuario inválido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("conversations/{firebaseUserId}")]
        [ProducesResponseType(typeof(GetAllConversationsResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> GetAllConversations([FromRoute] string firebaseUserId)
        {
            var request = new GetAllConversationsRequest { FirebaseUserId = firebaseUserId };
            GetAllConversationsResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Obtiene todos los turnos de una conversación específica
        /// </summary>
        /// <param name="sessionId">ID de la sesión</param>
        /// <returns>Lista completa de turnos de la conversación</returns>
        /// <response code="200">Conversación obtenida exitosamente</response>
        /// <response code="404">Sesión no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("conversation/{sessionId}")]
        [ProducesResponseType(typeof(GetConversationBySessionIdResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> GetConversationBySessionId(GetConversationBySessionIdRequest request)
        {
            GetConversationBySessionIdResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Obtiene un resumen estadístico de una sesión específica
        /// </summary>
        /// <param name="sessionId">ID de la sesión</param>
        /// <returns>Resumen estadístico de la sesión</returns>
        /// <response code="200">Resumen obtenido exitosamente</response>
        /// <response code="404">Sesión no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("conversation/{sessionId}/summary")]
        [ProducesResponseType(typeof(GetSessionSummaryResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> GetSessionSummary(GetSessionSummaryRequest request)
        {
            GetSessionSummaryResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Elimina una sesión completa de conversación
        /// </summary>
        /// <param name="sessionId">ID de la sesión a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Sesión eliminada exitosamente</response>
        /// <response code="404">Sesión no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("conversation/{sessionId}")]
        [ProducesResponseType(typeof(DeleteSessionResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> DeleteSession(DeleteSessionRequest request)
        {
            DeleteSessionResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Busca conversaciones que contengan un término específico
        /// </summary>
        /// <param name="searchRequest">Datos de la búsqueda</param>
        /// <returns>Lista de conversaciones que coinciden con la búsqueda</returns>
        /// <response code="200">Búsqueda realizada exitosamente</response>
        /// <response code="400">Parámetros de búsqueda inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Query.SearchConversations.SearchConversationsResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> SearchConversations(SearchConversationsRequest searchRequest)
        {

            SearchConversationsResponse response = await _mediator.Send(searchRequest);
            return response.ToActionResult();
        }

    }
}

