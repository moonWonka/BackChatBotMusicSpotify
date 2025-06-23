using SpotifyMusicChatBot.Domain.Application.Model.Conversation;
using SpotifyMusicChatBot.Domain.Application.Model.Search;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.Tests.Integration.Infrastructure;

public class FakeChatBotRepository : IChatBotRepository
{
    private readonly List<ConversationTurn> _turns = new();

    public string GenerateSessionId() => Guid.NewGuid().ToString();

    public Task<bool> SaveConversationAsync(string userPrompt, string aiResponse, string sessionId)
    {
        _turns.Add(new ConversationTurn
        {
            Id = _turns.Count + 1,
            Timestamp = DateTime.UtcNow,
            UserPrompt = userPrompt,
            AiResponse = aiResponse,
        });
        return Task.FromResult(true);
    }

    public Task<IEnumerable<ConversationSession>> GetAllConversationsAsync() =>
        Task.FromResult<IEnumerable<ConversationSession>>(new List<ConversationSession>());

    public Task<IEnumerable<ConversationTurn>> GetConversationBySessionIdAsync(string sessionId) =>
        Task.FromResult<IEnumerable<ConversationTurn>>(_turns);

    public Task<SessionSummary?> GetSessionSummaryAsync(string sessionId) =>
        Task.FromResult<SessionSummary?>(null);

    public Task<bool> DeleteSessionAsync(string sessionId) => Task.FromResult(true);

    public Task<IEnumerable<SearchResult>> SearchConversationsAsync(string searchTerm) =>
        Task.FromResult<IEnumerable<SearchResult>>(new List<SearchResult>());
}
