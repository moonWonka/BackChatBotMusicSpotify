using System.Net.Http.Json;
using SpotifyMusicChatBot.API.Application.Command.SaveConversation;
using SpotifyMusicChatBot.Tests.Integration.Infrastructure;

namespace SpotifyMusicChatBot.Tests.Integration.Controllers;

public class ChatControllerTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;

    public ChatControllerTests(TestApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostConversation_ReturnsOk()
    {
        var request = new SaveConversationRequest { UserPrompt = "hi", AiResponse = "hello" };
        var response = await _client.PostAsJsonAsync("/api/chat/conversation", request);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetSessionSummary_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/chat/conversation/123/summary");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
