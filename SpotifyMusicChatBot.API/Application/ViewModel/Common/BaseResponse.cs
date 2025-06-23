using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SpotifyMusicChatBot.API.Application.ViewModel.Common
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "-";
        public string Error { get; set; } = "-";        // Constructor por defecto (StatusCode = 200)
        public BaseResponse() { }        // Constructor para casos exitosos con mensaje
        public BaseResponse(string message)
        {
            StatusCode = 200;
            Message = message;
        }        // Constructor para casos de error con status code específico
        public BaseResponse(int statusCode, string message = "-", string error = "-")
        {
            StatusCode = statusCode;
            Message = message;
            Error = error;
        }

        // Métodos helper para respuestas comunes
        public static BaseResponse Ok(string message = "Operación exitosa")
        {
            return new BaseResponse(message);
        }

        public static BaseResponse BadRequest(string message = "Solicitud inválida")
        {
            return new BaseResponse(400, message);
        }

        public static BaseResponse NotFound(string message = "Recurso no encontrado")
        {
            return new BaseResponse(404, message);
        }

        public static BaseResponse InternalServerError(string message = "Error interno del servidor")
        {
            return new BaseResponse(500, message);
        }

        /// <summary>
        /// Convierte automáticamente la respuesta a IActionResult basado en el StatusCode
        /// </summary>
        public IActionResult ToActionResult()
        {
            return StatusCode switch
            {
                (int)HttpStatusCode.OK => new OkObjectResult(this),
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(this),
                (int)HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(this),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(this),
                (int)HttpStatusCode.InternalServerError => new ObjectResult(this) { StatusCode = 500 },
                _ => new ObjectResult(this) { StatusCode = StatusCode }
            };
        }
    }
}