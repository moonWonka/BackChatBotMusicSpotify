using MediatR;

namespace SpotifyMusicChatBot.API.Application.Query.GetModelIA
{
    public class GetModelIAHandler : IRequestHandler<GetModelIARequest, GetModelIAResponse>
    {
        public async Task<GetModelIAResponse> Handle(GetModelIARequest request, CancellationToken cancellationToken)
        {

            // Simular obtención de datos del usuario (caso exitoso)
            var response = new GetModelIAResponse
            {
                // StatusCode = 200 por defecto, no necesario especificar
                Message = "Información del usuario obtenida exitosamente",
                UserInfo = $"Usuario ID: {request.UserId}",
                MusicPreferences = "Preferencias musicales del usuario"
            };

            return await Task.FromResult(response);
        }
    }
}