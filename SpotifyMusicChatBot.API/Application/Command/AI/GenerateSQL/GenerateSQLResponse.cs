using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Command.AI.GenerateSQL
{
    /// <summary>
    /// Respuesta de la generación de consulta SQL
    /// </summary>
    public class GenerateSQLResponse : BaseResponse
    {
        /// <summary>
        /// Pregunta original en lenguaje natural
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Consulta SQL generada
        /// </summary>
        public string GeneratedSQL { get; set; } = string.Empty;

        /// <summary>
        /// Consulta SQL optimizada (si aplicó alguna optimización)
        /// </summary>
        public string? OptimizedSQL { get; set; }

        /// <summary>
        /// Explicación de la consulta generada
        /// </summary>
        public string SQLExplanation { get; set; } = string.Empty;

        /// <summary>
        /// Tablas involucradas en la consulta
        /// </summary>
        public List<string> TablesUsed { get; set; } = new List<string>();

        /// <summary>
        /// Campos seleccionados en la consulta
        /// </summary>
        public List<string> FieldsSelected { get; set; } = new List<string>();

        /// <summary>
        /// Condiciones WHERE aplicadas
        /// </summary>
        public List<string> WhereConditions { get; set; } = new List<string>();

        /// <summary>
        /// Tipo de operación SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        public string OperationType { get; set; } = "SELECT";

        /// <summary>
        /// Nivel de complejidad de la consulta (SIMPLE, MEDIUM, COMPLEX)
        /// </summary>
        public string ComplexityLevel { get; set; } = "SIMPLE";

        /// <summary>
        /// Modelo de IA utilizado para la generación
        /// </summary>
        public string AIModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo de procesamiento en millisegundos
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Nivel de confianza en la consulta generada (0-100)
        /// </summary>
        public int ConfidenceLevel { get; set; }

        /// <summary>
        /// Advertencias sobre la consulta generada
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
