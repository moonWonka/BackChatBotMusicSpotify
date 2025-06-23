using Microsoft.Extensions.Logging;
using Moq;
using SpotifyMusicChatBot.API.Application.Query.SearchConversations;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.Tests.Unit.Handlers;

public class SearchConversationsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenSearchTermIsEmpty()
    {
        var repoMock = new Mock<IChatBotRepository>();
        var loggerMock = new Mock<ILogger<SearchConversationsHandler>>();
        var handler = new SearchConversationsHandler(repoMock.Object, loggerMock.Object);

        var request = new SearchConversationsRequest { SearchTerm = "" };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("El término de búsqueda es requerido", result.Message);
    }
}
