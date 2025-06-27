using SpotifyMusicChatBot.API.Application.ViewModel.Common;

namespace SpotifyMusicChatBot.API.Application.Command.AI.GenerateNaturalResponse
{
    /// <summary>
    /// Respuesta de la generación de respuesta en lenguaje natural
    /// </summary>
    public class GenerateNaturalResponseResponse : BaseResponse
    {
        /// <summary>
        /// Pregunta original del usuario
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Respuesta generada en lenguaje natural
        /// </summary>
        public string NaturalResponse { get; set; } = string.Empty;

        /// <summary>
        /// Respuesta alternativa (si se generó una opción adicional)
        /// </summary>
        public string? AlternativeResponse { get; set; }

        /// <summary>
        /// Resumen de los datos procesados
        /// </summary>
        public string DataSummary { get; set; } = string.Empty;

        /// <summary>
        /// Información estadística extraída de los datos
        /// </summary>
        public ResponseStatistics Statistics { get; set; } = new ResponseStatistics();

        /// <summary>
        /// Sugerencias de preguntas relacionadas
        /// </summary>
        public List<string> RelatedQuestions { get; set; } = new List<string>();

        /// <summary>
        /// Elementos destacados de la respuesta
        /// </summary>
        public List<string> Highlights { get; set; } = new List<string>();

        /// <summary>
        /// Tono utilizado en la respuesta
        /// </summary>
        public string ResponseTone { get; set; } = string.Empty;

        /// <summary>
        /// Longitud de la respuesta generada
        /// </summary>
        public string ResponseLength { get; set; } = string.Empty;

        /// <summary>
        /// Modelo de IA utilizado para la generación
        /// </summary>
        public string AIModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo de procesamiento en millisegundos
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Nivel de confianza en la respuesta generada (0-100)
        /// </summary>
        public int ConfidenceLevel { get; set; }

        /// <summary>
        /// Número de elementos procesados de la base de datos
        /// </summary>
        public int ProcessedItemsCount { get; set; }
    }

    /// <summary>
    /// Estadísticas extraídas de los datos para la respuesta
    /// </summary>
    public class ResponseStatistics
    {
        /// <summary>
        /// Número total de elementos encontrados
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Valor promedio (si aplicable)
        /// </summary>
        public double? AverageValue { get; set; }

        /// <summary>
        /// Valor máximo encontrado
        /// </summary>
        public string? MaxValue { get; set; }

        /// <summary>
        /// Valor mínimo encontrado
        /// </summary>
        public string? MinValue { get; set; }

        /// <summary>
        /// Categorías principales identificadas
        /// </summary>
        public List<string> MainCategories { get; set; } = new List<string>();

        /// <summary>
        /// Tendencias identificadas en los datos
        /// </summary>
        public List<string> Trends { get; set; } = new List<string>();
    }
}
