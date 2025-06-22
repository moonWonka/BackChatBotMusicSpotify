namespace SpotifyMusicChatBot.Domain.Application
{
    public interface IAbstractRepository
    {
        /// <summary>
        /// Obtiene un registro por su ID usando una consulta personalizada
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a retornar</typeparam>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <returns>El objeto encontrado o null si no existe</returns>
        Task<T?> GetByIdAsync<T>(string query, object parameters);

        /// <summary>
        /// Obtiene todos los registros usando una consulta personalizada
        /// </summary>
        /// <typeparam name="T">Tipo de los objetos a retornar</typeparam>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta (opcional)</param>
        /// <returns>Colección de objetos encontrados</returns>
        Task<IEnumerable<T>> GetAllAsync<T>(string query, object? parameters = null);

        /// <summary>
        /// Ejecuta una consulta que no retorna datos (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <returns>Número de filas afectadas</returns>
        Task<int> ExecuteAsync(string query, object parameters);

        /// <summary>
        /// Ejecuta una consulta que retorna un valor escalar
        /// </summary>
        /// <typeparam name="T">Tipo del valor a retornar</typeparam>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros para la consulta</param>
        /// <returns>El valor escalar resultado de la consulta</returns>
        Task<T> ExecuteScalarAsync<T>(string query, object parameters);

        /// <summary>
        /// Verifica si la conexión a la base de datos está disponible
        /// </summary>
        /// <returns>True si la conexión es exitosa, false en caso contrario</returns>
        Task<bool> TestConnectionAsync();
    }
}