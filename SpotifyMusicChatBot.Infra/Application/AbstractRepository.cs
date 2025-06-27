using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Dapper;
using SpotifyMusicChatBot.Domain.Application;

namespace SpotifyMusicChatBot.Infra.Application
{    public abstract class AbstractRepository : IAbstractRepository
    {
        protected readonly string _connectionString;
        protected readonly ILogger? _logger;

        /// <summary>
        /// Constructor que recibe connection string directamente
        /// </summary>
        protected AbstractRepository(string connectionString, ILogger? logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        /// <summary>
        /// Constructor que obtiene connection string desde una variable de entorno específica
        /// </summary>
        /// <param name="environmentVariableName">Nombre de la variable de entorno que contiene la cadena de conexión completa (ej: "CHATDB")</param>
        protected AbstractRepository(string environmentVariableName, bool fromEnvironment, ILogger? logger = null)
        {
            if (!fromEnvironment)
                throw new ArgumentException("Este constructor requiere fromEnvironment = true");

            _connectionString = Environment.GetEnvironmentVariable(environmentVariableName)
                ?? throw new InvalidOperationException($"Variable de entorno '{environmentVariableName}' no encontrada");
            _logger = logger;
        }
        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }        // Métodos base para operaciones CRUD con Dapper - Implementación de IAbstractRepository
        public async Task<T?> GetByIdAsync<T>(string query, object parameters)
        {
            try
            {
                _logger?.LogDebug("🔍 Ejecutando consulta GetByIdAsync: {QueryType}", typeof(T).Name);
                using SqlConnection connection = CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
                
                _logger?.LogDebug("✅ Consulta GetByIdAsync completada: {QueryType}, Resultado: {HasResult}", 
                    typeof(T).Name, result != null ? "Encontrado" : "No encontrado");
                return result;
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en GetByIdAsync: {QueryType} | ErrorNumber: {ErrorNumber} | Query: {Query}", 
                    typeof(T).Name, ex.Number, query);
                throw new InvalidOperationException($"Error en consulta SQL: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en GetByIdAsync: {QueryType} | Query: {Query}", 
                    typeof(T).Name, query);
                throw new InvalidOperationException($"Error inesperado en consulta: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string query, object? parameters = null)
        {
            try
            {
                _logger?.LogDebug("🔍 Ejecutando consulta GetAllAsync: {QueryType}", typeof(T).Name);
                using SqlConnection connection = CreateConnection();
                var result = await connection.QueryAsync<T>(query, parameters);
                
                _logger?.LogDebug("✅ Consulta GetAllAsync completada: {QueryType}, Registros: {Count}", 
                    typeof(T).Name, result.Count());
                return result;
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en GetAllAsync: {QueryType} | ErrorNumber: {ErrorNumber} | Query: {Query}", 
                    typeof(T).Name, ex.Number, query);
                throw new InvalidOperationException($"Error en consulta SQL: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en GetAllAsync: {QueryType} | Query: {Query}", 
                    typeof(T).Name, query);
                throw new InvalidOperationException($"Error inesperado en consulta: {ex.Message}", ex);
            }
        }        public async Task<int> ExecuteAsync(string query, object parameters)
        {
            try
            {
                _logger?.LogDebug("🔄 Iniciando transacción ExecuteAsync");
                using SqlConnection connection = CreateConnection();
                await connection.OpenAsync();
                using SqlTransaction transaction = connection.BeginTransaction();
                
                try
                {
                    int result = await connection.ExecuteAsync(query, parameters, transaction);
                    await transaction.CommitAsync();
                    
                    _logger?.LogInformation("✅ Transacción ExecuteAsync completada exitosamente. Filas afectadas: {AffectedRows}", result);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    _logger?.LogWarning("🔄 Transacción ExecuteAsync revertida (rollback)");
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "❌ Error SQL en ExecuteAsync | ErrorNumber: {ErrorNumber} | Severity: {Severity} | Query: {Query}", 
                    ex.Number, ex.Class, query);
                throw new InvalidOperationException($"Error en ejecución SQL: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado en ExecuteAsync | Query: {Query}", query);
                throw new InvalidOperationException($"Error inesperado en ejecución: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ejecuta una operación sin transacción (para casos donde no se requiere)
        /// </summary>
        public async Task<int> ExecuteWithoutTransactionAsync(string query, object parameters)
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                return await connection.ExecuteAsync(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error en ejecución SQL: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado en ejecución: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ejecuta múltiples operaciones en una sola transacción
        /// </summary>
        public async Task<int> ExecuteMultipleAsync(params (string query, object parameters)[] operations)
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                await connection.OpenAsync();
                using SqlTransaction transaction = connection.BeginTransaction();
                
                try
                {
                    int totalAffectedRows = 0;
                    foreach (var (query, parameters) in operations)
                    {
                        totalAffectedRows += await connection.ExecuteAsync(query, parameters, transaction);
                    }
                    
                    await transaction.CommitAsync();
                    return totalAffectedRows;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error en ejecución múltiple SQL: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado en ejecución múltiple: {ex.Message}", ex);
            }
        }        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters)
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                await connection.OpenAsync();
                using SqlTransaction transaction = connection.BeginTransaction();
                
                try
                {
                    T? result = await connection.ExecuteScalarAsync<T>(query, parameters, transaction);
                    await transaction.CommitAsync();
                    return result!; // Se asume que T es non-nullable en este contexto
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error en consulta SQL escalar: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado en consulta escalar: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ejecuta una consulta escalar sin transacción (para casos donde no se requiere)
        /// </summary>
        public async Task<T> ExecuteScalarWithoutTransactionAsync<T>(string query, object parameters)
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                T? result = await connection.ExecuteScalarAsync<T>(query, parameters);
                return result!; // Se asume que T es non-nullable en este contexto
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error en consulta SQL escalar: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado en consulta escalar: {ex.Message}", ex);
            }
        }        // Método para verificar conexión
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                await connection.OpenAsync();
                
                _logger?.LogInformation("✅ Conexión a base de datos exitosa");
                return true;
            }
            catch (SqlException ex)
            {
                // Log específico para errores de SQL Server con Application Insights
                _logger?.LogError(ex, "❌ Error de conexión SQL: {ErrorMessage} | ErrorNumber: {ErrorNumber} | Severity: {Severity}", 
                    ex.Message, ex.Number, ex.Class);
                return false;
            }
            catch (Exception ex)
            {
                // Log para otros tipos de errores
                _logger?.LogError(ex, "❌ Error inesperado de conexión: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión configurada
        /// </summary>
        /// <returns>La cadena de conexión</returns>
        public string GetConnectionString()
        {
            return _connectionString;
        }

        /// <summary>
        /// Inicializa una nueva conexión y transacción
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tupla con la conexión y transacción inicializadas</returns>
        public async Task<(SqlConnection connection, SqlTransaction transaction)> InitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var transaction = connection.BeginTransaction();
                
                _logger?.LogDebug("🔄 Transacción inicializada exitosamente");
                return (connection, transaction);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inicializando transacción: {Message}", ex.Message);
                throw new InvalidOperationException($"Error al inicializar transacción: {ex.Message}", ex);
            }
        }

/// <summary>
/// Ejecuta una operación con transacción opcional (si es null, crea una nueva)
/// </summary>
public async Task<int> ExecuteWithTransactionAsync(string query, object parameters, SqlTransaction? transaction = null)
{
    bool shouldManageTransaction = transaction == null;
    SqlConnection? connection = null;
    SqlTransaction? managedTransaction = transaction;
    
    try
    {
        if (shouldManageTransaction)
        {
            connection = CreateConnection();
            await connection.OpenAsync();
            managedTransaction = connection.BeginTransaction();
        }
        
        // Usar la conexión correcta según el contexto
        var connectionToUse = shouldManageTransaction ? connection! : managedTransaction!.Connection!;
        int result = await connectionToUse.ExecuteAsync(query, parameters, managedTransaction);
        
        if (shouldManageTransaction)
        {
            await managedTransaction!.CommitAsync();
            _logger?.LogInformation("✅ Transacción ExecuteWithTransactionAsync completada exitosamente. Filas afectadas: {AffectedRows}", result);
        }
        
        return result;
    }
    catch (SqlException ex)
    {
        // Hacer rollback solo si estamos manejando la transacción
        if (shouldManageTransaction && managedTransaction != null)
        {
            try
            {
                await managedTransaction.RollbackAsync();
                _logger?.LogWarning("🔄 Transacción ExecuteWithTransactionAsync revertida (rollback)");
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "❌ Error durante rollback en ExecuteWithTransactionAsync");
            }
        }
        
        _logger?.LogError(ex, "❌ Error SQL en ExecuteWithTransactionAsync | ErrorNumber: {ErrorNumber} | Query: {Query}", 
            ex.Number, query);
        throw new InvalidOperationException($"Error en ejecución SQL: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        // Hacer rollback solo si estamos manejando la transacción
        if (shouldManageTransaction && managedTransaction != null)
        {
            try
            {
                await managedTransaction.RollbackAsync();
                _logger?.LogWarning("🔄 Transacción ExecuteWithTransactionAsync revertida (rollback)");
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "❌ Error durante rollback en ExecuteWithTransactionAsync");
            }
        }
        
        _logger?.LogError(ex, "❌ Error inesperado en ExecuteWithTransactionAsync | Query: {Query}", query);
        throw new InvalidOperationException($"Error inesperado en ejecución: {ex.Message}", ex);
    }
    finally
    {
        // Solo hacer dispose de recursos que creamos nosotros
        if (shouldManageTransaction)
        {
            managedTransaction?.Dispose();
            connection?.Dispose();
        }
    }
}

/// <summary>
/// Ejecuta una consulta escalar con transacción opcional
/// </summary>
public async Task<T> ExecuteScalarWithTransactionAsync<T>(string query, object parameters, SqlTransaction? transaction = null)
{
    bool shouldManageTransaction = transaction == null;
    SqlConnection? connection = null;
    SqlTransaction? managedTransaction = transaction;
    
    try
    {
        if (shouldManageTransaction)
        {
            connection = CreateConnection();
            await connection.OpenAsync();
            managedTransaction = connection.BeginTransaction();
        }
        
        // Usar la conexión correcta según el contexto
        var connectionToUse = shouldManageTransaction ? connection! : managedTransaction!.Connection!;
        T? result = await connectionToUse.ExecuteScalarAsync<T>(query, parameters, managedTransaction);
        
        if (shouldManageTransaction)
        {
            await managedTransaction!.CommitAsync();
            _logger?.LogInformation("✅ Transacción escalar ExecuteScalarWithTransactionAsync completada exitosamente.");
        }
        
        return result!;
    }
    catch (SqlException ex)
    {
        if (shouldManageTransaction && managedTransaction != null)
        {
            try
            {
                await managedTransaction.RollbackAsync();
                _logger?.LogWarning("🔄 Transacción escalar ExecuteScalarWithTransactionAsync revertida (rollback)");
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "❌ Error durante rollback en ExecuteScalarWithTransactionAsync");
            }
        }
        
        _logger?.LogError(ex, "❌ Error SQL en ExecuteScalarWithTransactionAsync | ErrorNumber: {ErrorNumber} | Query: {Query}", 
            ex.Number, query);
        throw new InvalidOperationException($"Error en consulta SQL escalar: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        if (shouldManageTransaction && managedTransaction != null)
        {
            try
            {
                await managedTransaction.RollbackAsync();
                _logger?.LogWarning("🔄 Transacción escalar ExecuteScalarWithTransactionAsync revertida (rollback)");
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "❌ Error durante rollback en ExecuteScalarWithTransactionAsync");
            }
        }
        
        _logger?.LogError(ex, "❌ Error inesperado en ExecuteScalarWithTransactionAsync | Query: {Query}", query);
        throw new InvalidOperationException($"Error inesperado en consulta escalar: {ex.Message}", ex);
    }
    finally
    {
        // Solo hacer dispose de recursos que creamos nosotros
        if (shouldManageTransaction)
        {
            managedTransaction?.Dispose();
            connection?.Dispose();
        }
    }
}
    }
}