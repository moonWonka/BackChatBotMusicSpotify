using MediatR;
using System.Net;

namespace SpotifyMusicChatBot.API.Application.Query.GetModelIA
{
    public class GetModelIAHandler : IRequestHandler<GetModelIARequest, GetModelIAResponse>
    {
        public async Task<GetModelIAResponse> Handle(GetModelIARequest request, CancellationToken cancellationToken)
        {
            // Aquí iría tu lógica de negocio
            // Por ahora devolvemos una respuesta de ejemplo

            if (string.IsNullOrEmpty(request.UserId))
            {
                return new GetModelIAResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "UserId es requerido",
                    Error = "El parámetro UserId no puede estar vacío",
                    UserInfo = string.Empty,
                    MusicPreferences = string.Empty
                };
            }

            // Simular obtención de datos del usuario
            var response = new GetModelIAResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Información del usuario obtenida exitosamente",
                Error = null,
                UserInfo = $"Usuario ID: {request.UserId}",
                MusicPreferences = "Preferencias musicales del usuario"
            };

            return await Task.FromResult(response);
        }
    }
}