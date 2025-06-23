using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SpotifyMusicChatBot.API;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.Tests.Integration.Infrastructure;

public class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace repository with fake implementation
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IChatBotRepository));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddSingleton<IChatBotRepository, FakeChatBotRepository>();
        });
    }
}
