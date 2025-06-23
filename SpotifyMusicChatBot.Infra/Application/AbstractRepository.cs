using Microsoft.Data.SqlClient;
using Dapper;
using SpotifyMusicChatBot.Domain.Application;

namespace SpotifyMusicChatBot.Infra.Application
{
    public abstract class AbstractRepository : IAbstractRepository
    {
        protected readonly string _connectionString;

        /// <summary>
        /// Constructor que recibe connection string directamente
        /// </summary>
        protected AbstractRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Constructor que obtiene connection string desde una variable de entorno específica
        /// </summary>
        /// <param name="environmentVariableName">Nombre de la variable de entorno que contiene la cadena de conexión completa (ej: "CHATDB")</param>
        protected AbstractRepository(string environmentVariableName, bool fromEnvironment)
        {
            if (!fromEnvironment)
                throw new ArgumentException("Este constructor requiere fromEnvironment = true");
                
            _connectionString = Environment.GetEnvironmentVariable(environmentVariableName)
                ?? throw new InvalidOperationException($"Variable de entorno '{environmentVariableName}' no encontrada");
        }
        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Métodos base para operaciones CRUD con Dapper - Implementación de IAbstractRepository
        public async Task<T?> GetByIdAsync<T>(string query, object parameters)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string query, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object parameters)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters)
        {
            using var connection = CreateConnection();
            var result = await connection.ExecuteScalarAsync<T>(query, parameters);
            return result!; // Se asume que T es non-nullable en este contexto
        }

        // Método para verificar conexión
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}