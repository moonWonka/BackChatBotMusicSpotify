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

    // Probar conexión a base de datos
    var dbConnected = await EnvironmentConfiguration.TestDatabaseConnectionAsync(app.Logger);
    if (!dbConnected)
    {
        app.Logger.LogWarning("⚠️ BD no disponible. Algunas funcionalidades pueden fallar.");
    }

    app.Logger.LogInformation("🚀 Aplicación configurada y lista.");
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
    // Registrar connection string usando configuración estática
    services.AddScoped<string>(_ => EnvironmentConfiguration.GetDatabaseConnectionString());

    // services.AddScoped<ISpotifyService, SpotifyService>();
    // services.AddScoped<IOpenAIService, OpenAIService>();
}

static void ConfigureRepositoryDependencies(IServiceCollection services)
{
    // services.AddScoped<IUserRepository, UserRepository>();
    // services.AddScoped<IChatBotRepository, ChatBotRepository>();
}
