using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Command.AI.ProcessQuestion;
using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    /// <summary>
    /// Controlador para el procesamiento de preguntas musicales con IA (Gemini)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AIController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AIController> _logger;

        public AIController(IMediator mediator, ILogger<AIController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Procesa una pregunta completa del usuario siguiendo todo el flujo de IA
        /// </summary>
        /// <param name="request">Datos de la pregunta y contexto</param>
        /// <returns>Respuesta procesada por la IA</returns>
        /// <response code="200">Pregunta procesada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="422">Pregunta fuera de contexto o requiere aclaración</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("process-question")]
        [ProducesResponseType(typeof(ProcessQuestionResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 422)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> ProcessQuestion([FromBody] ProcessQuestionRequest request)
        {
            ProcessQuestionResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }
    }
}
