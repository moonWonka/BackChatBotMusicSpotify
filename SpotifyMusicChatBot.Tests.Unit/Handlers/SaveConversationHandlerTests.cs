using Microsoft.Extensions.Logging;
using Moq;
using SpotifyMusicChatBot.API.Application.Command.SaveConversation;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.Tests.Unit.Handlers;

public class SaveConversationHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSuccess_WhenRepositorySavesConversation()
    {
        // Arrange
        var repoMock = new Mock<IChatBotRepository>();
        repoMock.Setup(r => r.GenerateSessionId()).Returns("session1");
        repoMock.Setup(r => r.SaveConversationAsync(It.IsAny<string>(), It.IsAny<string>(), "session1"))
            .ReturnsAsync(true);

        var loggerMock = new Mock<ILogger<SaveConversationHandler>>();
        var handler = new SaveConversationHandler(repoMock.Object, loggerMock.Object);

        var request = new SaveConversationRequest { UserPrompt = "hi", AiResponse = "hello" };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("session1", result.SessionId);
    }
}
