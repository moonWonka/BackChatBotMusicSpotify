using MediatR;
using SpotifyMusicChatBot.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Swagger configuración básica
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SpotifyMusicChatBot API",
        Version = "v1",
        Description = "API para el ChatBot de Música de Spotify"
    });
});

// 3. MediatR
builder.Services.AddMediatR(typeof(Program));

// 4. Inyecciones separadas
ConfigureServiceDependencies(builder.Services);
ConfigureRepositoryDependencies(builder.Services);

var app = builder.Build();

// VALIDACIÓN DE CONFIGURACIÓN AL ARRANQUE
try
{
    // Validar variables de entorno requeridas
    EnvironmentConfiguration.ValidateEnvironmentVariables(app.Logger);

    // Probar conexiones a bases de datos
    var chatDbConnected = await EnvironmentConfiguration.TestChatDbConnectionAsync(app.Logger);
    var spotifyDbConnected = await EnvironmentConfiguration.TestSpotifyDbConnectionAsync(app.Logger);
    
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

// Middleware y pipeline
app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotifyMusicChatBot API v1");
    });
}

app.MapControllers();
app.Run();


// ----------- Métodos privados para DI -----------
static void ConfigureServiceDependencies(IServiceCollection services)
{
    // Ya no necesitamos registrar connection string como servicio
    // Los repositorios obtienen sus connection strings directamente desde variables de entorno
    
    // services.AddScoped<ISpotifyService, SpotifyService>();
    // services.AddScoped<IOpenAIService, OpenAIService>();
}

static void ConfigureRepositoryDependencies(IServiceCollection services)
{
    // Ejemplos de cómo registrar repositorios que usan el nuevo patrón de variable única:
    
    // Los repositorios se crean automáticamente desde variables de entorno
    // services.AddScoped<ChatBotRepository>(); // Usará automáticamente "CHATDB"
    // services.AddScoped<SpotifyRepository>();  // Usará automáticamente "SpotifyDB"
    
    // Para repositorio con connection string directo (casos especiales):
    // services.AddScoped<AnalyticsRepository>(provider =>
    //     new AnalyticsRepository("connection_string_directa_aqui")
    // );
    
    // Futuras implementaciones con interfaces:
    // services.AddScoped<IChatBotRepository, ChatBotRepository>();
    // services.AddScoped<ISpotifyDataRepository, SpotifyRepository>();
}
