using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Command.SaveConversation;
using SpotifyMusicChatBot.API.Application.Command.DeleteSession;
using SpotifyMusicChatBot.API.Application.Query.GetAllConversations;
using SpotifyMusicChatBot.API.Application.Query.GetConversationBySessionId;
using SpotifyMusicChatBot.API.Application.Query.GetSessionSummary;
using SpotifyMusicChatBot.API.Application.Query.SearchConversations;
using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IMediator mediator, ILogger<ChatController> logger)
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
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving conversation");
                return BaseResponse.InternalServerError("Error interno del servidor").ToActionResult();
            }
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetAllConversations()
        {
            try
            {
                GetAllConversationsResponse response = await _mediator.Send(new GetAllConversationsRequest());
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations");
                return BaseResponse.InternalServerError("Error interno del servidor").ToActionResult();
            }
        }

        [HttpGet("conversation/{sessionId}")]
        public async Task<IActionResult> GetConversationBySessionId(string sessionId)
        {
            try
            {
                GetConversationBySessionIdResponse response = await _mediator.Send(new GetConversationBySessionIdRequest { SessionId = sessionId });
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation by session ID");
                return BaseResponse.InternalServerError("Error interno del servidor").ToActionResult();
            }
        }

        [HttpGet("conversation/{sessionId}/summary")]
        public async Task<IActionResult> GetSessionSummary(string sessionId)
        {
            try
            {
                GetSessionSummaryResponse response = await _mediator.Send(new GetSessionSummaryRequest { SessionId = sessionId });
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session summary");
                return BaseResponse.InternalServerError("Error interno del servidor").ToActionResult();
            }
        }

        [HttpDelete("conversation/{sessionId}")]
        public async Task<IActionResult> DeleteSession(string sessionId)
        {
            try
            {
                DeleteSessionResponse response = await _mediator.Send(new DeleteSessionRequest { SessionId = sessionId });
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session");
                return BaseResponse.InternalServerError("Error interno del servidor").ToActionResult();
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchConversations([FromQuery] string searchTerm)
        {
            try
            {
                SearchConversationsResponse response = await _mediator.Send(new SearchConversationsRequest { SearchTerm = searchTerm });
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching conversations");
                return BaseResponse.InternalServerError("Error interno del servidor").ToActionResult();
            }
        }
    }
}
