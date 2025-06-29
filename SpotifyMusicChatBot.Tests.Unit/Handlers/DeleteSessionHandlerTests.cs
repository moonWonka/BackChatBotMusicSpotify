using Microsoft.Extensions.Logging;
using Moq;
using SpotifyMusicChatBot.API.Application.Command.DeleteSession;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.Tests.Unit.Handlers;

public class DeleteSessionHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsNotFound_WhenRepositoryReturnsFalse()
    {
        // Arrange
        var repoMock = new Mock<IChatBotRepository>();
        repoMock.Setup(r => r.DeleteSessionAsync("session1")).ReturnsAsync(false);

        var loggerMock = new Mock<ILogger<DeleteSessionHandler>>();
        var handler = new DeleteSessionHandler(repoMock.Object, loggerMock.Object);

        var request = new DeleteSessionRequest { SessionId = "session1" };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Sesión no encontrada", result.Message);
    }
}
