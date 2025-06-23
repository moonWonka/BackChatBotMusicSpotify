using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.Domain.Application.Model.Search;

namespace SpotifyMusicChatBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatBotRepository chatRepository, ILogger<ChatController> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        [HttpPost("conversation")]
        public async Task<IActionResult> SaveConversation([FromBody] SaveConversationRequest request)
        {
            try
            {
                string sessionId = request.SessionId ?? _chatRepository.GenerateSessionId();
                bool success = await _chatRepository.SaveConversationAsync(request.UserPrompt, request.AiResponse, sessionId);
                
                if (success)
                {
                    return Ok(new { SessionId = sessionId, Message = "Conversación guardada exitosamente" });
                }
                else
                {
                    return BadRequest("Error al guardar la conversación");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving conversation");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetAllConversations()
        {
            try
            {
                IEnumerable<ConversationSession> conversations = await _chatRepository.GetAllConversationsAsync();
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("conversation/{sessionId}")]
        public async Task<IActionResult> GetConversationBySessionId(string sessionId)
        {
            try
            {
                IEnumerable<ConversationTurn> conversation = await _chatRepository.GetConversationBySessionIdAsync(sessionId);
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation by session ID");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("conversation/{sessionId}/summary")]
        public async Task<IActionResult> GetSessionSummary(string sessionId)
        {
            try
            {
                SessionSummary? summary = await _chatRepository.GetSessionSummaryAsync(sessionId);
                if (summary != null)
                {
                    return Ok(summary);
                }
                else
                {
                    return NotFound("Sesión no encontrada");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session summary");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("conversation/{sessionId}")]
        public async Task<IActionResult> DeleteSession(string sessionId)
        {
            try
            {
                bool deleted = await _chatRepository.DeleteSessionAsync(sessionId);
                if (deleted)
                {
                    return Ok(new { Message = "Sesión eliminada exitosamente" });
                }
                else
                {
                    return NotFound("Sesión no encontrada");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchConversations([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest("El término de búsqueda es requerido");
                }

                IEnumerable<SearchResult> results = await _chatRepository.SearchConversationsAsync(searchTerm);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching conversations");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    // DTOs para las requests
    public class SaveConversationRequest
    {
        public string UserPrompt { get; set; } = string.Empty;
        public string AiResponse { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }
}
