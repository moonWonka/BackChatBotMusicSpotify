using MediatR;
using SpotifyMusicChatBot.API.Configuration;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Infra.Application.Repository;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SpotifyMusicChatBot API",
        Version = "v1",
    });
});

// 3. MediatR - Registra todos los handlers del assembly actual
builder.Services.AddMediatR(typeof(Program).Assembly);

// 4. Inyecciones
ConfigureServiceDependencies(builder.Services);
ConfigureRepositoryDependencies(builder.Services);

var app = builder.Build();

// VALIDACIÓN DE CONFIGURACIÓN AL ARRANQUE
try
{
    EnvironmentConfiguration.ValidateEnvironmentVariables(app.Logger);

    bool chatDbConnected = await EnvironmentConfiguration.TestChatDbConnectionAsync(app.Logger);
    bool spotifyDbConnected = await EnvironmentConfiguration.TestSpotifyDbConnectionAsync(app.Logger);
    
    if (!chatDbConnected)
    {
        app.Logger.LogWarning("⚠️ CHATDB no disponible. Funcionalidades del chat pueden fallar.");
    }
    
    if (!spotifyDbConnected)
    {
        app.Logger.LogInformation("ℹ️ SpotifyDB no disponible o no configurada.");
    }

    app.Logger.LogInformation("🚀 Aplicación configurada y lista para usar.");
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "❌ FALLA CRÍTICA DE CONFIGURACIÓN: {Message}", ex.Message);
    app.Logger.LogCritical("🛑 La aplicación no puede continuar.");
    Environment.Exit(1);
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.MapControllers();
app.Run();


// ----------- Métodos privados para DI -----------
static void ConfigureServiceDependencies(IServiceCollection services)
{
    // services.AddScoped<ISpotifyService, SpotifyService>();
    // services.AddScoped<IOpenAIService, OpenAIService>();
}

static void ConfigureRepositoryDependencies(IServiceCollection services)
{
    services.AddScoped<IChatBotRepository, ChatIARepository>();
}
