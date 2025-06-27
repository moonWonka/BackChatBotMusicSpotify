namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Respuesta base de los modelos de IA
    /// </summary>
    public class AIModelResponse
    {
        /// <summary>
        /// Contenido de la respuesta
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mensaje de error o información adicional
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Tiempo de procesamiento en millisegundos
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Modelo utilizado para generar la respuesta
        /// </summary>
        public string ModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Número de tokens utilizados
        /// </summary>
        public int TokensUsed { get; set; }

        /// <summary>
        /// Nivel de confianza de la respuesta (0-100)
        /// </summary>
        public int ConfidenceLevel { get; set; }

        /// <summary>
        /// Metadata adicional de la respuesta
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Resultado de la contextualización de una pregunta
    /// </summary>
    public class ContextualizationResult : AIModelResponse
    {
        /// <summary>
        /// Pregunta original
        /// </summary>
        public string OriginalQuestion { get; set; } = string.Empty;

        /// <summary>
        /// Pregunta contextualizada
        /// </summary>
        public string ContextualizedQuestion { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la pregunta fue contextualizada
        /// </summary>
        public bool WasContextualized { get; set; }

        /// <summary>
        /// Tipo de análisis (INDEPENDIENTE o CONTEXTUALIZADA)
        /// </summary>
        public string AnalysisType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Resultado de la validación de una pregunta
    /// </summary>
    public class ValidationResult : AIModelResponse
    {
        /// <summary>
        /// Estado de validación (VALIDA, ACLARAR, FUERA_CONTEXTO)
        /// </summary>
        public string ValidationStatus { get; set; } = string.Empty;

        /// <summary>
        /// Razón de la validación
        /// </summary>
        public string? ValidationReason { get; set; }

        /// <summary>
        /// Sugerencias para mejorar la pregunta
        /// </summary>
        public List<string> Suggestions { get; set; } = new List<string>();

        /// <summary>
        /// Categoría musical identificada
        /// </summary>
        public string? IdentifiedCategory { get; set; }
    }

    /// <summary>
    /// Resultado de la generación de SQL
    /// </summary>
    public class SQLGenerationResult : AIModelResponse
    {
        /// <summary>
        /// Consulta SQL generada
        /// </summary>
        public string GeneratedSQL { get; set; } = string.Empty;

        /// <summary>
        /// Explicación de la consulta
        /// </summary>
        public string SQLExplanation { get; set; } = string.Empty;

        /// <summary>
        /// Tablas involucradas
        /// </summary>
        public List<string> TablesUsed { get; set; } = new List<string>();

        /// <summary>
        /// Nivel de complejidad
        /// </summary>
        public string ComplexityLevel { get; set; } = "SIMPLE";

        /// <summary>
        /// Advertencias sobre la consulta
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();
    }

    /// <summary>
    /// Resultado de la generación de respuesta natural
    /// </summary>
    public class NaturalResponseResult : AIModelResponse
    {
        /// <summary>
        /// Respuesta en lenguaje natural
        /// </summary>
        public string NaturalResponse { get; set; } = string.Empty;

        /// <summary>
        /// Resumen de los datos procesados
        /// </summary>
        public string DataSummary { get; set; } = string.Empty;

        /// <summary>
        /// Preguntas relacionadas sugeridas
        /// </summary>
        public List<string> RelatedQuestions { get; set; } = new List<string>();

        /// <summary>
        /// Elementos destacados
        /// </summary>
        public List<string> Highlights { get; set; } = new List<string>();

        /// <summary>
        /// Tono utilizado
        /// </summary>
        public string ResponseTone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Resultado del análisis y mejora de respuesta
    /// </summary>
    public class AnalysisResult : AIModelResponse
    {
        /// <summary>
        /// Respuesta original
        /// </summary>
        public string OriginalResponse { get; set; } = string.Empty;

        /// <summary>
        /// Respuesta mejorada
        /// </summary>
        public string ImprovedResponse { get; set; } = string.Empty;

        /// <summary>
        /// Mejoras aplicadas
        /// </summary>
        public List<string> ImprovementsApplied { get; set; } = new List<string>();

        /// <summary>
        /// Puntuación de calidad (0-100)
        /// </summary>
        public int QualityScore { get; set; }
    }

    /// <summary>
    /// Información sobre el modelo de IA
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        /// Nombre del modelo
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Versión del modelo
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Proveedor del modelo
        /// </summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Límite máximo de tokens
        /// </summary>
        public int MaxTokens { get; set; }

        /// <summary>
        /// Límite de contexto
        /// </summary>
        public int ContextWindow { get; set; }

        /// <summary>
        /// Capacidades del modelo
        /// </summary>
        public List<string> Capabilities { get; set; } = new List<string>();

        /// <summary>
        /// Indica si el modelo está disponible
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
