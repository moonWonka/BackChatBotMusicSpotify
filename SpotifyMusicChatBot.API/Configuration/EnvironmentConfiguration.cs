using Microsoft.Extensions.Logging;

namespace SpotifyMusicChatBot.API.Configuration
{
    /// <summary>
    /// Configuración estática para validar y gestionar variables de entorno al arranque
    /// Usa el patrón de variable única que contiene la cadena de conexión completa
    /// </summary>
    public static class EnvironmentConfiguration
    {        // Variables de entorno requeridas y opcionales
        private static readonly Dictionary<string, (bool isRequired, bool isSecret, string description)> EnvironmentVariables = new()
        {
            { "CHATDB", (true, true, "Cadena de conexión completa para la base de datos principal del chat") },
            { "SpotifyDB", (false, true, "Cadena de conexión completa para datos de Spotify (opcional)") },
            { "OPENAI_API_KEY", (false, true, "Clave API de OpenAI para funcionalidades de IA") },
            { "GEMINI_API_KEY", (false, true, "Clave API de Google Gemini") },
            { "ANTHROPIC_API_KEY", (false, true, "Clave API de Anthropic Claude") },
            { "APPLICATIONINSIGHTS_CONNECTION_STRING", (false, true, "Connection String de Azure Application Insights para telemetría") }
        };

        /// <summary>
        /// Valida que todas las variables de entorno requeridas estén configuradas
        /// Se llama durante el startup de la aplicación
        /// </summary>
        public static void ValidateEnvironmentVariables(ILogger logger)
        {
            logger.LogInformation("🔍 Validando configuración de entorno...");
            
            var missingRequired = new List<string>();
            var secretCount = 0;
            var publicCount = 0;

            foreach (var (varName, (isRequired, isSecret, description)) in EnvironmentVariables)
            {
                var value = Environment.GetEnvironmentVariable(varName);
                
                if (string.IsNullOrEmpty(value))
                {
                    if (isRequired)
                    {
                        missingRequired.Add(varName);
                        logger.LogError("❌ REQUERIDA: {VarName} - {Description}", varName, description);
                    }
                    else
                    {
                        logger.LogWarning("⚠️ OPCIONAL: {VarName} - {Description}", varName, description);
                    }
                }
                else
                {
                    if (isSecret)
                    {
                        secretCount++;
                        logger.LogInformation("✅ [SECRET] {VarName}: **** - {Description}", varName, description);
                    }
                    else
                    {
                        publicCount++;
                        logger.LogInformation("✅ [PUBLIC] {VarName}: {Value} - {Description}", varName, value, description);
                    }
                }
            }

            // Resumen de configuración
            logger.LogInformation("📊 Resumen de configuración:");
            logger.LogInformation("   🔒 Variables secretas configuradas: {SecretCount}", secretCount);
            logger.LogInformation("   🌐 Variables públicas configuradas: {PublicCount}", publicCount);

            if (missingRequired.Any())
            {
                var missing = string.Join(", ", missingRequired);
                logger.LogCritical("❌ FALLO EN CONFIGURACIÓN: Variables requeridas faltantes: {MissingVars}", missing);
                throw new InvalidOperationException($"Configuración incompleta. Variables requeridas faltantes: {missing}");
            }

            logger.LogInformation("✅ Configuración de entorno válida");
        }

        /// <summary>
        /// Obtiene una cadena de conexión desde una variable de entorno específica
        /// </summary>
        public static string GetConnectionString(string environmentVariableName)
        {
            return Environment.GetEnvironmentVariable(environmentVariableName)
                ?? throw new InvalidOperationException($"Variable de entorno '{environmentVariableName}' no encontrada");
        }

        /// <summary>
        /// Obtiene la cadena de conexión principal del chat
        /// </summary>
        public static string GetChatDbConnectionString()
        {
            return GetConnectionString("CHATDB");
        }

        /// <summary>
        /// Obtiene la cadena de conexión de Spotify (opcional)
        /// </summary>
        public static string? GetSpotifyDbConnectionString()
        {
            return Environment.GetEnvironmentVariable("SpotifyDB");
        }

        /// <summary>
        /// Obtiene una API Key específica
        /// </summary>
        public static string? GetApiKey(string service)
        {
            var envVarName = $"{service.ToUpper()}_API_KEY";
            return Environment.GetEnvironmentVariable(envVarName);
        }

        /// <summary>
        /// Verifica la conectividad a la base de datos principal al arranque
        /// </summary>
        public static async Task<bool> TestChatDbConnectionAsync(ILogger logger)
        {
            try
            {
                var connectionString = GetChatDbConnectionString();
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();
                
                logger.LogInformation("✅ Conexión a CHATDB verificada exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error al conectar con CHATDB: {Error}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Verifica la conectividad a la base de datos de Spotify (si está configurada)
        /// </summary>
        public static async Task<bool> TestSpotifyDbConnectionAsync(ILogger logger)
        {
            try
            {
                var connectionString = GetSpotifyDbConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    logger.LogInformation("ℹ️ SpotifyDB no configurada, omitiendo test de conexión");
                    return true; // No es error si no está configurada
                }

                using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();
                
                logger.LogInformation("✅ Conexión a SpotifyDB verificada exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "⚠️ Error al conectar con SpotifyDB: {Error}", ex.Message);
                return false;
            }
        }
    }
}
