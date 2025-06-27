using SpotifyMusicChatBot.API.Application.Query.GetSessionSummary;
using SpotifyMusicChatBot.API.Application.ViewModel.GetSessionSummary;
using SpotifyMusicChatBot.Domain.Application.Model.Conversation;

namespace SpotifyMusicChatBot.API.Application.Mappers
{
    /// <summary>
    /// Mapper para conversiones de GetSessionSummary entre capas
    /// </summary>
    public static class GetSessionSummaryMapper
    {
        /// <summary>
        /// Convierte SessionSummary de Domain a ViewModel
        /// </summary>
        public static SessionSummaryViewModel ToViewModel(SessionSummary domainModel)
        {
            var durationMinutes = domainModel.SessionEnd != domainModel.SessionStart 
                ? (domainModel.SessionEnd - domainModel.SessionStart).TotalMinutes 
                : 0;

            return new SessionSummaryViewModel
            {
                TotalTurns = domainModel.TotalTurns,
                SessionStart = domainModel.SessionStart,
                SessionEnd = domainModel.SessionEnd,
                FirstPrompt = domainModel.FirstPrompt,
                DurationMinutes = Math.Round(durationMinutes, 2),
                DurationFormatted = FormatDuration(durationMinutes)
            };
        }

        /// <summary>
        /// Crea una respuesta exitosa con el resumen
        /// </summary>
        public static GetSessionSummaryResponse ToSuccessResponse(SessionSummary summary)
        {
            return new GetSessionSummaryResponse
            {
                Summary = ToViewModel(summary),
                StatusCode = 200,
                Message = "Resumen de sesión obtenido exitosamente"
            };
        }

        /// <summary>
        /// Crea una respuesta cuando no se encuentra la sesión
        /// </summary>
        public static GetSessionSummaryResponse ToNotFoundResponse(string sessionId)
        {
            return new GetSessionSummaryResponse
            {
                Summary = null,
                StatusCode = 404,
                Message = $"Sesión '{sessionId}' no encontrada"
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static GetSessionSummaryResponse ToErrorResponse(
            int statusCode,
            string message)
        {
            return new GetSessionSummaryResponse
            {
                Summary = null,
                StatusCode = statusCode,
                Message = message
            };
        }

        /// <summary>
        /// Formatea la duración en un texto legible
        /// </summary>
        private static string FormatDuration(double totalMinutes)
        {
            if (totalMinutes < 1)
                return "Menos de 1 minuto";
            
            if (totalMinutes < 60)
                return $"{Math.Round(totalMinutes)} minutos";
            
            var hours = Math.Floor(totalMinutes / 60);
            var minutes = Math.Round(totalMinutes % 60);
            
            return minutes > 0 
                ? $"{hours} horas y {minutes} minutos"
                : $"{hours} horas";
        }
    }
}
