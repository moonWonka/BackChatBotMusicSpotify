using SpotifyMusicChatBot.Domain.Application.Services;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    public class GeminiIAService : IGeminiIAService
    {
        public string ModelName => "Gemini";

        public string Provider => "Google";

        public Task<AIModelResponse> ExecuteModelAsync(string prompt, float temperature = 0.7F, int maxTokens = 1000, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}