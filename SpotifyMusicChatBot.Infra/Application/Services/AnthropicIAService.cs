using SpotifyMusicChatBot.Domain.Application.Services;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    public class AnthropicIAService : IAnthropicIAService
    {
        public string ModelName => "Claude";

        public string Provider => "Anthropic";

        public Task<AIModelResponse> ExecuteModelAsync(string prompt, float temperature = 0.7F, int maxTokens = 1000, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AIModelResponse
            {
                Content = "This is a mock response from Anthropic IA Service.",
                IsSuccess = true,
                Message = "Mock response generated successfully."
            });
        }
    }
}