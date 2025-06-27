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
using System.ComponentModel.DataAnnotations;

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

        public ChatController(IMediator mediator, ILogger<ChatController> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
        /// Obtiene todas las sesiones de conversación disponibles
        /// </summary>
        /// <returns>Lista de todas las sesiones con información básica</returns>
        /// <response code="200">Conversaciones obtenidas exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("conversations")]
        [ProducesResponseType(typeof(GetAllConversationsResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> GetAllConversations()
        {
            GetAllConversationsResponse response = await _mediator.Send(new GetAllConversationsRequest());
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

