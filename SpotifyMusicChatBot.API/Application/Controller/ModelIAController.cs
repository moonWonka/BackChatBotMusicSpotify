using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Query.GetModelIA;
using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ModelIAController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ModelIAController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene el perfil detallado del usuario incluyendo sus preferencias musicales y estadísticas.
        /// </summary>
        /// <param name="id">Identificador único del usuario.</param>
        /// <returns>
        /// Códigos de estado posibles:
        /// - 200: Información del perfil recuperada exitosamente
        /// - 400: Solicitud inválida o parámetros incorrectos
        /// - 401: Usuario no autenticado
        /// - 500: Error interno del servidor
        /// </returns>
        [HttpGet("{id}/info")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetModelIAResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]        public async Task<IActionResult> GetIUser([FromRoute] long id)
        {
            GetModelIARequest request = new() { UserId = id.ToString() };
            // Puedes obtener el UID del usuario autenticado así:
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Se envía la solicitud al handler a través de MediatR
            var response = await _mediator.Send(request);

            // Se devuelve la respuesta automáticamente basada en el StatusCode
            return response.ToActionResult();
        }
    }
}