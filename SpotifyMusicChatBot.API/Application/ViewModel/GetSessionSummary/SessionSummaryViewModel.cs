using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.ViewModel.GetSessionSummary
{
    /// <summary>
    /// Modelo de vista para el resumen de una sesión de conversación
    /// </summary>
    public class SessionSummaryViewModel
    {
        /// <summary>
        /// Número total de turnos en la sesión
        /// </summary>
        [Required]
        public int TotalTurns { get; set; }

        /// <summary>
        /// Fecha y hora de inicio de la sesión
        /// </summary>
        [Required]
        public DateTime SessionStart { get; set; }

        /// <summary>
        /// Fecha y hora de fin de la sesión
        /// </summary>
        [Required]
        public DateTime SessionEnd { get; set; }

        /// <summary>
        /// Primer prompt de la sesión
        /// </summary>
        [Required]
        public string FirstPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Duración de la sesión en minutos
        /// </summary>
        public double DurationMinutes { get; set; }

        /// <summary>
        /// Duración formateada como texto legible
        /// </summary>
        public string DurationFormatted { get; set; } = string.Empty;
    }
}
