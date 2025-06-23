using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Command.SaveConversation;
using SpotifyMusicChatBot.API.Application.Command.DeleteSession;
using SpotifyMusicChatBot.API.Application.Query.GetAllConversations;
using SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId;
using SpotifyMusicChatBot.API.Application.Query.GetSessionSummary;
using SpotifyMusicChatBot.API.Application.Query.SearchConversations;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChatController> _logger;        public ChatController(IMediator mediator, ILogger<ChatController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("conversation")]
        public async Task<IActionResult> SaveConversation([FromBody] SaveConversationRequest request)
        {
            try
            {
                SaveConversationResponse response = await _mediator.Send(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
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
                GetAllConversationsRequest request = new GetAllConversationsRequest();
                GetAllConversationsResponse response = await _mediator.Send(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
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
                GetConversationBySessionIdRequest request = new GetConversationBySessionIdRequest { SessionId = sessionId };
                GetConversationBySessionIdResponse response = await _mediator.Send(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
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
                GetSessionSummaryRequest request = new GetSessionSummaryRequest { SessionId = sessionId };
                GetSessionSummaryResponse response = await _mediator.Send(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else if (response.Message == "Sesión no encontrada")
                {
                    return NotFound(response);
                }
                else
                {
                    return BadRequest(response);
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
                DeleteSessionRequest request = new DeleteSessionRequest { SessionId = sessionId };
                DeleteSessionResponse response = await _mediator.Send(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else if (response.Message == "Sesión no encontrada")
                {
                    return NotFound(response);
                }
                else
                {
                    return BadRequest(response);
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
                SearchConversationsRequest request = new SearchConversationsRequest { SearchTerm = searchTerm };
                SearchConversationsResponse response = await _mediator.Send(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching conversations");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
