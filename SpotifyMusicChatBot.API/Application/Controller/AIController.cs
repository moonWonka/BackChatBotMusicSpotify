using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Command.AI.ProcessQuestion;
using SpotifyMusicChatBot.API.Application.Command.AI.ValidateQuestion;
using SpotifyMusicChatBot.API.Application.Command.AI.ContextualizeQuestion;
using SpotifyMusicChatBot.API.Application.Command.AI.GenerateSQL;
using SpotifyMusicChatBot.API.Application.Command.AI.GenerateNaturalResponse;
using SpotifyMusicChatBot.API.Application.ViewModel.Common;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    /// <summary>
    /// Controlador para la interacción con modelos de IA (Gemini, Anthropic)
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

        /// <summary>
        /// Contextualiza una pregunta del usuario basándose en el historial de conversación
        /// </summary>
        /// <param name="request">Pregunta y historial de conversación</param>
        /// <returns>Pregunta contextualizada o independiente</returns>
        /// <response code="200">Pregunta contextualizada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("contextualize-question")]
        [ProducesResponseType(typeof(ContextualizeQuestionResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> ContextualizeQuestion([FromBody] ContextualizeQuestionRequest request)
        {
            ContextualizeQuestionResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Valida si una pregunta es relevante para el asistente musical
        /// </summary>
        /// <param name="request">Pregunta a validar</param>
        /// <returns>Resultado de la validación</returns>
        /// <response code="200">Pregunta validada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="422">Pregunta fuera de contexto o requiere aclaración</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("validate-question")]
        [ProducesResponseType(typeof(ValidateQuestionResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 422)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> ValidateQuestion([FromBody] ValidateQuestionRequest request)
        {
            ValidateQuestionResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Genera una consulta SQL a partir de una pregunta en lenguaje natural
        /// </summary>
        /// <param name="request">Pregunta validada</param>
        /// <returns>Consulta SQL generada</returns>
        /// <response code="200">SQL generado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="422">No es posible generar SQL para esta consulta</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("generate-sql")]
        [ProducesResponseType(typeof(GenerateSQLResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 422)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> GenerateSQL([FromBody] GenerateSQLRequest request)
        {
            GenerateSQLResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Genera una respuesta en lenguaje natural a partir de los resultados de la base de datos
        /// </summary>
        /// <param name="request">Pregunta original y resultados de la base de datos</param>
        /// <returns>Respuesta en lenguaje natural</returns>
        /// <response code="200">Respuesta generada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("generate-natural-response")]
        [ProducesResponseType(typeof(GenerateNaturalResponseResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public async Task<IActionResult> GenerateNaturalResponse([FromBody] GenerateNaturalResponseRequest request)
        {
            GenerateNaturalResponseResponse response = await _mediator.Send(request);
            return response.ToActionResult();
        }
    }
}
