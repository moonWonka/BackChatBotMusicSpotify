using Microsoft.Extensions.Logging;

namespace SpotifyMusicChatBot.API.Configuration
{
    /// <summary>
    /// Configuración estática para validar y gestionar variables de entorno al arranque
    /// Se ejecuta durante el build/startup de la aplicación, no es un servicio de negocio
    /// </summary>
    public static class EnvironmentConfiguration
    {
        // Variables requeridas con su clasificación de seguridad
        private static readonly Dictionary<string, bool> RequiredVariables = new()
        {
            { "DB_HOST", false },          // Público
            { "DB_NAME", false },          // Público  
            { "DB_USER", true },           // Secreto
            { "DB_PASSWORD", true },       // Secreto
            { "OPENAI_API_KEY", true },    // Secreto
            { "GEMINI_API_KEY", true },    // Secreto
            { "ANTHROPIC_API_KEY", true }  // Secreto
        };

        /// <summary>
        /// Valida que todas las variables de entorno requeridas estén configuradas
        /// Se llama durante el startup de la aplicación
        /// </summary>
        public static void ValidateEnvironmentVariables(ILogger logger)
        {
            logger.LogInformation("🔍 Validando configuración de entorno al arranque...");
            
            var missingVars = new List<string>();
            var secretCount = 0;
            var publicCount = 0;

            foreach (var (varName, isSecret) in RequiredVariables)
            {
                var value = Environment.GetEnvironmentVariable(varName);
                
                if (string.IsNullOrEmpty(value))
                {
                    missingVars.Add(varName);
                }
                else
                {
                    if (isSecret)
                    {
                        secretCount++;
                        logger.LogInformation("✅ [SECRET] {VarName}: ****", varName);
                    }
                    else
                    {
                        publicCount++;
                        logger.LogInformation("✅ [PUBLIC] {VarName}: {Value}", varName, value);
                    }
                }
            }

            // Resumen de carga
            logger.LogInformation("📊 Configuración de entorno:");
            logger.LogInformation("   🔒 Variables secretas: {SecretCount}", secretCount);
            logger.LogInformation("   🌐 Variables públicas: {PublicCount}", publicCount);

            if (missingVars.Any())
            {
                var missing = string.Join(", ", missingVars);
                logger.LogCritical("❌ Variables de entorno faltantes: {MissingVars}", missing);
                throw new InvalidOperationException($"Configuración incompleta. Variables faltantes: {missing}");
            }

            logger.LogInformation("✅ Configuración de entorno válida");
        }

        /// <summary>
        /// Construye la cadena de conexión a la base de datos
        /// </summary>
        public static string GetDatabaseConnectionString()
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST") 
                ?? throw new InvalidOperationException("DB_HOST no configurado");
            var database = Environment.GetEnvironmentVariable("DB_NAME") 
                ?? throw new InvalidOperationException("DB_NAME no configurado");
            var user = Environment.GetEnvironmentVariable("DB_USER") 
                ?? throw new InvalidOperationException("DB_USER no configurado");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") 
                ?? throw new InvalidOperationException("DB_PASSWORD no configurado");

            return $"Server={host};Database={database};User Id={user};Password={password};TrustServerCertificate=true;";
        }

        /// <summary>
        /// Obtiene una API Key específica
        /// </summary>
        public static string GetApiKey(string service)
        {
            var envVarName = $"{service.ToUpper()}_API_KEY";
            return Environment.GetEnvironmentVariable(envVarName) 
                ?? throw new InvalidOperationException($"{envVarName} no configurado");
        }

        /// <summary>
        /// Verifica la conectividad a la base de datos al arranque
        /// </summary>
        public static async Task<bool> TestDatabaseConnectionAsync(ILogger logger)
        {
            try
            {
                var connectionString = GetDatabaseConnectionString();
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();
                
                logger.LogInformation("✅ Conexión a base de datos verificada");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "⚠️ Conexión a base de datos falló: {Error}", ex.Message);
                return false;
            }
        }
    }
}
