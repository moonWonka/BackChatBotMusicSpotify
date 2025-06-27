namespace SpotifyMusicChatBot.Domain.Application.Services
{
    /// <summary>
    /// Factory para crear instancias de servicios de IA
    /// </summary>
    public interface IAIServiceFactory
    {
        /// <summary>
        /// Crea una instancia del servicio de IA especificado
        /// </summary>
        /// <param name="modelName">Nombre del modelo (Gemini, Anthropic)</param>
        /// <returns>Instancia del servicio de IA</returns>
        IBaseAIService CreateAIService(string modelName);

        /// <summary>
        /// Obtiene la lista de modelos disponibles
        /// </summary>
        /// <returns>Lista de nombres de modelos disponibles</returns>
        Task<List<string>> GetAvailableModelsAsync();

        /// <summary>
        /// Valida si un modelo está disponible
        /// </summary>
        /// <param name="modelName">Nombre del modelo</param>
        /// <returns>True si el modelo está disponible</returns>
        Task<bool> IsModelAvailableAsync(string modelName);

        /// <summary>
        /// Obtiene información de todos los modelos disponibles
        /// </summary>
        /// <returns>Diccionario con información de los modelos</returns>
        Task<Dictionary<string, ModelInfo>> GetAllModelsInfoAsync();

        /// <summary>
        /// Obtiene el modelo recomendado para un tipo de tarea específica
        /// </summary>
        /// <param name="taskType">Tipo de tarea (contextualization, validation, sql_generation, natural_response, analysis)</param>
        /// <returns>Nombre del modelo recomendado</returns>
        string GetRecommendedModelForTask(string taskType);
    }

    /// <summary>
    /// Tipos de tareas para recomendación de modelos
    /// </summary>
    public static class AITaskTypes
    {
        public const string CONTEXTUALIZATION = "contextualization";
        public const string VALIDATION = "validation";
        public const string SQL_GENERATION = "sql_generation";
        public const string NATURAL_RESPONSE = "natural_response";
        public const string ANALYSIS = "analysis";
    }

    /// <summary>
    /// Constantes para nombres de modelos
    /// </summary>
    public static class AIModelNames
    {
        public const string GEMINI = "Gemini";
        public const string ANTHROPIC = "Anthropic";
        public const string CLAUDE = "Claude";
        
        /// <summary>
        /// Lista de todos los modelos disponibles
        /// </summary>
        public static readonly List<string> ALL_MODELS = new List<string>
        {
            GEMINI,
            ANTHROPIC
        };

        /// <summary>
        /// Valida si un nombre de modelo es válido
        /// </summary>
        /// <param name="modelName">Nombre del modelo</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidModelName(string modelName)
        {
            return ALL_MODELS.Contains(modelName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Normaliza el nombre del modelo
        /// </summary>
        /// <param name="modelName">Nombre del modelo</param>
        /// <returns>Nombre normalizado</returns>
        public static string NormalizeModelName(string modelName)
        {
            if (string.IsNullOrWhiteSpace(modelName))
                return GEMINI; // Default

            var normalized = modelName.Trim();
            
            // Mapear variantes de nombres
            if (normalized.Equals("Claude", StringComparison.OrdinalIgnoreCase))
                return ANTHROPIC;

            return ALL_MODELS.FirstOrDefault(m => 
                m.Equals(normalized, StringComparison.OrdinalIgnoreCase)) ?? GEMINI;
        }
    }
}
