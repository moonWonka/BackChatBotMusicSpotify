using MediatR;
using SpotifyMusicChatBot.API.Configuration;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Infra.Application.Repository;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 1.1 Configurar logging para producción
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (builder.Environment.IsProduction())
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

// 2. Azure Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(connectionString))
    {
        options.ConnectionString = connectionString;
        Console.WriteLine("📊 Application Insights configurado correctamente");
    }
    else
    {
        Console.WriteLine("⚠️ Application Insights no configurado (variable APPLICATIONINSIGHTS_CONNECTION_STRING no encontrada)");
    }
});

// 3. Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SpotifyMusicChatBot API",
        Version = "v1",
    });
});

// 4. MediatR - Registra todos los handlers del assembly actual
builder.Services.AddMediatR(typeof(Program).Assembly);

// 5. Inyecciones
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

// Swagger disponible en todos los entornos para debugging
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    if (app.Environment.IsProduction())
    {
        options.RoutePrefix = "swagger"; // Accesible en /swagger
    }
});

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
