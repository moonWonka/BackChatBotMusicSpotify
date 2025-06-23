using SpotifyMusicChatBot.Infra.Application;

namespace SpotifyMusicChatBot.Infra.Application.Repository
{    /// <summary>
    /// Repositorio principal para chat usando variable de entorno CHATDB
    /// </summary>
    public class ChatBotRepository : AbstractRepository
    {
        private const string DB = "CHATDB";

        // Usa la variable de entorno "CHATDB" que contiene la cadena de conexión completa
        public ChatBotRepository() : base(DB, fromEnvironment: true)
        {
        }

        public async Task<ChatMessage?> GetMessageByIdAsync(int messageId)
        {
            var query = "SELECT * FROM ChatMessages WHERE MessageId = @MessageId";
            return await GetByIdAsync<ChatMessage>(query, new { MessageId = messageId });
        }

        public async Task<IEnumerable<ChatMessage>> GetUserMessagesAsync(string userId)
        {
            var query = "SELECT * FROM ChatMessages WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            return await GetAllAsync<ChatMessage>(query, new { UserId = userId });
        }

        public async Task<int> SaveMessageAsync(string message, string userId)
        {
            var query = @"INSERT INTO ChatMessages (Message, UserId, CreatedAt) 
                         VALUES (@Message, @UserId, @CreatedAt)";
            return await ExecuteAsync(query, new { Message = message, UserId = userId, CreatedAt = DateTime.Now });
        }
    }

    /// <summary>
    /// Repositorio para datos de Spotify usando variable de entorno SpotifyDB
    /// </summary>
    public class SpotifyRepository : AbstractRepository
    {
        // Usa la variable de entorno "SpotifyDB" para datos de Spotify
        public SpotifyRepository() : base("SpotifyDB", fromEnvironment: true)
        {
        }

        public async Task<UserProfile?> GetUserProfileAsync(string spotifyUserId)
        {
            var query = "SELECT * FROM SpotifyProfiles WHERE SpotifyUserId = @SpotifyUserId";
            return await GetByIdAsync<UserProfile>(query, new { SpotifyUserId = spotifyUserId });
        }
    }

    /// <summary>
    /// Ejemplo usando connection string directo
    /// </summary>
    public class AnalyticsRepository : AbstractRepository
    {
        public AnalyticsRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<int> GetTotalUsersAsync()
        {
            var query = "SELECT COUNT(*) FROM Users";
            return await ExecuteScalarAsync<int>(query, new { });
        }
    }

    // Modelos de ejemplo
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UserProfile
    {
        public string SpotifyUserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
